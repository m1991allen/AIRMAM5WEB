using NReco.VideoConverter;
using System;
using System.Drawing;
using System.IO;

/* 【相關類別文檔參考】
 * 
 * 一、Bitmap Class
 *  https://docs.microsoft.com/zh-tw/dotnet/api/system.drawing.bitmap?view=dotnet-plat-ext-3.1
 * 
 * 二、Video Converter for .NET
 *  https://www.nrecosite.com/video_converter_net.aspx
 *  Video Converter provides C# API for executing FFMpeg process from .NET applications. 
 *  It can convert media files, cut or combine video, get video thumbnail, capture screen, create video from images, decode video frames as bitmaps etc.
 *  (專案參考後組件readme位置 ..\AIRMAM5WEB\packages\NReco.VideoConverter.1.1.4)
 * 
 * 三、Graphics Class
 *  https://docs.microsoft.com/zh-tw/dotnet/api/system.drawing.graphics?view=dotnet-plat-ext-3.1
 * 
 * 
 * */

namespace AIRMAM5.Utility.Common
{
    /// <summary>
    /// 影頻轉換 類別。Video Converter for .NET
    /// </summary>
    /// <remarks> 使用(nuget)組件: NReco.VideoConverter.dll 
    /// </remarks>
    public class CommonVideoConvert
    {
        FFMpegConverter ffmpeg = new FFMpegConverter();

        private string _fileName;

        /// <summary>
        /// 影頻轉換 類別。Video Converter for .NET
        /// </summary>
        public CommonVideoConvert() : base() { }

        /// <summary>
        /// 影頻轉換 類別
        /// </summary>
        /// <param name="filename">來源檔案名稱(完整路徑) </param>
        public CommonVideoConvert(string filename)
        {
            this.FileName = filename;
        }

        #region >>> 參數
        /// <summary>
        /// 來源檔案名稱(完整路徑)
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
            }
        }
        #endregion

        /// <summary>
        /// FOR TEST : 擷取影片指定秒數圖片、轉為音頻mp3
        /// </summary>
        /// <returns></returns>
        public void GetImageAtTime(float sec = 12, int width=480, int height=270)
        {
            int targetWidth = width, targetHeight = height;

            if (File.Exists(_fileName))
            {
                string _orig = @"D:\_Project.Other\新增資料夾.mp4\_Westworld.mp4";
                string _target = @"D:\_Project.Other\新增資料夾.mp4\_WestworldThumbnail.jpg"
                        , _target2 = @"D:\_Project.Other\新增資料夾.mp4\_WestworldThumbnail_2.jpg";

                // Get thumbnail at a specified time in seconds
                Console.WriteLine("Generating image...");
                ffmpeg.GetVideoThumbnail(_orig, _target);
                ffmpeg.GetVideoThumbnail(_orig, _target2, sec);

                string _targetAudio = @"D:\_Project.Other\新增資料夾.mp4\_WestworldToAudio.mp3";
                //Extract Audio from Video ,視頻檔案轉換為音頻檔案(.mp3)
                Console.WriteLine("Extracting Audio...");
                ffmpeg.ConvertMedia(_orig, _targetAudio, "mp3");
            }
        }

        /// <summary>
        /// 在特定時間獲取圖像
        /// </summary>
        /// <param name="sec">指定秒數 </param>
        /// <param name="width">指定圖寬度 </param>
        /// <param name="height">指定圖高度 </param>
        /// <returns></returns>
        public Bitmap GetImageAtSpecificTime(float sec, int width, int height)
        {
            int targetWidth = width, targetHeight = height;
            Image newImg = null;

            try
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    throw new InvalidOperationException("cannot retrieve the frame because the FileName property has not been set yet");
                }

                if (File.Exists(_fileName))
                {
                    /* 讀寫記憶體 方式*/
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        //FFMpegConverter ffmpeg = new FFMpegConverter();
                        ffmpeg.GetVideoThumbnail(_fileName, memStream, sec);
                        using (Image image = Image.FromStream(memStream, true, false))
                        {
                            //newImg = ScaleImage(image, 128, 128);
                            newImg = ScaleImage(image, targetWidth, targetHeight);
                        }
                    }

                    Color background = Color.Empty;
                    Image target = new Bitmap(1, 1);    //指定大小，初始化 Bitmap
                    (target as Bitmap).SetPixel(0, 0, background);  //設定 Bitmap 中指定像素的色彩
                    target = new Bitmap(target, targetWidth, targetHeight); //從指定的影像(已縮放至指定之大小)，初始化 Bitmap

                    // Creates a new Graphics from the specified Image.
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        // Clears the entire drawing surface and fills it with the specified background color.
                        g.Clear(background);

                        /* ↓最後圖片要留白邊作法↓ */
                        //int x = (targetWidth - temp.Width) / 2;
                        //int y = (targetHeight - temp.Height) / 2;
                        //g.DrawImage(temp, x, y);

                        // Draws the specified Image at the specified location and with the original size.
                        g.DrawImage(newImg, 0, 0);
                    }
                }
                else
                {
                    //return null;
                    throw new ArgumentException(String.Format(" the file \"{0}\" does not exist. ", _fileName));
                }
            }
            catch (Exception ex)
            {
                //return null;
                throw new ArgumentException(
                    String.Format("unable to open the file \"{0}\", VideoConvert reported the following error: {1}", _fileName, ex.Message));
            }

            return (Bitmap)newImg;
        }

        #region >>> Method
        /// <summary>
        /// Scales a Image to make it fit inside of a Height/Width
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxwidth"></param>
        /// <param name="maxheight"></param>
        /// <returns></returns>
        private static Image ScaleImage(Image image, int maxwidth, int maxheight)
        {
            if (image == null || maxwidth <= 0 || maxheight <= 0)
            {
                return null;
            }

            double ratioX = (double)maxwidth / image.Width;
            double ratioY = (double)maxheight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            Bitmap newImage = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
        #endregion

    }
}
