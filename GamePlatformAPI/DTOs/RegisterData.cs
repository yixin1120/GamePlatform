//登入資料model

namespace GamePlatformAPI.DTOs;
public class RegisterData{
    public required string PlayerName { get; set;}
    public required string Email { get; set;}
    public required string Password { get; set;}
    public required DateOnly Birthday { get; set;}
    public required string Gender { get; set;}

}