using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thorlabs.LC100;

namespace SelfServer
{
    public class ThorlabLC100
    {
        private LC100_Drv lc100Driver;
        private short[] lc100Data;

        private static ThorlabLC100 _instance = new ThorlabLC100();
        private ThorlabLC100()
        {
            lc100Data = new short[2048];

        }

        public static ThorlabLC100 Instance
        {
            get { return _instance; }
        }

        public void Init()
        {
            string resourceName = "USB0::0x1313::0x80A0::M00415801::RAW";
            lc100Driver = new LC100_Drv(resourceName, false, false);
            //set integration time
            int status;
            int res = lc100Driver.getDeviceStatus(out status);

            res = lc100Driver.setIntegrationTime((double)1.0);

        }

        public short[] GetData()
        {
            int status;
            int res = lc100Driver.getDeviceStatus(out status);
            // has the device started?
            res = lc100Driver.getDeviceStatus(out status);

            // wait 3 sec for a new data transfer
            if ((status & 0x00000001) == 0 && (status & 0x00000002) == 0)
            {
                DateTime startTime = DateTime.Now;
                TimeSpan elapsedTime = DateTime.Now - startTime;

                while (elapsedTime.Seconds < 3)
                {
                    elapsedTime = DateTime.Now - startTime;
                }
            }
            if ((status & 0x00000001) > 0 || (status & 0x00000002) > 0)
            {

                res = lc100Driver.getScanData(lc100Data);
                if (res == 0)
                {
                    return lc100Data;

                }
            }
            return null;
        }

    }
}
