using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Result
{
    public int Id { get; set; }

    // Stored as string (e.g., "01:23:45") to prevent SQLite testing crashes
    [Required] public string FinishTime { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsDisqualified { get; set; } = false;

    public int EnrolmentId { get; set; }
    [ForeignKey("EnrolmentId")]
    public Enrolment? Enrolment { get; set; }
}