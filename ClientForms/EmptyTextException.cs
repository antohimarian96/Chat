using System;
using System.Runtime.Serialization;

namespace ClientForms
{
    [Serializable]
    internal class EmptyTextException : Exception
    {
        public EmptyTextException() : base("Please introduce a valid nickname") { }
    }
}