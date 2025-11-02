using DatabaseInitializer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Add DbContext with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();

// Apply database migrations
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        Console.WriteLine("Applying database migrations...");
        
        // Apply pending migrations
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            Console.WriteLine($"Found {pendingMigrations.Count()} pending migration(s):");
            foreach (var migration in pendingMigrations)
            {
                Console.WriteLine($"  - {migration}");
            }
            
            await context.Database.MigrateAsync();
            Console.WriteLine("Migrations applied successfully!");
        }
        else
        {
            Console.WriteLine("Database is up to date. No pending migrations.");
        }
        
        Console.WriteLine($"Database: {context.Database.GetDbConnection().Database}");
        
        // Seed database with random data
        var seeder = new DataSeeder(context);
        await seeder.SeedAsync(personCount: 50, companyCount: 10);
        
        Console.WriteLine("\nDatabase initialization completed. Press any key to exit...");
        Console.ReadKey();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
        Environment.Exit(1);
    }
}

Console.WriteLine("Application finished.");