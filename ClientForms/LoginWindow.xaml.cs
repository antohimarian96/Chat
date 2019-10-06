using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Chat;

namespace ClientForms
{
    public partial class LoginWindow : Window
    {
        private Participant participant;
        private TcpClient client;
        private string nickname;

        public LoginWindow()
        {
           InitializeComponent();
        }

        public LoginWindow(string nickname) : this()
        {
            this.nickname = nickname;
            if (nickname != null)
                loginText.Text = nickname;
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            try
            {
                if (loginText.Text.Length == 0)
                {
                    throw new EmptyTextException();
                }
                new Thread(() =>
                {
                    try
                    {
                        client = new TcpClient();
                        client.Connect("127.0.0.1", 8001);
                        ConnectClient(client);
                    }
                    catch (SocketException)
                    {
                        Dispatcher.Invoke(() => ConnectingIssuesText("Server is offline"));
                    }
                }).Start();
            }
            catch (EmptyTextException exception)
            {
                Dispatcher.Invoke(() => ConnectingIssuesText(exception.Message));
            }
        }

        private void ConnectClient(TcpClient client)
        {
            participant = new Participant(new TCPChannel(client.GetStream()));
            ConnectParticipant(participant);
        }

        private void ConnectParticipant(Participant participant)
        {
            if (nickname == null)
            {
                Dispatcher.Invoke(() => nickname = loginText.Text);
            }
            BeginTryToConnectParticipant(participant, nickname);
        }

        private void BeginTryToConnectParticipant(Participant participant, string nickname)
        {
            participant.BeginSend(new Message(nickname),()=>
            {
                participant.BeginReceive(OnMessage, exception => { throw exception; });
            },OnError);
        }

        private void OnMessage(Message message)
        {
            try
            {
                string confirmationMessage = message.ToString();
                if (confirmationMessage == "No")
                {
                    Dispatcher.Invoke(() => loginText.Text = default(string));
                    throw new NicknameAlreadyExistException();
                }
                else
                {
                    participant.Nickname = nickname;
                    Dispatcher.Invoke(() => Close());
                }
            }
            catch(Exception exception)
            {
                OnError(exception);
            }
        }

        private void OnError(Exception exception)
        {
            if (exception is NicknameAlreadyExistException)
            {
                Dispatcher.Invoke(() => ConnectingIssuesText(exception.Message));
                nickname = null;
            }
        }

        private void ConnectingIssuesText(string text)
        {
            connectionInformations.Visibility = Visibility;
            connectionInformations.Foreground = Brushes.Gray;
            connectionInformations.Text = text;
        }

        public Participant GetParticipant()
        {
            return participant;
        }

        public TcpClient GetClient()
        {
            return client;
        }

        private void EnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login(sender, e);
        }
    }
}

