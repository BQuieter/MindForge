using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using MindForgeClasses;

using System.Runtime.CompilerServices;
using BCrypt.Net;
using System.Diagnostics.CodeAnalysis;

namespace MindForgeServer
{
    public class InitialHelper
    {
        static public async Task<object> Login(HttpContext context, MindForgeDbContext db)
        {
            var authorizationInformation = await context.Request.ReadFromJsonAsync<UserLoginInformation>();
            if (authorizationInformation == null)
                return Results.BadRequest(new { message = "Некорректный запрос" });
            var user = db.Users.ToList().FirstOrDefault(u => u.Login == authorizationInformation.Login);
            if (user == null)
                return Results.NotFound(new { message = "Пользователь не найден"});

            if (!BCrypt.Net.BCrypt.Verify(authorizationInformation.Password, user.Password))
                return Results.Unauthorized();

            user.RoleNavigation = db.Roles.Find(user.Role)!;

            return Results.Ok(new { message = CreateJwtToken(user) });
        }

        static public async Task<object> Registration(HttpContext context, MindForgeDbContext db)
        {
            var registrationInformation = await context.Request.ReadFromJsonAsync<UserLoginInformation>();
            if (registrationInformation == null)
                return Results.BadRequest(new { message = "Некорректный запрос"});
            bool loginNotUnique = await db.Users.AnyAsync(u => u.Login == registrationInformation!.Login);
            if (loginNotUnique)
                return Results.Conflict( new { message = "Логин уже существует"});

            User user = new User { Login = registrationInformation!.Login, Password = registrationInformation.Password, Role = 1, RoleNavigation = db.Roles.Find(1)!};
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            if (!await CreateProfile(db,user))
                return Results.Conflict(new {message = "Профиль не создан. Данный профиль пользовтеля существует"});
            return Results.Ok(new { message = CreateJwtToken(user)});
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

        static private async Task<bool> CreateProfile(MindForgeDbContext db, User user)
        {
            if (db.Profiles.FirstOrDefault(p => p.User == user.UserId) is not null)
                return false;

            Profile profile = new();
            profile.User = user.UserId;
            profile.Status = 1;
            profile.ProfileRegistrationDate = DateOnly.FromDateTime(DateTime.Now); ;
            db.Profiles.Add(profile);
            await db.SaveChangesAsync();
            return true;
        }
    }
}
