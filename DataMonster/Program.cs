using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonster
{
    class Program
    {
        static void Main(string[] args)
        {
            var device = new Device(0, new Address(12, 96));
            device.Write("*IDN?");
            string data = device.ReadString();
            Console.WriteLine(data);
            Console.ReadLine();

            device.Write("DAT:SOU CH1");
            device.Write("DAT:ENC RIB;WID 1");
            device.Write("HOR:RECORDL 500");
            device.Write("DAT:STAR 1");
            device.Write("DAT:STOP 500");
            device.Write("HEAD OFF");
            device.Write("ACQ:STATE RUN");


            //device.Write("WFMPRE:CH1:NR_PT?;YOFF?;YMULT?;XINCR?;PT_OFF?;XUNIT?;YUNIT?WFMPRE:CH1:NR_PT?;YOFF?;YMULT?;XINCR?;PT_OFF?;XUNIT?;YUNIT?");
            //data = device.ReadString();
            //Console.WriteLine(data);
            //Console.ReadLine();
            for (int i = 0; i < 100; i++)
            {
                device.Write("curv?");
                device.ReadByteArray(1);
                int length = Convert.ToInt32(device.ReadByteArray(1)[0]);
                device.Write("curv?");
                data = device.ReadString();
                Console.WriteLine(data);
            }
            Console.ReadLine();
        }
    }
}
