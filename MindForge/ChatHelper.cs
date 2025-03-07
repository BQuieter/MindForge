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
        public static void AddMessage(Dictionary<int, ObservableCollection<MessageGroup>> chats, MessageInformation messageInformation, int chatId)
        {
            if (chats.ContainsKey(chatId))
            {
                chats.TryGetValue(chatId, out ObservableCollection<MessageGroup> chat);
                MessageGroup group = null;
                if (chat.Count- 1 >= 0)
                    group = chat[chat.Count - 1];
                if (group is null || group.Messages.Count <= 0)
                {
                    CreateMessageGroup(messageInformation, chat, true, true);
                    return;
                }
                MessageInformation lastMessage = group.Messages.Last();

                if (lastMessage.Year != messageInformation.Year)
                {
                    CreateMessageGroup(messageInformation, chat, true, true);
                    return;
                }
                if (lastMessage.Month != messageInformation.Month || lastMessage.Date != messageInformation.Date)
                {
                    CreateMessageGroup(messageInformation, chat, true);
                    return;
                }
                if (lastMessage.SenderName != messageInformation.SenderName)
                {
                    CreateMessageGroup(messageInformation, chat);
                    return;
                }

               group.Messages.Add(messageInformation);
            }
            else
            {
                ObservableCollection<MessageGroup> chat = new ObservableCollection<MessageGroup>() { new MessageGroup(messageInformation.SenderName, messageInformation) };
                chats.Add(chatId, chat);
            }
        }

        private static void CreateMessageGroup(MessageInformation message, ObservableCollection<MessageGroup> chat, bool showDate = false, bool showYear = false)
        {
            MessageGroup newGroup = new MessageGroup();
            newGroup.ShowDate = showDate;
            newGroup.ShowYear = showYear;
            newGroup.SenderName = message.SenderName;
            if (showDate)
                newGroup.DateString = $"{message.Date} {Enums.Months[message.Month]}";
            if (showYear)
                newGroup.DateString = $"{message.Year}, {message.Date} {Enums.Months[message.Month]}";
            chat.Add(newGroup);
            newGroup.Messages.Add(message);
        }
    }
}
