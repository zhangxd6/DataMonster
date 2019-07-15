using Microsoft.AspNet.SignalR;
using NationalInstruments.NI4882;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SelfServer
{
    public class ScopeHubBase : Hub, IScopeHub
    {
        protected static CancellationTokenSource cts;
        protected static CancellationToken ct;
        private Logger logger = LogManager.GetCurrentClassLogger();
        protected Logger raw = LogManager.GetLogger("raw");
        protected Logger translated = LogManager.GetLogger("translated");
        protected int curveNumber = 0;
        protected List<CurvePoint> sumDData = new List<CurvePoint>();
        protected Device device;
        protected CurveMetaData meta = new CurveMetaData();
        public virtual void Start()
        {
            cts = new CancellationTokenSource();
            ct = cts.Token;

        }

        public virtual void Stop()
        {
            cts?.Cancel();
        }

        protected void ConfigureLogger()
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("raw", fileTarget);

            var fileTranlatedTarget = new FileTarget();
            config.AddTarget("transalted", fileTarget);

            // Step 3. Set target properties 
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/data/raw/file_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
            fileTarget.Layout = "${message}";

            fileTranlatedTarget.FileName = "${basedir}/data/translated/file_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
            fileTranlatedTarget.Layout = "${message}";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("raw", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule2);

            var rule3 = new LoggingRule("translated", LogLevel.Debug, fileTranlatedTarget);
            config.LoggingRules.Add(rule3);
            // Step 5. Activate the configuration
            LogManager.Configuration = config;
            logger.Trace("Starting collecting the waveform");
        }

        protected void InitScope()
        {
            device = new Device(0, new Address(13, 96));
            device.Write("*IDN?");
            string data = device.ReadString();
            logger.Trace(String.Format("the scope infomation:{0}", data));

            string channel = "CH2";
            device.Reset();
            device.Write(string.Format("DAT:SOU {0}", channel));
            device.Write("DAT:ENC RIB;WID 1");
            device.Write("DAT:STAR 1");
            device.Write("DAT:STOP 2500");
            device.Write("HEAD OFF");
            device.Write("ACQ:STATE RUN");

            logger.Trace(string.Format("Waveform from {0}", channel));
            ///get wave parameters



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
            curveNumber = 0;
        }

        protected void GetScopeCurve(string pathprefix)
        {
            try
            {
                device.Write("Curv?");

                device.ReadByteArray(1);
                int count = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(1)));
                int npt = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(count)));
                Byte[] waveform = device.ReadByteArray(npt);
                var data = device.ReadString();
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
                        X = meta.Xzero + meta.Xincr * (j - meta.Ptoff),
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

                //raw.Trace(JsonConvert.SerializeObject(curvedata.Orginal));
                //translated.Debug(JsonConvert.SerializeObject(curvedata.Points));
                Clients.All.getData(curvedata);
                if (!string.IsNullOrEmpty(pathprefix))
                    Task.Run(()=>File.WriteAllText(System.IO.Path.Combine(pathprefix, $"raw_{curveNumber}"), JsonConvert.SerializeObject(curvedata.Orginal)));
                //curveNumber++;
                Interlocked.Increment(ref curveNumber);
            }
            catch (Exception)
            {


            }
            finally
            {
                device.Clear();
                //may need sleep to allow camera 
                //Thread.Sleep(1000);
            }
        }

        protected void AggreateCurve(string pathprefix)
        {
            logger.Trace(string.Format("{0} waveforms are acquired", curveNumber));
            //translated.Debug(string.Format("{0} waveforms are acquired", curveNumber));

            //translated.Debug(string.Format("Sumed curve data :{0}", JsonConvert.SerializeObject(sumDData)));
            foreach (var point in sumDData)
            {
                point.Y /= curveNumber;
            }
            //translated.Debug(string.Format("Averaged Curve data :{0}", JsonConvert.SerializeObject(sumDData)));
            curveNumber = 0;
            if(!string.IsNullOrEmpty(pathprefix))
            Task.Run(()=>File.WriteAllText(System.IO.Path.Combine(pathprefix, $"aggreated{curveNumber}"), String.Join(Environment.NewLine, sumDData.Select(x=>
            {
                return $"{x.X},{x.Y}";
            }).ToArray())));
            
        }
    }
}
