$(document).ready(() => {
    console.log("asdas")
    loadCustomerBookingPieChart();
})

function loadCustomerBookingPieChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetMemberAndBookinLineChartData",
        type: "GET",
        dataType: "json",
        success: (data) => {
            alert(data)
            loadLineChart("newMembersAndBookingsLineChart", data);
            
            $(".chart-spinner").hide();
        }
    })
}

async function loadLineChart(id, data) {
    await getChartColorsArray(id).then((chartColors) => {
        var options = {
            series: data.series,
            colors: chartColors,
            chart: {
                type: 'line',
                height: 350
            },
            stroke: {
                show: false
            },
            legend: {
                position: 'bottom',
                horizontalAlign: 'center',
                labels: {
                    colors: "#fff",
                    useSeriesColors: true
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#" + id), options);

        chart.render();
    });
}
