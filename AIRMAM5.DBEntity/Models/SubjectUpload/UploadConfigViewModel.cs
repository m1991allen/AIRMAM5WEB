using System;

namespace AIRMAM5.DBEntity.Models.SubjectUpload
{
    /// <summary>
    /// 檔案上傳相關參數資訊(Resumable.js)
    /// </summary>
    public class UploadConfigViewModel
    {
        /// <summary>
        /// 檔案上傳相關參數資訊(Resumable.js)
        /// </summary>
        public UploadConfigViewModel()
        {
            UploadFileBuffer = 200;
            SimultaneousUploads = 3;
            TempFolder = Guid.NewGuid().ToString().Replace("-", "");
        }

        #region >>> 屬性/欄位定義
        /// <summary>
        /// 檔案上傳目的地URL
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// 每個(切割)檔案上傳大小 (單位:MB) (預設值：200mb)
        /// </summary>
        public int UploadFileBuffer { get; set; }

        /// <summary>
        /// 同時上傳數（默認值：3）
        /// </summary>
        public int SimultaneousUploads { get; set; }

        /// <summary>
        /// 上傳者 UserName
        /// </summary>
        public string LoginId { get; set; } = string.Empty;

        /// <summary>
        /// 上傳後暫存資料夾名稱 fsFOLDER
        /// </summary>
        public string TempFolder { get; set; }

        /// <summary>
        /// 檔案上傳時,前端等候時間(秒)
        /// </summary>
        public int TimeoutSec { get; set; } = 5;
        #endregion
    }

}
