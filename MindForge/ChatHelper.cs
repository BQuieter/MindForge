using MindForgeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MindForgeClient
{
    internal class ChatHelper
    {
        public static void AddMessage(ApplicationData applicationData, MessageInformation messageInformation, int chatId)
        {
            if (applicationData.PersonalChats.ContainsKey(chatId))
            {
                applicationData.PersonalChats.TryGetValue(chatId, out ObservableCollection<MessageGroup> chat);
                MessageGroup group = null;
                if (chat.Count- 1 >= 0)
                    group = chat[chat.Count - 1];
                if (group is not null && group.Messages.Count > 0 && group.SenderName == messageInformation.SenderName) 
                {
                    group.Messages.Add(messageInformation);
                }
                else
                {
                    MessageGroup newGroup = new MessageGroup();
                    newGroup.SenderName = messageInformation.SenderName;
                    chat.Add(newGroup);
                    newGroup.Messages.Add(messageInformation);
                }
            }
            else
            {
                ObservableCollection<MessageGroup> chat = new ObservableCollection<MessageGroup>() { new MessageGroup(messageInformation.SenderName, messageInformation) };
                applicationData.PersonalChats.Add(chatId, chat);
            }
        }
    }
}
