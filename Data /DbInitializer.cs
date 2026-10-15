using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher<UserAccount> hasher)
    {
        if (await db.Users.AnyAsync()) return;

        var orgAcc = new UserAccount { Email = "organiser@raceday.co.za", Role = "Organiser", PasswordHash = "" };
        orgAcc.PasswordHash = hasher.HashPassword(orgAcc, "Organiser@123");
        
        var partAcc = new UserAccount { Email = "participant@raceday.co.za", Role = "Participant", PasswordHash = "" };
        partAcc.PasswordHash = hasher.HashPassword(partAcc, "Participant@123");

        db.Users.AddRange(orgAcc, partAcc);
        await db.SaveChangesAsync();

        var org = new Organiser { FullName = "Test Organiser", CompanyName = "RaceDay Events", IsApproved = true, UserAccountId = orgAcc.Id };
        var part = new Participant { FullName = "Test Participant", UserAccountId = partAcc.Id };
        
        db.Organisers.Add(org);
        db.Participants.Add(part);
        await db.SaveChangesAsync();

        var ev = new Event { Name = "Joburg Marathon", Description = "Annual marathon", EventDate = DateTime.UtcNow.AddMonths(3), Location = "JHB", DistanceKm = 42.2, EventType = "Run", OrganiserId = org.Id };
        db.Events.Add(ev);
        await db.SaveChangesAsync();

        db.Categories.Add(new Category { Name = "Open 42km", DistanceKm = 42.2, EntryFee = 250, EventId = ev.Id });
        await db.SaveChangesAsync();
    }
}
