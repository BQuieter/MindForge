using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MindForgeDbClasses;

namespace MindForgeServer;

public partial class MindForgeDbContext : DbContext
{
    public MindForgeDbContext()
    {
    }

    public MindForgeDbContext(DbContextOptions<MindForgeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ChatType> ChatTypes { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<FriendshipStatus> FriendshipStatuses { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<OnlineStatus> OnlineStatuses { get; set; }
    //public virtual DbSet<UsersProfession> UsersProfessions { get; set; }

    public virtual DbSet<Profession> Professions { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("PK__Chats__FD040B17A5880278");

            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.ChatCreatedTime).HasColumnName("chat_created_time");
            entity.Property(e => e.ChatName)
                .HasMaxLength(50)
                .HasColumnName("chat_name");
            entity.Property(e => e.ChatPhoto).HasColumnName("chat_photo");
            entity.Property(e => e.ChatType).HasColumnName("chat_type");
            entity.Property(e => e.User1Id).HasColumnName("user1_id");
            entity.Property(e => e.User2Id).HasColumnName("user2_id");

            entity.HasOne(d => d.ChatTypeNavigation).WithMany(p => p.Chats)
                .HasForeignKey(d => d.ChatType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Chats__chat_type__17F790F9");

            entity.HasOne(d => d.User1).WithMany(p => p.ChatUser1s)
                .HasForeignKey(d => d.User1Id)
                .HasConstraintName("FK__Chats__user1_id__18EBB532");

            entity.HasOne(d => d.User2).WithMany(p => p.ChatUser2s)
                .HasForeignKey(d => d.User2Id)
                .HasConstraintName("FK__Chats__user2_id__19DFD96B");

            entity.HasMany(d => d.Users).WithMany(p => p.Chats)
                .UsingEntity<Dictionary<string, object>>(
                    "ChatUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ChatUsers__user___245D67DE"),
                    l => l.HasOne<Chat>().WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ChatUsers__chat___236943A5"),
                    j =>
                    {
                        j.HasKey("ChatId", "UserId").HasName("PK__ChatUser__169FE86788C228B1");
                        j.ToTable("ChatUsers");
                        j.IndexerProperty<int>("ChatId").HasColumnName("chat_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });
        modelBuilder.Entity<ChatType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__ChatType__2C000598E6C5F2FD");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(20)
                .HasColumnName("type_name");
        });
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__0BBF6EE6E79068C2");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.ChatId).HasColumnName("chat_id");
            entity.Property(e => e.FileData).HasColumnName("file_data");
            entity.Property(e => e.TimeSent).HasColumnName("time_sent");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FileType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("file_type");
            entity.Property(e => e.MessageText).HasColumnName("messageText");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__chat_i__1CBC4616");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__sender__1DB06A4F");
        });
        modelBuilder.Entity<OnlineStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__OnlineSt__3683B531DE4634E5");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Profession>(entity =>
        {
            entity.HasKey(e => e.ProfessionId).HasName("PK__Professi__21EE4CA282F5A188");

            entity.Property(e => e.ProfessionId).HasColumnName("profession_id");
            entity.Property(e => e.ProfessionName)
                .HasMaxLength(50)
                .HasColumnName("profession_name");
            entity.Property(e => e.ProfessionColor)
                .HasMaxLength(7)
                .HasColumnName("profession_color");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__Profiles__AEBB701FCF528668");

            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.ProfileDescription)
                .HasMaxLength(200)
                .HasColumnName("profile_description");
            entity.Property(e => e.ProfilePhoto).HasColumnName("profile_photo");
            entity.Property(e => e.ProfileRegistrationDate).HasColumnName("profile_registration_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Profiles__status__52593CB8");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Profiles__user__5165187F");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Login)
                .HasMaxLength(30)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");

            entity.HasMany(d => d.Professions).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersProfession",
                    r => r.HasOne<Profession>().WithMany()
                        .HasForeignKey("Profession")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Users_pro__profe__4E88ABD4"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("User")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Users_prof__user__4D94879B"),
                    j =>
                    {
                        j.HasKey("User", "Profession").HasName("PK_Users_professions");
                        j.ToTable("UsersProfessions");
                        j.IndexerProperty<int>("User").HasColumnName("user");
                        j.IndexerProperty<int>("Profession").HasColumnName("profession");
                    });
        });


        modelBuilder.Entity<FriendshipStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Friendsh__3683B531F03FC31D");

            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendships");
            entity.HasKey(e => e.User1).HasName("PK_Friendships");
            entity.HasKey(e => e.User2).HasName("PK_Friendships");

            entity.Property(e => e.User1).HasColumnName("user_1");
            entity.Property(e => e.User2).HasColumnName("user_2");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.User1Navigation).WithMany(p => p.FriendshipUser1Navigations)
                .HasForeignKey(d => d.User1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friendshi__user___03F0984C");

            entity.HasOne(d => d.User2Navigation).WithMany(p => p.FriendshipUser2Navigations)
                .HasForeignKey(d => d.User2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friendshi__user___04E4BC85");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Friendships)
                .HasForeignKey(d => d.User2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friendshi__statu__05D8E0BE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
