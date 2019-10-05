using System;
using System.Collections.Generic;
using System.Text;

namespace Chat
{
    public interface IChannel
    {
        int Read(byte[] buffer, int start, int receiveBufferSize);
        void Write(byte[] buffer, int start, int bufferLength);
        IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state);
        IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state);
        int EndRead(IAsyncResult asyncResult);
        void EndWrite(IAsyncResult asyncResult);
    }
}
