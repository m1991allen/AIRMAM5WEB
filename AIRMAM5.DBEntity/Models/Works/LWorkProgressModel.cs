//using AIRMAM5.DBEntity.DBEntity;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Works
{
    /// <summary>
    /// 轉檔工作編號進度資料
    /// </summary>
    public class LWorkProgressModel : LWorkIdModel
    {
        /// <summary>
        /// 轉檔工作編號進度資料
        /// </summary>
        public LWorkProgressModel() { }

        #region >>>欄位定義
        /// <summary>
        /// 進度 % [fsPROGRESS]
        /// </summary>
        [Display(Name = "轉檔進度")]
        [JsonProperty(PropertyName = "Progress")]
        public string Progress { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔狀態代碼  [fsSTATUS]
        /// </summary>
        public string WorkStatus { get; set; }

        /// <summary>
        /// 轉檔狀態代碼中文
        /// </summary>
        public string WorkStatusName { get; set; } = string.Empty;

        /// <summary>
        /// 狀態顏色表示
        /// </summary>
        public string StatusColor { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔開始時間
        /// </summary>
        public string WorkSTime { get; set; } = string.Empty;

        /// <summary>
        /// 轉檔結束時間
        /// </summary>
        public string WorkETime { get; set; } = string.Empty;
        #endregion

        /* marked_&_modified_20211007 */
        /// <summary>
        /// 轉檔工作編號進度資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_L_WORK_MERGE_Result"/> </typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public LWorkProgressModel FormatConversion<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnWORK_ID")
                    this.fnWORK_ID = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsPROGRESS") this.Progress = val.ToString();
                if (info.Name == "fsSTATUS") this.WorkStatus = val.ToString();
                if (info.Name == "C_sSTATUSNAME") this.WorkStatusName = val.ToString();
                if (info.Name == "C_sSTATUSCOLOR")
                    this.StatusColor = string.IsNullOrEmpty(val.ToString()) ? "grey" : val.ToString();

                if (info.Name == "fdSTIME")
                {
                    DateTime.TryParse(val.ToString(), out DateTime _dt);
                    this.WorkSTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", _dt);
                }

                if (info.Name == "fdETIME")
                {
                    DateTime.TryParse(val.ToString(), out DateTime _dt);
                    this.WorkETime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", _dt);
                }
            }

            return this;
        }
    }

}
