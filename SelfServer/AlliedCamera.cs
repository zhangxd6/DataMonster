using AForge.Imaging;
using AVT.VmbAPINET;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SelfServer
{
    public class SyncAlliedCamera
    {
        public static SyncAlliedCamera Instance { get
            {
                return _instance;
            }
        }
        private Camera camera = null;
        private long payloadSize;
        private Frame frame;
        private static SyncAlliedCamera _instance = new SyncAlliedCamera();
        private SyncAlliedCamera()
        {

        }

        public void Start()
        {
            Vimba vimba = new Vimba();
            vimba.Startup();
            if (vimba.Cameras.Count == 0)
                throw new Exception("No Camera avaiable");
            camera = vimba.Cameras[0]; //use the 1st camera

            //open camera
            camera.Open(VmbAccessModeType.VmbAccessModeFull);
            var playloadFeature = camera.Features["PayloadSize"];
            payloadSize = playloadFeature.IntValue;
            frame = new Frame(payloadSize);
            camera.AnnounceFrame(frame);

            
        }

        public string  AccquireImage(string pathsuffix)
        {
            var datetime = DateTime.Now;
            var path = $"{pathsuffix}/{datetime.Month.ToString("D2")}_{datetime.Day.ToString("D2")}_{datetime.Year.ToString("D4")}__{datetime.Hour.ToString("D2")}_{datetime.Minute.ToString("D2")}_{datetime.Second.ToString("D2")}";

            while (true)
            {
                camera.AcquireSingleImage(ref frame, 5000); //timout at 5secons
                if (frame.ReceiveStatus == VmbFrameStatusType.VmbFrameStatusComplete)
                {
                    System.IO.Directory.CreateDirectory(path);
                    var fileName = System.IO.Path.Combine(path, "cameraimage.bmp");
                    Bitmap bitmap = null;
                    bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);
                    frame.Fill(ref bitmap);
                    ImageStatistics rgbStatistics = new ImageStatistics(bitmap);
                    int[] redValues = rgbStatistics.Red.Values;
                    int[] greenValues = rgbStatistics.Green.Values;
                    int[] blueValues = rgbStatistics.Blue.Values;
                    System.IO.File.WriteAllText(System.IO.Path.Combine(path, "red.txt"), string.Join(",", redValues));
                    System.IO.File.WriteAllText(System.IO.Path.Combine(path, "green.txt"), string.Join(",", greenValues));
                    System.IO.File.WriteAllText(System.IO.Path.Combine(path, "blue.txt"), string.Join(",", blueValues));

                    bitmap.Save(fileName, ImageFormat.Bmp);
                    Console.WriteLine("Frame status complete");
                    return path;
                    
                }
                //wait 100 milliseconds and try again
                System.Threading.Thread.Sleep(100);
            }
        }

        public void StopCamera()
        {
            //FeatureCollection features = camera.Features;
            //Feature feature = features["AcquisitionStop"];
            //feature.RunCommand();
            //camera.EndCapture();
            camera.FlushQueue();
            camera.RevokeAllFrames();
            camera.Close();
        }
    }

    //public class AsyncAlliedCamera
    //{
    //    private Camera m_Camera = null;

    //    private void StartCamera()
    //    {
    //        Vimba sys = new Vimba();
    //        CameraCollection cameras = null;
    //        FeatureCollection features = null;
    //        Feature feature = null;
    //        long payloadSize;
    //        Frame[] frameArray = new Frame[3];
    //        sys.Startup();
    //        cameras = sys.Cameras;
    //        m_Camera = cameras[0];
    //        m_Camera.Open(VmbAccessModeType.VmbAccessModeFull);
    //        m_Camera.OnFrameReceived +=
    //        new Camera.OnFrameReceivedHandler(OnFrameReceived);
    //        features = m_Camera.Features;
    //        feature = features["PayloadSize"];
    //        payloadSize = feature.IntValue;
    //        for (int index = 0; index < frameArray.Length; ++index)
    //        {
    //            frameArray[index] = new Frame(payloadSize);
    //            m_Camera.AnnounceFrame(frameArray[index]);
    //        }
    //        m_Camera.StartCapture();
    //        for (int index = 0; index < frameArray.Length; ++index)
    //        {
    //            m_Camera.QueueFrame(frameArray[index]);
    //        }

    //        feature = features["AcquisitionMode"];
    //        feature.EnumValue = "Continuous";
    //        feature = features["AcquisitionStart"];
    //        feature.RunCommand();
    //    }
    //    private void StopCamera()
    //    {
    //        FeatureCollection features = m_Camera.Features;
    //        Feature feature = features["AcquisitionStop"];
    //        feature.RunCommand();
    //        m_Camera.EndCapture();
    //        m_Camera.FlushQueue();
    //        m_Camera.RevokeAllFrames();
    //        m_Camera.Close();
    //    }
    //    private void OnFrameReceived(Frame frame)
    //    {
           
    //        if (VmbFrameStatusType.VmbFrameStatusComplete == frame.ReceiveStatus)
    //        {
    //            Bitmap bitmap = null;
    //            bitmap = new Bitmap((int)frame.Width, (int)frame.Height,PixelFormat.Format24bppRgb);
    //            frame.Fill(ref bitmap);
    //            bitmap.Save("filename.bmp", ImageFormat.Bmp);
    //            Console.WriteLine("Frame status complete");
    //        }
    //        m_Camera.QueueFrame(frame);
    //    }
    //}
}
