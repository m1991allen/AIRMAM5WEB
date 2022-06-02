using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Material
{
    /// <summary>
    /// 段落剪輯/粗剪 資料
    /// </summary>
    public class MaterialFilmModel
    {
        /// <summary>
        /// 段落剪輯/粗剪 資料
        /// </summary>
        public MaterialFilmModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 檔案編號 [fsFILE_NO], <see cref="spGET_MATERIAL_BY_MARKED_BY_Result.fsFILE_NO"/> 
        /// </summary>
        [Display(Name = "檔案編號")]
        public string FileNo { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類型 [fsTYPE]= V,A,P,D, <see cref="spGET_MATERIAL_BY_MARKED_BY_Result.fsTYPE"/> 
        /// </summary>
        [Display(Name = "媒資類型")]
        public string FileCategory { get; set; } = string.Empty;

        /// <summary>
        /// 影片長度(時長) [_sVIDEO_MAX_TIME], <see cref="spGET_MATERIAL_BY_MARKED_BY_Result.C_sVIDEO_MAX_TIME"/> 
        /// </summary>
        [Display(Name = "時長")]
        public string VideoMaxTime { get; set; } = string.Empty;

        ///// <summary>
        ///// 描述 [fsDESCRIPTION]
        ///// </summary>
        //[Display(Name = "描述")]
        //public string FilmDesc { get; set; } = string.Empty;

        ///// <summary>
        ///// 備註 [fsNOTE]
        ///// </summary>
        //[Display(Name = "備註")]
        //public string FilmNote { get; set; } = string.Empty;

        /// <summary>
        /// 低解路徑 [_sFILE_URL] <see cref="spGET_MATERIAL_BY_MARKED_BY_Result.C_sFILE_URL"/> 
        /// </summary>
        [Display(Name = "低解路徑")]
        public string FileDLowUrl { get; set; } = string.Empty;
        #endregion

        public MaterialFilmModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var _Properties = typeof(T).GetProperties();

            foreach (var info in _Properties)
            {
                var val = info.GetValue(data) ?? string.Empty;

                if (info.Name == "fsFILE_NO") this.FileNo = val.ToString();
                if (info.Name == "fsTYPE") this.FileCategory = val.ToString();
                if (info.Name == "C_sVIDEO_MAX_TIME") this.VideoMaxTime = val.ToString();

                if (info.Name == "C_sFILE_URL") this.FileDLowUrl = val.ToString();
            }
            return this;
        }
    }

}
