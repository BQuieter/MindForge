using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeDbClasses
{
    public partial class ChatUser
    {
        public int User { get; set; }

        public int Chat { get; set; }

        public virtual Chat ChatNavigation { get; set; } = null!;

        public virtual User UserNavigation { get; set; } = null!;
    }
}
