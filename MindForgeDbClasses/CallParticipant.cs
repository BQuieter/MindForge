using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class CallParticipant
{
    public int ParticipantId { get; set; }

    public int CallId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinTime { get; set; }

    public DateTime? LeftTime { get; set; }

    public virtual Call Call { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
