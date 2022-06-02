using AIRMAM5.DBEntity.Models.Subject;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.SubjectUpload
{
    /// <summary>
    /// 置換檔案資訊 ViewModel。　繼承參考 <see cref="SubjectIdModel"/> 
    /// </summary>
    public class ChangeUploadViewModel : SubjectIdModel
    {
        /// <summary>
        /// 置換檔案資訊 ViewModel
        /// </summary>
        public ChangeUploadViewModel() { }

        #region >>> 屬性/欄位定義
        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public string fsFILE_NO { set; get; } = string.Empty;

        /// <summary>
        /// 媒體類型副檔名 List: <see cref="FileExtensionViewModel"/>
        /// <para>🔔-置換只能針對單一媒體類別,這裡固定為單一媒體類別可上傳的副檔名資料。 </para>
        /// </summary>
        public List<FileExtensionViewModel> MediaFileExtension { get; set; }

        /// <summary>
        /// 上傳相關參數資訊 <see cref="UploadConfigViewModel"/>
        /// </summary>
        public UploadConfigViewModel UploadConfig { get; set; }

        /// <summary>
        /// 檔案機密 選單: <see cref="SelectListItem"/>
        /// </summary>
        [Display(Name = "機密等級")]
        public List<SelectListItem> FileSecretList { get; set; } = new List<SelectListItem>();
        public int FileSecret { get; set; }

        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片 FileCategory
        /// </summary>
        [Display(Name = "類別")]
        public string FileCategory { get; set; } = string.Empty;

        /// <summary>
        /// 置換檔案燈箱:詢問字串
        /// </summary>
        public string DisplayQuestion { get; set; } = string.Empty;

        /// <summary>
        /// 版權 選單
        /// </summary>
        /// <remarks> 20210914_ADDED </remarks>
        [Display(Name = "版權")]
        public List<SelectListItem> FileLicenseList { get; set; } = new List<SelectListItem>();
        /// <summary>
        /// 版權
        /// </summary>
        /// <remarks> 20210914_ADDED </remarks>
        public string FileLicense { get; set; }
        #endregion
    }
}
