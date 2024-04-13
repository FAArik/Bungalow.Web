$(document).ready(() => {
    loadTotalRevenueRadChart();
})

function loadTotalRevenueRadChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetRevenueChartData",
        type: "GET",
        dataType: "json",
        success: (data) => {
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;
            var sectionCurrentCount = document.createElement("span");
            if (data.ratioIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class=bi bi-arrow-up-right-circle me-1></i> <span>' + data.countInCurrentMonth + '</span>'
            }
            else {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class=bi bi-arrow-down-right-circle me-1></i> <span>' + data.countInCurrentMonth + '</span>'
            }
            document.querySelector("#sectionRevenueCount").append(sectionCurrentCount);
            document.querySelector("#sectionRevenueCount").append('since last month');

            loadradialBarChart("totalRevenueRadialChart", data);
            $(".chart-spinner").hide();

        }
    })
}
