using Microsoft.Owin.Hosting;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9000/";
            WebApp.Start<Startup>(url: baseAddress);
            System.Diagnostics.Process.Start(baseAddress);
            Console.ReadLine();
        }
    }
}
