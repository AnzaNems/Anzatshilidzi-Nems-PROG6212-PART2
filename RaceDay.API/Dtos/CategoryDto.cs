using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Dtos;

public class CategoryDto
{
    [Required] public string Name { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public decimal EntryFee { get; set; }
}