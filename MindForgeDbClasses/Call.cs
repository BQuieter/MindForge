using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class Call
{
    public int CallId { get; set; }

    public int ChatId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<CallParticipant> CallParticipants { get; set; } = new List<CallParticipant>();

    public virtual Chat Chat { get; set; } = null!;

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
