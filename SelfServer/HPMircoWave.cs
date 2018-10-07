using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class HPMircoWave
    {
        private static HPMircoWave instnace = new HPMircoWave();

        public static HPMircoWave Instance
        {
            get
            {
                return instnace;
            }
        }

        private Device device;

        private HPMircoWave()
        {
            device = new Device(0, new Address(19, 96));
            //Set it as remote
            device.Write("REM");

        }
        /// <summary>
        /// Q345678Z0 => 3.45678 GZ
        /// 
        /// </summary>
        /// <param name="freq"></param>
        public void SetFrequnecy(string freq)
        {
            device.Write(freq);
        }
    }
}
