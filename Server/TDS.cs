using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Threading;
//using NationalInstruments.NI4882;
using System.Diagnostics;

namespace Server
{
    public class TDS : Hub
    {
       static CancellationTokenSource cts;
       static CancellationToken ct;
        //Device device;
        int t = 0;
        public void Start()
        {
            //device = new Device(0, new Address(12, 96));

            cts = new CancellationTokenSource();
            ct = cts.Token;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        //device.Write("Curv?");
                        //string data = device.ReadString();
                       IEnumerable<double> data = Enumerable.Range(0, 500).Select((i) => 100 * Math.Sin(0.1 * i + (t + 54)));
                        Clients.All.getData(data);
                        Thread.Sleep(1000);
                        t++;
                    }
                    else
                    {
                        Debug.WriteLine("cancelled!");
                        break;

                    }
                }
            }, ct);
        }

        public void Stop()
        {
            cts.Cancel();
        }

    }
}