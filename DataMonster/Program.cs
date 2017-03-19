using NationalInstruments.NI4882;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataMonster
{
    class Program
    {
        static void Main(string[] args)
        {
            var device = new Device(0, new Address(2, 96));
            device.Write("*IDN?");
            string data = device.ReadString();
            Console.WriteLine(data);
            Console.ReadLine();

      device.Write(":INST:NSEL 1");
      device.Write(":OUTP ON");
      device.Write(":SOUR:VOLT 1.1");
      device.Write(":INST:NSEL?");
     var result = device.ReadString();
      if (result == "1")
      {
        Console.WriteLine("success");
      }


       device.Write(":MEAS:VOLT?");
      result = device.ReadString();
      Console.WriteLine(result);
      
      for(double v = 0.0; v <= 10.0; v = v + 0.1)
      {
        var instruction = $":SOUR:VOLT {v}";
        //device.Write(":INST:NSEL 1");
        //device.Write(":OUTP ON");
        device.Write(instruction);
        device.Write(":MEAS:VOLT?");
        result = device.ReadString();
        Console.WriteLine(result);
        Thread.Sleep(2000);

      }
      //device.Write("DAT:SOU CH1");
      //device.Write("DAT:ENC RIB;WID 1");
      //device.Write("HOR:RECORDL 500");
      //device.Write("DAT:STAR 1");
      //device.Write("DAT:STOP 500");
      //device.Write("HEAD OFF");
      //device.Write("ACQ:STATE RUN");


      //device.Write("WFMPRE:CH1:NR_PT?;YOFF?;YMULT?;XINCR?;PT_OFF?;XUNIT?;YUNIT?WFMPRE:CH1:NR_PT?;YOFF?;YMULT?;XINCR?;PT_OFF?;XUNIT?;YUNIT?");
      //data = device.ReadString();
      //Console.WriteLine(data);
      //Console.ReadLine();
      //for (int i = 0; i < 100; i++)
      //{
      //    device.Write("curv?");
      //    device.ReadByteArray(1);
      //    int length = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(1)));
      //    int count = Convert.ToInt32(System.Text.Encoding.ASCII.GetString(device.ReadByteArray(length)));
      //    Byte[] curve = device.ReadByteArray(count);
      //    foreach(byte b in curve)
      //    {
      //        int number = Convert.ToInt32( b);
      //        Console.Write(number+",");
      //    }
      //    Console.WriteLine();

      //    device.Write("curv?");
      //    data = device.ReadString();
      //    Console.WriteLine(data.Length);
      //    length = Convert.ToInt32(data[1].ToString());
      //    for(int j= 2 + length; j < data.Length; j++)
      //    {
      //        int number = (int)data[j] ;
      //        Console.Write(number + ",");
      //    }
      //    Console.WriteLine();
      //    Console.WriteLine(data);
      //}
      Console.ReadLine();
        }
    }
}
