using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 系統目錄:操作使用權限
    /// </summary>
    public enum OperationAuthorityEnum
    {
        /// <summary>
        /// 檢視
        /// </summary>
        [Description("檢視")]
        V,
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        I,
        /// <summary>
        /// 刪除
        /// </summary>
        [Description("刪除")]
        D,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        U,
        /// <summary>
        /// 調用
        /// </summary>
        [Description("調用")]
        B
    }

}
