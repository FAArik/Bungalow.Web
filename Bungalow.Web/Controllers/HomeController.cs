using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Web.Models;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Presentation;

namespace BungalowApi.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                BungalowList = _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now)
            };
            return View(homeVM);
        }

        [HttpPost]
        public IActionResult GetBungalowsByDate(int nights, DateOnly checkInDate)
        {
            Thread.Sleep(2000);
            var bungalowList = _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity").ToList();
            var bungalowNumbersList = _unitOfWork.BungalowNumber.GetAll().ToList();
            var bookedBungalows = _unitOfWork.Booking
                .GetAll(x => x.Status == SD.StatusApproved || x.Status == SD.StatusCheckedIn).ToList();
            foreach (var bungalow in bungalowList)
            {
                int roomsAvailable = SD.BungalowRoomsAvailable_Count(bungalow.Id, bungalowNumbersList, checkInDate,
                    nights, bookedBungalows);
                bungalow.IsAvailable = roomsAvailable > 0;
            }

            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                BungalowList = bungalowList,
                Nights = nights
            };
            return PartialView("_BungalowList", homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GeneratePPTExport(int id)
        {
            var bungalow = _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity")
                .FirstOrDefault(x => x.Id == id);
            if (bungalow is null)
            {
                return RedirectToAction(nameof(Error));
            }

            string webRootPath = _webHostEnvironment.WebRootPath;
            string filePath = webRootPath + @"/Exports/ExportBungalowDetails.pptx";

            using IPresentation presentation = Presentation.Open(filePath);

            ISlide slide = presentation.Slides[0];

            IShape shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtBungalowName") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = bungalow.Name;
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtBungalowDescription") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = bungalow.Description;
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtOccupancy") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Bungalow Occupancy: {0} adults", bungalow.Occupancy);
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtBungalowSize") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Bungalow Size: {0} sqft", bungalow.Sqft);
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtPricePerNight") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("USD {0}/night", bungalow.Price.ToString("C"));
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtBungalowAmenities") as IShape;
            if (shape is not null)
            {
                List<string> listItems = bungalow.BungalowAmenity.Select(x => x.Name).ToList();
                shape.TextBody.Text = "";

                foreach (var item in listItems)
                {
                    IParagraph paragraph = shape.TextBody.AddParagraph();
                    ITextPart textPart = paragraph.AddTextPart(item);

                    paragraph.ListFormat.Type = ListType.Bulleted;
                    paragraph.ListFormat.BulletCharacter = '\u2022';
                    textPart.Font.FontName = "system-ui";
                    textPart.Font.FontSize = 18;
                    textPart.Font.Color = ColorObject.FromArgb(144, 148, 152);
                }
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "imgBungalow") as IShape;
            if (shape is not null)
            {
                byte[] imgData;
                string imgUrl;
                try
                {
                    imgUrl = string.Format("{0}{1}", webRootPath, bungalow.ImageUrl);
                    imgData = System.IO.File.ReadAllBytes(imgUrl);
                }
                catch (Exception e)
                {
                    imgUrl = string.Format("{0}{1}", webRootPath, "/images/placeholder.png");
                    imgData = System.IO.File.ReadAllBytes(imgUrl);
                }

                slide.Shapes.Remove(shape);
                using MemoryStream memoryStream = new(imgData);
                IPicture picture = slide.Pictures.AddPicture(memoryStream, 60, 120, 300, 200);
            }

            MemoryStream stream = new MemoryStream();
            presentation.Save(stream);
            stream.Position = 0;
            return File(stream, "application/pdf", "Bungalow.pptx");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}