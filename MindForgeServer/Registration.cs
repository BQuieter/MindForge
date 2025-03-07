using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using BCrypt.Net;
using System.Diagnostics.CodeAnalysis;
using MindForgeDbClasses;
using MindForgeClasses;

namespace MindForgeServer
{
    public class Registration
    {
        static public async Task<object> Registrate(HttpContext context, MindForgeDbContext db, ITokenService tokenService)
        {
            var registrationInformation = await context.Request.ReadFromJsonAsync<UserLoginInformation>();
            if (registrationInformation == null)
                return Results.BadRequest(new ErrorResponse { ErrorCode = 400, Message = "Некорректный запрос" });
            bool loginNotUnique = await db.Users.AnyAsync(u => u.Login == registrationInformation!.Login);
            if (loginNotUnique)
                return Results.Conflict( new ErrorResponse { ErrorCode = 409,Message = "Логин уже существует"});

            User user = new User { Login = registrationInformation!.Login, Password = registrationInformation.Password, Role = 1, RoleNavigation = db.Roles.Find(1)!};
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            if (!await CreateProfile(db,user))
                return Results.Conflict(new ErrorResponse {ErrorCode = 409,Message = "Профиль не создан. Данный профиль пользовтеля существует"});
           
            return Results.Ok(tokenService.GetJwtToken(user, tokenService));
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
