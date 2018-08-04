using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfServer
{
    public class AlliedCameraScope : ScopeHubBase
    {
        private SyncAlliedCamera cameraCtl = SyncAlliedCamera.Instance;
        public override void Start()
        {
            base.Start();
            cameraCtl.Start();
        }

        public Results TakePicture()
        {
            int total = 0;
           var path= cameraCtl.AccquireImage("wwwroot",out total);

            //path = path.Replace("wwwroot/", "");
            var result = new Results()
            {
                ImageSrc = $"{path.Replace("wwwroot/", "")}/cameraimage.bmp",
                Red = File.ReadAllText($"{path}/red.txt").Split(new char[] { ',' }).ToList().Select(i => Convert.ToInt32(i)),
                Green = File.ReadAllText($"{path}/green.txt").Split(new char[] { ',' }).ToList().Select(i => Convert.ToInt32(i)),
                Blue = File.ReadAllText($"{path}/blue.txt").Split(new char[] { ',' }).ToList().Select(i => Convert.ToInt32(i)),
            };
            return result;
        }

        public override void Stop()
        {
            base.Stop();
            cameraCtl.StopCamera();

        }
    }

    public class Results
    {
        public string ImageSrc { get; set; }
        public IEnumerable<int> Red { get; set; }
        public IEnumerable<int> Green { get; set; }
        public IEnumerable<int> Blue { get; set; }
    }
}
