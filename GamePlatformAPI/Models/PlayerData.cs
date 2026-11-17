using System.ComponentModel.DataAnnotations;
namespace GamePlatformAPI.Models;

//玩家資料Model
public class PlayerData
{
    [MaxLength(50)]
    public required string PlayerName {get; set;}

    [MaxLength(20)]
    public required string PlayerId {get; set;}

    [MaxLength(100)]
    public required string Email {get; set;}

    [MaxLength(100)]
    public required string Password {get; set;}
    public required DateOnly Birthday {get; set;}

    [MaxLength(10)]
    public required string Gender {get; set;}
    public int Level {get; set;}
}