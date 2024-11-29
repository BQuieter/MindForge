using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class Friendship
{
    public int User1 { get; set; }
    public int User2 { get; set; }
    public int Status { get; set; }


    public virtual User User1Navigation { get; set; } = null!;

    public virtual User User2Navigation { get; set; } = null!;
    public virtual FriendshipStatus StatusNavigation { get; set; } = null!;
}
