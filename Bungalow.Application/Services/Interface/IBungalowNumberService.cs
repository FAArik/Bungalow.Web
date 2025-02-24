using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Services.Interface;

public interface IBungalowNumberService
{
    bool RoomNumberExists(int bungalowNumber);
    IEnumerable<BungalowNumber> GetAllBungalowNumbers();
    BungalowNumber GetBungalowNumberById(int bungalowNumberId);
    void CreateBungalowNumber(BungalowNumber bungalowNumber);
    void UpdateBungalowNumber(BungalowNumber bungalowNumber);
    bool DeleteBungalowNumber(int bungalowNumberId);
}