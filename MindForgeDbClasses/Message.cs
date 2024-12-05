using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class Message
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int SenderId { get; set; }

    public string? MessageText { get; set; }

    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public byte[]? FileData { get; set; }
    public DateTime TimeSent { get; set; }
    public virtual Chat Chat { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
