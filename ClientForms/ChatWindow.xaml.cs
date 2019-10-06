using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
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
        private List<string> onlineParticipants = new List<string>();
        private TcpClient client;

        public ChatWindow()
        {
            InitializeComponent();
        }

        private void Receive(Participant participant)
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        string text = participant.Receive().ToString();
                        if (CheckOnlineParticipants(text))
                            UpdateOnlineParticipants();
                        text = CheckSpecialMessage(text);
                        Dispatcher.Invoke(() =>
                        {
                            messageChatBox.Text += text + "\n";
                            messageIssues.Text = string.Empty;
                        });
                    }
                    catch (System.IO.IOException)
                    {
                        LoginAgain(participant);
                    }
                    catch (ObjectDisposedException) { return; }
                }
            }).Start();
        }

        private void BeginReceive(Participant participant)
        {
            participant.BeginReceive(value =>
            {
                string text = value.ToString();
                if (CheckOnlineParticipants(text))
                    UpdateOnlineParticipants();
                text = CheckSpecialMessage(text);
                Dispatcher.Invoke(() =>
                {
                    messageChatBox.Text += text + "\n";
                    messageIssues.Text = string.Empty;
                });
                BeginReceive(participant);
            }, OnErrorMessageBox);
        }

        private void OnErrorMessageBox(Exception exception)
        {
            if (exception is System.IO.IOException)
            {
                LoginAgain(participant);
            }
            if (exception is ObjectDisposedException)
            {
                return;
            }
        }

        private void LoginAgain(Participant participant)
        {
            Dispatcher.Invoke(() =>
            {
                string nickname = participant.Nickname;
                client.Close();
                TryToLogin(nickname);
            });
        }

        private static string CheckSpecialMessage(string text)
        {
            return text.EndsWith("$2019#$") ? text.Substring(0, text.Length - 7) : text;
        }

        private void UpdateOnlineParticipants()
        {
            string result = null;
            foreach (var participant in onlineParticipants)
            {
                result += participant + "\n";
            }
            Dispatcher.Invoke(() => participantsBox.Text = result);
        }

        private bool CheckOnlineParticipants(string text)
        {
            return CheckIfSomeoneHasJoined(text) || CheckIfSomeoneHasLeave(text) ? true : false;
        }

        private bool CheckIfSomeoneHasLeave(string text)
        {
            if (text.EndsWith(" left from chat$2019#$"))
            {
                text = text.Substring(0, text.Length - 22);
                onlineParticipants.Remove(text);
                return true;
            }
            return false;
        }

        private bool CheckIfSomeoneHasJoined(string text)
        {
            if (text.EndsWith(" has joined$2019#$"))
            {
                text = text.Substring(0, text.Length - 18);
                onlineParticipants.Add(text);
                return true;
            }
            return false;
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
                catch (EmptyMessageException exception)
                {
                    Dispatcher.Invoke(() => ShowMessageIssues(exception.Message));
                }
            }).Start();
        }

        private void MessageChatBox_Loaded(object sender, RoutedEventArgs e)
        {
            TryToLogin(default(string));
        }

        private void TryToLogin(string nickname)
        {
            var loginWindow = new LoginWindow(nickname);
            Hide();
            loginWindow.ShowDialog();
            participant = loginWindow.GetParticipant();
            client = loginWindow.GetClient();
            if (participant == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                participant.BeginReceive(OnParticipants, OnError);
            }
        }

        private void OnError(Exception exception)
        {
            if (exception is ObjectDisposedException)
            {
                return;
            }
        }

        private void OnParticipants(Message message)
        {
            string receiveServerMessage = message.ToString();
            string[] nicknames = receiveServerMessage.Split('\n');
            foreach (var nickname in nicknames)
            {
                if (nickname != "")
                    onlineParticipants.Add(nickname);
            }
            Dispatcher.Invoke(() =>
            {
                participantsBox.Text = receiveServerMessage.TrimEnd('\n');
                Show();
            });
            BeginReceive(participant);
        }


        private void GetOnlineParticipants()
        {
            new Thread(() =>
            {
                string receiveServerMessage = participant.Receive().ToString();
                string[] nicknames = receiveServerMessage.Split('\n');
                foreach (var nickname in nicknames)
                {
                    if (nickname != "")
                        onlineParticipants.Add(nickname);
                }
                Dispatcher.Invoke(() => participantsBox.Text = receiveServerMessage.TrimEnd('\n'));
            }).Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = messageBox.Text;
            BeginWrite(participant, text);
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                client.Close();
            }
            catch (NullReferenceException)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
