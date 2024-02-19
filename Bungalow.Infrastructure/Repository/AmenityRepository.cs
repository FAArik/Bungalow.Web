using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using BungalowApi.Infrastructure.Data;

namespace BungalowApi.Infrastructure.Repository;

public class AmenityRepository : Repository<Amenity>, IAmenityRepository
{
    private readonly ApplicationDbContext _context;
    public AmenityRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public void Update(Amenity amenity)
    {
        _context.Amenities.Update(amenity);
    }
}
