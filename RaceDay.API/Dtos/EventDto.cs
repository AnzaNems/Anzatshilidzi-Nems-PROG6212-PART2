using System.ComponentModel.DataAnnotations;
namespace RaceDay.API.Dtos;

public class EventDto
{
    [Required] public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    [Required] public string Location { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    [Required] public string EventType { get; set; } = string.Empty;
}