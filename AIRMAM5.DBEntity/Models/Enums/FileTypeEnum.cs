
using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 檔案分類 : 影.音.圖.文。樣板類型 : A聲音, D文件, P圖片, S主題, V影片
    /// </summary>
    public enum FileTypeEnum
    {
        #region TemplateTable: 樣板類型
        /// <summary>
        /// A聲音
        /// </summary>
        [Description("聲音")]
        A,
        /// <summary>
        /// D文件
        /// </summary>
        [Description("文件")]
        D,
        /// <summary>
        /// P圖片
        /// </summary>
        [Description("圖片")]
        P,
        /// <summary>
        /// S主題
        /// </summary>
        [Description("主題")]
        S,
        /// <summary>
        /// V影片
        /// </summary>
        [Description("影片")]
        V,
        #endregion

        #region >>> 不使用
        ///// <summary>
        ///// 影片
        ///// </summary>
        //[Description("影片")]
        //VIDEO,
        //[Description("影片")]
        //tbmARC_VIDEO,

        ///// <summary>
        ///// 聲音
        ///// </summary>
        //[Description("聲音")]
        //AUDIO,
        //[Description("聲音")]
        //tbmARC_AUDIO,

        ///// <summary>
        ///// 文件
        ///// </summary>
        //[Description("文件")]
        //Doc,
        //[Description("文件")]
        //tbmARC_DOC,

        ///// <summary>
        ///// 圖片
        ///// </summary>
        //[Description("圖片")]
        //PHOTO,
        //[Description("圖片")]
        //tbmARC_PHOTO,
        #endregion
    }

}
