using System;
using System.Collections.Generic;
using System.Text;

namespace Chat
{
    public interface IChannel
    {
        int Read(byte[] buffer, int start, int receiveBufferSize);
        void Write(byte[] buffer, int start, int bufferLength);
    }
}
