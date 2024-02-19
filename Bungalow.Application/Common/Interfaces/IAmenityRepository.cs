using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Common.Interfaces;

public interface IAmenityRepository : IRepository<Amenity>
{
    void Update(Amenity amenity);
}
