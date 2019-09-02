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
            room.Join(participant);
            Message message = participant.Receive();
            room.Broadcast(message);
        }
    }
}
