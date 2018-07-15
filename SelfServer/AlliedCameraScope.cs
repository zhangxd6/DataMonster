using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class AlliedCameraScope : ScopeHubBase
    {
        private SyncAlliedCamera cameraCtl = new SyncAlliedCamera();
        public override void Start()
        {
            base.Start();
            cameraCtl.Start();
        }

        public void TakePicture()
        {
            cameraCtl.AccquireImage("bame");
        }

        public override void Stop()
        {
            base.Stop();
            cameraCtl.StopCamera();

        }
    }
}
