using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat
{
    public class Participant
    {
        private readonly IChannel channel;
        private int readCount = 0;
        private byte[] readBuffer = new byte[255];
        

        public Participant(IChannel channel)
        {
            this.channel = channel;
        }

        public string Nickname { get; set; }
        public Action<Participant> OnLeave { get; set; }

        public void Send(Message message)
        {
            byte[] buffer = message.ToByte();
            channel.Write(buffer, 0, buffer.Length);
        }

        public Message Receive()
        {
            int end;

            while ((end = GetEndIndex()) < 0)
            {
                EnsureReadBufferCapacity();
                int count = channel.Read(readBuffer, readCount, readBuffer.Length - readCount);
                if (count == 0)
                {
                    OnLeave?.Invoke(this);
                    throw new CantReadException();
                }
                readCount += count;
            }
            return GetMessage(end);
        }

        private void EnsureReadBufferCapacity()
        {
            if (readBuffer.Length == readCount)
            {
                Array.Resize(ref readBuffer, readBuffer.Length * 2);
            }
        }

        private Message GetMessage(int index)
        {
            var result = new Message(readBuffer, 0, index);
            readBuffer = readBuffer.Skip(index + 1).ToArray();
            readCount -= index + 1;
            return result;
        }

        private int GetEndIndex()
        {
            return Encoding.Default.GetString(readBuffer, 0, readCount).IndexOf("\0");
        }
    }
}
