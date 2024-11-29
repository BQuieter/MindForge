using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class Profile
{
    public int ProfileId { get; set; }

    public int User { get; set; }

    public byte[]? ProfilePhoto { get; set; }

    public string? ProfileDescription { get; set; }

    public DateOnly ProfileRegistrationDate { get; set; }

    public int Status { get; set; }

    public virtual OnlineStatus StatusNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
