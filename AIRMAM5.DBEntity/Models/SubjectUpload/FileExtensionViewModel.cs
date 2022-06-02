
namespace AIRMAM5.DBEntity.Models.SubjectUpload
{
    /// <summary>
    /// 媒體類型副檔名資料
    /// </summary>
    public class FileExtensionViewModel
    {
        /// <summary>
        /// 媒體類型
        /// </summary>
        public string MediaType { get; set; }

        /// <summary>
        /// 媒體類型 副檔名(;為分隔符號)
        /// </summary>
        public string FileExtension { get; set; }
    }

}
