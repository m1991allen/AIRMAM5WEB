
namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// spGET_DIRECTORIES_LOAD_ON_DEMAND 查詢參數。 繼承參考<see cref="DirIdModel"/>
    /// <para> DirId, UserName, KeyWord, ShowSubJ,  </para>
    /// </summary>
    public class GetDirLoadOnDemandSearchModel : DirIdModel
    {
        /// <summary>
        /// spGET_DIRECTORIES_LOAD_ON_DEMAND 查詢參數
        /// </summary>
        public GetDirLoadOnDemandSearchModel() : base() { }

        /// <summary>
        /// spGET_DIRECTORIES_LOAD_ON_DEMAND 查詢參數
        /// </summary>
        /// <param name="dirid"> 目錄編號 ID </param>
        public GetDirLoadOnDemandSearchModel(long dirid)
        {
            DirId = dirid;
        }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 系統帳號
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string KeyWord { get; set; } = string.Empty;

        /// <summary>
        /// 是否顯示主題Counter tbmSUBJECT  (Default=false, 不顯示)
        /// </summary>
        public bool ShowSubJ { get; set; } = false;

        /// <summary>
        /// 是否顯示所有目錄 (Default=false, 取P開放的目錄)
        /// <para> 針對 tbmDIRECTORIES.[fsSHOWTYPE] 判斷: P開放/L鎖定(隱藏)。 </para>
        /// </summary>
        /// <remarks>2020.9.4 Added </remarks>
        public bool IsShowAll { get; set; } = false;
        #endregion
    }

}
