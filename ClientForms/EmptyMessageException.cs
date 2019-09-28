using System;
using System.Runtime.Serialization;

namespace ClientForms
{
    [Serializable]
    internal class EmptyMessageException : Exception
    {
        public EmptyMessageException() : base("Your message is empty, try again") { }
    }
}