using AIRMAM5.DBEntity.Models.Template;
using AIRMAM5.DBEntity.Models.Subject;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;

namespace AIRMAM5.DBEntity.Models.SubjectUpload
{
    /// <summary>
    /// 上傳檔案資訊 ViewModel。　繼承參考 <see cref="SubjectIdModel"/> 
    /// </summary>
    public class SubjectUploadViewModel : SubjectIdModel
    {
        /// <summary>
        /// 上傳檔案資訊 ViewModel
        /// </summary>
        public SubjectUploadViewModel()
        {
            this.DateInFileNo = string.Format($"{DateTime.Now:yyyy-MM-dd}");
        }

        #region >>> 屬性/欄位定義
        /// <summary>
        /// 目錄節點Id [fnDIR_ID]
        /// </summary>
        public long DirId { get; set; }

        /// <summary>
        /// 檔案編號中的日期(預設當日yyyyMMdd)
        /// </summary>
        [Display(Name = "檔案編號日期")]
        public string DateInFileNo { get; set; }

        /// <summary>
        /// 目錄節點-"主題影音圖文" 自訂欄位樣板編號 List: <see cref="TemplateBaseModel"/>
        /// </summary>
        public List<TemplateBaseModel> DirTemplate { get; set; } = new List<TemplateBaseModel>();

        /// <summary>
        /// 媒體類型下拉選單: <see cref="SelectListItem"/>
        /// </summary>
        public List<SelectListItem> MediaTypeList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 預編詮釋資料下拉選單: <see cref="SelectListItem"/>
        /// </summary>
        public List<SelectListItem> ArcPreTempList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 媒體類型副檔名 List: <see cref="FileExtensionViewModel"/>
        /// </summary>
        public List<FileExtensionViewModel> MediaFileExtension { get; set; } = new List<FileExtensionViewModel>();

        /// <summary>
        /// 上傳相關參數資訊: <see cref="UploadConfigViewModel"/>
        /// </summary>
        public UploadConfigViewModel UploadConfig { get; set; } = new UploadConfigViewModel();

        /// <summary>
        /// 檔案機密 選單: <see cref="SelectListItem"/>
        /// </summary>
        [Display(Name = "機密等級")]
        public List<SelectListItem> FileSecretList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 版權 選單
        /// </summary>
        /// <remarks> 20210913_ADDED </remarks>
        [Display(Name = "版權")]
        public List<SelectListItem> FileLicenseList { get; set; } = new List<SelectListItem>();
        #endregion
    }

}
