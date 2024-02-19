using BungalowApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BungalowApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Bungalow> Bungalows { get; set; }
    public DbSet<BungalowNumber> BungalowNumbers { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
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

        modelBuilder.Entity<Amenity>().HasData(
            new Amenity
            {
                Id = 1,
                BungalowId = 1,
                Name = "Private Pool"
            },
            new Amenity
            {
                Id = 2,
                BungalowId = 1,
                Name = "Microwave"
            },
            new Amenity
            {
                Id = 3,
                BungalowId = 1,
                Name = "Private Balcony"
            },
            new Amenity
            {
                Id = 4,
                BungalowId = 1,
                Name = "1 king bed and 1 sofa bed"
            },
            new Amenity
            {
                Id = 5,
                BungalowId = 2,
                Name = "Private Plunge Pool"
            },
            new Amenity
            {
                Id = 6,
                BungalowId = 2,
                Name = "Microwave and Mini Refrigerator"
            },
            new Amenity
            {
                Id = 7,
                BungalowId = 2,
                Name = "Private Balcony"
            },
            new Amenity
            {
                Id = 8,
                BungalowId = 2,
                Name = "king bed or 2 double beds"
            },
            new Amenity
            {
                Id = 9,
                BungalowId = 3,
                Name = "Private Pool"
            },
            new Amenity
            {
                Id = 10,
                BungalowId = 3,
                Name = "Jacuzzi"
            },
            new Amenity
            {
                Id = 11,
                BungalowId = 3,
                Name = "Private Balcony"
            });
    }
}
