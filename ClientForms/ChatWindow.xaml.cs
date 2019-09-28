using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Chat;


namespace ClientForms
{
    public partial class ChatWindow : Window
    {
        private Participant participant;
        private List<string> users;

        public ChatWindow()
        {
            InitializeComponent();
        }

        private void BeginReceive(Participant participant)
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        string text = participant.Receive().ToString();
                        Dispatcher.Invoke(() => messageChatBox.Text += text + "\n");
                    }
                    catch (System.IO.IOException)
                    {
                        Dispatcher.Invoke(() => ShowMessageIssues("Server is offline..."));
                    }
                }
            }).Start();
        }

        private void BeginWrite(Participant participant, string text)
        {
            new Thread(() =>
            {
                try
                {
                    if (text == string.Empty)
                    {
                        throw new EmptyMessageException();
                    }
                    Message message = new Message(text);
                    participant.Send(message);
                    Dispatcher.Invoke(() => messageBox.Text = string.Empty);
                }
                catch(EmptyMessageException exception)
                {
                    Dispatcher.Invoke(() => ShowMessageIssues(exception.Message));
                }
            }).Start();
        }

        private void MessageChatBox_Loaded(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            Hide();
            loginWindow.ShowDialog();
            participant = loginWindow.GetParticipant();
            if (participant == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Show();
                BeginReceive(participant);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = messageBox.Text;
            BeginWrite(participant,text);
        }

        private void EnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(sender, e);
        }

        private void ShowMessageIssues(string text)
        {
            messageIssues.Visibility = Visibility;
            messageIssues.Foreground = Brushes.WhiteSmoke;
            messageIssues.Text = text;
        }
    }
}
