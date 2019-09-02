using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat
{
    public class Room
    {
        private List<Participant> participants;

        public Room()
        {
            participants = new List<Participant>();
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
                catch(System.IO.IOException)
                {
                    Leave(participant);
                }
            }
        }

        public void Join(Participant participant)
        {
            participant.OnLeave = Leave;
            participants.Add(participant);
            participant.Joined = true;
        }

        public void Leave(Participant participant)
        {
            participants.Remove(participant);
        }

    }
}
