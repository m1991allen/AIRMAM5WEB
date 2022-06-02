using AIRMAM5.DBEntity.Models.Subject;

namespace AIRMAM5.FileUpload.Models
{
    /// <summary>
    /// 文件檢視(解密)檔案 參數
    /// </summary>
    public class ViewerFileModel : SubjFileNoModel
    {
        /// <summary>
        /// 使用者ID
        /// </summary>
        public string ViewUserId { get; set; }

        /// <summary>
        /// 使用者顯示名稱
        /// </summary>
        public string ViewUserName { get; set; }

        /// <summary>
        /// 區別檔案: 正常檔案normal、刪除檔案del, 預設normal
        /// </summary>
        public string Kind { get; set; } = "normal";
    }

}