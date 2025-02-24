using BungalowApi.Application.Services.Interface;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Presentation;

namespace BungalowApi.Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IBungalowService _bungalowService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IBungalowService bungalowService, IWebHostEnvironment webHostEnvironment)
        {
            _bungalowService = bungalowService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                BungalowList = _bungalowService.GetAllBungalow(),
                Nights=1,
                CheckInDate =DateOnly.FromDateTime(DateTime.Now),
            };
            return View(homeVM);
        }

        [HttpPost]
        public IActionResult GetBungalowsByDate(int nights, DateOnly checkInDate) 
        {
           
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                BungalowList = _bungalowService.GetBungalowsAvailabilityByDate(nights,checkInDate),
                Nights = nights
            };

            return PartialView("_BungalowList",homeVM);
        }

        [HttpPost]
        public IActionResult GeneratePPTExport(int id)
        {
            var bungalow = _bungalowService.GetBungalowById(id);
            if (bungalow is null)
            {
                return RedirectToAction(nameof(Error));
            }

            string basePath = _webHostEnvironment.WebRootPath;
            string filePath = basePath + @"/Exports/ExportBungalowDetails.pptx";


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
                byte[] imageData;
                string imageUrl;
                try
                {
                    imageUrl= string.Format("{0}{1}", basePath, bungalow.ImageUrl);
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                catch (Exception)
                {
                    imageUrl = string.Format("{0}{1}", basePath, "/images/placeholder.png");
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                slide.Shapes.Remove(shape); 
                using MemoryStream imageStream = new(imageData);
                IPicture newPicture = slide.Pictures.AddPicture(imageStream, 60,120,300,200);
            }
            MemoryStream memoryStream = new();
            presentation.Save(memoryStream);
            memoryStream.Position = 0;
            return File(memoryStream,"application/pptx","Bungalow.pptx");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}