﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace MindForgeDbClasses;

public partial class Chat
{
    public int ChatId { get; set; }

    public int ChatType { get; set; }

    public int? User1Id { get; set; }

    public int? User2Id { get; set; }

    public string? ChatName { get; set; }

    public int? CallId { get; set; }
    public virtual Call? Call { get; set; }

    public virtual List<Call> Calls { get; set; } = new List<Call>();

    public DateTime? ChatCreatedTime { get; set; }

    public byte[]? ChatPhoto { get; set; }

    public virtual ChatType ChatTypeNavigation { get; set; } = null!;

    public virtual List<Message> Messages { get; set; } = new List<Message>();

    public virtual User? User1 { get; set; }

    public virtual User? User2 { get; set; }

    public virtual List<User> Users { get; set; } = new List<User>();
}
