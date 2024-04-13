﻿$(document).ready(() => {
    loadTotalUserRadChart();
})

function loadTotalUserRadChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetRegisteredUserChartData",
        type: "GET",
        dataType: "json",
        success: (data) => {
            document.querySelector("#spanTotalUserCount").innerHTML = data.totalCount;
            var sectionCurrentCount = document.createElement("span");
            if (data.ratioIncreased) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class=bi bi-arrow-up-right-circle me-1></i> <span>' + data.countInCurrentMonth + '</span>'
            }
            else {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class=bi bi-arrow-down-right-circle me-1></i> <span>' + data.countInCurrentMonth + '</span>'
            }
            document.querySelector("#sectionUserCount").append(sectionCurrentCount);
            document.querySelector("#sectionUserCount").append('since last month');

            loadradialBarChart("totalUserRadialChart", data);
            $(".chart-spinner").hide();

        }
    })
}
