using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MindForgeClasses;
using Microsoft.AspNetCore.Authorization;
using MindForgeDbClasses;
using Microsoft.AspNetCore.SignalR;
using System.Collections.ObjectModel;

namespace MindForgeServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            builder.Services.AddTransient<ITokenService, TokenService>();
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
            app.MapGet("/profile/{user?}", [Authorize] (string? user, MindForgeDbContext db, HttpContext context) =>
            {
                if (user is null)
                    user = context.User.Identity!.Name!;
                var profile = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == user);
                if (profile == null)
                    return Results.NotFound(new ErrorResponse {
                        ErrorCode = 404,
                        Message = "Профиль пользователя не найден"
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
                    return Results.BadRequest(new ErrorResponse { ErrorCode = 400, Message = "Некорректный запрос" });

                string login = context.User.Identity!.Name!;
                var profile = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == login);
                if (profile == null)
                    return Results.NotFound(new ErrorResponse { ErrorCode = 404, Message = "Профиль пользователя не найден" });
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
                        Message = "Пользователя с данным логином не существует"
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
                    if (relationshipRequest is null)
                        return Results.BadRequest(new ErrorResponse
                        {
                            ErrorCode = 400,
                            Message = "Некорректный запрос"
                        });
                    int userId = (await db.Users.FirstOrDefaultAsync(p => p.Login == user))!.UserId;
                    var targetInstance = await db.Users.FirstOrDefaultAsync(p => p.Login == target);
                    if (targetInstance is null)
                        return Results.NotFound(new ErrorResponse
                        {
                            ErrorCode = 404,
                            Message = "Пользователь не найден"
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
                                    Message = "Связь между пользователями уже есть"
                                });
                            method = "FriendRequestReceived";
                            db.Friendships.Add(new Friendship { User1 = userId, User2 = targetId, Status = 2 });
                            break;
                        case RelationshipAction.Delete:
                            if (relation is null)
                                return Results.NotFound(new ErrorResponse
                                {
                                    ErrorCode = 404,
                                    Message = "Связи между пользователями нет"
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
                                    Message = "Запроса на добавление нет"
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
                                   where chat.ChatTypeNavigation.TypeName == "Личный" && (chat.User1.Login == user || chat.User2.Login == user)
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
                var messages = chat.Messages.OrderByDescending(m => m.TimeSent).Select(m => new MessageInformation { Message = m.MessageText, SenderName = m.Sender is not null ? m.Sender.Login : db.Users.FirstOrDefault(u => u.UserId == m.SenderId).Login, Time = m.TimeSent.ToShortTimeString(), Year = m.TimeSent.Year, Month = m.TimeSent.Month, Date = m.TimeSent.Day }).ToList();
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
                    .Include(c => c.User1)
                    .Include(c => c.Users)
                    .Where(c => c.Users.Contains(userDb))
                    .Select(c => new GroupChatInformation
                    {
                        ChatId = c.ChatId,
                        Creator = c.User1.Login,
                        Name = c.ChatName,
                        ImageByte = c.ChatPhoto,
                        Members = new ObservableCollection<ProfileInformation>(db.Profiles.Include(p => p.UserNavigation).Where(p => c.Users.Contains(p.UserNavigation)).Select(p =>
                                  new ProfileInformation
                                  {
                                      Login = p.UserNavigation.Login,
                                      Description = p.ProfileDescription,
                                      ImageByte = p.ProfilePhoto
                                  }).ToList())
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
                return Results.Ok(information.ChatId);
            });
            app.MapPost("/groupchats/delete/{name}", [Authorize] async (string name, MindForgeDbContext db, HttpContext context, IHubContext<PersonalChatHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                GroupChatInformation information = await context.Request.ReadFromJsonAsync<GroupChatInformation>();
                var profileUser = information.Members.FirstOrDefault(m => m.Login == name);
                var chat = db.Chats.Include(c => c.User1).Include(c => c.Users).FirstOrDefault(c => c.ChatId == information.ChatId);
                if (chat.User1.Login == user || user == name)
                    chat.Users.Remove(db.Users.FirstOrDefault(u => u.Login == name));
                else
                    return Results.Unauthorized();
                await db.SaveChangesAsync();
                List<string> members = chat.Users.Where(m => m.Login != user).Select(m => m.Login).ToList();
                Console.WriteLine(members.Count);
                await hubContext.Clients.Users(members).SendAsync("DeleteMember", profileUser, chat.ChatId);
                if (user != name)
                    await hubContext.Clients.User(name).SendAsync("YouDeleted", chat.ChatId);
                return Results.Ok();
            });
            app.MapPost("/groupchats/add/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context, IHubContext<PersonalChatHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                List<ProfileInformation> profiles = await context.Request.ReadFromJsonAsync<List<ProfileInformation>>();
                List<string> usersNames = profiles.Select(p => p.Login).ToList();
                var usersToAdd = db.Users.Where(u => usersNames.Contains(u.Login));
                var chat = db.Chats.Include(c => c.User1).Include(c => c.Users).FirstOrDefault(c => c.ChatId == chatId);

                if (chat.Users.Contains(db.Users.FirstOrDefault(u => u.Login == user)))
                    chat.Users.AddRange(usersToAdd);
                else
                    return Results.Unauthorized();
                await db.SaveChangesAsync();
                var members = chat.Users.Where(m => m.Login != user && !usersNames.Contains(m.Login)).Select(m => m.Login);
                await hubContext.Clients.Users(members).SendAsync("AddMember", profiles, chatId);
                var chatInform = new GroupChatInformation() 
                { 
                    ChatId = chatId, 
                    Creator = chat.User1.Login, 
                    ImageByte = chat.ChatPhoto, 
                    Name = chat.ChatName, 
                    Members = new ObservableCollection<ProfileInformation>(db.Profiles.Include(p => p.UserNavigation).Where(p => chat.Users.Contains(p.UserNavigation)).Select(p =>
                        new ProfileInformation
                        {
                            Login = p.UserNavigation.Login,
                            Description = p.ProfileDescription,
                            ImageByte = p.ProfilePhoto
                        }).ToList())
                };
                await hubContext.Clients.Users(usersNames).SendAsync("GroupChatCreated", chatInform);
                return Results.Ok();
            });
            app.MapGet("/call/create/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context, IHubContext<CallsHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.Include(c => c.Call).Include(c => c.Users).Include(u => u.User1).Include(u => u.User2).FirstOrDefault(c => c.ChatId == chatId);
                User userDb;
                if (chat.ChatType == 2)
                    userDb = chat.Users.FirstOrDefault(u => u.Login == user);
                else
                    userDb = chat.User1.Login == user ? chat.User1 : chat.User2;
                if (userDb is null)
                    return Results.Unauthorized();
                if (chat.CallId is not null)
                    return Results.Conflict(new ErrorResponse() { ErrorCode = 409, Message="Звонок уже создан"});
                chat.Call = new Call { ChatId = chatId, StartTime = DateTime.Now };
                chat.Call.CallParticipants.Add(new CallParticipant() { CallId = chat.Call.CallId, UserId = userDb.UserId, JoinTime = DateTime.Now });
                await db.SaveChangesAsync();
                var profileDb = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == user);
                var profile = new ProfileInformation
                {
                    Login = profileDb.UserNavigation.Login,
                    Description = profileDb.ProfileDescription!,
                    ImageByte = profileDb.ProfilePhoto!
                };
                var sendTo = chat.Users.Select(u => u.Login).ToList();
                if (chat.User1 is not null && chat.User2 is not null)
                    sendTo.Add(chat.User1.Login == user ? chat.User2.Login : chat.User1.Login);
                sendTo.Remove(user);
                await hubContext.Clients.Users(sendTo).SendAsync("UserJoin", profile, chatId);
                return Results.Ok();
            });
            app.MapGet("/call/leave/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context, IHubContext<CallsHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.Include(c => c.Call.CallParticipants).Include(c => c.Users).Include(u => u.User1).Include(u => u.User2).FirstOrDefault(c => c.ChatId == chatId);
                User userDb;
                if (chat.ChatType == 2)
                    userDb = chat.Users.FirstOrDefault(u => u.Login == user);
                else
                    userDb = chat.User1.Login == user ? chat.User1 : chat.User2;
                if (userDb is null)
                    return Results.Unauthorized();
                chat.Call.CallParticipants.Remove(chat.Call.CallParticipants.FirstOrDefault(c => c.User.Login == user));
                if (chat.Call.CallParticipants.Count == 0)
                    db.Calls.Remove(chat.Call);
                await db.SaveChangesAsync();
                var profileDb = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == user);
                if (profileDb == null)
                    return Results.NotFound(new ErrorResponse
                    {
                        ErrorCode = 404,
                        Message = "Профиль пользователя не найден"
                    });

                var profile=  new ProfileInformation
                {
                    Login = profileDb.UserNavigation.Login,
                    Description = profileDb.ProfileDescription!,
                    ImageByte = profileDb.ProfilePhoto!
                };
                var sendTo = chat.Users.Select(u => u.Login).ToList();
                if (chat.User1 is not null && chat.User2 is not null)
                    sendTo.Add(chat.User1.Login == user ? chat.User2.Login : chat.User1.Login);

                await hubContext.Clients.Users(sendTo).SendAsync("UserLeave",profile,chatId);
                return Results.Ok();
            });
            app.MapGet("/call/join/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context, IHubContext<CallsHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.Include(c => c.Call.CallParticipants).Include(c => c.Users).Include(u => u.User1).Include(u => u.User2).FirstOrDefault(c => c.ChatId == chatId);
                User userDb;
                if (chat.ChatType == 2)
                    userDb = chat.Users.FirstOrDefault(u => u.Login == user);
                else
                    userDb = chat.User1.Login == user ? chat.User1 : chat.User2;
                if (userDb is null)
                    return Results.Unauthorized();
                chat.Call.CallParticipants.Add(new CallParticipant { CallId = (int)chat.CallId, UserId = db.Users.FirstOrDefault(u => u.Login == user).UserId, JoinTime = DateTime.Now });
                await db.SaveChangesAsync();
                var profileDb = db.Profiles.Include(p => p.UserNavigation).FirstOrDefault(p => p.UserNavigation.Login == user);
                if (profileDb == null)
                    return Results.NotFound(new ErrorResponse
                    {
                        ErrorCode = 404,
                        Message = "Профиль пользователя не найден"
                    });

                var profile = new ProfileInformation
                {
                    Login = profileDb.UserNavigation.Login,
                    Description = profileDb.ProfileDescription!,
                    ImageByte = profileDb.ProfilePhoto!
                };
                var sendTo = chat.Users.Select(u => u.Login).ToList();
                if (chat.User1 is not null && chat.User2 is not null)
                    sendTo.Add(chat.User1.Login == user ? chat.User2.Login : chat.User1.Login);
                await hubContext.Clients.Users(sendTo).SendAsync("UserJoin", profile, chatId);

                return Results.Ok();
            });
            app.MapGet("/call/get/{chatId}", [Authorize] async (int chatId, MindForgeDbContext db, HttpContext context, IHubContext<PersonalChatHub> hubContext) => {
                string user = context.User.Identity!.Name!;
                var chat = db.Chats.Include(c => c.Call.CallParticipants).Include(c => c.Users).Include(u => u.User1).Include(u => u.User2).FirstOrDefault(c => c.ChatId == chatId);
                User userDb;
                if (chat.ChatType == 2)
                    userDb = chat.Users.FirstOrDefault(u => u.Login == user);
                else
                    userDb = chat.User1.Login == user ? chat.User1 : chat.User2;
                if (userDb is null)
                    return Results.Unauthorized();

                if (chat.CallId is null)
                    return Results.NoContent();
                List<string> participantsLogins = chat.Call.CallParticipants.Select(p => p.User.Login).ToList();
                var participants = new ObservableCollection<ProfileInformation>(db.Profiles.Include(p => p.UserNavigation).Where(p => participantsLogins.Contains(p.UserNavigation.Login)).Select(p =>
                        new ProfileInformation
                        {
                            Login = p.UserNavigation.Login,
                            Description = p.ProfileDescription,
                            ImageByte = p.ProfilePhoto
                        }).ToList());
                
                return Results.Ok(participants);
            });
            
            app.MapHub<FriendHub>("/friendhub");
            app.MapHub<PersonalChatHub>("/personalchathub");
            app.MapHub<CallsHub>("/callshub");

            app.Map("/", (HttpContext context) => Results.Ok());

            app.Run();
        }
    }
}
