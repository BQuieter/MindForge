using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using MindForgeClasses;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using MindForgeDbClasses;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.VisualBasic;


namespace MindForgeServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.AddDbContext<MindForgeDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault();

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = accessToken.Substring("Bearer ".Length).Trim();
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            builder.Services.AddAuthorization();
            var app = builder.Build();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapPost("/registration", Registration.Registrate);

            app.MapPost("/login", Authorization.Login);

            app.MapPost("/logout", Authorization.Logout);
            //����� ������ ������ � ������ ���� ������

            app.MapGet("/profile/{user?}", [Authorize] (string? user, MindForgeDbContext db, HttpContext context) =>
            {
                if (user is null)
                    user = context.User.Identity!.Name!;
                var profile = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == user);
                if (profile == null)
                    return Results.NotFound(new ErrorResponse {
                        ErrorCode = 404,
                        Message = "������� ������������ �� ������"
                    });

                return Results.Ok(new ProfileInformation {
                    Login = profile.UserNavigation.Login,
                    Description = profile.ProfileDescription!,
                    ImageByte = profile.ProfilePhoto! });
            });

            app.MapPut("/profile", [Authorize] async (MindForgeDbContext db, HttpContext context) =>
            {
                ProfileInformation profileInformation = await context.Request.ReadFromJsonAsync<ProfileInformation>();
                if (profileInformation == null)
                    return Results.BadRequest(new ErrorResponse { ErrorCode = 400, Message = "������������ ������" });

                string login = context.User.Identity!.Name!;
                var profile = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == login);
                if (profile == null)
                    return Results.NotFound(new ErrorResponse { ErrorCode = 404, Message = "������� ������������ �� ������" });
                profile.ProfileDescription = profileInformation.Description;
                profile.ProfilePhoto = profileInformation.ImageByte;
                db.SaveChanges();

                return Results.Ok();
            });

            app.MapGet("/professions", [Authorize] async (MindForgeDbContext db, HttpContext context) =>
            {
                List<ProfessionInformation> professions = await db.Professions.Select
                (p => new ProfessionInformation {
                    Name = p.ProfessionName,
                    Color = p.ProfessionColor
                }).ToListAsync();
                return Results.Ok(professions);
            });

            app.MapGet("/professions/{user}", [Authorize] async (string user, MindForgeDbContext db, HttpContext context) =>
            {
                var professions = await db.Professions.Include(a => a.Users).Where(a => a.Users.FirstOrDefault(u => u.Login == user) != null)
                    .Select(p => new ProfessionInformation
                    {
                        Name = p.ProfessionName,
                        Color = p.ProfessionColor
                    }).ToListAsync();
                return Results.Ok(professions);

            });

            app.MapPut("/professions", [Authorize] async (MindForgeDbContext db, HttpContext context) =>
            {
                string userLogin = context.User.Identity!.Name!;
                var userDb = await db.Users.Include(u => u.Professions).FirstOrDefaultAsync(u => u.Login == userLogin);

                if (userDb == null)
                    return Results.NotFound();

                List<ProfessionInformation> userProfessions = await context.Request.ReadFromJsonAsync<List<ProfessionInformation>>();
                var professionNames = new HashSet<string>(userProfessions.Select(up => up.Name));
                var professionsToAdd = await db.Professions
                    .Where(p => professionNames.Contains(p.ProfessionName))
                    .ToListAsync();

                userDb.Professions.Clear(); 
                userDb.Professions.AddRange(professionsToAdd);
                await db.SaveChangesAsync();
                return Results.Ok();

            });

            app.MapGet("/relationship/{target}", [Authorize] async (string target, MindForgeDbContext db, HttpContext context) =>
            {
                string user = context.User.Identity!.Name!;
                var targetDb = await db.Users.FirstOrDefaultAsync(u => u.Login == user);
                if (targetDb is null)
                    return Results.NotFound(new ErrorResponse {
                        ErrorCode = 404,
                        Message = "������������ � ������ ������� �� ����������"
                    });
                int targetId = targetDb.UserId;
                Console.WriteLine(target);
                var firstSearch = await db.Friendships.FirstOrDefaultAsync(u => u.User1Navigation.Login == user && u.User2Navigation.Login == target);
                var secondSearch = await db.Friendships.FirstOrDefaultAsync(u => u.User1Navigation.Login == target && u.User2Navigation.Login == user);
                if (firstSearch is null && secondSearch is null)
                    return Results.Ok(new UserRelationshipResponse
                    {
                        Relationship = Relationship.None
                    });
                int relationshipId = firstSearch is null ? secondSearch!.Status : firstSearch.Status;
                Relationship status = (Relationship)Enum.GetValues(typeof(Relationship)).GetValue(relationshipId - 1)!;
                bool isUserInitiator = firstSearch is null ? false : true;
                return Results.Ok(new UserRelationshipResponse
                {
                    Relationship = status,
                    IsYouInitiator = isUserInitiator
                });
            });

            app.MapPost("/relationship/{target}", [Authorize] async (string target, MindForgeDbContext db, HttpContext context, IHubContext<FriendHub> hubContext) =>
            {
                string user = context.User.Identity!.Name!;
                var relationshipRequest = await context.Request.ReadFromJsonAsync<RelationshipRequest>();
                // ������ ��� ����� ����� � ���� ������ � ��� ������� switch ���� �����
                if (relationshipRequest is null)
                    return Results.BadRequest(new ErrorResponse
                    {
                        ErrorCode = 400,
                        Message = "������������ ������"
                    });
                int userId = (await db.Users.FirstOrDefaultAsync(p => p.Login == user))!.UserId;
                var targetInstance = await db.Users.FirstOrDefaultAsync(p => p.Login == target);
                if (targetInstance is null)
                    return Results.NotFound(new ErrorResponse
                    {
                        ErrorCode = 404,
                        Message = "������������ �� ������"
                    });
                int targetId = targetInstance.UserId;
                var relation = await db.Friendships.FirstOrDefaultAsync(p => (p.User1Navigation.Login == user || p.User1Navigation.Login == target) && (p.User2Navigation.Login == user || p.User2Navigation.Login == target));
                var userProfile = await db.Profiles.FirstOrDefaultAsync(p => p.UserNavigation.Login == user);
                ProfileInformation userInformation = new ProfileInformation { Login = user, Description = userProfile.ProfileDescription, ImageByte = userProfile.ProfilePhoto };
                string method = "";
                switch (relationshipRequest.RelationshipAction)
                {
                    case RelationshipAction.Request:
                        if (relation is not null)
                            return Results.Conflict(new ErrorResponse {
                                ErrorCode = 409,
                                Message = "����� ����� �������������� ��� ����"
                            });
                        method = "FriendRequestReceived";
                        db.Friendships.Add(new Friendship { User1 = userId, User2 = targetId, Status = 2 });
                        break;
                    case RelationshipAction.Delete:
                        if (relation is null)
                            return Results.NotFound(new ErrorResponse
                            {
                                ErrorCode = 404,
                                Message = "����� ����� �������������� ���"
                            });
                        if (relation.Status == 1)
                            method = "FriendDeleted";
                        else if (relation.Status == 2 && relation.User1Navigation.Login == user)
                            method = "FriendRequestDeleted";
                        else if (relation.Status == 2)
                            method = "FriendRequestRejected";
                        db.Friendships.Remove(relation);
                        break;
                    case RelationshipAction.Apply:
                        if (relation is null)
                            return Results.NotFound(new ErrorResponse
                            {
                                ErrorCode = 404,
                                Message = "������� �� ���������� ���"
                            });
                        method = "FriendAdded";
                        relation.Status = 1;
                        break;
                }
                await hubContext.Clients.User(target).SendAsync(method, userInformation);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            app.MapGet("/friends/all", [Authorize] async (MindForgeDbContext db, HttpContext context) =>
            {
                string user = context.User.Identity!.Name!;
                var relationshipFriends = db.Friendships.Where(f => (f.User1Navigation.Login == user || f.User2Navigation.Login == user) && f.Status == 1)
                     .Select(f => f.User1Navigation.Login == user ? f.User2Navigation.UserId : f.User1Navigation.UserId);
                var profiles = db.Profiles.Where(p => relationshipFriends
                    .Contains(p.UserNavigation.UserId))
                    .Select(p => new ProfileInformation {
                        Login = p.UserNavigation.Login,
                        Description = p.ProfileDescription,
                        ImageByte = p.ProfilePhoto }).ToList();
                if (profiles is null)
                    profiles = new List<ProfileInformation>();
                return Results.Ok(profiles);
            });

            app.MapGet("/requests/incoming", [Authorize] async (MindForgeDbContext db, HttpContext context) =>
            {
                string user = context.User.Identity!.Name!;
                var incomingRequests = db.Friendships.Where(f => f.User2Navigation.Login == user && f.Status == 2)
                     .Select(f => f.User1Navigation.UserId);
                var profiles = db.Profiles.Where(p => incomingRequests
                    .Contains(p.UserNavigation.UserId))
                    .Select(p => new ProfileInformation
                    {
                        Login = p.UserNavigation.Login,
                        Description = p.ProfileDescription,
                        ImageByte = p.ProfilePhoto }).ToList();
                if (profiles is null)
                    profiles = new List<ProfileInformation>();
                return Results.Ok(profiles);
            });

            app.MapGet("/requests/outgoing", [Authorize] async (MindForgeDbContext db, HttpContext context) =>
            {
                string user = context.User.Identity!.Name!;
                var outgoingRequests = db.Friendships.Where(f => f.User1Navigation.Login == user && f.Status == 2)
                     .Select(f => f.User2Navigation.UserId);
                var profiles = db.Profiles.Where(p => outgoingRequests
                    .Contains(p.UserNavigation.UserId))
                    .Select(p => new ProfileInformation
                    {
                        Login = p.UserNavigation.Login,
                        Description = p.ProfileDescription,
                        ImageByte = p.ProfilePhoto }).ToList();
                if (profiles is null)
                    profiles = new List<ProfileInformation>();
                return Results.Ok(profiles);
            });

            app.MapGet("/personalchats", [Authorize] async (MindForgeDbContext db, HttpContext context) => {
                string user = context.User.Identity!.Name!;
                var chats = await (from chat in db.Chats
                   .Include(c => c.ChatTypeNavigation)
                   .Include(c => c.User1)
                   .Include(c => c.User2)
                                   where chat.ChatTypeNavigation.TypeName == "������" && (chat.User1.Login == user || chat.User2.Login == user)
                                   join profile in db.Profiles on (chat.User1.Login == user ? chat.User2.UserId : chat.User1.UserId) equals profile.UserNavigation.UserId into profiles
                                   from profile in profiles.DefaultIfEmpty() 
                                   select new PersonalChatInformation
                                   {
                                       Login = chat.User1.Login == user ? chat.User2.Login : chat.User1.Login,
                                       ImageByte = profile.ProfilePhoto,
                                       ChatId = chat.ChatId
                                   }).ToListAsync();
                if (chats is null)
                    chats = new List<PersonalChatInformation>();
                return Results.Ok(chats);
            });

            app.MapGet("/personalchats/{target}", [Authorize] async (string target, MindForgeDbContext db, HttpContext context, IHubContext<PersonalChatHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.FirstOrDefault(c => (c.User1.Login == user && c.User2.Login == target) || (c.User2.Login == user && c.User1.Login == target));
                if (chat is null)
                {
                    chat = new Chat
                    {
                        ChatType = 1,
                        User1Id = db.Users.FirstOrDefault(u => u.Login == user).UserId,
                        User2Id = db.Users.FirstOrDefault(u => u.Login == target).UserId,
                        ChatCreatedTime = DateTime.Now
                    };
                    db.Chats.Add(chat);
                    var userProfile = await db.Profiles.FirstOrDefaultAsync(p => p.UserNavigation.Login == user);
                    await db.SaveChangesAsync();
                    var chatInform = new PersonalChatInformation { ChatId = chat.ChatId, ImageByte = userProfile.ProfilePhoto, Login = user };
                    Console.WriteLine(chatInform.ChatId);
                    await hubContext.Clients.User(target).SendAsync("PersonalChatCreated", chatInform);
                    Console.WriteLine(chat.ChatId.ToString());
                }
                return Results.Ok(chat.ChatId);
            });

            app.MapGet("/messages/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context) =>
            {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.Include(c => c.Users).Include(c => c.Messages).Include(c => c.User1).Include(c => c.User2).FirstOrDefault(c => c.ChatId == chatId);
                bool permission = chat.Users.FirstOrDefault(c => c.Login == user) is not null || (chat.User1 != null && chat.User1.Login == user) || (chat.User2 != null && chat.User2.Login == user);
                if (!permission)
                    return Results.Unauthorized();
                var messages = chat.Messages.OrderByDescending(m => m.TimeSent).Take(50).Select(m => new MessageInformation { Message = m.MessageText, SenderName = m.Sender.Login, DateTime = m.TimeSent.ToShortTimeString() }).ToList();
                return Results.Ok(messages);
            });

            app.MapPost("/chats/message/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context, IHubContext<PersonalChatHub> hubContext) =>
            {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.Include(c => c.Users).Include(c => c.User1).Include(c => c.User2).FirstOrDefault(c => c.ChatId == chatId);
                bool permission = chat.Users.FirstOrDefault(c => c.Login == user) is not null || (chat.User1 != null && chat.User1.Login == user) || (chat.User2 != null && chat.User2.Login == user);

                if (!permission)
                    return Results.Unauthorized();
                MessageInformation messageInformation = await context.Request.ReadFromJsonAsync<MessageInformation>();
                int senderId = (await db.Users.FirstOrDefaultAsync(s => s.Login == messageInformation.SenderName)).UserId;
                var message = new Message { ChatId = chatId, SenderId = senderId, MessageText = messageInformation.Message, TimeSent = DateTime.Now };
                db.Messages.Add(message);
                await db.SaveChangesAsync();
                if (chat.User1 is not null && chat.User2 is not null)
                {
                    var login = chat.User1Id == senderId ? chat.User2.Login : chat.User1.Login;
                    await hubContext.Clients.User(login).SendAsync("MessageSent", messageInformation, chatId, false);
                }
                else
                {
                    var members = message.Chat.Users.Where(m => m.Login != user).Select(m => m.Login);
                    await hubContext.Clients.Users(members).SendAsync("MessageSent", messageInformation, chatId, true);
                }
                return Results.Ok();
            });

            app.MapGet("/groupchats", [Authorize] async (MindForgeDbContext db, HttpContext context) => {
                string user = context.User.Identity!.Name!;
                User userDb = db.Users.FirstOrDefault(u => u.Login == user);
                var chats = await db.Chats
                    .Include(c => c.Users)
                    .Where(c => c.Users.Contains(userDb))
                    .Select(c => new GroupChatInformation
                    {
                        ChatId = c.ChatId,
                        Name = c.ChatName,
                        ImageByte = c.ChatPhoto,
                        Members = db.Profiles.Include(p => p.UserNavigation).Where(p => c.Users.Contains(p.UserNavigation)).Select(p =>
                                  new ProfileInformation
                                  {
                                      Login = p.UserNavigation.Login,
                                      Description = p.ProfileDescription,
                                      ImageByte = p.ProfilePhoto
                                  }).ToList()
                    }).ToListAsync();
                if (chats is null)
                    chats = new List<GroupChatInformation>();
                return Results.Ok(chats);
            });

            app.MapPost("/groupchats/create", [Authorize] async (MindForgeDbContext db, HttpContext context, IHubContext<PersonalChatHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                GroupChatInformation information = await context.Request.ReadFromJsonAsync<GroupChatInformation>();
                List<string> users = information.Members.Select(m => m.Login).ToList();
                var chat = new Chat
                {
                    ChatType = 2,
                    User1Id = db.Users.FirstOrDefault(u => u.Login == user).UserId,
                    ChatName = information.Name,
                    ChatPhoto = information.ImageByte,
                    Users = db.Users.Where(u => users.Contains(u.Login)).ToList(),
                    ChatCreatedTime = DateTime.Now
                };
                db.Chats.Add(chat);
                await db.SaveChangesAsync();
                information.ChatId = chat.ChatId;
                var members = information.Members.Where(m => m.Login != user).Select(m => m.Login);
                await hubContext.Clients.Users(members).SendAsync("GroupChatCreated", information);
                await hubContext.Clients.Users(members).SendAsync("AddToGroup", chat.ChatName);
                return Results.Ok(information.ChatId);
            });

            app.MapHub<FriendHub>("/friendhub");
            app.MapHub<PersonalChatHub>("/personalchathub");

            app.Map("/", (HttpContext context) => Results.Ok());

            app.Run();
        }
    }
}
