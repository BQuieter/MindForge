using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MindForgeClasses;
using MindForgeDbClasses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MindForgeServer
{
    public class Authorization
    {
        static public async Task<object> Login(HttpContext context, MindForgeDbContext db)
        {
            var authorizationInformation = await context.Request.ReadFromJsonAsync<UserLoginInformation>();
            if (authorizationInformation == null)
                return Results.BadRequest(new ErrorResponse { ErrorCode = 400, Message = "Некорректный запрос" });
            var user = db.Users.FirstOrDefault(u => u.Login == authorizationInformation.Login);
            if (user == null)
                return Results.NotFound(new ErrorResponse { ErrorCode = 404, Message = "Пользователь не найден" });

            if (!BCrypt.Net.BCrypt.Verify(authorizationInformation.Password, user.Password))
                return Results.Unauthorized();

            user.RoleNavigation = db.Roles.Find(user.Role)!;

            Profile profile = db.Profiles.FirstOrDefault(p => p.User == user.UserId)!;
            if (profile == null)
                return Results.NotFound(new ErrorResponse { ErrorCode = 404, Message = "Профиль не найден" });
            profile.Status = 1;
            db.SaveChanges();

            return Results.Ok(JwtTokenHelper.CreateJwtToken(user));
        }

        [Authorize]
        static public object Logout(HttpContext context, MindForgeDbContext db)
        {
            var login = context.User.Identity!.Name;
            var profile = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == login);
            if (profile == null)
                return Results.NotFound(new ErrorResponse { ErrorCode = 404, Message = "Профиль не найден" });
            profile.Status = 2;
            db.SaveChanges();
            return Results.Ok();
        }
    }
}
