using System;
using System.Collections.Generic;

namespace MindForgeClasses;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<UsersProfession> UsersProfessions { get; set; } = new List<UsersProfession>();
}
