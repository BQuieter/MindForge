using Microsoft.IdentityModel.Tokens;
using MindForgeClasses;
using MindForgeDbClasses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MindForgeServer
{
    public interface ITokenService
    {
        public JwtTokenResponse GetJwtToken(User user, ITokenService tokenService);
    }

    public class TokenService : ITokenService
    {
        public JwtTokenResponse GetJwtToken(User user, ITokenService tokenService)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.RoleNavigation.RoleName ?? "1") };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtTokenResponse { Token = new JwtSecurityTokenHandler().WriteToken(jwt) };
        }
    }
}
// return   new JwtSecurityTokenHandler().WriteToken(jwt); };