using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Application.Common.DTO;
using BungalowApi.Application.Services.Interface;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BungalowApi.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            return Json(await _dashboardService.GetTotalBookingRadialChartData());
        }

        [HttpGet]
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            return Json(await _dashboardService.GetRegisteredUserChartData());
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueChartData()
        {
            return Json(await _dashboardService.GetRevenueChartData());
        }

        [HttpGet]
        public async Task<IActionResult> GetTotalBookinPieChartData()
        {
            return Json(await _dashboardService.GetTotalBookinPieChartData());
        }

        public async Task<IActionResult> GetMemberAndBookinLineChartData()
        {
            return Json(await _dashboardService.GetMemberAndBookinLineChartData());
        }
    }
}