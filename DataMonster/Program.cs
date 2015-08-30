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
            for (int i = 0; i < 100; i++)
            {
                device.Write("curv?");
                byte[] cu = device.ReadByteArray();
                Console.WriteLine(cu);
            }
            Console.ReadLine();
        }
    }
}
