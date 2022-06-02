
namespace AIRMAM5.FileUpload.Models
{
    /// <summary>
    /// 文件檢視api 回覆當次解密後臨時檔名
    /// </summary>
    public class ViewerTempFileNameModel
    {
        /// <summary>
        /// 文件檢視 解密後臨時檔名(不包含副檔名)
        /// </summary>
        public string TempFileName { get; set; } = string.Empty;
    }

}