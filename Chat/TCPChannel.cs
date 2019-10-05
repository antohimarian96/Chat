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

        public IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            return stream.BeginRead(buffer, offset, size, callback, state);
        }

        public IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
        {
            return stream.BeginWrite(buffer, offset, size, callback, state);
        }

        public int EndRead(IAsyncResult asyncResult)
        {
            return stream.EndRead(asyncResult);
        }

        public void EndWrite(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
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
