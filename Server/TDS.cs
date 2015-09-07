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
            device.Write("*IDN?");
            string data = device.ReadString();
            device.Reset();
            device.Write("DAT:SOU CH1");
            device.Write("DAT:ENC RIB;WID 1");
            device.Write("HOR:RECORDL 3000");
            device.Write("DAT:STAR 1");
            device.Write("DAT:STOP 3000");
            device.Write("HEAD OFF");
            device.Write("ACQ:STATE RUN");
            cts = new CancellationTokenSource();
            ct = cts.Token;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!ct.IsCancellationRequested)
                    {
                        try
                        {
                            device.Write("Curv?");

                            data = device.ReadString();
                            int length = Convert.ToInt32(data[1].ToString());
                            List<int> curve = new List<int>();
                            for (int j = 2 + length; j < data.Length; j++)
                            {
                                curve.Add((int)data[j]);
                            }

                            Clients.All.getData(curve);

                        }
                        catch (Exception)
                        {


                        }
                        finally
                        {
                            device.Clear();
                            Thread.Sleep(1000);
                        }
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