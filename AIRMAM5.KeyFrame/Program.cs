using Alex.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.KeyFrame
{
    class Program
    {


        static void Main(string[] args)
        {
            string fsSOURCE_FILE = @"";
            string fsTARGET_PATH = @"";
            string fsFILE_NAME = @"";
            string fsEXT = @"";
            double fnTIME = 0.0;
            int fnQUALITY = 0;
            int fnWIDTH = 0;
            int fnHEIGHT = 0;

            if (args != null && args.Count() > 0)
            {
                fsSOURCE_FILE = args[0];
                fsTARGET_PATH = args[1];
                fsFILE_NAME = args[2];
                fsEXT = args[3];
                fnTIME = double.Parse(args[4]);
                fnQUALITY = int.Parse(args[5]);
                fnWIDTH = int.Parse(args[6]);
                fnHEIGHT = int.Parse(args[7]);
            }

            Bitmap ImageExporter = null;

            try
            {
                FrameGrabber frame = new FrameGrabber(fsSOURCE_FILE);
                if (frame == null || frame.FrameCount == 0)
                    Console.Write("Error-無法解析媒體檔長度(null or 0)");

                EncoderParameters ImageEncoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                ImageEncoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, fnQUALITY);

                // 先取一張沒用的
                ImageExporter = frame.GetImageAtTime(0, fnWIDTH, fnHEIGHT);

                ImageExporter = frame.GetImageAtTime(fnTIME, fnWIDTH, fnHEIGHT);
                ImageExporter.Save(fsTARGET_PATH + fsFILE_NAME + "." + fsEXT, SelectTargetEncoder(fsEXT), ImageEncoderParams);

                Console.Write("Success");
            }
            catch (Exception ex)
            {
                Console.Write("Error-" + ex.Message);
            }
            finally
            {
                if (ImageExporter != null)
                    ImageExporter.Dispose();
            }
            
        }


        // 判斷要用哪種 Encoder 
        protected static System.Drawing.Imaging.ImageCodecInfo SelectTargetEncoder(string extName)
        {
            foreach (var codec in System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders())
                if (codec.FilenameExtension.Contains(extName.ToUpper()))
                    return codec;
            return null;
        }
    }

    
}
