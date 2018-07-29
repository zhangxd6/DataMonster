using NationalInstruments.NI4882;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class SRSDG535
    {
        private static SRSDG535 _instance = new SRSDG535();
        protected Device device;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public static SRSDG535 Instance
        {
            get
            {
                return _instance;
            }
        }
        private SRSDG535()
        {
            device = new Device(0, new Address(15, 96));
            device.Write("DS init");
            logger.Trace(String.Format("the SRSDG535 infomation:{0}", ""));
            device.Write("DS");

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">A is 2, channel B is 3 and trigger is 1</param>
        /// <param name="delay"></param>
        /// <param name="source"></param>
        public void SetDelay(int target, double delay, int source=1)
        {
            device.Write($"DT {target},{source},{delay}");

        }
    }
}
