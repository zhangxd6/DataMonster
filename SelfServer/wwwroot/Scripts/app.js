$(function () {
    $('#main-tab a:first').tab('show');
    var moveShapeHub = $.connection.tDS;
    var stepHub = $.connection.stepScope;
    var cameraHub = $.connection.alliedCameraScope;

    $('#stepstart').click(function () {
      var init = $('#initialVotage').val();
      var end = $('#finalVoltage').val();
      var step = $('#stepVoltage').val();
      var numberCurve = $('#numberCurve').val();
      stepHub.server.start(init,end,step,numberCurve);
    });
    $('#stepend').click(function () {
      stepHub.server.stop();
    });

    $('#start').click(function () {
        moveShapeHub.server.start();
    });

    $('#end').click(function () {
        moveShapeHub.server.stop();
    });
    $('#startCamera').click(() => {
        cameraHub.server.start().done(() => {
            alert('Camera Started')
        });
    });
    $('#endCamera').click(() => {
        cameraHub.server.stop().done(() => {
            alert('Camera Stoped ')
        })
    });
    $('#takePicture').click(() => {
        cameraHub.server.takePicture().done(response => {
            $('#snapshot').attr('src', response.ImageSrc);
            $('#hist').highcharts({
                title: { text: "Histograms" },
                subtitle: { text: "raw" },
                legend: {
                    layout: 'vertical',
                    align: 'right',
                    verticalAlign: 'middle'
                },
                plotOptions: {
                    series: {
                        label: {
                            connectorAllowed: false
                        }
                    }
                },
                series: [
                    {
                        name: 'red',
                        data: response.Red
                    }, {
                        name: 'blue',
                        data: response.Blue
                    }, {
                        name: 'green',
                        data: response.Green
                    }
                ]
            })
        });
    })
    function mapping(point) {
        var arr = [];
        arr.push(point.X);
        arr.push(point.Y);
        return arr;
    }
    moveShapeHub.client.getData = function (data) {
        //$('#info').append(data);
        // d3_plot(data);
        $('#orginalchart').highcharts({
            title: {
              text: 'raw'+ data.MetaData.CurveId,
                x: -20 //center
            },
            subtitle: {
                text: 'Raw',
                x: -20
            },
            yAxis: {
                title: {
                  text: 'Point'
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
                data: data.Orginal
            }],
            plotOptions: {
                line: {
                    animation: false
                }
            }
        });
        $('#curveChart').highcharts({
            title: {
                text: data.MetaData.CurveId,
                x: -20 //center
            },
            yAxis: {
                title: {
                    text: 'Voltage(V)'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            }
            ,
            series: [{
                name: "SCOPE" ,
                data: data.Points.map(mapping)
            }],
            plotOptions: {
                line: {
                    animation: false
                }
            }
        })
    }
    stepHub.client.getData = function (data) {
      //$('#info').append(data);
      // d3_plot(data);
      $('#tabpanelorginalchart').highcharts({
        title: {
          text: 'raw' + data.MetaData.CurveId,
          x: -20 //center
        },
        subtitle: {
          text: 'Raw',
          x: -20
        },
        yAxis: {
          title: {
            text: 'Point'
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
          data: data.Orginal
        }],
        plotOptions: {
          line: {
            animation: false
          }
        }
      });
      $('#tabpanelcurveChart').highcharts({
        title: {
          text: data.MetaData.CurveId,
          x: -20 //center
        },
        yAxis: {
          title: {
            text: 'Voltage(V)'
          },
          plotLines: [{
            value: 0,
            width: 1,
            color: '#808080'
          }]
        }
          ,
        series: [{
          name: "SCOPE",
          data: data.Points.map(mapping)
        }],
        plotOptions: {
          line: {
            animation: false
          }
        }
      })
    }
    stepHub.client.getAtoms = function (data) {
        $('#tabpanelatomchart').highcharts({
            title: {
                text: 'Atom Count' ,
                x: -20 //center
            },
           
            yAxis: {
                title: {
                    text: 'Count'
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
                data: data.map(d => [d.V, d.Count])
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