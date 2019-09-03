using System;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
    class Program
    {
        static TcpListener listen;
        static Room room = new Room();

        static void Main(string[] args)
        {
            listen = new TcpListener(IPAddress.Parse("127.0.0.1"), 8001);
            DoListen();
        }

        private static void DoListen()
        {
            listen.Start();
            Console.WriteLine("Server: Started server!");
            while (true)
            {
                Console.WriteLine("Server: Waiting...");
                TcpClient client = listen.AcceptTcpClient();
                Console.WriteLine("Server: Waited");
                ConnectClient(client);
            }
        }

        private static void ConnectClient(TcpClient client)
        {
            var participant = new Participant(new TCPChannel(client.GetStream()));
            while (!participant.Joined)
            {
                participant.Nickname = participant.Receive().ToString();
                room.Join(participant);
                if (participant.Joined)
                    participant.Send(new Message("Yes"));
                else
                    participant.Send(new Message("No"));
            }
            Message message = participant.Receive();
            room.Broadcast(message);
        }
    }
}
