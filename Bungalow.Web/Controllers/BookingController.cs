using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BungalowApi.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    public BookingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [Authorize]
    public IActionResult FinalizeBooking(int bungalowId, DateOnly checkInDate, int nights)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ApplicationUser user = _unitOfWork.User.Get(x => x.Id == userId);
        Booking booking = new()
        {
            BungalowId = bungalowId,
            Bungalow = _unitOfWork.Bungalow.Get(b => b.Id == bungalowId, includeProperties: "BungalowAmenity"),
            CheckInDate = checkInDate,
            Nights = nights,
            CheckOutDate = checkInDate.AddDays(nights),
            UserId = userId,
            Phone = user.PhoneNumber,
            Email = user.Email,
            Name = user.Name
        };
        booking.TotalCost = booking.Bungalow.Price * nights;
        return View(booking);
    }
    [Authorize]
    [HttpPost]
    public IActionResult FinalizeBooking(Booking booking)
    {
        var bungalow = _unitOfWork.Bungalow.Get(u => u.Id == booking.BungalowId);
        booking.TotalCost = bungalow.Price * booking.Nights;

        booking.Status = SD.StatusPending;
        booking.BookingDate = DateTime.Now;

        _unitOfWork.Booking.Add(booking);
        _unitOfWork.Save();

        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
            CancelUrl = domain + $"booking/FinalizeBooking?bungalowId={booking.BungalowId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
        };
        options.LineItems.Add(new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                UnitAmount = (long)(booking.TotalCost * 100),
                Currency = "try",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = bungalow.Name,
                    // Images = new List<string> { domain + bungalow.ImageUrl },
                },
            },
            Quantity = 1,
        });

        var service = new SessionService();
        Session session = service.Create(options);

        _unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();
        Response.Headers.Add("Location", session.Url);

        return new StatusCodeResult(303);
    }

    [Authorize]
    public IActionResult BookingConfirmation(int bookingId)
    {
        Booking bookingfromdb = _unitOfWork.Booking.Get(x => x.Id == bookingId, includeProperties: "User,Bungalow");

        if (bookingfromdb.Status == SD.StatusPending)
        {
            var service = new SessionService();
            Session session = service.Get(bookingfromdb.StripeSessionId);

            if (session.PaymentStatus == "paid")
            {
                _unitOfWork.Booking.UpdateStatus(bookingfromdb.Id, SD.StatusApproved);
                _unitOfWork.Booking.UpdateStripePaymentId(bookingfromdb.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
            }
        }

        return View(bookingId);
    }

    #region apiCalls
    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<Booking> bookings;
        if (User.IsInRole(SD.Role_Admin))
        {
            bookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Bungalow");
        }
        else
        {
            var claims = (ClaimsIdentity)User.Identity;
            var userId = claims.FindFirst(ClaimTypes.NameIdentifier).Value;
            bookings = _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Bungalow");
        }
        if (string.IsNullOrEmpty(status))
        {
            bookings = bookings.Where(x => x.Status.ToLower().Equals(status.ToLower()));
        }
        return Json(new { data = bookings });
    }
    #endregion
}
