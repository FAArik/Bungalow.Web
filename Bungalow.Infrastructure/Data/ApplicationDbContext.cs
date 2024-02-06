using BungalowApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BungalowApi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    protected ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
    }
    public DbSet<Bungalow> Bungalows { get; set; }
}
