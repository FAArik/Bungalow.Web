using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;

namespace BungalowApi.Web.Controllers;

public class BookingController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    public BookingController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
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
        var bookedBungalows = _unitOfWork.Booking
            .GetAll(x => x.Status == SD.StatusApproved || x.Status == SD.StatusCheckedIn).ToList();

        int roomsAvailable = SD.BungalowRoomsAvailable_Count(bungalow.Id, bungalowNumbersList, booking.CheckInDate,
            booking.Nights, bookedBungalows);

        if (roomsAvailable == 0)
        {
            TempData["error"] = "Room has been sold out!";
            return RedirectToAction(nameof(FinalizeBooking),
                new { bungalowId = booking.BungalowId, chkinDate = booking.CheckInDate, nights = booking.Nights });
        }


        _unitOfWork.Booking.Add(booking);
        _unitOfWork.Save();

        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new SessionCreateOptions
        {
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
            SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
            CancelUrl = domain +
                        $"booking/FinalizeBooking?bungalowId={booking.BungalowId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
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

            booking.BungalowNumbers = _unitOfWork.BungalowNumber.GetAll().Where(u =>
                    u.BungalowId == booking.BungalowId && availableBungalowNumber.Any(x => x == u.Bungalow_Number))
                .ToList();
        }

        return View(booking);
    }


    public IActionResult GenerateInvoice(int id, string downloadType)
    {
        string basePath = _webHostEnvironment.WebRootPath;

        WordDocument doc = new WordDocument();

        string dataPath = basePath + @"/exports/BookingDetails.docx";
        using FileStream fileStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        doc.Open(fileStream, FormatType.Automatic);

        Booking bookingFromDb = _unitOfWork.Booking.Get(x => x.Id == id, includeProperties: "User,Bungalow");

        #region Change Text on document

        TextSelection selection = doc.Find("xx_customer_name", false, true);
        WTextRange textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.Name;

        selection = doc.Find("xx_customer_phone", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.Phone;

        selection = doc.Find("xx_customer_email", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.Email;

        selection = doc.Find("xx_payment_date", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.PaymentDate.ToShortDateString();

        selection = doc.Find("xx_checkin_date", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.CheckInDate.ToShortDateString();

        selection = doc.Find("xx_checkout_date", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.CheckOutDate.ToShortDateString();

        selection = doc.Find("xx_booking_total", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = bookingFromDb.TotalCost.ToString("C");

        selection = doc.Find("XX_BOOKING_NUMBER", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = "BOOKING NUMBER - " + bookingFromDb.Id;

        selection = doc.Find("XX_BOOKING_DATE", false, true);
        textRange = selection.GetAsOneRange();
        textRange.Text = "BOOKING DATE - " + bookingFromDb.BookingDate.ToShortDateString();

        #endregion

        WTable table = new(doc);
        table.TableFormat.Borders.LineWidth = 1f;
        table.TableFormat.Borders.Color = Color.Black;
        table.TableFormat.Paddings.Top = 7f;
        table.TableFormat.Paddings.Bottom = 7f;
        table.TableFormat.Borders.Horizontal.LineWidth = 1f;

        int rows = bookingFromDb.BungalowNumber > 0 ? 3 : 2;
        table.ResetCells(rows, 4);
        WTableRow row0 = table.Rows[0];
        row0.Cells[0].AddParagraph().AppendText("NIGHTS");
        row0.Cells[0].Width = 80;
        row0.Cells[1].AddParagraph().AppendText("BUNGALOW");
        row0.Cells[1].Width = 220;
        row0.Cells[2].AddParagraph().AppendText("PRICE PER NIGHT");
        row0.Cells[3].AddParagraph().AppendText("TOTAL");
        row0.Cells[3].Width = 80;

        WTableRow row1 = table.Rows[1];
        row1.Cells[0].AddParagraph().AppendText(bookingFromDb.Nights.ToString());
        row0.Cells[0].Width = 80;
        row1.Cells[1].AddParagraph().AppendText(bookingFromDb.Bungalow.Name);
        row0.Cells[1].Width = 220;
        row1.Cells[2].AddParagraph().AppendText((bookingFromDb.TotalCost / bookingFromDb.Nights).ToString("C"));
        row1.Cells[3].AddParagraph().AppendText(bookingFromDb.TotalCost.ToString("C"));
        row0.Cells[3].Width = 80;

        if (bookingFromDb.BungalowNumber > 0)
        {
            WTableRow row2 = table.Rows[2];
            row2.Cells[0].Width = 80;
            row2.Cells[1].AddParagraph().AppendText("Bungalow Number - " + bookingFromDb.BungalowNumber.ToString());
            row2.Cells[1].Width = 220;
            row2.Cells[3].Width = 80;
        }

        WTableStyle tableStyle = doc.AddTableStyle("CustomStyle");
        tableStyle.TableProperties.RowStripe = 1;
        tableStyle.TableProperties.ColumnStripe = 2;
        tableStyle.TableProperties.Paddings.Top = 2;
        tableStyle.TableProperties.Paddings.Bottom = 1;
        tableStyle.TableProperties.Paddings.Left = 5.4f;
        tableStyle.TableProperties.Paddings.Right = 5.4f;

        ConditionalFormattingStyle firstRowStyle =
            tableStyle.ConditionalFormattingStyles.Add(ConditionalFormattingType.FirstRow);
        firstRowStyle.CharacterFormat.Bold = true;
        firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
        firstRowStyle.CellProperties.BackColor = Color.Black;
        table.ApplyStyle("CustomStyle");

        TextBodyPart bodyPart = new(doc);
        bodyPart.BodyItems.Add(table);

        doc.Replace("<ADDTABLEHERE>", bodyPart, false, false);

        using DocIORenderer renderer = new DocIORenderer();
        MemoryStream stream = new MemoryStream();
        if (downloadType == "word")
        {
            doc.Save(stream, FormatType.Docx);
            stream.Position = 0;
            return File(stream, "application/docx", "BookingDetails.docx");
        }

        PdfDocument pdfDoc = renderer.ConvertToPDF(doc);
        pdfDoc.Save(stream);
        stream.Position = 0;
        return File(stream, "application/pdf", "BookingDetails.pdf");
    }

    private List<int> AssignAvailableBungalowNumberByBungalow(int bungalowId)
    {
        List<int> availableBungalowNumbers = new();
        var bungalwoNumbers = _unitOfWork.BungalowNumber.GetAll(x => x.BungalowId == bungalowId);

        var checkedInBungalow = _unitOfWork.Booking
            .GetAll(x => x.BungalowId == bungalowId && x.Status == SD.StatusCheckedIn).Select(x => x.BungalowNumber);

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