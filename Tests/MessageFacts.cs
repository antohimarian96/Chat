using System;
using Xunit;
using Chat;

namespace Tests
{
    public class MessageFacts
    {
        [Fact]
        public void ConstructorWithString()
        {
            var result = new Message("Marius");
            Assert.Equal("Marius", result.ToString());
        }

        [Fact]
        public void ConstructorGetByteArray()
        {
            byte[] bytes = { 97, 110, 97 };
            var message = new Message(bytes);
            Assert.Equal("ana", message.ToString());
        }

        [Fact]
        public void NullMessage()
        {
            byte[] bytes = { };
            var message = new Message(bytes);
            Assert.Equal("", message.ToString());
        }
    }
}
