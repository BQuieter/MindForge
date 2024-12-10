using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public class MessageSentEventArgs : EventArgs
    {
        public MessageInformation Message { get; set; }
        public int Index { get; set; }
        public bool IsGroup = false;

        public MessageSentEventArgs(MessageInformation message, int index, bool isGroup = false)
        {
            Message = message;
            Index = index;
            IsGroup = isGroup;
        }
    }

    public class MemberAddEventArgs : EventArgs
    {
        public List<ProfileInformation> Users { get; set; }
        public int ChatId { get; set; }

        public MemberAddEventArgs(List<ProfileInformation> users, int chatID)
        {
            Users = users;
            ChatId = chatID;
        }
    }

    public class MemberEventArgs : EventArgs
    {
        public ProfileInformation User { get; set; }
        public int ChatId { get; set; }

        public MemberEventArgs(ProfileInformation user, int chatID)
        {
            User = user;
            ChatId = chatID;
        }
    }
}
