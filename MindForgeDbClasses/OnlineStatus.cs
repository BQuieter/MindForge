using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class OnlineStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
