using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
   public class DelayLC100: ScopeHubBase
    {
        private SRSDG535 sRSDG535 = SRSDG535.Instance;

        private ThorlabLC100 lc100 = ThorlabLC100.Instance;
        List<AtomDelayCount> atomCounts = new List<AtomDelayCount>();

        public DelayLC100()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">2sec</param>
        /// <param name="stop">3sec</param>
        /// <param name="step">0.1sec</param>
        public void Start(double start, double stop, double step, int lowerIndex, int highIndex)
        {
            base.Start();
            Task.Factory.StartNew(() =>
            {
                ConfigureLogger();
                lc100.Init();

                atomCounts = new List<AtomDelayCount>();
                var datetime = DateTime.Now;
                var path = $"{datetime.Month.ToString("D2")}_{datetime.Day.ToString("D2")}_{datetime.Year.ToString("D4")}__{datetime.Hour.ToString("D2")}_{datetime.Minute.ToString("D2")}_{datetime.Second.ToString("D2")}";


                for (double d = start; d < stop; d = d + step)
                {
                    var pathprefix = $"data/{path}/deay_{d}";
                    System.IO.Directory.CreateDirectory(pathprefix);

                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                    sRSDG535.SetDelay(2, d*1e-6);

                    //get data
                    var cldata = lc100.GetData();
                    int sum = 0;
                    for (int i = lowerIndex; i <= highIndex; i++)
                    {
                        sum += cldata[i];
                    }
                    Task.Run(()=>System.IO.File.WriteAllText(System.IO.Path.Combine(pathprefix, "trace.txt"), string.Join(",", cldata)));

                    atomCounts.Add(new AtomDelayCount() {  D = d, Count = sum });
                    Clients.All.getCameraAtoms(atomCounts);

                }
                Task.Run(()=>System.IO.File.WriteAllText(System.IO.Path.Combine("data", path, "atomnumbersvsdelay.txt"), string.Join(Environment.NewLine,atomCounts.Select(x=>
                {
                    return $"{x.D},{x.Count}";
                }).ToArray())));

            }, ct);
        }
    }
}
