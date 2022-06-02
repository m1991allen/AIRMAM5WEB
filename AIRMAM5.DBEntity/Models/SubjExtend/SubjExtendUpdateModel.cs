
namespace AIRMAM5.DBEntity.Models.SubjExtend
{
    /// <summary>
    ///  擴充功能{新聞文稿/公文系統} (共用/統一)選取更新資料 參數model
    /// </summary>
    /// <typeparam name="T"> 指定參數:Fields,Values 的資料型態。這裡是前端傳入的資料型態。 </typeparam>
    /// <remarks> 
    /// 1、查詢參數不固定，由前端決定傳入幾個查詢條件(動態)
    /// 2、
    /// </remarks>
    public class SubjExtendUpdateModel<T>
    {
        /// <summary>
        /// 主題與檔案-擴充功能類型: 新聞文稿, 合約/公文對應 <see cref="SubjExtendTypeEnum"/>
        /// </summary>
        public string ExecType { get; set; }

        /// <summary>
        /// 檔案編號 @fsFILE_NO
        /// </summary>
        public string FileNo { get; set; }

        /// <summary>
        /// 更新資料的欄位名稱, 前端:字串陣列格式、預存@lstCOLUMNs:字串格式(;是分隔符號) 
        /// </summary>
        public T Fields { get; set; }
        /// <summary>
        /// 更新資料的欄位值, 前端:字串陣列格式、預存@lstVALUEs:字串格式(;是分隔符號) 
        /// </summary>
        public T Values { get; set; }
    }
}
