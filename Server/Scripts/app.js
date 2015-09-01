$(function () {
    $('#main-tab a:first').tab('show');
    var moveShapeHub = $.connection.tDS;
    //$shape = $("#shape"),
    //shapeModel = {
    //    left: 100,
    //    top: 100
    //};


    $('#start').click(function () {
        moveShapeHub.server.start();
    });

    $('#end').click(function () {
        moveShapeHub.server.stop();
    });

    moveShapeHub.client.getData = function (data) {
        //$('#info').append(data);
        // d3_plot(data);
        $('#container').highcharts({
            title: {
                text: 'Monthly Average Temperature',
                x: -20 //center
            },
            subtitle: {
                text: 'Source: WorldClimate.com',
                x: -20
            },
            yAxis: {
                title: {
                    text: 'Temperature (°C)'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            }
            ,
            series: [{
                name: 'Data',
                data: data
            }],
            plotOptions: {
                line: {
                    animation: false
                }
            }
        });
    }
    $.connection.hub.start().done(function () {
        //$shape.draggable({
        //    drag: function () {
        //        shapeModel = $shape.offset();
        //        moveShapeHub.server.updateModel(shapeModel);
        //    }
        //});
        // $('#info').html(moveShapeHub.client.getData());
    });
});