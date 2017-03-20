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
using NLog.Config;
using NLog.Targets;
using SelfServer;

namespace Server
{
  public class TDS : ScopeHubBase
  {
    public override void Start()
    {
      base.Start();
      this.ConfigureLogger();
      this.InitScope();
      Task.Factory.StartNew(() =>
      {
        while (true)
        {
          if (ct.IsCancellationRequested)
          {
            this.AggreateCurve();
          }
          else
          {
            this.GetScopeCurve();
          }        
        }
      }, ct);
    }

  }
  //public class TDS : Hub,IScopeHub
  //{
  //  static CancellationTokenSource cts;
  //  static CancellationToken ct;
  //  Device device;
  //  Logger logger = LogManager.GetCurrentClassLogger();
  //  Logger raw = LogManager.GetLogger("raw");
  //  Logger translated = LogManager.GetLogger("translated");
  //  int curveNumber = 0;
  //  List<CurvePoint> sumDData = new List<CurvePoint>();
  //  public void Start()
  //  {

  //    var config = new LoggingConfiguration();
  //    var consoleTarget = new ColoredConsoleTarget();
  //    config.AddTarget("console", consoleTarget);

  //    var fileTarget = new FileTarget();
  //    config.AddTarget("raw", fileTarget);

  //    var fileTranlatedTarget = new FileTarget();
  //    config.AddTarget("transalted", fileTarget);

  //    // Step 3. Set target properties 
  //    consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
  //    fileTarget.FileName ="${basedir}/data/raw/file_"+DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")+".txt";
  //    fileTarget.Layout = "${message}";

  //    fileTranlatedTarget.FileName = "${basedir}/data/translated/file_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".txt";
  //    fileTranlatedTarget.Layout = "${message}";

  //    // Step 4. Define rules
  //    var rule1 = new LoggingRule("*", LogLevel.Trace, consoleTarget);
  //    config.LoggingRules.Add(rule1);

  //    var rule2 = new LoggingRule("raw", LogLevel.Trace, fileTarget);
  //    config.LoggingRules.Add(rule2);

  //    var rule3 = new LoggingRule("translated", LogLevel.Debug, fileTranlatedTarget);
  //    config.LoggingRules.Add(rule3);
  //    // Step 5. Activate the configuration
  //    LogManager.Configuration = config;
  //    logger.Trace("Starting collecting the waveform");

  //    device = new Device(0, new Address(13, 96));
  //    device.Write("*IDN?");
  //    string data = device.ReadString();
  //    logger.Trace(String.Format("the scope infomation:{0}", data));

  //    string channel = "CH2";
  //    device.Reset();
  //    device.Write(string.Format("DAT:SOU {0}", channel));
  //    device.Write("DAT:ENC RIB;WID 1");
  //    device.Write("DAT:STAR 1");
  //    device.Write("DAT:STOP 2500");
  //    device.Write("HEAD OFF");
  //    device.Write("ACQ:STATE RUN");

  //    logger.Trace(string.Format("Waveform from {0}", channel));
  //    ///get wave parameters


  //    CurveMetaData meta = new CurveMetaData();
  //    device.Write("WFMPre:WFId?");
  //    meta.CurveId = device.ReadString();
  //    device.Write("WFMPre:Xincr?");
  //    meta.Xincr = Convert.ToDouble(device.ReadString());
  //    device.Write("WFMPre:Xzero?");
  //    meta.Xzero = Convert.ToDouble(device.ReadString());
  //    device.Write("WFMPre:PT_Off?");
  //    meta.Ptoff = Convert.ToDouble(device.ReadString());
  //    device.Write("WFMPre:Ymult?");
  //    meta.YMult = Convert.ToDouble(device.ReadString());
  //    device.Write("WFMPre:Yoff?");
  //    meta.Yoff = Convert.ToDouble(device.ReadString());
  //    device.Write("WFMPre:YZero?");
  //    meta.Yzero = Convert.ToDouble(device.ReadString());

  //    logger.Trace(string.Format("Waveform metadata : {0}", JsonConvert.SerializeObject(meta)));

  //    cts = new CancellationTokenSource();
  //    ct = cts.Token;
  //    curveNumber = 0;
  //    Task.Factory.StartNew(() =>
  //    {
  //      while (true)
  //      {
  //        if (!ct.IsCancellationRequested)
  //        {
  //          try
  //          {
  //            device.Write("Curv?");

  //            device.ReadByteArray(1);
  //            int count = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(1)));
  //            int npt = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(count)));
  //            Byte[] waveform = device.ReadByteArray(npt);
  //            data = device.ReadString();
  //            CurveData curvedata = new CurveData()
  //            {
  //              MetaData = meta,
  //              Points = new List<CurvePoint>(),
  //              Orginal = new List<int>()
  //            };
  //                  // int length = Convert.ToInt32(data[1].ToString());
  //             for (int j = 0; j < npt; j++)
  //            {
  //              int pointData = (int)waveform[j];
  //              curvedata.Orginal.Add(pointData);
  //              CurvePoint point = new CurvePoint()
  //              {
  //                X = meta.Xzero + meta.Xincr * (j - meta.Ptoff),
  //                Y = meta.Yzero + meta.YMult * (pointData - meta.Yoff)
  //              };
  //              curvedata.Points.Add(point);
  //                    //sum up
  //              if (curveNumber == 0)
  //              {
  //                sumDData.Add(point);
  //              }
  //              else
  //              {
  //                sumDData[j].Y += point.Y;
  //              }
  //            }

  //            raw.Trace( JsonConvert.SerializeObject(curvedata.Orginal));
  //            translated.Debug(JsonConvert.SerializeObject(curvedata.Points));
  //            Clients.All.getData(curvedata);
  //            //curveNumber++;
  //            Interlocked.Increment(ref curveNumber);
  //          }
  //          catch (Exception)
  //          {


  //          }
  //          finally
  //          {
  //            device.Clear();
  //            Thread.Sleep(1000);
  //          }
  //        }
  //        else
  //        {
  //          logger.Trace(string.Format("{0} waveforms are acquired", curveNumber));
  //          translated.Debug(string.Format("{0} waveforms are acquired", curveNumber));

  //          translated.Debug(string.Format("Sumed curve data :{0}", JsonConvert.SerializeObject(sumDData)));
  //          foreach (var point in sumDData)
  //          {
  //            point.Y /= curveNumber;
  //          }
  //          translated.Debug(string.Format("Averaged Curve data :{0}", JsonConvert.SerializeObject(sumDData)));
  //          curveNumber = 0;
  //          Debug.WriteLine("cancelled!");
  //          break;

  //        }
  //      }
  //    }, ct);
  //  }

  //  public void Stop()
  //  {

  //    cts?.Cancel();
  //  }

  //}

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