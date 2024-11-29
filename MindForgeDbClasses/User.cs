using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<UsersProfession> UsersProfessions { get; set; } = new List<UsersProfession>();
    public virtual ICollection<Friendship> Friendships1 { get; set; } = new List<Friendship>();
    public virtual ICollection<Friendship> Friendships2 { get; set; } = new List<Friendship>();
}
