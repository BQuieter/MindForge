using System;
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
    public byte[] ChatPhoto { get; set; }    

    public DateTime? ChatCreatedTime { get; set; }

    public virtual ChatType ChatTypeNavigation { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual User? User1 { get; set; }

    public virtual User? User2 { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
}
