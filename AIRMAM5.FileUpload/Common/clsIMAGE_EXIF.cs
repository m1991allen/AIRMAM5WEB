//==============================================================
//<2016/08/31><David.Sin><新增本程式><取得圖片的EXIF資訊>
//==============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;

namespace AIRMAM5.FileUpload.Common
{
    /// <summary>
    /// 取得圖片的EXIF資訊
    /// </summary>
    public class clsIMAGE_EXIF
    {
        readonly SerilogService _serilogService = new SerilogService();

        /// <summary>
        /// 取圖片EXIT資訊
        /// </summary>
        /// <param name="fsFILE_PATH"></param>
        /// <returns></returns>
        public ImageEXIF GetImage_EXIF(string fsFILE_PATH)
        {
            ImageEXIF model = new ImageEXIF();

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(fsFILE_PATH);

                //20210401_改下列寫法↓↓↓
                int.TryParse(image.PhysicalDimension.Width.ToString(), out int width);
                model.WIDTH = width; //int.Parse(image.PhysicalDimension.Width.ToString());
                int.TryParse(image.PhysicalDimension.Height.ToString(), out int height);
                model.HEIGHT = height; //int.Parse(image.PhysicalDimension.Height.ToString());

                image.Dispose();
                ExifNET.Exif exif = new ExifNET.Exif(fsFILE_PATH);
                //20210526_Exception_修正 >>> "Message":"輸入字串格式不正確。"
                model.XDPI = (exif.XResolution.HasValue ? Convert.ToInt32(exif.XResolution.Value) : 0);//(exif.XResolution.HasValue ? int.Parse(exif.XResolution.Value.ToString()) : 0);
                model.YDPI = (exif.YResolution.HasValue ? Convert.ToInt32(exif.YResolution.Value) : 0);//(exif.YResolution.HasValue ? int.Parse(exif.YResolution.Value.ToString()) : 0);

                model.CAMERA_MAKE = exif.Make;
                model.CAMERA_MODEL = exif.Model;
                model.FOCAL_LENGTH = (exif.FocalLength.HasValue ? exif.FocalLength.Value.ToString() : string.Empty);
                model.EXPOSURE_TIME = (exif.ExposureTime == null ? "" : "1/" + (1 / exif.ExposureTime.Value).ToString());
                model.APERTURE = (exif.ApertureValue.HasValue ? exif.ApertureValue.Value.ToString() : string.Empty);
                model.ISO = (exif.IsoSpeedRatings.HasValue ? int.Parse(exif.IsoSpeedRatings.Value.ToString()) : 0);

                //dicEXIF.Add("fnXDPI", (exif.YResolution.HasValue ? exif.XResolution.Value.ToString() : "0"));
                //dicEXIF.Add("fnYDPI", (exif.XResolution.HasValue ? exif.XResolution.Value.ToString() : "0"));
                //dicEXIF.Add("fsCAMERA_MAKE", exif.Make);
                //dicEXIF.Add("fsCAMERA_MODEL", exif.Model);
                //dicEXIF.Add("fsFOCAL_LENGTH", (exif.FocalLength.HasValue ? exif.FocalLength.Value.ToString() : string.Empty));
                //dicEXIF.Add("fsEXPOSURE_TIME", (exif.ExposureTime == null ? "" : "1/" + (1 / exif.ExposureTime.Value).ToString()));
                //dicEXIF.Add("fsAPERTURE", (exif.ApertureValue.HasValue ? exif.ApertureValue.Value.ToString() : string.Empty));
                //dicEXIF.Add("fnISO", (exif.IsoSpeedRatings.HasValue ? exif.IsoSpeedRatings.Value.ToString() : "0"));

                //ExifLibrary.ImageFile img = ExifLibrary.ImageFile.FromFile(fsFILE_PATH);
                //foreach (var item in img.Properties)
                //{
                //    //EXIFEnum exif = (EXIFEnum)Enum.Parse(typeof(EXIFEnum), item.Name);
                //    switch (item.Name)
                //    {
                //        case "XResolution":
                //            model.XDPI = int.Parse(item.Value.ToString().Replace("/1", ""));
                //            break;
                //        case "YResolution":
                //            model.YDPI = int.Parse(item.Value.ToString().Replace("/1", ""));
                //            break;
                //        case "Make":
                //            model.CAMERA_MAKE = item.Value.ToString().Replace("/1", "");
                //            break;
                //        case "Model":
                //            model.CAMERA_MODEL = item.Value.ToString().Replace("/1", "");
                //            break;
                //        case "FocalLength":
                //            model.FOCAL_LENGTH = item.Value.ToString().Replace("/1", "");
                //            break;
                //        case "ExposureTime":
                //            model.EXPOSURE_TIME = item.Value.ToString().Replace("/1", "");
                //            break;
                //        case "ApertureValue":
                //            model.APERTURE = item.Value.ToString().Replace("/1", "");
                //            break;
                //        case "ISOSpeedRatings":
                //            model.ISO = int.Parse(item.Value.ToString().Replace("/1", ""));
                //            break;
                //        //case "":
                //        //    break;
                //        default:
                //            break;
                //    }
                //}
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "clsIMAGE_EXIF",
                    Method = "[GetImage_EXIF]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { fsFILE_PATH, Exception = ex },
                    LogString = "抓取EXIF.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw ex;
            }

