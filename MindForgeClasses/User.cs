namespace MindForgeClasses
{
    public partial class User
    {
        public int UserId { get; set; }

        public string Login { get; set; } = null!;

        public string? Email { get; set; }

        public string Password { get; set; } = null!;

        public int Role { get; set; }

        public virtual Role RoleNavigation { get; set; } = null!;
    }
}