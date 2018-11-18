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
    public class StepLC100 : ScopeHubBase
    {
        private SyncAlliedCamera cameraCtl = SyncAlliedCamera.Instance;
        private ThorlabLC100 lc100 = ThorlabLC100.Instance;
        protected Device voltageDevice;
        List<AtomCount> atomCounts = new List<AtomCount>();
        public void Start(double startV, double endV, double stepV, int lowerIndex, int highIndex)
        {
            base.Start();
            cameraCtl.Start();

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
                lc100.Init();
                atomCounts = new List<AtomCount>();
                var datetime = DateTime.Now;
                var path = $"{datetime.Month.ToString("D2")}_{datetime.Day.ToString("D2")}_{datetime.Year.ToString("D4")}__{datetime.Hour.ToString("D2")}_{datetime.Minute.ToString("D2")}_{datetime.Second.ToString("D2")}";
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine("data", path));

                for (double v = startV; v < endV; v = v + stepV)
                {

                    var pathprefix = $"data/{path}/volt_{v}/";
                    System.IO.Directory.CreateDirectory(pathprefix);

                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                    //set voltage
                    var instruction = $":SOUR:VOLT {v}";
                    voltageDevice.Write(instruction);

                    //get data
                    var cldata = lc100.GetData();
                    int sum = 0;
                    for (int i = lowerIndex; i <= highIndex; i++)
                    {
                        sum += cldata[i];
                    }

                    this.curveNumber = 0;
                    this.sumDData = new List<Server.CurvePoint>();
                    raw.Trace($"{v} V");
                    translated.Debug($"{v} V");

                    atomCounts.Add(new AtomCount() { V = v, Count = sum });
                    Clients.All.getLC100Atoms(atomCounts);
                }


                System.IO.File.WriteAllText(System.IO.Path.Combine("data", path, "atomnumbersvsvoltage.txt"), JsonConvert.SerializeObject(atomCounts));
            


            }, ct);
        }
    }

    
}
