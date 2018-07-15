using AVT.VmbAPINET;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SelfServer
{
    public class SyncAlliedCamera
    {
        private Camera camera = null;
        private long payloadSize;
        private Frame frame;

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

        public void AccquireImage(string pathsuffix)
        {
            while (true)
            {
                camera.AcquireSingleImage(ref frame, 5000); //timout at 5secons
                if (frame.ReceiveStatus == VmbFrameStatusType.VmbFrameStatusComplete)
                {
                    var fileName = $"camera_{pathsuffix}.bmp";
                    Bitmap bitmap = null;
                    bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);
                    frame.Fill(ref bitmap);
                    bitmap.Save(fileName, ImageFormat.Bmp);
                    Console.WriteLine("Frame status complete");
                    break;
                }
                //wait 100 milliseconds and try again
                System.Threading.Thread.Sleep(100);
            }
        }

        public void StopCamera()
        {
            FeatureCollection features = camera.Features;
            Feature feature = features["AcquisitionStop"];
            feature.RunCommand();
            camera.EndCapture();
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
