using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class MotorK10RC1: ScopeHubBase
    {
        private K10RC1 motor = K10RC1.Instance;

        List<AtomMotorPositionCount> atomCounts = new List<AtomMotorPositionCount>();

        public MotorK10RC1()
        {

        }

        public  void Start(decimal start, decimal stop, decimal step, int lowerIndex, int highIndex, decimal maxV, decimal acceleration, decimal motorstep, int numberCurve = 10)
        {
            base.Start();
            Task.Factory.StartNew(() =>
            {
                ConfigureLogger();
                motor.Init();

                var datetime = DateTime.Now;
                var path = $"{datetime.Month.ToString("D2")}_{datetime.Day.ToString("D2")}_{datetime.Year.ToString("D4")}__{datetime.Hour.ToString("D2")}_{datetime.Minute.ToString("D2")}_{datetime.Second.ToString("D2")}";

                InitScope($"data/{path}");

                for (decimal d = start; d < stop; d = d + step)
                {
                    var pathprefix = $"data/{path}/motor_{d}";
                    System.IO.Directory.CreateDirectory(pathprefix);

                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }
                    motor.MoveTo(d, maxV, acceleration,motorstep);
                    //get data
                    this.sumDData = new List<Server.CurvePoint>();
                    raw.Trace($"{d} degree");
                    for (int i = 0; i < numberCurve; i++)
                    {
                        this.GetScopeCurve(pathprefix);
                    }
                    this.AggreateCurve(pathprefix);
                }

            });
        }

        public override void Stop()
        {
            motor.Stop();
            base.Stop();
        }
    }

    public class AtomMotorPositionCount
    {
        public decimal P { get; set; }
        public int Count { get; set; }
    }
}
