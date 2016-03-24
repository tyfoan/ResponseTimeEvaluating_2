function ShowChart() {
    if ($('#radioAll').is(':checked')) {
        $('#ContainerAll').show();
        $('#ContainerMinMax').hide();
        $('#ContainerTenSlowest').hide();
    } else if ($('#radioMinMax').is(':checked')) {
        $('#ContainerAll').hide();
        $('#ContainerMinMax').show();
        $('#ContainerTenSlowest').hide();
    } else if ($('#radioTenSlowest').is(':checked')) {
        $('#ContainerAll').hide();
        $('#ContainerMinMax').hide();
        $('#ContainerTenSlowest').show();
    }
}

$(function () {
    //function DrawChart(responseTimeMs) {
    $('#ContainerAll').highcharts({
        chart: {
            renderTo: 'ContainerAll'
        },
        title: {
            text: 'Measure response time'
        },
        subtitle: {
            text: 'Source: WorldClimate.com'
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Time (msec)'
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.1f} ms</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        legend: {
            align: 'right',
            verticalAlign: 'top',
            layout: 'vertical',
            x: 0,
            y: 100
        },
        xAxis: {
            categories: ['1', '2', '3', '4', '5']
        }
    });
});


$("#siteMap a").click(MeasureResponseTime);
$('#siteMap a').click(function () {
    return false; //for good measure
});

var responseData = [];

function MeasureResponseTime() {

    var sendDate = (new Date()).getTime();
    var url = this.getAttribute('href');

    $.ajax({
        type: "HEAD",
        url: "http://localhost:25472" + url,
        success: function () {
            var receiveDate = (new Date()).getTime();
            var responseTimeMs = receiveDate - sendDate;

            responseData.push({
                Url: url,
                ResponseTimeMs: responseTimeMs
            });

            DrawChartWithAllResponses(url, responseTimeMs);      //url is ID and name.
            DrawChartMinMaxResponses();
            DrawChartTenSlowest();
        }
    });
    //$.ajax({
    //    url: "http://localhost:25472/" + "home/index",
    //    type: "POST",
    //    data: JSON.stringify(responseData),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json"
    //});  
}

function DrawChartWithAllResponses(url, responseTimeMs) {
    var chart = $('#ContainerAll').highcharts();

    if (chart.get(url)) {
        chart.get(url).addPoint(responseTimeMs);
    } else {
        chart.addSeries({ id: url, name: url })
             .addPoint(responseTimeMs);
    }
    //chart.plotOptions({
    //    column: {
    //        pointPadding: 1,
    //        borderWidth: 0
    //    }
    //});
}


function DrawChartMinMaxResponses() {
    responseData.sort(function (a, b) {
        return parseFloat(a.ResponseTimeMs) - parseFloat(b.ResponseTimeMs);
    });

    var minResponse = responseData[0];                       //min
    var maxResponse = responseData[responseData.length - 1]; // max

    $('#ContainerMinMax').highcharts({
        chart: {
            renderTo: 'ContainerMinMax',
            type: 'column'
        },
        title: {
            text: 'Min and max response time'
        },
        subtitle: {
            text: 'Source: WorldClimate.com'
        },
        xAxis: {
            categories: ['min/max'],
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Time (msec)'
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span> </br>' +
                          '<table>',
            pointFormat: '<tr>' +
                            '<td style="color:{series.color};padding:0">{series.name}: </td>' +
                            '<td style="padding:0"><b>{point.y:.1f} ms</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },
        series: [
            {
                name: 'min: ' + minResponse.Url,
                data: [minResponse.ResponseTimeMs]
            }, {
                name: 'max: ' + maxResponse.Url,
                data: [maxResponse.ResponseTimeMs]
            }
        ]
    });
}


function DrawChartTenSlowest() {
    $('#ContainerTenSlowest').highcharts({
        chart: {
            renderTo: 'ContainerTenSlowest',
            type: 'column'
        },
        title: {
            text: 'Min and max response time'
        },
        subtitle: {
            text: 'Source: WorldClimate.com'
        },
        xAxis: {
            categories: ["Ten slowest responses"],
            crosshair: true
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Time (msec)'
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span> </br>' +
                          '<table>',
            pointFormat: '<tr>' +
                            '<td style="color:{series.color};padding:0">{series.name}: </td>' +
                            '<td style="padding:0"><b>{point.y:.1f} ms</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        }
    });

    SlowestResponses();
}

function SlowestResponses() {
    responseData.sort(function (a, b) {
        return parseFloat(a.ResponseTimeMs) - parseFloat(b.ResponseTimeMs);
    });
    var chart = $('#ContainerTenSlowest').highcharts();
    if (responseData.length < 10) {
        for (var i = 0; i < responseData.length; i++) {
            chart.addSeries({ id: responseData[i].Url, name: responseData[i].Url })
             .addPoint(responseData[i].ResponseTimeMs);
        }
    } else {
        for (var i = responseData.length - 1; i >= responseData.length - 10; i--) {
            chart.addSeries({ id: responseData[i].Url, name: responseData[i].Url })
             .addPoint(responseData[i].ResponseTimeMs);

        }
    }
}
