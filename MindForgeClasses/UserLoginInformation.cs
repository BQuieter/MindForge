namespace MindForgeClasses
{
    public class UserLoginInformation
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public UserLoginInformation(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
