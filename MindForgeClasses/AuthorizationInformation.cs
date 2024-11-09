namespace MindForgeClasses
{
    public class AuthorizationInformation
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public AuthorizationInformation(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
