using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models;

public class UserAccount
{
    public int Id { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty; // "Organiser" or "Participant"

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Participant? ParticipantProfile { get; set; }
    public Organiser? OrganiserProfile { get; set; }
}