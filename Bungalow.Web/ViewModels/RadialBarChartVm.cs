namespace BungalowApi.Web.ViewModels;

public class RadialBarChartVm
{
    public decimal TotalCount { get; set; }
    public decimal CountInCurrentMonth { get; set; }
    public bool hasRatioIncreased { get; set; }
    public int[] Series { get; set; }
}