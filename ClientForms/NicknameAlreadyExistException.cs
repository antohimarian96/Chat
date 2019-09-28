using System;
using System.Runtime.Serialization;

namespace ClientForms
{
    [Serializable]
    internal class NicknameAlreadyExistException : Exception
    {
        public NicknameAlreadyExistException() : base("Nickname already exists, try again") { }
    }
}