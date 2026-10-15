using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UserAccount> Users { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Organiser> Organisers { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Enrolment> Enrolments { get; set; }
    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserAccount>().HasIndex(u => u.Email).IsUnique();

        // 1-to-1 Relationships
        builder.Entity<UserAccount>().HasOne(u => u.ParticipantProfile).WithOne(p => p.UserAccount)
            .HasForeignKey<Participant>(p => p.UserAccountId).OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<UserAccount>().HasOne(u => u.OrganiserProfile).WithOne(o => o.UserAccount)
            .HasForeignKey<Organiser>(o => o.UserAccountId).OnDelete(DeleteBehavior.Cascade);

        // 1-to-1 Result to Enrolment
        builder.Entity<Result>().HasOne(r => r.Enrolment).WithOne(e => e.Result)
            .HasForeignKey<Result>(r => r.EnrolmentId).OnDelete(DeleteBehavior.Cascade);

        // Prevent multiple cascade paths error in SQL Server
        builder.Entity<Enrolment>().HasOne(e => e.Event).WithMany(ev => ev.Enrolments)
            .HasForeignKey(e => e.EventId).OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<Enrolment>().HasOne(e => e.Category).WithMany(c => c.Enrolments)
            .HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Enrolment>().HasOne(e => e.Participant).WithMany(p => p.Enrolments)
            .HasForeignKey(e => e.ParticipantId).OnDelete(DeleteBehavior.Cascade);

        // Business Rule: Unique Enrolment
        builder.Entity<Enrolment>().HasIndex(e => new { e.ParticipantId, e.EventId, e.CategoryId }).IsUnique();

        builder.Entity<Category>().Property(c => c.EntryFee).HasPrecision(18, 2);
    }
}
