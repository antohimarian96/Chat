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

        public void BeginReceive(Action<Message> onMessage, Action<Exception> onError)
        {
            int end;

            if ((end = GetEndIndex()) >= 0)
            {
                onMessage(GetMessage(end));
                return;
            }

            EnsureReadBufferCapacity();
            channel.BeginRead(readBuffer, readCount ,readBuffer.Length-readCount, OnReadCompleteWithErrorHandling, null);

            void OnReadCompleteWithErrorHandling(IAsyncResult asyncResult)
            {
                try
                {
                    OnReadComplete(asyncResult);
                }
                catch(System.IO.IOException exception)
                {
                    onError(exception);
                }
            }

            void OnReadComplete(IAsyncResult asyncResult)
            {
                var count = channel.EndRead(asyncResult);
                if (count == 0)
                {
                    OnLeave?.Invoke(this);
                    onError(new CantReadException());
                    return;
                }
                readCount += count;
                BeginReceive(onMessage, onError);
            }
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
