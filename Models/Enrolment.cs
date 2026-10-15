using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Enrolment
{
    public int Id { get; set; }
    public DateTime EnrolmentDate { get; set; } = DateTime.UtcNow;

    public int ParticipantId { get; set; }
    [ForeignKey("ParticipantId")]
    public Participant? Participant { get; set; }

    public int EventId { get; set; }
    [ForeignKey("EventId")]
    public Event? Event { get; set; }

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public Result? Result { get; set; }
}
