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
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 8001);
            Console.WriteLine("Connected");

            Participant participant = new Participant(new TCPChannel(client.GetStream()));
            ConnectClient(participant);

            BeginWrite(participant);
            BeginReceive(participant);
        }

        private static void ConnectClient(Participant participant)
        {
            while (participant.Nickname == null)
            {
                Console.Write("Nickname: ");
                string nickname = Console.ReadLine();
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

        private static void BeginReceive(Participant participant)
        {
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine(participant.Receive());
                    }
                    catch (System.IO.IOException)
                    {
                        Console.WriteLine("Server offline");
                        Console.ReadLine();
                    }
                }
            }).Start();
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
