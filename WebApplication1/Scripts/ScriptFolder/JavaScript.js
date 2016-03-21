$(function () {
    //function DrawChart(responseTimeMs) {
    $('#container').highcharts({
        chart: {
            type: 'column',
            renderTo: 'container'
        },
        title: {
            text: 'Measure response time'
        },
        subtitle: {
            text: 'Source: WorldClimate.com'
        },
        xAxis: {
            categories: [
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',
                '10',
                '11',
                '12'
            ],
            crosshair: true
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
                '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
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
        series: [{
            name: 'Show all responses',
            data: []
        }, {
            name: 'Show min/max responses',
            data: []
        }, {
            name: 'Show 10 the slowest responses',
            data: []
        }]
    });
});


$("#siteMap a").click(MeasureResponseTime);
$('#siteMap a').click(function () {
    return false; //for good measure
});

//var responseData = {};

function MeasureResponseTime() {

    var sendDate = (new Date()).getTime();
    var url = this.getAttribute('href');

    $.ajax({
        type: "HEAD",
        url: "http://localhost:25472" + url,
        success: function () {
            var receiveDate = (new Date()).getTime();
            var responseTimeMs = receiveDate - sendDate;

            DrawChart(receiveDate - sendDate);
            //responseData = {
            //    Url: url,
            //    ResponseTime: responseTimeMs
            //}
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

function DrawChart(responseTimeMs, itemNumb) {
    var chart = $('#container').highcharts();
    chart.series[0].addPoint([                      //series[0] => 'All responses'
        itemNumb, responseTimeMs
    ]);
}

$("#evalueate").click(DataEval);

function DataEval() {

    var seriesData = [];
    var slowestData = [];

    var chart = $('#container').highcharts();


    for (var i = 0; i < chart.series[0].data.length; i++) {
        seriesData.push(chart.series[0].data[i].y);
    }

    var max = seriesData.sort()[seriesData.length - 1];
    var min = seriesData.sort()[0];

    seriesData.reverse();
    for (var j = 0; j < 10; j++) {
        slowestData.push(seriesData[j]);  //get 10 the slowest responses
    }

    AddSerieses(slowestData, min, max);
}


function AddSerieses(slowestData, min, max) {
    var chart = $('#container').highcharts();

    for (var i = 0; i < slowestData.length; i++) {
        chart.series[2].addPoint([     //series[2] => '10 the slowest'
        i, slowestData[i]
        ]);
    }

    chart.series[1].addPoint([0, min]); //series[1] => 'min/max responses' MIN
    chart.series[1].addPoint([1, max]); //series[1] => 'min/max responses' MAX
}