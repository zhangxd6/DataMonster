using Microsoft.AspNet.SignalR;
using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SelfServer
{
    public class StepScope : ScopeHubBase
    {
       // private SyncAlliedCamera cameraCtl = SyncAlliedCamera.Instance;
        private SRSDG535 sRSDG535 = SRSDG535.Instance;
        protected Device voltageDevice;
        List<AtomCount> atomCounts = new List<AtomCount>();
        public void Start(double startV, double endV, double stepV, int numberCurve = 10)
        {
            base.Start();
            //cameraCtl.Start();

            voltageDevice = new Device(0, new Address(2, 96));
            voltageDevice.Write("*IDN?");
            string data = voltageDevice.ReadString();

            voltageDevice.Write(":INST:NSEL 1");
            voltageDevice.Write(":OUTP ON");
            voltageDevice.Write(":SOUR:VOLT 1.1");
            voltageDevice.Write(":INST:NSEL?");

            Task.Factory.StartNew(() =>
            {
                ConfigureLogger();
                atomCounts = new List<AtomCount>();
                var datetime = DateTime.Now;
                var path = $"{datetime.Month.ToString("D2")}_{datetime.Day.ToString("D2")}_{datetime.Year.ToString("D4")}__{datetime.Hour.ToString("D2")}_{datetime.Minute.ToString("D2")}_{datetime.Second.ToString("D2")}";
                InitScope($"data/{path}");

                for (double v = startV; v < endV; v = v + stepV)
                {
                    //d is SRSDG535 delay
                   // double d = 0;// no delay
                    
                    for (double d = 0.0; d < 1.0e-5; d = d + 1.0e-6)  // with delay
                    {
                        var pathprefix = $"data/{path}/volt_{v}/delay_{d}";
                        System.IO.Directory.CreateDirectory(pathprefix);

                        if (ct.IsCancellationRequested)
                        {
                            return;
                        }
                        sRSDG535.SetDelay(2, d);

                        var instruction = $":SOUR:VOLT {v}";
                        voltageDevice.Write(instruction);
                        this.curveNumber = 0;
                        this.sumDData = new List<Server.CurvePoint>();
                        raw.Trace($"{v} V");
                       // translated.Debug($"{v} V");
                        for (int i = 0; i < numberCurve; i++)
                        {
                            this.GetScopeCurve(pathprefix);
                        }
                        this.AggreateCurve(pathprefix);
                        int total = 0;
                        //uncommet to take images
                        //cameraCtl.AccquireImage(pathprefix,out total);
                        atomCounts.Add(new AtomCount() { V = v, Count = total });
                        Clients.All.getAtoms(atomCounts);
                    }
                }
                Task.Run(()=>System.IO.File.WriteAllText(System.IO.Path.Combine("data",path, "atomnumbersvsvoltage.txt"), string.Join(Environment.NewLine,atomCounts.Select(x=>$"{x.V},{x.Count}")), System.Text.Encoding.ASCII));
                //cameraCtl.StopCamera();


            }, ct);
        }
    }

    public class AtomCount
    {
        public double V { get; set; }
        public int Count { get; set; }
    }
}
