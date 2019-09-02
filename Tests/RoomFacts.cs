﻿using Chat;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests
{
    public class RoomFacts
    {
        [Fact]
        public void BroadcastMessage()
        {
            var firstMock = new MockChannel("ana");
            var secondMock = new MockChannel("are");
            var room = new Room();
            room.Join(new Participant(firstMock));
            room.Join(new Participant(secondMock));
            room.Broadcast(new Message("mere"));
            Assert.True(firstMock.CheckWriteMessage("mere\0"));
            Assert.True(secondMock.CheckWriteMessage("mere\0"));
        }

        [Fact]
        public void LeaveParticipant()
        {
            var firstMock = new MockChannel("dan");
            var secondMock = new MockChannel("mirel");
            var firstParticipant = new Participant(firstMock);
            var secondParticipant = new Participant(secondMock);
            var room = new Room();
            room.Join(firstParticipant);
            room.Join(secondParticipant);
            room.Leave(secondParticipant);
            room.Broadcast(new Message("ion"));
            Assert.True(firstMock.CheckWriteMessage("ion\0"));
            Assert.Throws<InvalidOperationException> (() => secondMock.CheckWriteMessage("ion\0"));
        }

        [Fact]
        public void ParticipantIsRemovedFromRoomWhenHeClosedTheConnection()
        {
            var firstMock = new MockChannel("mare\0","nisip\0");
            var secondMock = new MockChannel("doi\0");
            var firstParticipant = new Participant(firstMock);
            var secondParticipant = new Participant(secondMock);
            var room = new Room();
            room.Join(firstParticipant);
            room.Join(secondParticipant);
            secondParticipant.Receive();
            Assert.Throws<CantReadException>(() => secondParticipant.Receive());
            room.Broadcast(new Message("dan"));
            Assert.Throws<InvalidOperationException> (() => secondMock.CheckWriteMessage("dan\0"));
            Assert.True(firstMock.CheckWriteMessage("dan\0"));
        }
    }
}
