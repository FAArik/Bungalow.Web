namespace BungalowApi.Application.Common.Interfaces;

public interface IUnitOfWork
{ 
    IBungalowRepository Bungalow { get; }
    IBungalowNumberRepository BungalowNumber { get; }
    IAmenityRepository Amenity { get; }
    void Save();
}
