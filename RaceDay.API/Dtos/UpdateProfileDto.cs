using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Dtos;

public class UpdateProfileDto
{
    [Required] public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? CompanyName { get; set; }
}