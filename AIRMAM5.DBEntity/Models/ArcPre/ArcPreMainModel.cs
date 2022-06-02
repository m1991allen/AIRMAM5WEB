using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋: 資訊欄位。 繼承參考 <see cref="TbmArcPreIdModel"/>
    /// </summary>
    public class ArcPreMainModel : TbmArcPreIdModel
    {
        /// <summary>
        /// 預編詮釋: 資訊欄位
        /// </summary>
        public ArcPreMainModel() { }

        #region >>>>> spGET_ARC_PRE_Result 部分欄位 
        /// <summary>
        /// 預編名稱 fsNAME
        /// </summary>
        [Required]
        [Display(Name = "預編名稱")]
        public string fsNAME { get; set; } = string.Empty;

        /// <summary>
        /// 類型 fsTYPE = S、V、A、P、D
        /// </summary>
        [Required]
        [Display(Name = "類型")]
        public string fsTYPE { get; set; } = string.Empty;

        /// <summary>
        /// 類型名稱 fsTYPE_NAME
        /// </summary>
        [Display(Name = "類型")]
        public string fsTYPE_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 樣板 fnTEMP_ID
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "樣板")]
        public int fnTEMP_ID { get; set; } = 0;

        /// <summary>
        /// 樣板名稱 fsTEMP_NAME
        /// </summary>
        [Display(Name = "使用樣板")]
        public string fsTEMP_NAME { get; set; } = string.Empty;

        /// <summary>
        /// 標題 fsTITLE
        /// </summary>
        [Required]
        [Display(Name = "標題")]
        public string fsTITLE { get; set; } = string.Empty;

        /// <summary>
        /// 描述 fsDESCRIPTION
        /// </summary>
        [Display(Name = "描述")]
        public string fsDESCRIPTION { get; set; } = string.Empty;

        /// <summary>
        /// 自訂標籤 HashTag
        /// </summary>
        /// <remarks> ★資料欄位為字串型態, ^為分隔符號。#符號剔除不存入資料欄位。 20211123_ADDED </remarks>
        [Display(Name = "自訂標籤")]
        public string fsHashTag { get; set; }
        /// <summary>
        /// 自訂標籤 HashTag Array 
        /// </summary>
        /// <remarks> ★資料欄位為字串型態, ^為分隔符號。#符號剔除不存入資料欄位。 20211123_ADDED </remarks>
        [Display(Name = "自訂標籤")]
        public string[] HashTag { get; set; }
        #endregion

        /* Marked_BY_20211005 */
        /// <summary>
        ///  預編詮釋: 資訊欄位 - 資料格式轉換 
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public ArcPreMainModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                string val = pp.GetValue(data) == null ? string.Empty : pp.GetValue(data).ToString();

                if (pp.Name == "fnPRE_ID")
                {
                    if (long.TryParse(val, out long idx)) { this.fnPRE_ID = idx; }
                }
                if (pp.Name == "fsNAME") { this.fsNAME = val; }
                if (pp.Name == "fsTYPE") { this.fsTYPE = val; }
                if (pp.Name == "fsTYPE_NAME") { this.fsTYPE_NAME = val; }
                if (pp.Name == "fnTEMP_ID")
                {
                    if (int.TryParse(val, out int idx)) { this.fnTEMP_ID = idx; }
                }
                if (pp.Name == "fsTEMP_NAME") { this.fsTEMP_NAME = val; }
                if (pp.Name == "fsTITLE") { this.fsTITLE = val; }
                if (pp.Name == "fsDESCRIPTION") { this.fsDESCRIPTION = val; }
                //20211123_ADDED)_自訂標籤, TIP: ★資料欄位為字串型態, ^為分隔符號。#符號剔除不存入資料欄位。
                if (pp.Name == "fsHASH_TAG")
                {
                    var b = val.ToString().Replace("#", "").Split(new char[] { '^' });
                    this.HashTag = b;
                    this.fsHashTag = val.ToString();
                }
            }

            return this;
        }
    }

}
