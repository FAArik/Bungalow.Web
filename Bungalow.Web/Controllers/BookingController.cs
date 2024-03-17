using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public IActionResult FinalizeBooking(int bungalowId, int nights, DateOnly chkinDate)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ApplicationUser user = _unitOfWork.User.Get(x => x.Id == userId);
        Booking booking = new()
        {
            BungalowId = bungalowId,
            Bungalow = _unitOfWork.Bungalow.Get(b => b.Id == bungalowId, includeProperties: "BungalowAmenity"),
            CheckInDate = chkinDate,
            Nights = nights,
            CheckOutDate = chkinDate.AddDays(nights),
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

        var bungalowNumbersList = _unitOfWork.BungalowNumber.GetAll().ToList();
        var bookedBungalows = _unitOfWork.Booking.GetAll(x => x.Status == SD.StatusApproved || x.Status == SD.StatusCheckedIn).ToList();

        int roomsAvailable = SD.BungalowRoomsAvailable_Count(bungalow.Id, bungalowNumbersList, booking.CheckInDate, booking.Nights, bookedBungalows);

        if (roomsAvailable == 0)
        {
            TempData["error"] = "Room has been sold out!";
            return RedirectToAction(nameof(FinalizeBooking), new { bungalowId = booking.BungalowId, chkinDate = booking.CheckInDate, nights = booking.Nights });
        }


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
                _unitOfWork.Booking.UpdateStatus(bookingfromdb.Id, SD.StatusApproved, 0);
                _unitOfWork.Booking.UpdateStripePaymentId(bookingfromdb.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
            }
        }

        return View(bookingId);
    }

    [Authorize]
    public IActionResult BookingDetails(int bookingId)
    {
        Booking booking = _unitOfWork.Booking.Get(x => x.Id == bookingId, includeProperties: "User,Bungalow");


        if (booking.BungalowNumber == 0 && booking.Status == SD.StatusApproved)
        {
            var availableBungalowNumber = AssignAvailableBungalowNumberByBungalow(booking.BungalowId);

            booking.BungalowNumbers = _unitOfWork.BungalowNumber.GetAll().Where(u => u.BungalowId == booking.BungalowId && availableBungalowNumber.Any(x => x == u.Bungalow_Number)).ToList();
        }

        return View(booking);
    }

    private List<int> AssignAvailableBungalowNumberByBungalow(int bungalowId)
    {
        List<int> availableBungalowNumbers = new();
        var bungalwoNumbers = _unitOfWork.BungalowNumber.GetAll(x => x.BungalowId == bungalowId);

        var checkedInBungalow = _unitOfWork.Booking.GetAll(x => x.BungalowId == bungalowId && x.Status == SD.StatusCheckedIn).Select(x => x.BungalowNumber);

        foreach (var bungalowNumber in bungalwoNumbers)
        {
            if (!checkedInBungalow.Contains(bungalowNumber.Bungalow_Number))
            {
                availableBungalowNumbers.Add(bungalowNumber.Bungalow_Number);
            }

        }

        return availableBungalowNumbers;
    }


    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CheckIn(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCheckedIn, booking.BungalowNumber);
        _unitOfWork.Save();
        TempData["Success"] = "Booking Updated Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }
    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CheckOut(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCompleted, booking.BungalowNumber);
        _unitOfWork.Save();
        TempData["Success"] = "Booking Completed Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
    }
    [HttpPost]
    [Authorize(Roles = SD.Role_Admin)]
    public IActionResult CancelBooking(Booking booking)
    {
        _unitOfWork.Booking.UpdateStatus(booking.Id, SD.StatusCanceled, 0);
        _unitOfWork.Save();
        TempData["Success"] = "Booking Canceled Successfully.";
        return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
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
        if (!string.IsNullOrEmpty(status))
        {
            bookings = bookings.Where(x => x.Status.ToLower().Equals(status.ToLower()));
        }
        return Json(new { data = bookings });
    }
    #endregion
}
