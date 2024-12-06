using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class Profession
{
    public int ProfessionId { get; set; }

    public string ProfessionName { get; set; } = null!;

    public string ProfessionColor { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
