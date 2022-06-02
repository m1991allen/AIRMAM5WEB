using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AIRMAM5
{
    /// <summary>
    /// 專案config檔案中的自訂義參數(appSettings)值
    /// </summary>
    public static class Config
    {
        public readonly static string SerilogPath = ConfigurationManager.AppSettings["SerilogPath"].ToString();
        
        /// <summary>
        /// 加密金鑰
        /// </summary>
        public readonly static string fsENC_KEY = ConfigurationManager.AppSettings["fsENC_KEY"].ToString();
        
        /// <summary>
        /// Search API URL
        /// </summary>
        public readonly static string SearchUrl = ConfigurationManager.AppSettings["fsSEARCH_API"].ToString();
        
        /// <summary>
        /// TSM API URL
        /// </summary>
        public readonly static string TsmUrl = ConfigurationManager.AppSettings["fsTSM_API"].ToString();
        
        /// <summary>
        /// 檔案上傳 URL
        /// </summary>
        public readonly static string UploadUrl = ConfigurationManager.AppSettings["fsUpload_API"].ToString();

        //20200910_移除AIRMAM5.KeyFrame 專案,整合到AIRMAM5
        ///// <summary>
        ///// 關鍵影格.exe
        ///// </summary>
        //public readonly static string KeyFrameExe = ConfigurationManager.AppSettings["fsKeyframe"].ToString();

        /// <summary>
        /// 檔案上傳時,前端等候時間(秒)
        /// </summary>
        public readonly static int UploadTimeOut = int.Parse(ConfigurationManager.AppSettings["UploadTimeout"].ToString());

        /// <summary>
        /// 專案是否使用TSM服務
        /// </summary>
        public readonly static string IsUseTSM = ConfigurationManager.AppSettings["IsUseTSM"].ToString();
        
        /// <summary>
        /// 非雲端功能開啟,true會開放雲端無法使用的功能,false只會顯示雲端能使用的功能
        /// </summary>
        public readonly static string IsNonCloud = ConfigurationManager.AppSettings["IsNonCloud"].ToString();
        
        /// <summary>
        /// 終止PostAsync Task 時間(秒)
        /// </summary>
        public readonly static int CancelPostAsync = int.Parse(ConfigurationManager.AppSettings["CancelPostAsync"].ToString());
        
        /// <summary>
        /// 系統報表Report Server URL
        /// </summary>
        public readonly static string ReportServer = ConfigurationManager.AppSettings["ReportServer"].ToString();
        
        /// <summary>
        /// 登入頁面產品描述(可以用HTML,但特殊字元需轉譯)
        /// </summary>
        public readonly static string VersionDescription = ConfigurationManager.AppSettings["versionDescription"].ToString();
        
        /// <summary>
        /// 客戶端Logo圖片
        /// </summary>
        public readonly static string BrandLogo = ConfigurationManager.AppSettings["brandLogo"].ToString();
        
        /// <summary>
        /// Hub: 儲存在Dictionary 的最長時間(時數)
        /// </summary>
        public readonly static double HubKeepTimes = double.Parse(ConfigurationManager.AppSettings["HubKeepTimes"].ToString());

        /// <summary>
        /// 預設密碼/ 重置密碼
        /// </summary>
        public readonly static string DefaultPaswd = ConfigurationManager.AppSettings["DefaultPaswd"].ToString();

        /// <summary>
        /// DocViewer BasePath
        /// </summary>
        public readonly static string DocViewerBasePath = ConfigurationManager.AppSettings["DocViewerPath"].ToString();

        /// <summary>
        /// Identity [AccessFailededCount] 帳號驗證失敗最多次數 
        /// </summary>
        public readonly static int AccessFailMax = int.Parse(ConfigurationManager.AppSettings["AccessFailMax"].ToString());
        /// <summary>
        /// Identity 驗證失敗鎖定時間(分鐘)
        /// </summary>
        public readonly static int LockoutTimeSpan = int.Parse(ConfigurationManager.AppSettings["LockoutTimeSpan"].ToString());
        /// <summary>
        /// Identity 是否啟用鎖定 [LockedEnable]
        /// </summary>
        public readonly static bool LockedEnable = ConfigurationManager.AppSettings["LockedEnable"].ToString().ToUpper() == "TRUE";
    }
}