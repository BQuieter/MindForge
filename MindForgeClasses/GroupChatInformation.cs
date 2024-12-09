using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public class GroupChatInformation : ChatInformation
    {
        public string Creator { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ProfileInformation> Members { get; set; }
    }
}
