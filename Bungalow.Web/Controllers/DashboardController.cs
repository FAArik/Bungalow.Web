using BungalowApi.Application.Common.Interfaces;
using BungalowApi.Application.Common.Utility;
using BungalowApi.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BungalowApi.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        static int previousMonth = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1;
        readonly DateTime previousMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
        readonly DateTime currentMonthStartDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTotalBookingRadialChartData()
        {
            var bookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCanceled);

            var currentMonthCount = bookings.Count(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now);

            var prevMonthCount = bookings.Count(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate);

            RadialBarChartVm vm = new();

            int increaseDecreaseRatio = 100;
            if (prevMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
            }
            vm.TotalCount = bookings.Count();
            vm.CountInCurrentMonth = currentMonthCount;
            vm.hasRatioIncreased = currentMonthStartDate > previousMonthStartDate;
            vm.Series = new int[] { increaseDecreaseRatio };

            return Json(GetRadialBarChartDataModel(bookings.Count(), currentMonthCount, prevMonthCount));
        }


        public async Task<IActionResult> GetRegisteredUserChartDataAsync()
        {
            var totalUsers = _unitOfWork.User.GetAll();

            var currentMonthCount = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.Now);

            var prevMonthCount = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate && u.CreatedAt <= currentMonthStartDate);

            return Json(GetRadialBarChartDataModel(totalUsers.Count(), currentMonthCount, prevMonthCount));
        }

        public async Task<IActionResult> GetRevenueChartDataAsync()
        {
            var totalBookings = _unitOfWork.Booking.GetAll(u => u.Status != SD.StatusPending || u.Status == SD.StatusCanceled);

            var totalRevenue = Convert.ToInt32(totalBookings.Sum(x => x.TotalCost));

            var currentMonthCount = totalBookings.Where(u => u.BookingDate >= currentMonthStartDate && u.BookingDate <= DateTime.Now).Sum(x => x.TotalCost);

            var prevMonthCount = totalBookings.Where(u => u.BookingDate >= previousMonthStartDate && u.BookingDate <= currentMonthStartDate).Sum(x => x.TotalCost);

            return Json(GetRadialBarChartDataModel(totalBookings.Count(), currentMonthCount, prevMonthCount));
        }

        private static RadialBarChartVm GetRadialBarChartDataModel(int totalCount, double currentMonthCount, double prevMonthCount)
        {
            RadialBarChartVm vm = new();

            int increaseDecreaseRatio = 100;
            if (prevMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - prevMonthCount) / prevMonthCount * 100);
            }
            vm.TotalCount = totalCount;
            vm.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            vm.hasRatioIncreased = currentMonthCount > prevMonthCount;
            vm.Series = new int[] { increaseDecreaseRatio };

            return vm;
        }
    }
}
