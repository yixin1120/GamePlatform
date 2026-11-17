//修改密碼資料Model

namespace GamePlatformAPI.DTOs;
public class ChangePasswordData
{
    public required string CurrentPassword { get; set ;}
    public required string NewPassword { get; set;}
}