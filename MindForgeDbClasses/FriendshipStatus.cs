using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class FriendshipStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();
}
