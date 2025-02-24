using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Services.Interface;

public interface IBungalowService
{
    IEnumerable<Bungalow> GetAllBungalow();
    Bungalow GetBungalowById(int id);
    void CreateBungalow(Bungalow bungalow);
    void UpdateBungalow(Bungalow bungalow);
    bool DeleteBungalow(int id);

    IEnumerable<Bungalow> GetBungalowsAvailabilityByDate(int nights, DateOnly checkInDate);
    bool IsBungalowAvailableByDate(int bungalowId, int nights, DateOnly checkInDate);
}