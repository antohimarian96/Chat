using Chat;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    public class ParticipantFacts
    {

        [Fact]
        public void Mock_ReadAllMessage()
        {
            var bytes = new byte[10];
            var mock = new MockChannel("ana are", "asdf", "razvan");
            Assert.Equal(7, mock.Read(bytes, 0, 7));
            Assert.Equal(4, mock.Read(bytes, 0, 4));
            Assert.Equal(6, mock.Read(bytes, 0, 6));
        }

        [Fact]
        public void Mock_ReadAPartOfMessage()
        {
            var bytes = new byte[50];
            var mock = new MockChannel("ana are mere", "tenis de camp", "razvan si dani");
            Assert.Equal(3, mock.Read(bytes, 4, 3));
            Assert.Equal(5, mock.Read(bytes, 0, 5));
            Assert.Equal(4, mock.Read(bytes, 10, 4));
        }

        [Fact]
        public void ReceiveOneSimpleParticipant()
        {
            var participant = new Participant(new MockChannel("vlad\0"));
            var receivedMessage = participant.Receive().ToString();
            Assert.Equal("vlad", receivedMessage);
        }

        [Fact]
        public void ReceiveOneExtensiveParticipant()
        {
            var participant = new Participant(new MockChannel("andrei si radu\0"));
            var receivedMessage = participant.Receive().ToString();
            Assert.Equal("andrei si radu", receivedMessage);
        }

        [Fact]
        public void ReceiveManyParticipants()
        {
            var participant = new Participant(new MockChannel("vlad", "maria","radu\0"));
            var receivedMessage = participant.Receive().ToString();
            Assert.Equal("vladmariaradu", receivedMessage);
        }

        [Fact]
        public void ReceiveOnePartOfManyMessage()
        {
            var participant = new Participant(new MockChannel("flaviu si\0 ion", "ut\0"));
            Assert.Equal("flaviu si", participant.Receive().ToString());
            Assert.Equal(" ionut", participant.Receive().ToString());
        }

        [Fact]
        public void ReceiveOnePartOfMessage()
        {
            var participant = new Participant(new MockChannel("flaviu si\0 ion\0dasda\0", "ut\0"));
            Assert.Equal("flaviu si", participant.Receive().ToString());
            Assert.Equal(" ion", participant.Receive().ToString());
            Assert.Equal("dasda", participant.Receive().ToString());
            Assert.Equal("ut", participant.Receive().ToString());
        }

        [Fact]
        public void ReceiveMessageLongerThanBuffer()
        {
            var participant = new Participant(new MockChannel("flaviu aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa\0"));
            Assert.Equal("flaviu aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", participant.Receive().ToString());
        }

        [Fact]
        public void SendOneSimpleMessage()
        {
            var mock = new MockChannel("doi");
            var participant = new Participant(mock);
            participant.Send(new Message("unu"));
            Assert.True(mock.CheckWriteMessage("unu\0"));
        }

        [Fact]
        public void ThrowExceptionWhenCantReceive()
        {
            var participant = new Participant(new MockChannel("flaviu\0"));
            Assert.Equal("flaviu", participant.Receive().ToString());
            Assert.Throws<CantReadException>(() => participant.Receive().ToString());
        }
    }
}
