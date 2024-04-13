async function loadradialBarChart(id, data) {
    await getChartColorsArray(id).then((chartColors) => {
        var options = {
            colors: chartColors,
            chart: {
                height: 90,
                width: 90,
                type: "radialBar",
                sparkline: {
                    enabled: true
                },
                offsetY: -10
            },

            series: data.series,

            plotOptions: {
                radialBar: {
                    dataLabels: {
                        value: {
                            offsetY: -10,
                            color: chartColors[0]
                        }
                    }
                }
            },
            labels: [""]
        };

        var chart = new ApexCharts(document.querySelector("#" + id), options);

        chart.render();
    });
}

async function getChartColorsArray(id) {
    if (document.getElementById(id) != null) {
        var colors = document.getElementById(id).getAttribute("data-colors")
        if (colors) {
            colors = JSON.parse(colors)
            return colors.map((value) => {
                var newValue = value.replace(" ", "");
                if (newValue.indexOf(",") === -1) {
                    var color = getComputedStyle(document.documentElement).getPropertyValue(newValue);
                    if (color) return color;
                    else return newValue;  
                }
            })
        }
    }
}