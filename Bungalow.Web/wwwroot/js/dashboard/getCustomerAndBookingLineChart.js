$(document).ready(() => {
    loadCustomerAndBookingLineChart();
})

function loadCustomerAndBookingLineChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetMemberAndBookinLineChartData",
        type: "GET",
        dataType: "json",
        success: (data) => {
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
                curve: 'smooth',

            },
            markers: {
                size: 6,
                strokeWidth: 0,
                hover: {
                    size: 9
                }
            },
            xaxis: {
                categories: data.categories,
                labels: {
                    style: {
                        colors: '#ddd'
                    }
                }
            },
            yaxis: {
                labels: {
                    style: {
                        colors: '#fff'
                    }
                }
            },
            legend: {
                labels: {
                    colors: '#fff'
                }
            },
            tooltip: {
                theme: 'dark'
            }
        };

        var chart = new ApexCharts(document.querySelector("#" + id), options);

        chart.render();
    });
}
