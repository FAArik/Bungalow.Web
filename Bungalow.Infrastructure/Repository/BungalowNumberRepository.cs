using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Domain.Entities;
using BungalowApi.Infrastructure.Data;

namespace BungalowApi.Infrastructure.Repository;

public class BungalowNumberRepository : Repository<BungalowNumber>, IBungalowNumberRepository
{
    private readonly ApplicationDbContext _context;
    public BungalowNumberRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public void Update(BungalowNumber entity)
    {
        _context.Update(entity);
    }
}
