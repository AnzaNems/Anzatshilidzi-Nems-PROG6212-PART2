using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Dtos;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResultsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public ResultsController(ApplicationDbContext db) => _db = db;
    private int CurrentProfileId => int.Parse(User.FindFirstValue("ProfileId")!);

    /// <summary>Captures result. Organisers only.</summary>
    [Authorize(Roles = "Organiser")]
    [HttpPost("enrolment/{enrolmentId}")]
    public async Task<IActionResult> CreateResult(int enrolmentId, ResultDto dto)
    {
        var enr = await _db.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.Id == enrolmentId);
        if (enr == null || enr.Event!.OrganiserId != CurrentProfileId) return NotFound();
        if (await _db.Results.AnyAsync(r => r.EnrolmentId == enrolmentId)) return Conflict("Result exists.");

        var res = new Result { EnrolmentId = enrolmentId, FinishTime = dto.FinishTime, Position = dto.Position, IsDisqualified = dto.IsDisqualified };
        _db.Results.Add(res);
        await _db.SaveChangesAsync();
        return Ok(res);
    }

    /// <summary>Gets my results. Participants only.</summary>
    [Authorize(Roles = "Participant")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyResults() => Ok(await _db.Results.Include(r => r.Enrolment).ThenInclude(e => e!.Event).Where(r => r.Enrolment!.ParticipantId == CurrentProfileId).ToListAsync());
}