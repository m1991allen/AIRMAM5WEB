
namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 預存:spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY 參數。 繼承參考<see cref="DirIdModel"/>
    /// <para> @fnDIR_ID, @fsTYPE </para>
    /// </summary>
    public class GetDirAuthoritySearchModel : DirIdModel
    {
        /// <summary>
        /// 預存:spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY 參數。(default='G')
        /// </summary>
        /// <param name="dirid"></param>
        public GetDirAuthoritySearchModel(long dirid, string auth = "G")
            : base()
        {
            DirId = dirid;
            AuthType = auth;
        }

        /// <summary>
        /// 目錄使用權限欄位類型 : G群組/U使用者(default='G')
        /// </summary>
        public string AuthType { get; set; } = "G";
    }

}
