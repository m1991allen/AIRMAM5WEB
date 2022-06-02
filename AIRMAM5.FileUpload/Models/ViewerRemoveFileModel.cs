using AIRMAM5.DBEntity.Models.Subject;

namespace AIRMAM5.FileUpload.Models
{
    /// <summary>
    /// 文件檢視 結束移除(解密)檔案 參數
    /// </summary>
    public class ViewerRemoveFileModel : SubjFileNoModel
    {
        /// <summary>
        /// 檔名.副檔名 (ex: J6Z00JTL8N0V.ppt)
        /// </summary>
        public string ViewFileName { get; set; }
    }

}