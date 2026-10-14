using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Event
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    [Required] public string Location { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    [Required] public string EventType { get; set; } = string.Empty; // Run, Walk, Cycle

    // Soft delete flag (Handled in controller queries to avoid EF Core warnings)
    public bool IsDeleted { get; set; } = false;

    public int OrganiserId { get; set; }
    [ForeignKey("OrganiserId")]
    public Organiser? Organiser { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}