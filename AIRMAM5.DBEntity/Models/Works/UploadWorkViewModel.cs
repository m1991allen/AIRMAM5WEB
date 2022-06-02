//using AIRMAM5.DBEntity.DBEntity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Works
{
    /// <summary>
    /// 手動上傳轉檔詳細內容Modal
    /// </summary>
    public class UploadWorkViewModel : UploadWorkEditModel
    {
        /// <summary>
        /// 手動上傳轉檔詳細內容Modal
        /// </summary>
        public UploadWorkViewModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "新增時間")]
        public DateTime CreatedTime { get; set; }
        
        //[Display(Name = "新增人員")]
        //public string CreatedBy { get; set; }

        /// <summary>
        /// 最後異動時間
        /// </summary>
        [Display(Name = "修改時間")]
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// 最後異動者
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedBy { get; set; }
        #endregion

        /* marked_&_modified_20211006 */
        /// <summary>
        /// 手動上傳轉檔詳細內容Modal, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_L_WORK_BY_TRANSCODE_Result"/> </typeparam>
        /// <param name="m">資料來源資料集合 <typeparamref name="T"/></param>
        public new UploadWorkViewModel FormatConversion<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();
            string insuser = null, insusernm = null, upduser = null, updusernm = null;

            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnWORK_ID")
                    this.fnWORK_ID = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsTYPE") this.fsTYPE = val.ToString();
                if (info.Name == "C_sTYPENAME") this.C_sTYPENAME = val.ToString();
                if (info.Name == "fsSTATUS") this.WorkStatus = val.ToString();
                if (info.Name == "C_sSTATUSNAME") this.StatusName = val.ToString();
                if (info.Name == "fsPRIORITY") this.fsPRIORITY = val.ToString();
                if (info.Name == "fsRESULT") this.fsRESULT = val.ToString();
                if (info.Name == "fsNOTE") this.fsNOTE = val.ToString();

                if (info.Name == "fdSTIME")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.fdSTIME = dt;
                }
                if (info.Name == "fdETIME")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.fdETIME = dt;
                }

                if (info.Name == "C_sFILE_INFO") this.C_sFILE_INFO = val.ToString();
                if (info.Name == "fsPARAMETERS") this.fsPARAMETERS = val.ToString();
                if (info.Name == "fsPROGRESS") this.Progress = val.ToString();

                if (info.Name == "fsSTATUS_COLOR")
                    this.StatusColor = string.IsNullOrEmpty(val.ToString()) ? "grey" : val.ToString();

                if (info.Name == "fsCREATED_BY") insuser = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") insusernm = val.ToString();
                if (info.Name == "fsUPDATED_BY") upduser = val.ToString();
                if (info.Name == "fsUPDATED_BY_NAME") updusernm = val.ToString();

                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.CreatedTime = dt;
                }
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.UpdatedTime = dt;
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
