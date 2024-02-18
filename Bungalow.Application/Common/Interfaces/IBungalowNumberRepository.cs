using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Common.Interfaces;

public interface IBungalowNumberRepository : IRepository<BungalowNumber>
{
    void Update(BungalowNumber entity);
}
