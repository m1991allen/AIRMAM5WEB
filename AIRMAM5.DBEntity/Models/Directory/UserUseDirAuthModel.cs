
namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 使用者帳號對DirId的{主題/影/音/圖/文}操作權限: V,I,U,D,B。 繼承參考<see cref="DirIdModel"/>
    /// </summary>DirectoriesAuthModel
    public class UserUseDirAuthModel : DirIdModel
    {
        /// <summary>
        /// 使用者帳號對DirId的{主題/影/音/圖/文}操作權限: V,I,U,D,B。
        /// </summary>
        public UserUseDirAuthModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 主題編號
        /// </summary>
        public string SubjectId { set; get; } = string.Empty;

        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FileNo { set; get; } = string.Empty;

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string LoginId { get; set; } = string.Empty;

        ///// <summary>
        ///// 檔案類別 : S,V,A,P,D 
        ///// </summary>
        //public string FileCategory { get; set; } = string.Empty;
        ///// <summary>
        ///// 類別可用權限: V,I,U,D,B
        ///// </summary>
        //public string LimitAuth { get; set; } = string.Empty;

        ///// <summary>
        ///// 主題可用權限: V,I,U,D,B
        ///// </summary>
        public string LimitSubject { get; set; } = string.Empty;
        ///// <summary>
        ///// 影片可用權限: V,I,U,D,B
        ///// </summary>
        public string LimitVideo { get; set; } = string.Empty;
        ///// <summary>
        ///// 聲音可用權限: V,I,U,D,B
        ///// </summary>
        public string LimitAudio { get; set; } = string.Empty;
        ///// <summary>
        ///// 圖片可用權限: V,I,U,D,B
        ///// </summary>
        public string LimitPhoto { get; set; } = string.Empty;
        ///// <summary>
        ///// 文件可用權限: V,I,U,D,B
        ///// </summary>
        public string LimitDoc { get; set; } = string.Empty;
        #endregion
    }

}
