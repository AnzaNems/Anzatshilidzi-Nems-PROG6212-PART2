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
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public EventsController(ApplicationDbContext db) => _db = db;
    private int CurrentProfileId => int.Parse(User.FindFirstValue("ProfileId")!);

    /// <summary>Gets all active events. Public.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _db.Events.Where(e => !e.IsDeleted).Include(e => e.Organiser).ToListAsync();
        return Ok(events.Select(e => new { e.Id, e.Name, e.Description, e.EventDate, e.Location, e.DistanceKm, e.EventType, Organiser = e.Organiser!.FullName }));
    }

    /// <summary>Gets a single event. Public.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var ev = await _db.Events.Where(e => !e.IsDeleted).Include(e => e.Categories).FirstOrDefaultAsync(e => e.Id == id);
        if (ev == null) return NotFound();
        return Ok(ev);
    }

    /// <summary>Creates an event. Organisers only.</summary>
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent(EventDto dto)
    {
        var ev = new Event { Name = dto.Name, Description = dto.Description, EventDate = dto.EventDate, Location = dto.Location, DistanceKm = dto.DistanceKm, EventType = dto.EventType, OrganiserId = CurrentProfileId };
        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ev);
    }

    /// <summary>Updates an event. Organisers only.</summary>
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(int id, EventDto dto)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null || ev.OrganiserId != CurrentProfileId) return NotFound();
        ev.Name = dto.Name; ev.Description = dto.Description; ev.EventDate = dto.EventDate; ev.Location = dto.Location; ev.DistanceKm = dto.DistanceKm; ev.EventType = dto.EventType;
        await _db.SaveChangesAsync();
        return Ok(ev);
    }

    /// <summary>Soft-deletes an event. Organisers only.</summary>
    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev == null || ev.OrganiserId != CurrentProfileId) return NotFound();
        ev.IsDeleted = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}