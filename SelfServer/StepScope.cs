using Microsoft.AspNet.SignalR;
using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class StepScope : ScopeHubBase
    {
        private SyncAlliedCamera cameraCtl = SyncAlliedCamera.Instance;
        private SRSDG535 sRSDG535 = SRSDG535.Instance;
        protected Device voltageDevice;
        public void Start(double startV, double endV, double stepV, int numberCurve = 10)
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
                InitScope();
                for (double v = startV; v < endV; v = v + stepV)
                {
                    for (double d = 0.0; d < 1.0e-5; d = d + 1.0e-6)
                    {
                        var pathprefix = $"data/volt_{v}/delay_{d}";
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
                        translated.Debug($"{v} V");
                        for (int i = 0; i < numberCurve; i++)
                        {
                            this.GetScopeCurve(pathprefix);
                        }
                        this.AggreateCurve(pathprefix);
                        cameraCtl.AccquireImage(pathprefix);
                    }
                }
                cameraCtl.StopCamera();


            }, ct);
        }
    }
}
