using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Services.Implementation;

public class BungalowNumberService : IBungalowNumberService
{
    private readonly IUnitOfWork _unitOfWork;

    public BungalowNumberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public bool RoomNumberExists(int bungalowNumber)
    {
        return _unitOfWork.BungalowNumber.Any(x =>
            x.Bungalow_Number == bungalowNumber);
    }

    public IEnumerable<BungalowNumber> GetAllBungalowNumbers()
    {
        return _unitOfWork.BungalowNumber.GetAll(includeProperties: "Bungalow");
    }

    public BungalowNumber GetBungalowNumberById(int bungalowNumberId)
    {
        return _unitOfWork.BungalowNumber.Get(x => x.Bungalow_Number == bungalowNumberId);
    }

    public void CreateBungalowNumber(BungalowNumber bungalowNumber)
    {
        _unitOfWork.BungalowNumber.Add(bungalowNumber);
        _unitOfWork.Save();
    }

    public void UpdateBungalowNumber(BungalowNumber bungalowNumber)
    {
        _unitOfWork.BungalowNumber.Update(bungalowNumber);
        _unitOfWork.Save();
    }

    public bool DeleteBungalowNumber(int bungalowNumberId)
    {
        try
        {
            BungalowNumber bungalowNumber = GetBungalowNumberById(bungalowNumberId);
            if (bungalowNumber is not null)
            {
                _unitOfWork.BungalowNumber.Delete(bungalowNumber);
                _unitOfWork.Save();
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}