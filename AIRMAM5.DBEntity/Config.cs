using System.Configuration;

namespace AIRMAM5.DBEntity
{
    /// <summary>
    /// 專案config檔案中的自訂義參數(appSettings)值
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Hub未回報逾期時間(分鐘)
        /// </summary>
        public readonly static string HubConnectOverdueTime = ConfigurationManager.AppSettings["HubConnectionOverdueTime"].ToString();
        
    }
}