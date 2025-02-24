using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Services.Interface;

public interface IAmenityService
{
    IEnumerable<Amenity> GetAllAmenities();
    void CreateAmenity(Amenity amenity);
    void UpdateAmenity(Amenity amenity);
    Amenity GetAmenityById(int id);
    bool DeleteAmenity(int id);
}