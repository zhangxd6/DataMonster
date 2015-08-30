using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Threading;
using NationalInstruments.NI4882;
using System.Diagnostics;

namespace Server
{
    public class TDS : Hub
    {
       static CancellationTokenSource cts;
       static CancellationToken ct;
        Device device;
        public void Start()
        {
            device = new Device(0, new Address(12, 96));

            cts = new CancellationTokenSource();
            ct = cts.Token;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        device.Write("Curv?");
                        string data = device.ReadString();
                        Clients.All.getData(data);
                        Task.Delay(500);
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