            return model;
        }

        [Obsolete("GetImage_EXIF")]
        public Dictionary<string, string> fnGET_IMAGE_EXIF(string fsFILE_PATH)
        {
            Dictionary<string, string> dicEXIF = new Dictionary<string, string>();

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(fsFILE_PATH);
                dicEXIF.Add("fnWIDTH", image.PhysicalDimension.Width.ToString());
                dicEXIF.Add("fnHEIGHT", image.PhysicalDimension.Height.ToString());
                image.Dispose();

                ExifLibrary.ImageFile img = ExifLibrary.ImageFile.FromFile(fsFILE_PATH);
                foreach (var item in img.Properties)
                {
                    if (item.Name == "XResolution")
                        dicEXIF.Add("fnXDPI", item.Value.ToString().Replace("/1",""));
                    else if (item.Name == "YResolution")
                        dicEXIF.Add("fnYDPI", item.Value.ToString().Replace("/1", ""));
                    else if (item.Name == "Make")
                        dicEXIF.Add("fsCAMERA_MAKE", item.Value.ToString().Replace("/1", ""));
                    else if (item.Name == "Model")
                        dicEXIF.Add("fsCAMERA_MODEL", item.Value.ToString().Replace("/1", ""));
                    else if (item.Name == "FocalLength")
                        dicEXIF.Add("fsFOCAL_LENGTH", item.Value.ToString().Replace("/1", ""));
                    else if (item.Name == "ExposureTime")
                        dicEXIF.Add("fsEXPOSURE_TIME", item.Value.ToString().Replace("/1", ""));
                    else if (item.Name == "ApertureValue")
                        dicEXIF.Add("fsAPERTURE", item.Value.ToString().Replace("/1", ""));
                    else if (item.Name == "ISOSpeedRatings")
                        dicEXIF.Add("fnISO", item.Value.ToString().Replace("/1", ""));
                }

                //Exif exif = new Exif(fsFILE_PATH);       

                //dicEXIF.Add("fnYDPI", (img.Properties[ExifLibrary.ExifTag.YResolution].Value != null ? img.Properties[ExifLibrary.ExifTag.YResolution].Value.ToString() : "0"));
                //dicEXIF.Add("fsCAMERA_MAKE", (img.Properties[ExifLibrary.ExifTag.Make].Value != null ? img.Properties[ExifLibrary.ExifTag.Make].Value.ToString() : string.Empty));
                //dicEXIF.Add("fsCAMERA_MODEL", (img.Properties[ExifLibrary.ExifTag.Model].Value != null ? img.Properties[ExifLibrary.ExifTag.Model].Value.ToString() : string.Empty));
                //dicEXIF.Add("fsFOCAL_LENGTH", (img.Properties[ExifLibrary.ExifTag.FocalLength].Value != null ? int.Parse(img.Properties[ExifLibrary.ExifTag.FocalLength].Value.ToString()).ToString() : string.Empty));
                //dicEXIF.Add("fsEXPOSURE_TIME", (img.Properties[ExifLibrary.ExifTag.ExposureTime].Value != null ? img.Properties[ExifLibrary.ExifTag.ExposureTime].Value.ToString() : string.Empty));
                //dicEXIF.Add("fsAPERTURE", (img.Properties[ExifLibrary.ExifTag.ApertureValue].Value != null ? int.Parse(img.Properties[ExifLibrary.ExifTag.ApertureValue].Value.ToString()).ToString() : string.Empty));
                //dicEXIF.Add("fnISO", (img.Properties[ExifLibrary.ExifTag.ISOSpeedRatings].Value != null ? img.Properties[ExifLibrary.ExifTag.ISOSpeedRatings].Value.ToString() : string.Empty));
                
            }
            catch (Exception ex)
            {
                #region Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "clsIMAGE_EXIF",
                    Method = "[fnGET_IMAGE_EXIF]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { fsFILE_PATH, Exception = ex },
                    LogString = "抓取EXIF.Exception",
                    ErrorMessage = ex.Message
                });
                #endregion
                throw ex;
            }

            return dicEXIF;
        }

    }

    /// <summary>
    /// Image EXIT 資訊
    /// </summary>
    public enum EXIFEnum
    {
        [Description("寬度")]
        WIDTH,

        [Description("高度")]
        HEIGHT,

        [Description("XDPI")]
        XDPI,

        [Description("YDPI")]
        YDPI,

        [Description("相機廠牌")]
        CAMERA_MAKE,

        [Description("相機型號")]
        CAMERA_MODEL,

        [Description("焦距")]
        FOCAL_LENGTH,

        [Description("曝光時間")]
        EXPOSURE_TIME,

        [Description("光圈")]
        APERTURE,

        [Description("ISO")]
        ISO
    }

    /// <summary>
    /// Image EXIT 資訊MODEL
    /// </summary>
    public class ImageEXIF
    {
        public int WIDTH = 0;
        public int HEIGHT = 0;
        public int XDPI = 0;
        public int YDPI = 0;
        public string CAMERA_MAKE = string.Empty;
        public string CAMERA_MODEL = string.Empty;
        public string FOCAL_LENGTH = string.Empty;
        public string EXPOSURE_TIME = string.Empty;
        public string APERTURE = string.Empty;
        public int ISO = 0;
    }
}