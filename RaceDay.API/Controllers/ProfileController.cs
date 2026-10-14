using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Dtos;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ProfileController(ApplicationDbContext db) => _db = db;

    private int CurrentUserAccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Gets the profile of the logged-in user.</summary>
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _db.Users.Include(u => u.ParticipantProfile).Include(u => u.OrganiserProfile)
            .FirstOrDefaultAsync(u => u.Id == CurrentUserAccountId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.Role,
            FullName = user.Role == "Organiser" ? user.OrganiserProfile?.FullName : user.ParticipantProfile?.FullName,
            IsApproved = user.OrganiserProfile?.IsApproved
        });
    }

    /// <summary>Updates the profile of the logged-in user.</summary>
    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await _db.Users.Include(u => u.ParticipantProfile).Include(u => u.OrganiserProfile)
            .FirstOrDefaultAsync(u => u.Id == CurrentUserAccountId);
        if (user == null) return NotFound();

        if (user.Role == "Organiser") user.OrganiserProfile!.FullName = dto.FullName;
        else user.ParticipantProfile!.FullName = dto.FullName;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Profile updated." });
    }
}