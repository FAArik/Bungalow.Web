using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Web.Models;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BungalowApi.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.BungalowList = _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity");
            foreach (var bungalow in homeVM.BungalowList)
            {
                if (bungalow.Id % 2 == 0)
                {
                    bungalow.IsAvailable = false;
                }
            }
            return View(homeVM);
        }
        public IActionResult GetBungalowsByDate(int nights, DateOnly checkInDate)
        {
            Thread.Sleep(2000);
            var bungalowList = _unitOfWork.Bungalow.GetAll(includeProperties: "BungalowAmenity").ToList();
            foreach (var bungalow in bungalowList)
            {
                if (bungalow.Id % 2 == 0)
                {
                    bungalow.IsAvailable = false;
                }
            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                BungalowList = bungalowList,
                Nights = nights
            };
            return PartialView("_BungalowList",homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}