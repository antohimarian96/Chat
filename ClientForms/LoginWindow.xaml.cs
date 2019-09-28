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

        public LoginWindow()
        {
            InitializeComponent();
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
            new Thread(() =>
            {
                try
                {
                    string nickname = null;
                    Dispatcher.Invoke(() => nickname = loginText.Text);
                    TryToConnectParticipant(participant, nickname);
                }
                catch (NicknameAlreadyExistException exception)
                {
                    Dispatcher.Invoke(() => ConnectingIssuesText(exception.Message));
                }
            }).Start();
        }

        private void TryToConnectParticipant(Participant participant, string nickname)
        {
            participant.Send(new Message(nickname));
            string confirmationMessage = participant.Receive().ToString();
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

        private void EnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Login(sender, e);
        }
    }
}

