using System;
using System.Runtime.Serialization;

namespace Chat
{
    [Serializable]
    public class CantReadException : Exception
    {
        public CantReadException()
        {
        }

        public CantReadException(string message) : base(message)
        {
        }

        public CantReadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CantReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}