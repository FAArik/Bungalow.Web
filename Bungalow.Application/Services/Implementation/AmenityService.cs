using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Services.Implementation
{
    public class AmenityService : IAmenityService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AmenityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public void CreateAmenity(Amenity amenity)
        {
            ArgumentNullException.ThrowIfNull(amenity);

            _unitOfWork.Amenity.Add(amenity);
            _unitOfWork.Save();
        }

        public bool DeleteAmenity(int id)
        {
            try
            {
                var amenity = _unitOfWork.Amenity.Get(u => u.Id == id);

                if (amenity != null)
                {

                    _unitOfWork.Amenity.Delete(amenity);
                    _unitOfWork.Save();
                    return true;
                }
                else
                {
                    throw new InvalidOperationException($"Amenity with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public IEnumerable<Amenity> GetAllAmenities()
        {
            return _unitOfWork.Amenity.GetAll(includeProperties: "Bungalow");
        }

        public Amenity GetAmenityById(int id)
        {
            return _unitOfWork.Amenity.Get(u => u.Id == id, includeProperties: "Bungalow");
        }

        public void UpdateAmenity(Amenity amenity)
        {
            ArgumentNullException.ThrowIfNull(amenity);

            _unitOfWork.Amenity.Update(amenity);
            _unitOfWork.Save();
        }


    }
}