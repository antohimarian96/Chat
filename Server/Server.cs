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
        static Participant participant;

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
                BeginConnectClient(client);
            }
        }

        private static void BeginConnectClient(TcpClient client)
        {
            participant = new Participant(new TCPChannel(client.GetStream()));
            TryToReceive(participant);
        }

        private static void TryToReceive(Participant participant)
        {
            participant.BeginReceive(OnReceiveMessage, OnError);
        }

        private static void OnReceiveMessage(Message message)
        {
            participant.Nickname = message.ToString();
            if (room.Join(participant))
            {
                Console.WriteLine("--" + participant.Nickname + "--");
                participant.BeginSend(new Message("Yes"), () =>
                {
                    participant.BeginSend(new Message(room.GetParticipantsNicknames()), () =>
                    {
                        BroadcastMessage(participant);
                    }, OnError);
                }, OnError);
            }
            else
            {
                participant.BeginSend(new Message("No"), () =>
                {
                    TryToReceive(participant);
                }, OnError);
            }
        }

        private static void BroadcastMessage(Participant participant)
        {
            participant.BeginReceive(messageReceived =>
            {
                room.Broadcast(new Message(participant.Nickname + ": " + messageReceived));
                BroadcastMessage(participant);
            }, OnError);
        }

        private static void OnError(Exception exception)
        {
            if (exception is System.IO.IOException || exception is CantReadException)
            {
                room.Broadcast(new Message(participant.Nickname + " left from chat$2019#$"));
                return;
            }
        }
    }
}
