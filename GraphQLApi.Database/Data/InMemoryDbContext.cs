using GraphQLApi.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphQLApi.Database.Data;

public class InMemoryDbContext : DbContext
{
    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : base(options) { }

    public DbSet<Payer> Payers { get; init; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payer>()
            .HasKey(p => p.ChhaId);

        base.OnModelCreating(modelBuilder);
    }

    public static void SeedData(InMemoryDbContext context)
    {
        if (!context.Payers.Any())
        {
            var payers = new List<Payer>
            {
                new Payer
                {
                    ChhaId = 1,
                    ChhaInitial = "ABC",
                    ChhaName = "ABC Health"
                },
                new Payer
                {
                    ChhaId = 2,
                    ChhaInitial = "XYZ",
                    ChhaName = "XYZ Health"
                }
            };

            context.Payers.AddRange(payers);
            context.SaveChanges();
        }
    }
}
