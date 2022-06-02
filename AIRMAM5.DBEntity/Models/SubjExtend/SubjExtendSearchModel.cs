using System;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.SubjExtend
{
    /// <summary>
    ///  擴充功能{新聞文稿/公文系統} (共用/統一)查詢參數model
    /// </summary>
    /// <typeparam name="T">資料型態，參數Params 需要指定的資料型態，用來指定<see cref="SearchParam"/>參數GenericValue </typeparam>
    /// <remarks> 
    /// 1、查詢參數不固定，由前端決定傳入幾個查詢條件(動態)
    /// 2、
    /// </remarks>
    public class SubjExtendSearchModel<T>
    {
        /// <summary>
        /// 主題與檔案-擴充功能類型: 新聞文稿, 合約/公文對應 <see cref="SubjExtendTypeEnum"/>
        /// </summary>
        public string ExecType { get; set; }

        /// <summary>
        /// 查詢條件(動態)
        /// </summary>
        public List<SearchParam<T>> ExecParams { get; set; }
    }
}
