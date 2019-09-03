using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

            while (participant.Nickname == null)
            {
                Console.WriteLine("Nickname: ");
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

            Message message = new Message(Console.ReadLine());
            participant.Send(message);

            try
            {
                while (true)
                {
                    Console.WriteLine(participant.Receive());
                }
            }
            catch(System.IO.IOException)
            {
                Console.WriteLine("Server offline");
                Console.ReadLine();
            }
        }
    }
}
