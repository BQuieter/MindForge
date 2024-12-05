using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public class GroupChatInformation : ChatInformation
    {
        public string Name { get; set; }
        public List<ProfileInformation> Members { get; set; }
    }
}
