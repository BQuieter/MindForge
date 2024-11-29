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

    public virtual DbSet<OnlineStatus> OnlineStatuses { get; set; }

    public virtual DbSet<Profession> Professions { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersProfession> UsersProfessions { get; set; }
    public virtual DbSet<FriendshipStatus> FriendshipStatuses { get; set; }
    public virtual DbSet<Friendship> Friendships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
        });

        modelBuilder.Entity<UsersProfession>(entity =>
        {
            entity.ToTable("Users_professions");
            entity.HasKey(e => e.User).HasName("PK_Users_professions");
            entity.HasKey(e => e.Profession).HasName("PK_Users_professions");

            entity.Property(e => e.Profession).HasColumnName("profession");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.ProfessionNavigation).WithMany(p => p.UsersProfessions)
                .HasForeignKey(d => d.Profession)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users_pro__profe__4E88ABD4");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.UsersProfessions)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users_prof__user__4D94879B");
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

            entity.HasOne(d => d.User1Navigation).WithMany(p => p.Friendships1)
                .HasForeignKey(d => d.User1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Friendshi__user___03F0984C");

            entity.HasOne(d => d.User2Navigation).WithMany(p => p.Friendships2)
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
