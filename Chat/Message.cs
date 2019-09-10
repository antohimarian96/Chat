using System;
using System.Collections.Generic;
using System.Text;

namespace Chat
{
    public class Message
    {
        private readonly string text;

        public Message(byte[] message, int start, int count): this(Encoding.Default.GetString(message, start, count)) { }
        public Message(byte[] message): this(message, 0, message.Length) { }

        public Message(string message)
        {
            text = message;
        }

        public byte[] ToByte()
        {

            return Encoding.Default.GetBytes(text.EndsWith("\0") ? text : text + "\0");
        }

        public override string ToString()
        {
            return text;
        }
    }
}
