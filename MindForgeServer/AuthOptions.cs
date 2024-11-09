using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MindForgeServer
{
    public class AuthOptions
    {
        public const string ISSUER = "MindForgeServer"; // издатель токена
        public const string AUDIENCE = "MindForgeClient"; // потребитель токена
        const string KEY = "NYjPUxW18yaLSJ0PwNic9NzhvY8E0eVr";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
