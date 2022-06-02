
namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 預存:spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND 參數。 繼承參考<see cref="DirIdModel"/>
    /// <para> (@fsTYPE,@fsFILE_NO,@fnDIR_ID,@fsLOGIN_ID,@fsKEYWORD) </para>
    /// <para> 取出符合相同V or A or P or D樣板的DIRECTORIES 給檔案搬遷主題用  </para>
    /// </summary>
    public class GetDirByFileNoLoadOnDemand : DirIdModel
    {
        #region >>>>> 欄位參數 
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FileNo { set; get; } = string.Empty;

        ///// <summary>
        ///// 系統目錄編號 fsDIR_ID
        ///// </summary>
        //public long DirId { get; set; }

        /// <summary>
        /// 系統帳號 fsLOGIN_ID
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; } = string.Empty;
        #endregion
    }

}
