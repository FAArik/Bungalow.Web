using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Domain.Entities;

namespace BungalowApi.Application.Services.Implementation;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void CreateBooking(Booking booking)
    {
        _unitOfWork.Booking.Add(booking);
        _unitOfWork.Save();
    }

    public IEnumerable<Booking> GetAllBookings(string userId = "", string? statusFilterList = "")
    {
        IEnumerable<string> statusList = statusFilterList.ToLower().Split(",");
        if (!string.IsNullOrEmpty(statusFilterList) && !string.IsNullOrEmpty(userId))
        {
            return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()) &&
                                                   u.UserId == userId, includeProperties: "User,Bungalow");
        }
        else
        {
            if (!string.IsNullOrEmpty(statusFilterList))
            {
                return _unitOfWork.Booking.GetAll(u => statusList.Contains(u.Status.ToLower()),
                    includeProperties: "User,Bungalow");
            }

            if (!string.IsNullOrEmpty(userId))
            {
                return _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Bungalow");
            }
        }

        return _unitOfWork.Booking.GetAll(includeProperties: "User,Bungalow");
    }

    public Booking GetBookingById(int bookingId)
    {
        return _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Bungalow");
    }

    public IEnumerable<int> GetCheckedInBungalowNumbers(int bungalowId)
    {
        return _unitOfWork.Booking.GetAll(u => u.BungalowId == bungalowId && u.Status == SD.StatusCheckedIn)
            .Select(u => u.BungalowNumber);
    }

    public void UpdateStatus(int bookingId, string bookingStatus, int bungalowNumber = 0)
    {
        var bookingFromDb = _unitOfWork.Booking.Get(m => m.Id == bookingId, tracked: true);
        if (bookingFromDb != null)
        {
            bookingFromDb.Status = bookingStatus;
            if (bookingStatus == SD.StatusCheckedIn)
            {
                bookingFromDb.BungalowNumber = bungalowNumber;
                bookingFromDb.ActualCheckInDate = DateTime.Now;
            }

            if (bookingStatus == SD.StatusCompleted)
            {
                bookingFromDb.ActualCheckOutDate = DateTime.Now;
            }
        }

        _unitOfWork.Save();
    }

    public void UpdateStripePaymentID(int bookingId, string sessionId, string paymentIntentId)
    {
        var bookingFromDb = _unitOfWork.Booking.Get(m => m.Id == bookingId, tracked: true);
        if (bookingFromDb != null)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                bookingFromDb.StripeSessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                bookingFromDb.StriptePaymentIntentId = paymentIntentId;
                bookingFromDb.PaymentDate = DateTime.Now;
                bookingFromDb.IsPaymentSuccessful = true;
            }
        }

        _unitOfWork.Save();
    }
}