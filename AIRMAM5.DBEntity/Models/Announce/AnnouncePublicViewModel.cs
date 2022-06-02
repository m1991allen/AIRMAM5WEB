using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Announce
{
    /// <summary>
    /// 系統首頁公告 資料MOdel。　繼承參考 <see cref="AnnounceIdModel"/>
    /// </summary>
    public class AnnouncePublicViewModel : AnnounceIdModel
    {
        public AnnouncePublicViewModel() { }

        #region >>> 欄位定義
        /// <summary>
        /// 公告標題
        /// </summary>
        [Required]
        [Display(Name = "公告標題")]
        public string AnnTitle { get; set; } = string.Empty;

        /// <summary>
        /// 公告內容
        /// </summary>
        [Required]
        [Display(Name = "公告內容")]
        public string AnnContent { get; set; } = string.Empty;

        /// <summary>
        /// 上架日期
        /// </summary>
        [Required]
        [Display(Name = "上架日期(起)")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime AnnSdate { get; set; }

        /// <summary>
        /// 下架日期
        /// </summary>
        [Display(Name = "上架日期(迄)")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? AnnEdate { get; set; }

        /// <summary>
        /// 公告分類
        /// </summary>
        public string AnnType { get; set; } = string.Empty;

        [Display(Name = "公告分類")]
        public string AnnTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 發佈單位
        /// </summary>
        [Display(Name = "發佈單位")]
        public string AnnPublishDept { get; set; } = string.Empty;

        [Display(Name = "備註")]
        public string AnnNote { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 系統首頁公告 資料
        /// </summary>
        /// <param name="m">資料來源: 預存回傳值</param>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_ANNOUNCE_BY_LOGIN_ID_Result"/>,<see cref="spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result"/> 預存回傳值</typeparam>
        public AnnouncePublicViewModel DataConvert<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();

            foreach (var info in _Properties)
            {
                var _val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnANN_ID")
                    this.AnnounceId = string.IsNullOrEmpty(_val.ToString()) ? 0 : int.Parse(_val.ToString());

                if (info.Name == "fsTITLE") this.AnnTitle = _val.ToString();
                if (info.Name == "fsCONTENT") this.AnnContent = _val.ToString();
                if (info.Name == "fdSDATE") this.AnnSdate = DateTime.Parse(_val.ToString());
                if (info.Name == "fdEDATE") this.AnnEdate = DateTime.Parse(_val.ToString());
                if (info.Name == "fsTYPE") this.AnnType = _val.ToString();
                if (info.Name == "fsTYPE_NAME") this.AnnTypeName = _val.ToString();
                if (info.Name == "fsDEPT_NAME") this.AnnPublishDept = _val.ToString();
            }

            return this;
        }
    }

}
