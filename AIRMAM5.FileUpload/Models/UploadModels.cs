
namespace AIRMAM5.FileUpload.Models
{
    /// <summary>
    /// 媒體檔案路徑參數 Model
    /// </summary>
    public class MediaFolerFileModel : ResumableJSModel
    {
        public MediaFolerFileModel() { }
        /// <summary>
        /// 暫存路徑 fsTEMP_FOLDER
        /// </summary>
        public string TempFolder { get; set; } = string.Empty;
        /// <summary>
        /// 目標路徑 fsTARGET_FOLDER
        /// </summary>
        public string TargetFolder { get; set; } = string.Empty;
        /// <summary>
        /// 暫存檔案 fsCHUNK_FILE
        /// </summary>
        public string TempFile { get; set; } = string.Empty;
        /// <summary>
        /// 目標檔案 fsTARGET_FILE
        /// </summary>
        public string TargetFile { get; set; } = string.Empty;
    }

    /*20200922*///-- 移至  AIRMAM5.FileUpload.Models\ResumableJSModel.cs
                ///// <summary>
                ///// 前端上傳進度套件Resumable.js參數 MODEL
                ///// </summary>
                //public class ResumableJSModel
                //{
                //    /// <summary>
                //    /// ResumableChunkNumber
                //    /// </summary>
                //    public int ResumableChunkNumber { get; set; }
                //    /// <summary>
                //    /// ResumableFilename
                //    /// </summary>
                //    public string ResumableFilename { get; set; }
                //    /// <summary>
                //    /// ResumableIdentifier
                //    /// </summary>
                //    public string ResumableIdentifier { get; set; }
                //    /// <summary>
                //    /// ResumableChunkSize
                //    /// </summary>
                //    public long ResumableChunkSize { get; set; }
                //    /// <summary>
                //    /// ResumableTotalSize
                //    /// </summary>
                //    public double ResumableTotalSize { get; set; }
                //}

    /*20200922*///-- 移至  AIRMAM5.FileUpload.Models\MediaProcessModel.cs
    ///// <summary>
    ///// 處理{影音圖文}檔案的參數 Model
    ///// </summary>
    //public class MediaProcessModel : MediaFolerFileModel
    //{
    //    public MediaProcessModel() { }

    //    /// <summary>
    //    /// 系統目錄編號 fsDIR_ID
    //    /// </summary>
    //    public long DirId { get; set; } = -1;

    //    /// <summary>
    //    /// 主題檔案編號
    //    /// </summary>
    //    public string SubjId { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 檔案類型 : MEDIATYPE_TO_V, MEDIATYPE_TO_A, MEDIATYPE_TO_P, MEDIATYPE_TO_D
    //    /// </summary>
    //    public string MediaType { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 標題定義(1.檔名為標題、2.主題標題為標題、3.自訂標題)、4.預編詮釋資料標題 為標題
    //    /// </summary>
    //    public int TitleDefine { get; set; } = 0;

    //    /// <summary>
    //    /// 標題(若為自訂標題) 
    //    /// </summary>
    //    public string CustomTitle { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 預編詮釋ID
    //    /// </summary>
    //    public int PreId { get; set; } = 0;

    //    /// <summary>
    //    /// 上傳者 fsLOGIN_ID(username)
    //    /// </summary>
    //    public string LoginId { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 被置換的檔案編號(新上傳不用寫值) fsFILE_NO
    //    /// </summary>
    //    public string FileNo { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 上傳暫存資料夾名稱 fsFOLDER
    //    /// </summary>
    //    public string Folder { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 置換後是否要刪除關鍵影格 fbDELETE_KF
    //    /// </summary>
    //    public string DeleteKeyframe { get; set; } = null;
    //    /// <summary>
    //    /// Added_20200302:機密等級。
    //    /// </summary>
    //    public int FileSecret { get; set; }
    //}

    #region 【文件檢視】
    /*20200922*///-- 移至  AIRMAM5.FileUpload.Models\ViewerFileModel.cs
                ///// <summary>
                ///// 文件檢視(解密)檔案 參數
                ///// </summary>
                //public class ViewerFileModel : SubjFileNoModel
                //{
                //    /// <summary>
                //    /// 使用者ID
                //    /// </summary>
                //    public string ViewUserId { get; set; }
                //    /// <summary>
                //    /// 使用者顯示名稱
                //    /// </summary>
                //    public string ViewUserName { get; set; }
                //    /// <summary>
                //    /// 區別檔案: 正常檔案normal、刪除檔案del, 預設normal
                //    /// </summary>
                //    public string Kind { get; set; } = "normal";
                //}

    /*20200922*///-- 移至  AIRMAM5.FileUpload.Models\ViewerRemoveFileModel.cs
                ///// <summary>
                ///// 文件檢視 結束移除(解密)檔案 參數
                ///// </summary>
                //public class ViewerRemoveFileModel : SubjFileNoModel
                //{
                //    /// <summary>
                //    /// 檔名.副檔名 (ex: J6Z00JTL8N0V.ppt)
                //    /// </summary>
                //    public string ViewFileName { get; set; }
                //}

    /*20200922*///-- 移至  AIRMAM5.FileUpload.Models\ViewerTempFileNameModel.cs
                ///// <summary>
                ///// 文件檢視api 回覆當次解密後臨時檔名
                ///// </summary>
                //public class ViewerTempFileNameModel
                //{
                //    /// <summary>
                //    /// 文件檢視 解密後臨時檔名(不包含副檔名)
                //    /// </summary>
                //    public string TempFileName { get; set; } = string.Empty;
                //}
    #endregion

}