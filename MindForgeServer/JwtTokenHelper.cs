using Microsoft.IdentityModel.Tokens;
using MindForgeClasses;
using MindForgeDbClasses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MindForgeServer
{
    public class JwtTokenHelper
    {
        static internal JwtTokenResponse CreateJwtToken(User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.RoleNavigation.RoleName ?? "1") };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(60)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtTokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(jwt) };
        }
    }
}
