using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public class MessageGroup
    {
        public string SenderName { get; set; }
        public bool ShowDate { get; set; } = false;
        public bool ShowYear { get; set; } = false;
        public string DateString {  get; set; }
        public ObservableCollection<MessageInformation> Messages { get; set; } = new();
        public MessageGroup(string senderName, MessageInformation message) 
        {
            SenderName = senderName;
            Messages.Add(message);
        }
        public MessageGroup()
        {
        }
    }
}
