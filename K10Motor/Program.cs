﻿using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K10Motor
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:9001/";
            WebApp.Start<Startup>(url: baseAddress);
            System.Diagnostics.Process.Start(baseAddress);
            Console.ReadLine();
        }
    }
}
