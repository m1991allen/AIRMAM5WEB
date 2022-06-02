
namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// spGET_ARC_PRE_ATTRIBUTE 參數。 繼承參考 <see cref="TbmArcPreIdModel"/>
    /// </summary>
    public class spGET_ARC_PRE_ATTRIBUTE_Param : TbmArcPreIdModel
    {
        public spGET_ARC_PRE_ATTRIBUTE_Param() : base() { PreTempId = 0; }

        public spGET_ARC_PRE_ATTRIBUTE_Param(long id, int tempid)
        {
            fnPRE_ID = id;
            PreTempId = tempid;
        }

        /// <summary>
        /// 使用樣板編號 fnTEMP_ID
        /// </summary>
        public int PreTempId { get; set; }
    }
}
