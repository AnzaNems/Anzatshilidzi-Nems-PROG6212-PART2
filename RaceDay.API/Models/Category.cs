using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Category
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public double DistanceKm { get; set; }
    public decimal EntryFee { get; set; }

    public int EventId { get; set; }
    [ForeignKey("EventId")]
    public Event? Event { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}