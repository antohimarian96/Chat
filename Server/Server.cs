using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Chat;


namespace Server
{
    class Server
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
            new Thread(() =>
            {
                while (true)
                {

                    participant.Nickname = participant.Receive().ToString();
                    if (room.Join(participant))
                    {
                        Console.WriteLine("--" + participant.Nickname + "--");
                        participant.Send(new Message("Yes"));
                        participant.Send(new Message(room.GetParticipantsNicknames()));
                        break;
                    }
                    participant.Send(new Message("No"));

                }
                BroadcastMessage(participant);
            }).Start();
        }

        private static void BroadcastMessage(Participant participant)
        {
            while (true)
            {
                try
                {
                    Message message = participant.Receive();
                    room.Broadcast(new Message(participant.Nickname + ": " + message));
                }
                catch (Exception exception)
                {
                    if (exception is System.IO.IOException || exception is CantReadException)
                    {
                        room.Broadcast(new Message(participant.Nickname + " left from chat$2019#$"));
                        return;
                    }
                }
            }
        }
    }
}
