using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class UsersProfession
{
    public int User { get; set; }

    public int Profession { get; set; }

    public virtual Profession ProfessionNavigation { get; set; } = null!;

    public virtual User UserNavigation { get; set; } = null!;
}
