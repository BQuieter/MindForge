using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using MindForgeClasses;

using System.Runtime.CompilerServices;
using BCrypt.Net;

namespace MindForgeServer
{
    public class InitialHelper
    {
        static public async Task<object> Login(HttpContext context, MindForgeDbContext db)
        {
            Console.WriteLine("a");
            var authorizationInformation = await context.Request.ReadFromJsonAsync<AuthorizationInformation>();
            if (authorizationInformation == null)
                return Results.BadRequest("Bad Request");
            var user = db.Users.ToList().FirstOrDefault(u => u.Login == authorizationInformation!.Login && BCrypt.Net.BCrypt.Verify(authorizationInformation.Password, u.Password, false, HashType.SHA384) );
            if (user == null)
                return Results.NotFound("User not found");
            user.RoleNavigation = db.Roles.Find(user.Role)!;

            Results.Ok();
            return CreateJwtToken(user);
        }

        static public async Task<object> Registration(HttpContext context, MindForgeDbContext db)
        {
            var registrationInformation = await context.Request.ReadFromJsonAsync<RegistrationInformation>();
            if (registrationInformation == null)
                return Results.BadRequest("Bad Request");
            bool loginNotUnique = await db.Users.AnyAsync(u => u.Login == registrationInformation!.Login);
            bool emailNotUnique = await db.Users.AnyAsync(u => u.Email == registrationInformation!.Email);
            if (loginNotUnique && emailNotUnique)
                return Results.Conflict("Login and email already exists");
            if (loginNotUnique)
                return Results.Conflict("Login already exists");
            if (emailNotUnique)
                return Results.Conflict("Email already exists");

            User user = new User { Login = registrationInformation!.Login, Password = registrationInformation.Password, Email = registrationInformation.Email, Role = 1, RoleNavigation = db.Roles.Find(1)!};
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            Results.Ok();
            return CreateJwtToken(user);
        }

        static private string CreateJwtToken(User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login), new Claim(ClaimTypes.Role, user.RoleNavigation.RoleName ?? "1") };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
