using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public class TCPChannel : IChannel
    {
        private readonly NetworkStream stream;

        public TCPChannel(NetworkStream stream)
        {
            this.stream = stream;
        }

        public int Read(byte[] buffer, int start, int size)
        {
            return stream.Read(buffer, start, size);
        }

        public void Write(byte[] buffer, int start, int size)
        {
            stream.Write(buffer, start, size);
        }
    }
}
