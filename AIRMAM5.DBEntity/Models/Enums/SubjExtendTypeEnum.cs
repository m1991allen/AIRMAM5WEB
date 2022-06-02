
using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 主題與檔案-擴充功能類型: 新聞文稿, 合約/公文對應
    /// TIP: 定義要和 dbo.tbmCOLUMN_MAPPING.[fsTYPE] 一樣。
    /// </summary>
    public enum SubjExtendTypeEnum
    {
        /// <summary>
        /// 新聞文稿
        /// </summary>
        [Description("新聞文稿")]
        INEWS = 0,

        /// <summary>
        /// 合約/公文對應
        /// </summary>
        [Description("公文對應")]
        CONTRACT = 1,

    }
}
