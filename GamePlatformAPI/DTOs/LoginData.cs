//登入資料Model

namespace GamePlatformAPI.DTOs;
public class LoginData
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}