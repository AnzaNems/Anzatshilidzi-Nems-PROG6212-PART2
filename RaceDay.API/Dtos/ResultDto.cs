using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Dtos;

public class ResultDto
{
    [Required, RegularExpression(@"^\d{2}:\d{2}:\d{2}$")] public string FinishTime { get; set; } = string.Empty;
    [Range(1, int.MaxValue)] public int Position { get; set; }
    public bool IsDisqualified { get; set; } = false;
}