using Microsoft.AspNetCore.Identity;
using GamePlatformAPI.Models;

//密碼處理工具
namespace GamePlatformAPI.Services;
public class PasswordService
{
    private readonly PasswordHasher<PlayerData> _hasher = new PasswordHasher<PlayerData>();
    public string HashPassword(PlayerData player, string password)
    {
        return _hasher.HashPassword(player, password);
    }

    public bool VerifyPassword(PlayerData player, string password)
    {
        var result  = _hasher.VerifyHashedPassword(
            player,
            player.Password,
            password
        );
        return result  != PasswordVerificationResult.Failed;
    }
}