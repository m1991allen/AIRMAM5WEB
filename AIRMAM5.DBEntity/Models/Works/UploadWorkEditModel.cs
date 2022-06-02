//using AIRMAM5.DBEntity.DBEntity;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Works
{
    /// <summary>
    /// 上傳紀錄內容 Edit Model
    /// </summary>
    public class UploadWorkEditModel
    {
        /// <summary>
        /// 上傳紀錄內容 Edit Model
        /// </summary>
        public UploadWorkEditModel() { }

        #region >>> 參數定義
        /// <summary>
        /// 轉檔編號 fnWORK_ID
        /// </summary>
        [Display(Name = "編號")]
        public long fnWORK_ID { get; set; }

        /// <summary>
        /// 工作/轉檔類別 fsTYPE , dbo.tbzCODE.fsCODE_ID = WORK001
        /// </summary>
        [Display(Name = "轉檔類別")]
        public string fsTYPE { get; set; }
        /// <summary>
        /// 工作/轉檔類別 中文
        /// </summary>
        public string C_sTYPENAME { get; set; } = string.Empty;

        /// <summary>
        /// 工作/轉檔狀態 fsSTATUS , tbzCODE.fsCODE_ID = WORK_TC/WORK_BK
        /// </summary>
        [Display(Name = "狀態")]
        public string WorkStatus { get; set; } = string.Empty;
        /// <summary>
        /// 轉檔狀態 [C_sSTATUSNAME]
        /// </summary>
        [Display(Name = "轉檔狀態")]
        public string StatusName { get; set; } = string.Empty;
        /// <summary>
        /// 狀態顏色表示
        /// </summary>
        public string StatusColor { get; set; } = string.Empty;

        /// <summary>
        /// 進度% [fsPROGRESS]
        /// </summary>
        [Display(Name = "進度%")]
        public string Progress { get; set; }
        /// <summary>
        /// 優先順序 fsPRIORITY
        /// </summary>
        [Display(Name = "優先順序")]
        //[Range(1, 9)]
        public string fsPRIORITY { get; set; }

        /// <summary>
        /// 轉檔結果 fsRESULT
        /// </summary>
        [Display(Name = "轉檔結果")]
        public string fsRESULT { get; set; } = string.Empty;

        /// <summary>
        /// 備註 fsNOTE
        /// </summary>
        [Display(Name = "備註")]
        public string fsNOTE { get; set; } = string.Empty;

        /// <summary> 
        /// 開始轉檔時間 fdSTIME
        /// </summary>
        [Display(Name = "開始轉檔時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? fdSTIME { get; set; }

        /// <summary>
        /// 結束轉檔時間 fdETIME
        /// </summary>
        [Display(Name = "結束轉檔時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? fdETIME { get; set; }

        /// <summary>
        /// 檔案資訊 C_sFILE_INFO
        /// </summary>
        [Display(Name = "檔案資訊")]
        public string C_sFILE_INFO { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔參數 fsPARAMETERS
        /// </summary>
        [Display(Name = "轉檔參數")]
        public string fsPARAMETERS { get; set; } = string.Empty;

        /// <summary>
        /// 建立者
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedBy { get; set; }
        #endregion

        /* marked_&_modified_20211006 */
        /// <summary>
        /// 上傳紀錄內容 Edit Model, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_L_WORK_BY_TRANSCODE_Result"/> </typeparam>
        /// <param name="m"></param>
        public UploadWorkEditModel FormatConversion<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();
            string insuser = null, insusernm = null;

            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnWORK_ID")
                    this.fnWORK_ID = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsTYPE") this.fsTYPE = val.ToString();
                if (info.Name == "C_sTYPENAME") this.C_sTYPENAME = val.ToString();
                if (info.Name == "fsSTATUS") this.WorkStatus = val.ToString();
                if (info.Name == "C_sSTATUSNAME") this.StatusName = val.ToString();
                if (info.Name == "fsSTATUS_COLOR")
                    this.StatusColor = string.IsNullOrEmpty(val.ToString()) ? "grey" : val.ToString();

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

                if (info.Name == "fsCREATED_BY") insuser = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") insusernm = val.ToString();
            }

            this.CreatedBy = string.Format("{0}{1}"
                , string.IsNullOrEmpty(insuser) ? string.Empty : insuser
                , string.IsNullOrEmpty(insusernm) ? string.Empty : string.Format($"({insusernm})"));

            return this;
        }
    }

}
