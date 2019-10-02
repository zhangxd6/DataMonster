using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thorlabs.MotionControl.DeviceManagerCLI;
using Thorlabs.MotionControl.GenericMotorCLI;
using Thorlabs.MotionControl.GenericMotorCLI.ControlParameters;
using Thorlabs.MotionControl.IntegratedStepperMotorsCLI;

namespace SelfServer
{
    public class K10RC1
    {
        private static K10RC1 _instance;
        CageRotator device;
        public static K10RC1 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new K10RC1();
                }
                return _instance;
            }
        }

        public void Init()
        {
            string serialNo = "55114644";
            DeviceManagerCLI.BuildDeviceList();
            List<string> serialNumbers = DeviceManagerCLI.GetDeviceList();
            if (!serialNumbers.Contains(serialNo))
                throw new Exception("Motor is not found!");

             device = CageRotator.CreateCageRotator(serialNo);
            if (device==null)
                throw new Exception("Motor can be accessed!");

            //connect to motor
            device.Connect(serialNo);
            // Wait for the device settings to initialize - timeout 5000ms
            if (!device.IsSettingsInitialized())
            {
                try
                {
                    device.WaitForSettingsInitialized(5000);
                }
                catch (Exception e)
                {
                    throw new Exception("Settings failed to initialize",e);
                }
            }

            // Start the device polling
            // The polling loop requests regular status requests to the motor to ensure the program keeps track of the device. 
            device.StartPolling(250);
            // Needs a delay so that the current enabled state can be obtained
            Thread.Sleep(500);
            // Enable the channel otherwise any move is ignored 
            device.EnableDevice();
            // Needs a delay to give time for the device to be enabled
            Thread.Sleep(500);

            device.LoadMotorConfiguration(serialNo);

            // Display info about device
            DeviceInfo deviceInfo = device.GetDeviceInfo();
            Console.WriteLine("Device {0} = {1}", deviceInfo.SerialNumber, deviceInfo.Name);



        }

        public void Stop()
        {
            device.StopPolling();
            device.Disconnect(true);

        }

        public void MoveTo(decimal position, decimal vel, decimal acceleration,decimal motorstep)
        {
            VelocityParameters velPars = device.GetVelocityParams();
            velPars.MaxVelocity = vel;
            velPars.Acceleration = acceleration;
            
            device.SetVelocityParams(velPars);


            device.SetJogStepSize(motorstep);
            device.MoveTo(position, 60000);
        }


    }
}
