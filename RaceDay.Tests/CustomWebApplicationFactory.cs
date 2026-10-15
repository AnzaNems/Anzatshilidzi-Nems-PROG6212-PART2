using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Linq;

namespace RaceDay.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        // Shared cache keeps the in-memory SQLite DB alive for the duration of the tests
        _connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Remove the real SQL Server database connection
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // 2. Tell the API to use the fake SQLite database
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));

            // 3. BULLETPROOF SEEDING: Build the DB and seed data right here, right now.
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated(); // Creates the tables in SQLite

            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserAccount>>();
            DbInitializer.SeedAsync(db, hasher).GetAwaiter().GetResult(); // Forces seed data
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection?.Dispose();
    }
}