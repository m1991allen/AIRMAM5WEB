
namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 預存:spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND 參數。 繼承參考<see cref="DirIdModel"/>
    /// <para> (@fsSUBJ_ID,@fnDIR_ID,@fsLOGIN_ID,@fsKEYWORD) </para>
    /// <para> 取出符合相同V or A or P or D樣板的DIRECTORIES 給主題搬遷目錄用 </para>
    /// </summary>
    public class GetDirBySubjIdLoadOnDemand : DirIdModel
    {
        /// <summary>
        /// 預存:spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND 參數。
        /// </summary>
        public GetDirBySubjIdLoadOnDemand() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 搬移的主題編號 @fsSUBJ_ID
        /// </summary>
        public string SubjId { set; get; } = string.Empty;

        ///// <summary>
        ///// 選擇目錄節點ID @fnDIR_ID : (列出目錄下符合的Q)
        ///// </summary>
        //public long DirId { get; set; }

        /// <summary>
        /// 系統帳號 @fsLOGIN_ID
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 關鍵字 @fsKEYWORD
        /// </summary>
        public string KeyWord { get; set; } = string.Empty;
        #endregion
    }
}
