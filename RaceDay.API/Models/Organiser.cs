using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.API.Models;

public class Organiser
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }

    //  PART 1 SECURITY FIX: Self-registered organisers are blocked until approved
    public bool IsApproved { get; set; } = false;

    public int UserAccountId { get; set; }
    [ForeignKey("UserAccountId")]
    public UserAccount? UserAccount { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
}