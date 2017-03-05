using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Threading;
using NationalInstruments.NI4882;
using System.Diagnostics;
using NLog;
using Newtonsoft.Json;

namespace Server
{
    public class TDS : Hub
    {
       static CancellationTokenSource cts;
       static CancellationToken ct;
        Device device;
        Logger logger = LogManager.GetCurrentClassLogger();
        int curveNumber = 0;
        List<CurvePoint> sumDData = new List<CurvePoint>();
        public void Start()
        {
            logger.Trace("Starting collecting the waveform");

            device = new Device(0, new Address(13, 96));
            device.Write("*IDN?");
            string data = device.ReadString();
            logger.Trace(String.Format("the scope infomation:{0}",data));

            string channel = "CH2";
            device.Reset();
            device.Write(string.Format("DAT:SOU {0}",channel));
            device.Write("DAT:ENC RIB;WID 1");
            device.Write("DAT:STAR 1");
            device.Write("DAT:STOP 2500");
            device.Write("HEAD OFF");
            device.Write("ACQ:STATE RUN");

            logger.Trace(string.Format("Waveform from {0}", channel));
            ///get wave parameters
            

            CurveMetaData meta = new CurveMetaData();
            device.Write("WFMPre:WFId?");
            meta.CurveId = device.ReadString();
            device.Write("WFMPre:Xincr?");
            meta.Xincr = Convert.ToDouble(device.ReadString());
            device.Write("WFMPre:Xzero?");
            meta.Xzero = Convert.ToDouble(device.ReadString());
            device.Write("WFMPre:PT_Off?");
            meta.Ptoff = Convert.ToDouble(device.ReadString());
            device.Write("WFMPre:Ymult?");
            meta.YMult = Convert.ToDouble(device.ReadString());
            device.Write("WFMPre:Yoff?");
            meta.Yoff = Convert.ToDouble(device.ReadString());
            device.Write("WFMPre:YZero?");
            meta.Yzero = Convert.ToDouble(device.ReadString());

            logger.Trace(string.Format("Waveform metadata : {0}", JsonConvert.SerializeObject(meta)));

            cts = new CancellationTokenSource();
            ct = cts.Token;
            curveNumber = 0;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        try
                        {
                            device.Write("Curv?");

                            device.ReadByteArray(1);
                            int count = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(1)));
                            int npt = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(count)));
                            Byte[] waveform = device.ReadByteArray(npt);
                            data = device.ReadString();
                            CurveData curvedata = new CurveData()
                            {
                                MetaData = meta,
                                Points = new List<CurvePoint>(),
                                Orginal = new List<int>()
                            };
                           // int length = Convert.ToInt32(data[1].ToString());
                            for (int j = 0; j < npt; j++)
                            {
                                int pointData = (int)waveform[j];
                                curvedata.Orginal.Add(pointData);
                                CurvePoint point = new CurvePoint()
                                {
                                    X = meta.Xzero + meta.Xincr * (j-meta.Ptoff),
                                    Y = meta.Yzero + meta.YMult * (pointData - meta.Yoff)
                                };
                                curvedata.Points.Add(point);
                                //sum up
                                if (curveNumber == 0)
                                {
                                    sumDData.Add(point);
                                }
                                else
                                {
                                    sumDData[j].Y += point.Y;
                                }
                            }

                            logger.Trace(string.Format("raw:{0}", JsonConvert.SerializeObject(curvedata.Orginal)));
                            logger.Trace(string.Format("converted:{0}", JsonConvert.SerializeObject(curvedata.Points)));
                            Clients.All.getData(curvedata);
                            curveNumber++;

                        }
                        catch (Exception)
                        {


                        }
                        finally
                        {
                            device.Clear();
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("cancelled!");
                        break;

                    }
                }
            }, ct);
        }

        public void Stop()
        {
            logger.Trace(string.Format("{0} waveforms are acqyured", curveNumber));
            logger.Trace(string.Format("Sumed curve data :{0}", JsonConvert.SerializeObject(sumDData)));
            foreach(var point in sumDData)
            {
                point.Y /= curveNumber;
            }
            logger.Trace(string.Format("Averaged Curve data :{0}", JsonConvert.SerializeObject(sumDData)));
            curveNumber = 0;
            cts?.Cancel();
        }

    }

    public class CurveData
    {
       public List<CurvePoint> Points { get; set; }
       public CurveMetaData MetaData { get; set; }
        public List<int> Orginal { get; set; }
    }

    public class CurveMetaData
    {
        public double Xzero { get; set; }
        public double Xincr { get; set; }
        public double Yzero { get; set; }
        public double YMult { get; set; }
        public double Yoff { get; set; }
        public string CurveId { get; set; }
        public double Ptoff { get; set; }
    }

    public class CurvePoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}