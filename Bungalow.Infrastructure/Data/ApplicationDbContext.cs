using BungalowApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BungalowApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Bungalow> Bungalows { get; set; }
    public DbSet<BungalowNumber> BungalowNumbers { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Bungalow>().HasData(
            new Bungalow
            {
                Id = 1,
                Name = "Royal Bungalow",
                Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://placehold.co/600x480",
                Occupancy = 4,
                Price = 200,
                Sqft = 550,
            },
            new Bungalow
            {
                Id = 2,
                Name = "Premium Pool Bungalow",
                Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://placehold.co/600x401",
                Occupancy = 4,
                Price = 300,
                Sqft = 550,
            },
            new Bungalow
            {
                Id = 3,
                Name = "Luxury Pool Bungalow",
                Description = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
                ImageUrl = "https://placehold.co/600x402",
                Occupancy = 4,
                Price = 400,
                Sqft = 750
            });
        modelBuilder.Entity<BungalowNumber>().HasData(
            new BungalowNumber
            {
                Bungalow_Number = 101,
                BungalowId = 1,
            },
            new BungalowNumber
            {
                Bungalow_Number = 102,
                BungalowId = 1,
            },
            new BungalowNumber
            {
                Bungalow_Number = 103,
                BungalowId = 1,
            },
            new BungalowNumber
            {
                Bungalow_Number = 104,
                BungalowId = 1,
            },
            new BungalowNumber
            {
                Bungalow_Number = 201,
                BungalowId = 2,
            },
            new BungalowNumber
            {
                Bungalow_Number = 202,
                BungalowId = 2,
            },
            new BungalowNumber
            {
                Bungalow_Number = 203,
                BungalowId = 2,
            },
            new BungalowNumber
            {
                Bungalow_Number = 301,
                BungalowId = 3,
            },
            new BungalowNumber
            {
                Bungalow_Number = 302,
                BungalowId = 3,
            });

    }
}
