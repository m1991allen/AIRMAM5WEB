
namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// spGET_ARC_PRE 參數。 繼承參考 <see cref="TbmArcPreIdModel"/>
    /// </summary>
    public class spGET_ARC_PRE_Param : TbmArcPreIdModel
    {
        /// <summary>
        /// 初始
        /// </summary>
        public spGET_ARC_PRE_Param() : base()
        {
            fnPRE_ID = 0;
            PreName = string.Empty;
            PreType = string.Empty;
            PreTempId = 0;
        }

        /// <summary>
        /// 指定參數值
        /// </summary>
        /// <param name="id">預編詮釋資料 編號 fnPRE_ID </param>
        /// <param name="name">預編名稱 fsNAME </param>
        /// <param name="type"> 類型: S、V、A、P、D </param>
        /// <param name="tempid"> 使用樣板編號 fnTEMP_ID </param>
        public spGET_ARC_PRE_Param(long id, string name, string type, int tempid)
        {
            fnPRE_ID = id;
            PreName = name;
            PreType = type;
            PreTempId = tempid;
        }

        /// <summary>
        /// 預編名稱 fsNAME
        /// </summary>
        public string PreName { get; set; }

        /// <summary>
        /// 類型: S、V、A、P、D
        /// </summary>
        public string PreType { get; set; }

        /// <summary>
        /// 使用樣板編號 fnTEMP_ID
        /// </summary>
        public int PreTempId { get; set; }
    }

}
