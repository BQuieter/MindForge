using System;
using System.Collections.Generic;

namespace MindForgeDbClasses;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public virtual ICollection<Chat> ChatUser1s { get; set; } = new List<Chat>();

    public virtual ICollection<Chat> ChatUser2s { get; set; } = new List<Chat>();

    public virtual ICollection<Friendship> FriendshipUser1Navigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipUser2Navigations { get; set; } = new List<Friendship>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Profession> Professions { get; set; } = new List<Profession>();
}
