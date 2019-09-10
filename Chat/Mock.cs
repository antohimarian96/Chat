using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat
{
    public class MockChannel : IChannel
    {
        private readonly Queue<string> readList;
        private readonly Queue<string> writeList;

        public MockChannel(params string[] readList)
        {
            this.readList = new Queue<string>(readList);
            writeList = new Queue<string>();
        }

        public int Read(byte[] buffer, int start, int receiveBufferSize)
        {
            string next;
            try
            {
                next = readList.Dequeue();
            }
            catch (InvalidOperationException)
            {
                return 0;
            }
            Encoding.Default.GetBytes(next).ToArray().CopyTo(buffer, start);
            return receiveBufferSize < next.Length ? receiveBufferSize : next.Length;
        }

        public void Write(byte[] buffer, int start, int bufferLength)
        {
            writeList.Enqueue(Encoding.Default.GetString(buffer,start,bufferLength));
        }

        public bool CheckWriteMessage(string message)
        {
           return message == writeList.Dequeue();
        }
    }
}
