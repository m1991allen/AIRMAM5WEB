using AIRMAM5.DBEntity.Models.Search;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.SearchResponse
{
    /// <summary>
    /// 檢索結果-【圖片 PHOTO】: SearchResponseBase() + EXIF資訊
    /// </summary>
    public class SearchResponsePhotoModel
    {
        /// <summary>
        /// 檢索結果-【圖片 PHOTO】
        /// </summary>
        public SearchResponsePhotoModel() { }

        /// <summary>
        /// 檢索結果-【圖片 PHOTO】
        /// </summary>
        /// <param name="m">檢索參數 <see cref="SearchParameterViewModel"/> </param>
        public SearchResponsePhotoModel(SearchParameterViewModel m) { SearchBase.SearchParam = m; }

        /// <summary>
        /// 檢索結果-Basic <see cref="SearchResponseBaseModel"/>
        /// </summary>
        public SearchResponseBaseModel SearchBase { get; set; } = new SearchResponseBaseModel();

        /// <summary>
        /// 圖片-EXIF資訊 <see cref="ExifInfo"/>
        /// </summary>
        public ExifInfo PhotoExifInfo { get; set; } = new ExifInfo();

        /// <summary>
        /// EXIF資訊 
        /// </summary>
        public class ExifInfo
        {
            /// <summary>
            /// 圖片寬
            /// </summary>
            [Display(Name= "圖片寬")]
            public int PhotoWidth { get; set; } = 0;

            /// <summary>
            /// 圖片高
            /// </summary>
            [Display(Name = "圖片高")]
            public int PhotoHeight { get; set; } = 0;

            /// <summary>
            /// XDPI
            /// </summary>
            [Display(Name = "XDPI")]
            public int PhotoXdpi { get; set; } = 0;

            /// <summary>
            /// YDPI
            /// </summary>
            [Display(Name = "YDPI")]
            public int PhotoYdpi { get; set; } = 0;

            /// <summary>
            /// 相機廠牌
            /// </summary>
            [Display(Name = "相機廠牌")]
            public string CameraMake { get; set; } = string.Empty;

            /// <summary>
            /// 相機型號
            /// </summary>
            [Display(Name = "相機型號")]
            public string CameraModel { get; set; } = string.Empty;

            /// <summary>
            /// 焦距
            /// </summary>
            [Display(Name = "焦距")]
            public string FocalLength { get; set; } = string.Empty;

            /// <summary>
            /// 曝光時間
            /// </summary>
            [Display(Name = "曝光時間")]
            public string ExposureTime { get; set; } = string.Empty;

            /// <summary>
            /// 光圈
            /// </summary>
            [Display(Name = "光圈")]
            public string Aperture { get; set; } = string.Empty;

            /// <summary>
            /// ISO
            /// </summary>
            [Display(Name = "ISO")]
            public int ISO { get; set; } = 0;
        }
    }

}
