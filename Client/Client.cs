using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Chat;

namespace Client
{
    public class Client
    {
        private static Participant participant;
        private static string nickname;

        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 8001);
            Console.WriteLine("Connected");

            participant = new Participant(new TCPChannel(client.GetStream()));
            BeginConnect(participant);
        }

        private static void BeginConnect(Participant participant)
        {
            Console.Write("Nickname: ");
            nickname = Console.ReadLine();
            participant.BeginSend(new Message(nickname), () =>
            {
                participant.BeginReceive(OnReceiveConfirmationMessage, OnError);
            }, OnError);
        }

        private static void OnReceiveConfirmationMessage(Message message)
        {
            string confirmationMessage = message.ToString();
            if (confirmationMessage == "Yes")
            {
                participant.Nickname = nickname;
                Console.WriteLine("Accepted");
                Receive(participant);
                BeginWrite(participant);
            }
            else
            {
                BeginConnect(participant);
            }
        }

        private static void OnError(Exception exception)
        {
            if(exception is System.IO.IOException)
            {
                Console.WriteLine("Server offline");
                Console.ReadLine();
            }
        }

        private static void Receive(Participant participant)
        {
            participant.BeginReceive(message =>
            {
                Console.WriteLine(message);
                Receive(participant);
            }, OnError);
        }

        private static void BeginWrite(Participant participant)
        {
            participant.BeginSend(new Message(Console.ReadLine()), () =>
            {
                BeginWrite(participant);
            }, OnError);
        }
    }
}
