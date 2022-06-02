using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.AcrDelete
{
    /// <summary>
    /// 刪除紀錄管理 - 刪除實體檔案確認頁面
    /// </summary>
    public class DeleteConfirmViewModel : TtbmARCIndexIdModel
    {
        /// <summary>
        /// 刪除紀錄管理 - 刪除實體檔案確認頁面
        /// </summary>
        public DeleteConfirmViewModel() { }

        #region >>>>> 屬性/欄位
        ///// <summary>
        ///// 編號 fnINDEX_ID (媒資刪除記錄)
        ///// </summary>
        //[Display(Name = "編號")]
        //public long IndexId { get; set; }

        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public string fsFILE_NO { set; get; } = string.Empty;

        /// <summary>
        /// 類別 
        /// </summary>
        [Display(Name = "類別")]
        public string fsTYPE { get; set; }
        public string C_sTYPE { get; set; }

        /// <summary>
        /// 標題
        /// </summary>
        [Display(Name = "標題")]
        public string C_sTITLE { get; set; }

        /// <summary>
        /// 刪除原因 [fsREASON]
        /// </summary>
        [Display(Name = "刪除原因")]
        public string DeleteReason { get; set; } = string.Empty;

        /// <summary>
        /// 新增(刪除資料)人員
        /// </summary>
        [Display(Name = "刪除人員")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 新增(刪除資料)時間
        /// </summary>
        [Display(Name = "刪除時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// (刪除資料)異動人員
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// (刪除資料)異動時間
        /// </summary>
        [Display(Name = "修改時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? UpdatedDate { get; set; }
        #endregion

        /// <summary>
        /// 刪除紀錄管理 - 刪除實體檔案確認頁面資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="sp_t_GET_ARC_INDEX_BY_CONDITION_Result"/> 預存回傳值</typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public DeleteConfirmViewModel DataConvert<T>(T m)
        {
            string insuser = null, insusernm = null, upduser = null, updusernm = null;
            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnINDEX_ID")
                    this.IndexId = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsFILE_NO") this.fsFILE_NO = val.ToString();
                if (info.Name == "fsTYPE") this.fsTYPE = val.ToString();
                if (info.Name == "C_sTYPE") this.C_sTYPE = val.ToString();
                if (info.Name == "C_sTITLE") this.C_sTITLE = val.ToString();
                if (info.Name == "fsREASON") this.DeleteReason = val.ToString();

                if (info.Name == "fsCREATED_BY") insuser = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") insusernm = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime insDate);
                    this.CreatedDate = insDate;
                }

                if (info.Name == "fsUPDATED_BY") upduser = val.ToString();
                if (info.Name == "fsUPDATED_BY_NAME") updusernm = val.ToString();
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime updDate);
                    if (string.IsNullOrEmpty(val.ToString())) this.UpdatedDate = null; else this.UpdatedDate = updDate;
                }

            }

            this.CreatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(insuser) ? string.Empty : insuser
                    , string.IsNullOrEmpty(insusernm) ? string.Empty : string.Format($"({insusernm})"));
            this.UpdatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(upduser) ? string.Empty : upduser
                    , string.IsNullOrEmpty(updusernm) ? string.Empty : string.Format($"({updusernm})"));

            return this;
        }
    }

}
