using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using GamePlatformAPI.DTOs;
using GamePlatformAPI.Models;
using GamePlatformAPI.Services;
using GamePlatformAPI;


public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

       builder.Services.AddDbContext<GamePlatformContext>(options =>
            options.UseSqlServer("Server=localhost;Database=GamePlatform;Trusted_Connection=True;TrustServerCertificate=True;"));

        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseCors("AllowFrontend");

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // 取得玩家資料
        app.MapGet("/api/player", async(string email, GamePlatformContext db) =>
        {
            var player = await db.PlayerData.FirstOrDefaultAsync(x => x.Email == email);

            if (player == null)
            {
                return Results.NotFound("找不到玩家資料");
            }

            return Results.Ok(new
            {
                player.PlayerId,
                player.PlayerName,
                player.Email,
                player.Birthday,
                player.Gender,
                player.Level
            });
        });

        // 修改玩家資料
        app.MapPost("/api/player", async(UpdatePlayerData data,string email, GamePlatformContext db) =>
        {
            var player = await db.PlayerData.FirstOrDefaultAsync(x => x.Email == email);
            
            if (player == null)
            {
                return Results.NotFound("找不到玩家資料");
            }

            if (string.IsNullOrWhiteSpace(data.PlayerName))
            {
                return Results.BadRequest("玩家名稱不能為空");
            }

            if (data.PlayerName.Length < 2)
            {
                return Results.BadRequest("玩家名稱至少需要2個字");
            }

            if (data.PlayerName.Length > 20)
            {
                return Results.BadRequest("玩家名稱不能超過20個字");
            }

            var nameAlreadyUsed =await db.PlayerData
                .AnyAsync(x => x.PlayerName == data.PlayerName && x.PlayerId != player.PlayerId);
            
            if (nameAlreadyUsed)
            {
                return Results.BadRequest("此玩家名稱已被使用");
            }

            if (string.IsNullOrWhiteSpace(data.Email))
            {
                return Results.BadRequest("電子郵件不能為空");
            }

            if (!MailAddress.TryCreate(data.Email, out _))
            {
                return Results.BadRequest("電子郵件格式錯誤");
            }

            var emailAlreadyUsed = await db.PlayerData
                .AnyAsync(x => x.Email == data.Email && x.PlayerId != player.PlayerId);

            if(emailAlreadyUsed)
            {
                return Results.BadRequest("此電子郵件已被使用");
            }

            if (data.Birthday > DateOnly.FromDateTime(DateTime.Today))
            {
                return Results.BadRequest("生日不能是未來日期");
            }

            player.PlayerName = data.PlayerName;
            player.Email = data.Email;
            player.Birthday = data.Birthday;
            player.Gender = data.Gender;

            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                player.PlayerId,
                player.PlayerName,
                player.Email,
                player.Birthday,
                player.Gender,
                player.Level
            });
        });

        app.MapPost("/api/login", async(LoginData data, GamePlatformContext db) =>
        {
            var player = await db.PlayerData
                .FirstOrDefaultAsync(x => x.Email == data.Email);
            if (player == null)
            {
                return Results.BadRequest("電子郵件或密碼錯誤");
            }

            var passwordService = new PasswordService();

            if (player.Password==data.Password)
            {
                player.Password = passwordService.HashPassword(player, data.Password);

                await db.SaveChangesAsync();
            }
            else
            {
                if(!passwordService.VerifyPassword(player, data.Password))
                {
                    return Results.BadRequest("電子郵件或密碼錯誤");
                }
            }

            return Results.Ok("登入成功");
        });

        //修改密碼
        app.MapPost("/api/change-password", async(ChangePasswordData data,string email,GamePlatformContext db) =>
        {
            var player = await db.PlayerData
                .FirstOrDefaultAsync(x => x.Email ==email);

            if (player ==null)
            {
                return Results.BadRequest("找不到玩家資料");
            }

            var passwordService = new PasswordService();
            if (!passwordService.VerifyPassword(player, data.CurrentPassword))
            {
                return Results.BadRequest("目前密碼錯誤");
            }

            if (data.NewPassword == data.CurrentPassword)
            {
                return Results.BadRequest("新密碼不能與舊密碼相同");
            }

            if (data.NewPassword.Length < 8)
            {
                return Results.BadRequest("新密碼至少需要8個字元");
            }

            player.Password = passwordService.HashPassword(player, data.NewPassword);
            await db.SaveChangesAsync();
            return Results.Ok("修改密碼成功");
        });

        app.MapPost("/api/register",async (RegisterData data, GamePlatformContext db) =>
        {
            if(string.IsNullOrWhiteSpace(data.PlayerName))
            {
                return Results.BadRequest("玩家名稱不能為空");
            }

            if(data.PlayerName.Length<2 || data.PlayerName.Length > 20)
            {
                return Results.BadRequest("玩家名稱必須介於2~20個字");
            }

            if (string.IsNullOrWhiteSpace(data.Email))
            {
                return Results.BadRequest("電子郵件不能為空");
            }

            if(!MailAddress.TryCreate(data.Email, out _))
            {
                return Results.BadRequest("電子郵件格式錯誤");
            }

            var existingPlayer = await db.PlayerData
            .FirstOrDefaultAsync(x => x.Email == data.Email);            
            
            if(existingPlayer != null)
            {
                return Results.BadRequest("此電子郵件已註冊");                
            }

            if (string.IsNullOrWhiteSpace(data.Password))
            {
                return Results.BadRequest("密碼不能為空");
            }

            if(data.Password.Length < 8)
            {
                return Results.BadRequest("密碼至少需要8個字元");
            }

            if(data.Birthday > DateOnly.FromDateTime(DateTime.Today))
            {
                return Results.BadRequest("生日不能是未來日期");
            }

            string newPlayerId;
            do
            {
                newPlayerId = GeneratePlayerId();
            }

            while(await db.PlayerData.AnyAsync(x => x.PlayerId == newPlayerId));

            var newPlayer = new PlayerData
            {
                PlayerId = newPlayerId,
                PlayerName = data.PlayerName,
                Email = data.Email,
                Password = "",
                Birthday = data.Birthday,
                Gender = data.Gender,
                Level = 1
            };

            var passwordService = new PasswordService();
            newPlayer.Password = passwordService.HashPassword(newPlayer, data.Password);

            db.PlayerData.Add(newPlayer);
            await db.SaveChangesAsync();

            return Results.Ok("註冊成功");
        });

        app.Run();
    }

    //產生玩家ID
    private static string GeneratePlayerId()
    {
        const string availableCharacters="ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        char[] id=new char[6];

        for(int i=0; i < id.Length; i++)
        {
            int randomIndex = RandomNumberGenerator.GetInt32(availableCharacters.Length);
            id[i] = availableCharacters[randomIndex];
        }

        return new string(id);
    }
}
