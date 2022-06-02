using AIRMAM5.DBEntity.Models.Subject;

namespace AIRMAM5.Models.DocViewer
{
    /// <summary>
    /// DocnutViewer文件檢視器 Index Data Model
    /// </summary>
    public class DocViewModel : SubjFileNoModel
    {
        public string ViewerScripts { get; set; } = string.Empty;

        public string ViewerCSS { get; set; } = string.Empty;

        public string ViewerID { get; set; } = string.Empty;

        public string ViewerObject { get; set; } = string.Empty;

        public string ViewerInit { get; set; } = string.Empty;

        public string token { get; set; } = string.Empty;

        /// <summary>
        /// 當次預覽檔名 ex: 0R48JPT0ZL4B.docx 
        /// </summary>
        public string ViewFileName { get; set; }

        ///// <summary>
        ///// 當次預覽檔路徑
        ///// </summary>
        //public string ViewerPath { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類別: 正常檔案normal、刪除檔案del
        /// </summary>
        public string FileKind { get; set; } = string.Empty;
    }

}