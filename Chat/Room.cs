using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat
{
    public class Room
    {
        private SafeList<Participant> participants;
        private readonly object lockObject = new object();

        public Room()
        {
            participants = new SafeList<Participant>();
        }

        public void Broadcast(Message message)
        {
            var list = new List<Participant>(participants);
            foreach (var participant in list)
            {
                try
                {
                    participant.Send(message);
                }
                catch (System.IO.IOException)
                {
                    Leave(participant);
                }
            }
        }

        public bool Join(Participant participant)
        {
            if (CheckNickname(participant.Nickname))
            {
                Broadcast(new Message(participant.Nickname + " has joined$2019#$"));
                participant.OnLeave = Leave;
                participants.Add(participant);
                return true;
            }
            return false;
        }

        public void Leave(Participant participant)
        {
             participants.Remove(participant);
        }

        private bool CheckNickname(string nickname)
        {
            foreach(var participant in participants)
            {
                if (participant.Nickname == nickname)
                    return false;
            }
            return true;
        }

        public string GetParticipantsNicknames()
        {
            string result = null;
            foreach(var participant in participants)
            {
                result += participant.Nickname + "\n";
            }
            return result;
        }
    }
}
