$(function () {
    $('#main-tab a:first').tab('show');
    var moveShapeHub = $.connection.tDS;
    var stepHub = $.connection.stepScope;
    var cameraHub = $.connection.alliedCameraScope;
    var microcamera = $.connection.microWaveCamera;
    var lc100 = $.connection.stepLC100;
    var microwaveLc100 = $.connection.microWaveLC100;
    var delayLc100 = $.connection.delayLC100;
    var microwavescope = $.connection.microWaveScope;

    var motor = $.connection.motorK10RC1;
    var motorLC100 = $.connection.motorK10RC1LC100;

    $('#microwavescopestart').click(function () {
        var init = $('#microwavescopeinitialfreq').val();
        var end = $('#microwavescopefinalfreq').val();
        var step = $('#microwavescopestepfreq').val();
        var numberCurve = $('#microwavescopenubmeroftrace').val();
        microwavescope.server.start(init, end, step, numberCurve);
    });
    $('#microwavescopeend').click(function () {
        microwavescope.server.stop();
    });
    $('#microwaveCamerastart').click(function () {
        var init = $('#initialfreq').val();
        var end = $('#finalfreq').val();
        var step = $('#stepfreq').val();
        microcamera.server.start(init, end, step);
    });

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
    $('#microwaveCameraend').click(() => {
        microcamera.server.stop().done(() => {
            alert('Camera Stoped ')
        })
    });

    $('#lc100end').click(() => {
        lc100.server.stop().done(() => {
            alert('Camera Stoped ')
        })
    });

    $('#dealylc100start').click(function () {
        var init = $('#initialdelaylc100').val();
        var end = $('#finaldelaylc100').val();
        var step = $('#stepdelaylc100').val();
        var lowerIndex = $('#lowerIndexlc100delay').val();
        var higherIndex = $('#higherIndexlc100delay').val();
        delayLc100.server.start(init, end, step, lowerIndex, higherIndex);
    });

    $('#delaylc100end').click(() => {
        delayLc100.server.stop().done(() => {
            alert('Camera Stoped ')
        })
    });


    $('#macrowavelc100start').click(function () {
        var init = $('#initialfreqlc100').val();
        var end = $('#finalfreqlc100').val();
        var step = $('#stepfreqlc100').val();
        var lowerIndex = $('#lowerIndexlc100').val();
        var higherIndex = $('#higherIndexlc100').val();
        microwaveLc100.server.start(init, end, step, lowerIndex, higherIndex);
    });

    $('#macrwavelc100end').click(() => {
        microwaveLc100.server.stop().done(() => {
            alert('Camera Stoped ')
        })
    });

    $('#lc100start').click(function () {
        var init = $('#iVol').val();
        var end = $('#fVol').val();
        var step = $('#stepVol').val();
        var lowerIndex = $('#lowerIndex').val();
        var higherIndex = $('#higherIndex').val();
        lc100.server.start(init, end, step, lowerIndex, higherIndex);
    });


    $('#motorstart').click(function () {
        
        var init = $('#initialdelaymotor').val();
        var end = $('#finaldelaymotor').val();
        var step = $('#stepdelaymotor').val();
        var lowerIndex = $('#lowerIndexmotor').val();
        var higherIndex = $('#higherIndexmotor').val();

        var maxVel = $('#maxVel').val();
        var acceleration = $('#acceleration').val();
        var motorstep = $('#motorstep').val();
        var motornubmeroftrace = $('#motornubmeroftrace').val();

        motor.server.start(init, end, step, lowerIndex, higherIndex, maxVel, acceleration, motorstep, motornubmeroftrace);
    });

    $('#motorend').click(function () {
        motor.server.stop().done(() => {
            alert('Camera Stoped ')
        })
    });


    $('#motorlc100start').click(function () {

        var init = $('#initialdelaymotorlc100').val();
        var end = $('#finaldelaymotorlc100').val();
        var step = $('#stepdelaymotorlc100').val();
        var lowerIndex = $('#lowerIndexmotorlc100').val();
        var higherIndex = $('#higherIndexmotorlc100').val();

        var maxVel = $('#maxVellc100').val();
        var acceleration = $('#accelerationlc100').val();
        var motorstep = $('#motorsteplc100').val();
        var motornubmeroftrace = $('#motornubmeroftracelc100').val();

        motorLC100.server.start(init, end, step, lowerIndex, higherIndex, maxVel, acceleration, motorstep, motornubmeroftrace);
    });

    $('#motorlc100end').click(function () {
        motor.server.stop().done(() => {
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

    microwavescope.client.getAtoms = function (data) {
        $('#tabpanelmicrowavescopechart').highcharts({
            title: {
                text: 'Atom Count',
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

    microcamera.client.getCameraAtoms = function (data) {
        $('#tabpanelmicroatomchart').highcharts({
            title: {
                text: 'Atom Count',
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
                data: data.map(d => [d.F, d.Count])
            }],
            plotOptions: {
                line: {
                    animation: false
                }
            }
        });
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

    lc100.client.getLC100Atoms = function (data) {
        $('#tabpanellc100chart').highcharts({
            title: {
                text: 'Atom Count',
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

    microwaveLc100.client.getCameraAtoms = function (data) {
        $('#tabpanellc100macrowavechart').highcharts({
            title: {
                text: 'Atom Count',
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
                data: data.map(d => [d.F, d.Count])
            }],
            plotOptions: {
                line: {
                    animation: false
                }
            }
        });
    }

    delayLc100.client.getCameraAtoms = function (data) {
        $('#tabpanellc100delaychart').highcharts({
            title: {
                text: 'Atom Count',
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
                data: data.map(d => [d.D, d.Count])
            }],
            plotOptions: {
                line: {
                    animation: false
                }
            }
        });
    }

    motorLC100.client.getMotorLC100Atoms = function (data) {
        $('#motorchartlc100').highcharts({
            title: {
                text: 'Atom Count',
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
                data: data.map(d => [d.S, d.Count])
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