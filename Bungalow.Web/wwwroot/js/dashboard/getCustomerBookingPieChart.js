$(document).ready(() => {
    loadCustomerBookingPieChart();
})

function loadCustomerBookingPieChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetTotalBookinPieChartData",
        type: "GET",
        dataType: "json",
        success: (data) => {

            loadPieChart("customerBookingsPieChart", data);
            $(".chart-spinner").hide();

        }
    })
}

async function loadPieChart(id, data) {
    await getChartColorsArray(id).then((chartColors) => {
        var options = {
            series: data.series,
            labels: data.labels,
            colors: chartColors,
            chart: {
                type: 'pie',
                width: 380
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
