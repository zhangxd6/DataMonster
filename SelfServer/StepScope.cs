using Microsoft.AspNet.SignalR;
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
    public void Start(double startV, double endV, double stepV, int unmberCurve)
    {
      voltageDevice = new Device(0, new Address(2, 96));
      device.Write("*IDN?");
      string data = device.ReadString();
      Task.Factory.StartNew(() =>
      {
        ConfigureLogger();
        InitScope();
        for(double v = startV; v < endV, v = v + stepV)
        {
          if (ct.IsCancellationRequested)
          {
            return;
          }
          this.curveNumber = 0;
          this.sumDData = new List<Server.CurvePoint>();
          raw.Trace($"{v} V");
          translated.Debug($"{v} V");
          for(int i=0;i< unmberCurve; i++)
          {
            this.GetScopeCurve();
          }
          this.AggreateCurve();
        }
      }, ct);
    }
  }
}
