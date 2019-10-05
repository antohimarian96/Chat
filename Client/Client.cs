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

            BeginWrite(participant);
        }

        private static void ConnectClient(Participant participant)
        {
            while (participant.Nickname == null)
            {
                Console.Write("Nickname: ");
                nickname = Console.ReadLine();
                participant.Send(new Message(nickname));
                string confirmationMessage = participant.Receive().ToString();

                if (confirmationMessage == "Yes")
                {
                    participant.Nickname = nickname;
                    Console.WriteLine("Accepted");
                }
                else
                {
                    Console.WriteLine("Try again");
                }
            }
        }

        private static void BeginConnect(Participant participant)
        {
            Console.Write("Nickname: ");
            nickname = Console.ReadLine();
            participant.Send(new Message(nickname));
            participant.BeginReceive(OnMessage, OnError);
        }

        private static void OnMessage(Message message)
        {
            string confirmationMessage = message.ToString();
            if (confirmationMessage == "Yes")
            {
                participant.Nickname = nickname;
                Console.WriteLine("Accepted");
                Receive(participant);
            }
            else
            {
                BeginConnect(participant);
            }
        }

        private static void OnError(Exception exception)
        {
            throw new System.IO.IOException();
        }

        private static void Receive(Participant participant)
        {
            try
            {
                participant.BeginReceive(message =>
                {
                    Console.WriteLine(message);
                    Receive(participant);
                }, OnError);
            }
            catch (System.IO.IOException)
            {
                Console.WriteLine("Server offline");
                Console.ReadLine();
            }
        }

        private static void BeginWrite(Participant participant)
        {
            new Thread(() =>
            {
                while (true) 
                {
                    Message message = new Message(Console.ReadLine());
                    participant.Send(message);
                }
            }).Start();
        }
    }
}
