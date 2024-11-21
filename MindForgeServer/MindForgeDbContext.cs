using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MindForgeClasses;

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
            entity.HasKey(e => e.UsersProfessionId).HasName("PK__Users_pr__A2F31E62B5309F5C");

            entity.ToTable("Users_professions");

            entity.Property(e => e.UsersProfessionId).HasColumnName("Users_profession_id");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
