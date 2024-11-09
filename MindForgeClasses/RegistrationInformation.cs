namespace MindForgeClasses
{
    public class RegistrationInformation
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public RegistrationInformation(string login, string password, string email)
        {
            Login = login;
            Password = password;
            Email = email;
        }
    }
}
