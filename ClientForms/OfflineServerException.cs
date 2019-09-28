using System;
using System.Runtime.Serialization;

namespace ClientForms
{
    [Serializable]
    internal class OfflineServerException : Exception
    {
        public OfflineServerException() : base("Server offline...") { }
    }
}