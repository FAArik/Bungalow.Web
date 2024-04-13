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
        [HttpGet]
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
        [HttpGet]
        public async Task<IActionResult> GetRegisteredUserChartData()
        {
            var totalUsers = _unitOfWork.User.GetAll();

            var currentMonthCount = totalUsers.Count(u => u.CreatedAt >= currentMonthStartDate && u.CreatedAt <= DateTime.Now);

            var prevMonthCount = totalUsers.Count(u => u.CreatedAt >= previousMonthStartDate && u.CreatedAt <= currentMonthStartDate);

            return Json(GetRadialBarChartDataModel(totalUsers.Count(), currentMonthCount, prevMonthCount));
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueChartData()
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
        [HttpGet]
        public async Task<IActionResult> GetTotalBookinPieChartData()
        {
            var bookings = _unitOfWork.Booking.GetAll(u => u.BookingDate >= DateTime.Now.AddDays(-30) && (u.Status != SD.StatusPending || u.Status == SD.StatusCanceled));

            var customerWithOneBooking = bookings.GroupBy(x => x.UserId).Where(x => x.Count() == 1).Select(x => x.Key);
            int bookingsByNewCustomer = customerWithOneBooking.Count();
            int bookinsByReturningCustomer = bookings.Count() - bookingsByNewCustomer;

            PieChartVM pieChartVM = new()
            {
                Labels = ["New Customer Bookings", "Returning Customer Bookings"],
                Series = [bookingsByNewCustomer, bookinsByReturningCustomer]
            };
            return Json(pieChartVM);
        }
        public async Task<IActionResult> GetMemberAndBookinLineChartData()
        {
            var booking = _unitOfWork.Booking.GetAll(x => x.BookingDate >= DateTime.Now.AddDays(-30) && x.BookingDate.Date <= DateTime.Now)
                .GroupBy(x => x.BookingDate.Date)
                .Select(u => new
                {
                    DateTime = u.Key,
                    NewBookingCount = u.Count()
                });

            var customerData = _unitOfWork.User.GetAll(x => x.CreatedAt >= DateTime.Now.AddDays(-30) && x.CreatedAt.Date <= DateTime.Now)
                .GroupBy(x => x.CreatedAt.Date)
                .Select(x => new
                {
                    DateTime = x.Key,
                    NewCustomerCount = x.Count()
                });

            var leftJoin = booking.GroupJoin(customerData, book => book.DateTime, customer => customer.DateTime,
                (book, customer) => new
                {
                    book.DateTime,
                    book.NewBookingCount,
                    NewCustomerCount = customer.Select(x => x.NewCustomerCount).FirstOrDefault()
                });

            var rightJoin = customerData.GroupJoin(booking, customer => customer.DateTime, book => book.DateTime,
                (customer, book) => new
                {
                    customer.DateTime,
                    NewBookingCount = book.Select(x => x.NewBookingCount).FirstOrDefault(),
                    customer.NewCustomerCount
                });

            var mergedData = leftJoin.Union(rightJoin).OrderBy(x => x.DateTime).ToList();

            var newbookingData = mergedData.Select(x => x.NewBookingCount).ToArray();
            var newcustomerData = mergedData.Select(x => x.NewCustomerCount).ToArray();
            var categories = mergedData.Select(x => x.DateTime.ToString("dd/MM/yyyy")).ToArray();

            List<ChartData> chartData = new()
            {
                new ChartData
                {
                    Name = "New Bookings",
                    Data=newbookingData
                },
                new ChartData
                {
                    Name="New Members",
                    Data=newcustomerData
                },
            };
            LineChartVM lineChartVM = new()
            {
                Categories = categories,
                Series=chartData
            };
            
            return Json(booking);
        }
    }
}
