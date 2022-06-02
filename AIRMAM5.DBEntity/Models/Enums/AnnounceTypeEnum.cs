using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 公告分類 fsTYPE
    /// </summary>
    public enum AnnounceTypeEnum
    {
        /// <summary>
        /// D	登入公告	LoginAnn
        /// </summary>
        [Description("登入公告")]
        D,
        /// <summary>
        /// Y	警告	Information
        /// </summary>
        [Description("警告")]
        Y,
        /// <summary>
        /// O	一般	Ordinary
        /// </summary>
        [Description("一般")]
        O,
        /// <summary>
        /// R	重要	Important
        /// </summary>
        [Description("重要")]
        R,
    }

}
