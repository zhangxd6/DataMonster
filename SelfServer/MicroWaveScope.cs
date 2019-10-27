using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class MicroWaveScope : ScopeHubBase
    {
        private HPMircoWave hPMircoWave = HPMircoWave.Instance;
        List<AtomCount> atomCounts = new List<AtomCount>();
        public MicroWaveScope()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">2.12345Gz</param>
        /// <param name="stop">3.12345Gz</param>
        /// <param name="step">0.00100Gz</param>
        public void Start(double start, double stop, double step, int numberCurve = 10)
        {
            base.Start();
            Task.Factory.StartNew(() =>
            {
                ConfigureLogger();

                atomCounts = new List<AtomCount>();
                var datetime = DateTime.Now;
                var path = $"{datetime.Month.ToString("D2")}_{datetime.Day.ToString("D2")}_{datetime.Year.ToString("D4")}__{datetime.Hour.ToString("D2")}_{datetime.Minute.ToString("D2")}_{datetime.Second.ToString("D2")}";

                InitScope($"data/{path}");

                for (int v = Convert.ToInt32(start*1000000); v <= stop*1000000; v = v + Convert.ToInt32(step *1000000))
                {
                    var pathprefix = $"data/{path}/freq_{v}";
                    System.IO.Directory.CreateDirectory(pathprefix);

                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }

                    string freq = $"Q{(v).ToString("D7")}Z0";
                    hPMircoWave.SetFrequnecy(freq);

                    this.curveNumber = 0;
                    this.sumDData = new List<Server.CurvePoint>();
                    raw.Trace($"{v} Hz");
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
                Task.Run(()=>System.IO.File.WriteAllText(System.IO.Path.Combine("data", path, "atomnumbersvsfreq.txt"), string.Join(Environment.NewLine, atomCounts.Select(x=>$"{x.V},{x.Count}")), System.Text.Encoding.ASCII));

            },ct);
        }
    }
}


