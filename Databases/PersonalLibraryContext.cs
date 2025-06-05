using PersonalLibrary.Models;
using Microsoft.EntityFrameworkCore;
namespace PersonalLibrary.Databases;

public class PersonalLibraryContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Collection> Collections { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(@"Server=localhost;Port=5432;Database=personalLibrary;User Id=personalLibrary;Password=p3rs0n4lLibrary;Include Error Detail=true;")
            .EnableSensitiveDataLogging();
    }
    
}