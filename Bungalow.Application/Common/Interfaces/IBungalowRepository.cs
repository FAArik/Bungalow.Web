using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Common.Interfaces;

public interface IBungalowRepository:IRepository<Bungalow>
{
    void Update(Bungalow entity);
} 
