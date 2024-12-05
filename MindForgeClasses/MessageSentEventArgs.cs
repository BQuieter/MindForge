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

        public MessageSentEventArgs(MessageInformation message, int index)
        {
            Message = message;
            Index = index;
        }
    }
}
