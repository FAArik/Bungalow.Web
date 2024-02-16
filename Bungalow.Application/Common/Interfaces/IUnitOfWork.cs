namespace BungalowApi.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IBungalowRepository Bungalow { get; }
}
