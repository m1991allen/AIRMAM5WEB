using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using AIRMAM5.Utility.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace AIRMAM5.Common
{
    /* 改寫說明：
     *  20200910 - 
     *    ①直接參考DirectShowLib-2005.dll、AlexFrameGrabber.dll，本機開發測試:失敗(有錯誤)
     *    ②將AlexFrameGrabber.dll內的Method: FrameGrabber.cs 直按寫到AIRMAM5專案中使用，本機開發測試:失敗(有錯誤)
     *  20200916 - 
     *    最後另找影片截圖組件(NReco.VideoConverter.dll)，可參閱 AIRMAM5.Utility專案: CommonVideoConvert.cs。
     * 
     * */

    /// <summary>
    /// 關鍵影格 KeyFrame 
    /// </summary>
    public class KeyFrame
    {
        readonly SerilogService _serilogService = new SerilogService();

        #region >>> 擷取關鍵影格 參數.定義
        /// <summary>
        /// 來源檔案 fsSOURCE_FILE
        /// </summary>
        public string SourceFile { get; set; } = string.Empty;

        /// <summary>
        /// 目標路徑 fsTARGET_PATH
        /// </summary>
        public string TargetPath { get; set; } = string.Empty;

        /// <summary>
        /// (影格)檔案名 fsFILE_NAME
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// (影格) fsEXT
        /// </summary>
        public string FeileEXT { get; set; } = string.Empty;

        /// <summary>
        /// (影格)時間 fnTIME
        /// </summary>
        public double FileTime { get; set; } = 0d;

        /// <summary>
        /// (影格)檔案品質 fnQUALITY
        /// </summary>
        public int FileQuality { get; set; } = 0;

        /// <summary>
        /// 寬 fnWIDTH
        /// </summary>
        public int FileWidth { get; set; } = 0;

        /// <summary>
        /// 高 fnHEIGHT
        /// </summary>
        public int FileHeight { get; set; } = 0;
        #endregion

        #region >>> Method
        /// <summary>
        /// 判斷要用哪種 Encoder 
        /// </summary>
        /// <param name="extName"></param>
        /// <returns></returns>
        protected static ImageCodecInfo SelectTargetEncoder(string extName)
        {
            foreach (var codec in ImageCodecInfo.GetImageEncoders())
                if (codec.FilenameExtension.Contains(extName.ToUpper()))
                    return codec;

            return null;
        }
        #endregion

        #region >>> VideoConverter_added_20200916
        /// <summary>
        /// 影片{指定秒數}擷取圖片 : using AIRMAM5.Utility.Common.CommonVideoConvert.cs
        /// </summary>
        /// <returns></returns>
        public VerifyResult CaptureConvert()
        {
            VerifyResult result = new VerifyResult();
            Bitmap ImageExporter = null;

            try
            {
                CommonVideoConvert capture = new CommonVideoConvert(SourceFile);
                /* 20200918 - 因為輸入的時間點都會比抓取的時間點少一格多一點，因此統一扣掉一格半
                 *    少數影片在指定秒數擷圖,會變成是下一格影格的圖示(經證實:原本的截圖方式也是有相同情況)
                 *    依指示 擷圖的秒數再扣除"1.5影格"的秒數。
                 * */
                float.TryParse((FileTime - ((1 / 29.97) * 1.5)).ToString(), out float filetimes);
                ImageExporter = capture.GetImageAtSpecificTime(filetimes, FileWidth, FileHeight);

                //因為輸入的時間點都會比抓取的時間點少一格多一點，因此統一扣掉一格半
                ImageExporter = capture.GetImageAtSpecificTime((float)(FileTime - (1 / 29.97 * 1.5)), FileWidth, FileHeight);

                EncoderParameters ImageEncoderParams = new EncoderParameters(1);
                ImageEncoderParams.Param[0] = new EncoderParameter(Encoder.Quality, FileQuality);

                ImageExporter.Save(
                    TargetPath + FileName + "." + FeileEXT,
                    SelectTargetEncoder(FeileEXT),
                    ImageEncoderParams);

                result.IsSuccess = true;
                result.Message = "影格擷取成功! ";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "KeyFrame",
                    Method = "[CaptureConvert]",
                    EventLevel = SerilogLevelEnum.Debug,
                    Input = new { Param = this, Exception = ex },
                    LogString = "擷取關鍵影格.Exception",
                    ErrorMessage = ex.Message,
                });
                #endregion
            }
            finally
            {
                if (ImageExporter != null)
                    ImageExporter.Dispose();
            }

            return result;
        }
        #endregion

    }

}