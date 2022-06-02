using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 檢索分類: 影.音.圖.文.主題
    /// </summary>
    public enum SearchTypeEnum
    {
        /// <summary>
        /// 影片
        /// </summary>
        [Description("影片")]
        Video_DEV,
        /// <summary>
        /// 聲音
        /// </summary>
        [Description("聲音")]
        Audio_DEV,
        /// <summary>
        /// 圖片
        /// </summary>
        [Description("圖片")]
        Photo_DEV,
        /// <summary>
        /// 文件
        /// </summary>
        [Description("文件")]
        Doc_DEV,
        /// <summary>
        /// 主題
        /// </summary>
        [Description("主題")]
        Subject_DEV,
    }

}
