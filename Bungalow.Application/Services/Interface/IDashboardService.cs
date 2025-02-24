using BungalowApi.Application.Common.DTO;
using BungalowApi.Web.ViewModels;

namespace BungalowApi.Application.Services.Interface;

public interface IDashboardService
{
    Task<RadialBarChartDTO> GetTotalBookingRadialChartData();
    Task<RadialBarChartDTO> GetRegisteredUserChartData();
    Task<RadialBarChartDTO> GetRevenueChartData();
    Task<PieChartDTO> GetTotalBookinPieChartData();
    Task<LineChartDTO> GetMemberAndBookinLineChartData();
}