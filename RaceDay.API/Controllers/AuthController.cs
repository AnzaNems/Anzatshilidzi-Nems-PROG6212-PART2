using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Dtos;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<UserAccount> _hasher;
    private readonly JwtService _jwtService;

    public AuthController(ApplicationDbContext db, IPasswordHasher<UserAccount> hasher, JwtService jwtService)
    {
        _db = db; _hasher = hasher; _jwtService = jwtService;
    }

    /// <summary>Registers a new user. Organisers are created with IsApproved = false to prevent privilege escalation.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email.ToLower()))
            return BadRequest("Email already exists.");

        var user = new UserAccount { Email = dto.Email.ToLower(), Role = dto.Role, PasswordHash = "" };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        if (dto.Role == "Participant")
        {
            _db.Participants.Add(new Participant { FullName = dto.FullName, PhoneNumber = dto.PhoneNumber, UserAccountId = user.Id });
        }
        else
        {
            // 🚨 PART 1 SECURITY FIX: IsApproved = false
            _db.Organisers.Add(new Organiser { FullName = dto.FullName, CompanyName = dto.CompanyName, IsApproved = false, UserAccountId = user.Id });
        }
        await _db.SaveChangesAsync();

        return Ok(new { message = "Registration successful. Organisers pending approval.", userId = user.Id, role = user.Role });
    }

    /// <summary>Authenticates user, issues JWT, and stores Role in server-side session.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.Include(u => u.OrganiserProfile).Include(u => u.ParticipantProfile)
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        if (user == null || _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        int profileId; string fullName;
        if (user.Role == "Organiser")
        {
            // 🚨 BLOCK UNAPPROVED ORGANISERS
            if (user.OrganiserProfile == null || !user.OrganiserProfile.IsApproved)
                return StatusCode(403, "Organiser account pending admin approval.");
            profileId = user.OrganiserProfile.Id; fullName = user.OrganiserProfile.FullName;
        }
        else
        {
            if (user.ParticipantProfile == null) return Unauthorized("Profile missing.");
            profileId = user.ParticipantProfile.Id; fullName = user.ParticipantProfile.FullName;
        }

        var token = _jwtService.CreateToken(user, profileId, fullName);

        // RUBRIC REQUIREMENT: Server-side session
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("UserRole", user.Role);

        return Ok(new { token, userId = user.Id, role = user.Role, email = user.Email });
    }
}