namespace BungalowApi.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IBungalowRepository Bungalow { get; }
    IBungalowNumberRepository BungalowNumber { get; }
    IAmenityRepository Amenity { get; }
    IBookingRepository Booking { get; }
    IApplicationUserRepository User { get; }
    void Save();
}
