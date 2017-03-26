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
    protected Device voltageDevice;
    public void Start(double startV, double endV, double stepV, int numberCurve=10)
    {
      base.Start();
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
        for(double v = startV; v < endV; v = v + stepV)
        {
          if (ct.IsCancellationRequested)
          {
            return;
          }
          var instruction = $":SOUR:VOLT {v}";
          voltageDevice.Write(instruction);
          this.curveNumber = 0;
          this.sumDData = new List<Server.CurvePoint>();
          raw.Trace($"{v} V");
          translated.Debug($"{v} V");
          for(int i=0;i< numberCurve; i++)
          {
            this.GetScopeCurve();
          }
          this.AggreateCurve();
        }
      }, ct);
    }
  }
}
