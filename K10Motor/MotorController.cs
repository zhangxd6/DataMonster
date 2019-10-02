using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace K10Motor
{
    [RoutePrefix("api/motor")]
    public class MotorController : ApiController
    {
        [Route("init")]
        [HttpPost]
        public void Init()
        {
            Console.WriteLine("init");
           
            SelfServer.K10RC1.Instance.Init();
        }
        [Route("move")]
        [HttpPost]

        public void MoveTo(decimal position, decimal vel, decimal acceleration, decimal motorstep)
        {
            Console.WriteLine($"move to pos: {position} with vel: {vel} acl: {acceleration} and step: {motorstep}");
            SelfServer.K10RC1.Instance.MoveTo(position, vel, acceleration, motorstep);
        }
        [Route("stop")]
        [HttpPost]

        public void Stop()
        {
            Console.WriteLine("stop");
            SelfServer.K10RC1.Instance.Stop();
        }
    }
}
