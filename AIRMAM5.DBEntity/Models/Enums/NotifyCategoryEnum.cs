
using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 訊息通知 類別
    /// </summary>
    public enum NotifyCategoryEnum
    {
        /// <summary>
        /// 預設(全部帳號)
        /// </summary>
        [Description("預設")]
        預設,//Default,
        /// <summary>
        /// 角色群組
        /// </summary>
        [Description("角色群組")]
        角色群組,//Groups,
        /// <summary>
        /// 指定帳號
        /// </summary>
        [Description("指定帳號")]
        指定帳號,//Assign,
    }

}
