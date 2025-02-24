using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace BungalowApi.Application.Services.Implementation
{
    public class BungalowService : IBungalowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BungalowService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public void CreateBungalow(Bungalow bungalow)
        {
            if (bungalow.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bungalow.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\BungalowImage");

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                bungalow.Image.CopyTo(fileStream);

                bungalow.ImageUrl = @"\images\BungalowImage\" + fileName;
            }
            else
            {
                bungalow.ImageUrl = "https://placehold.co/600x400";
            }

            _unitOfWork.Bungalow.Add(bungalow);
            _unitOfWork.Save();
        }

        public bool DeleteBungalow(int id)
        {
            try
            {
                Bungalow? objFromDb = _unitOfWork.Bungalow.Get(u => u.Id == id);
                if (objFromDb is not null)
                {
                    if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                            objFromDb.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    _unitOfWork.Bungalow.Delete(objFromDb);
                    _unitOfWork.Save();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<Bungalow> GetAllBungalow()
        {
            return _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity");
        }

        public Bungalow GetBungalowById(int id)
        {
            return _unitOfWork.Bungalow.Get(u => u.Id == id, includeProperties: "BungalowAmenity");
        }

        public IEnumerable<Bungalow> GetBungalowsAvailabilityByDate(int nights, DateOnly checkInDate)
        {
            var bungalowList = _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity").ToList();
            var bungalowNumbersList = _unitOfWork.BungalowNumber.GetAll().ToList();
            var bookedBungalows = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
                                                               u.Status == SD.StatusCheckedIn).ToList();


            foreach (var bungalow in bungalowList)
            {
                int roomAvailable = SD.BungalowRoomsAvailable_Count
                    (bungalow.Id, bungalowNumbersList, checkInDate, nights, bookedBungalows);

                bungalow.IsAvailable = roomAvailable > 0 ? true : false;
            }

            return bungalowList;
        }

        public bool IsBungalowAvailableByDate(int bungalowId, int nights, DateOnly checkInDate)
        {
            var bungalowNumbersList = _unitOfWork.BungalowNumber.GetAll().ToList();
            var bookedBungalows = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved ||
                                                               u.Status == SD.StatusCheckedIn).ToList();

            int roomAvailable = SD.BungalowRoomsAvailable_Count
                (bungalowId, bungalowNumbersList, checkInDate, nights, bookedBungalows);

            return roomAvailable > 0;
        }

        public void UpdateBungalow(Bungalow bungalow)
        {
            if (bungalow.Image != null)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(bungalow.Image.FileName);
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\BungalowImage");

                if (!string.IsNullOrEmpty(bungalow.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, bungalow.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
                bungalow.Image.CopyTo(fileStream);

                bungalow.ImageUrl = @"\images\BungalowImage\" + fileName;
            }

            _unitOfWork.Bungalow.Update(bungalow);
            _unitOfWork.Save();
        }
    }
}