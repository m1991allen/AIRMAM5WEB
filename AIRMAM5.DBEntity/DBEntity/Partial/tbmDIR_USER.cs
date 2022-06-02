using AIRMAM5.DBEntity.Models.Directory;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 自訂欄位樣板: 目錄使用權限-使用者 tbmDIR_USER
    /// </summary>
    [MetadataType(typeof(tbmDIR_USERMetadata))]
    public partial class tbmDIR_USER
    {
        public tbmDIR_USER() : base()
        {
            fdCREATED_DATE = DateTime.Now;
            AuthType = "U";
        }

        /// <summary>
        /// 指定 系統目錄編號 fnDIR_ID
        /// </summary>
        /// <param name="id"></param>
        public tbmDIR_USER(long id)
        {
            this.fnDIR_ID = id;
        }

        /// <summary>
        /// 指定 欄位值 
        /// </summary>
        public tbmDIR_USER(DirAuthEditModel m)
        {
            this.fnDIR_ID = m.DirId;
            this.fsLOGIN_ID = m.LoginId;
            this.fsLIMIT_SUBJECT = string.Join(",", m.LimitSubject);
            this.fsLIMIT_VIDEO = string.Join(",", m.LimitVideo);
            this.fsLIMIT_AUDIO = string.Join(",", m.LimitAudio);
            this.fsLIMIT_PHOTO = string.Join(",", m.LimitPhoto);
            this.fsLIMIT_DOC = string.Join(",", m.LimitDoc);
        }

        /// <summary>
        /// (新增時)欄位類型 : G群組 / U使用者
        /// </summary>
        public string AuthType { get; set; } = "U";

        /// <summary>
        /// 自訂欄位樣板: 目錄使用權限-使用者 tbmDIR_USER Metadata
        /// </summary>
        public class tbmDIR_USERMetadata
        {
            /// <summary> 
            /// 系統目錄編號 fnDIR_ID
            /// </summary>
            [Display(Name = "系統目錄編號")]
            [Required]
            public long fnDIR_ID { get; set; }

            /// <summary>
            /// 使用者帳號 fsLOGIN_ID
            /// </summary>
            [Display(Name = "使用者帳號")]
            [Required]
            public string fsLOGIN_ID { get; set; }

            /// <summary>
            /// 主題 可用權限
            /// </summary>
            [Display(Name = "主題")]
            [Required]
            public string fsLIMIT_SUBJECT { get; set; }

            /// <summary>
            /// 影片 可用權限
            /// </summary>
            [Display(Name = "影片")]
            [Required]
            public string fsLIMIT_VIDEO { get; set; }

            /// <summary>
            /// 聲音 可用權限
            /// </summary>
            [Display(Name = "聲音")]
            [Required]
            public string fsLIMIT_AUDIO { get; set; }

            /// <summary>
            /// 圖片 可用權限
            /// </summary>
            [Display(Name = "圖片")]
            [Required]
            public string fsLIMIT_PHOTO { get; set; }

            /// <summary>
            /// 文件 可用權限
            /// </summary>
            [Display(Name = "文件")]
            [Required]
            public string fsLIMIT_DOC { get; set; }

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public System.DateTime fdCREATED_DATE { get; set; }
            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }
            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }
        }
    }
}
