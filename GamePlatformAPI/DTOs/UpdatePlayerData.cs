//修改玩家資料Model

namespace GamePlatformAPI.DTOs;
public class UpdatePlayerData
{
    public required string PlayerName { get; set; }
    public required string Email { get; set; }
    public required DateOnly Birthday { get; set; }
    public required string Gender { get; set; }
}
