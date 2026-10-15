using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Participant
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public int UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public UserAccount? UserAccount { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
