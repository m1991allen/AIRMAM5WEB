
namespace AIRMAM5.FileUpload.Models
{
    /// <summary>
    /// 處理{影音圖文}檔案的參數 Model
    /// </summary>
    public class MediaProcessModel : MediaFolerFileModel
    {
        /// <summary>
        /// 處理{影音圖文}檔案的參數 Model
        /// </summary>
        public MediaProcessModel() { }

        /// <summary>
        /// 系統目錄編號 fsDIR_ID
        /// </summary>
        public long DirId { get; set; } = -1;

        /// <summary>
        /// 主題檔案編號
        /// </summary>
        public string SubjId { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類型 : MEDIATYPE_TO_V, MEDIATYPE_TO_A, MEDIATYPE_TO_P, MEDIATYPE_TO_D
        /// </summary>
        public string MediaType { get; set; } = string.Empty;

        /// <summary>
        /// 標題定義(1.檔名為標題、2.主題標題為標題、3.自訂標題)、4.預編詮釋資料標題 為標題
        /// </summary>
        public int TitleDefine { get; set; } = 0;

        /// <summary>
        /// 標題(若為自訂標題) 
        /// </summary>
        public string CustomTitle { get; set; } = string.Empty;

        /// <summary>
        /// 預編詮釋ID
        /// </summary>
        public int PreId { get; set; } = 0;

        /// <summary>
        /// 上傳者 fsLOGIN_ID(username)
        /// </summary>
        public string LoginId { get; set; } = string.Empty;

        /// <summary>
        /// 被置換的檔案編號(新上傳不用寫值) fsFILE_NO
        /// </summary>
        public string FileNo { get; set; } = string.Empty;

        /// <summary>
        /// 上傳暫存資料夾名稱 fsFOLDER
        /// </summary>
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// 置換後是否要刪除關鍵影格 fbDELETE_KF
        /// </summary>
        public string DeleteKeyframe { get; set; } = null;

        /// <summary>
        /// Added_20200302:機密等級。
        /// </summary>
        public int FileSecret { get; set; }
        /// <summary>
        /// 20210913_ADDED_檔案版權
        /// </summary>
        public string FileLicense { get; set; }
    }

}