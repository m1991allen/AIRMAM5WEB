using AIRMAM5.DBEntity.Models.Directory;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /* 20200925_前端view直接使用 tbmDIR_GROUP Meatadata 
     *  _DirGroupAuthCreate.cshtml
     *  _DirUserAuthCreate.cshtml
     *  */
    /// <summary>
    /// 自訂欄位樣板: 目錄使用權限-群組 tbmDIR_GROUP
    /// </summary>
    [MetadataType(typeof(tbmDIR_GROUPMatadata))]
    public partial class tbmDIR_GROUP
    {
        public tbmDIR_GROUP() : base()
        {
            fdCREATED_DATE = DateTime.Now;
            AuthType = "G";
        }

        /// <summary>
        /// 指定 系統目錄編號 fnDIR_ID
        /// </summary>
        public tbmDIR_GROUP(long id) : base()
        {
            fnDIR_ID = id;
        }

        /// <summary>
        /// 指定 欄位值  
        /// </summary>
        public tbmDIR_GROUP(DirAuthEditModel m)
        {
            this.fnDIR_ID = m.DirId;
            this.fsGROUP_ID = m.GroupId;
            this.fsLIMIT_SUBJECT = string.Join(",", m.LimitSubject);
            this.fsLIMIT_VIDEO = string.Join(",", m.LimitVideo);
            this.fsLIMIT_AUDIO = string.Join(",", m.LimitAudio);
            this.fsLIMIT_PHOTO = string.Join(",", m.LimitPhoto);
            this.fsLIMIT_DOC = string.Join(",", m.LimitDoc);
        }

        /// <summary>
        /// (新增時)欄位類型 : G群組 / U使用者
        /// </summary>
        public string AuthType { get; set; } = "G";

        /// <summary>
        /// 自訂欄位樣板: 目錄使用權限-群組 tbmDIR_GROUP Metadata
        /// </summary>
        public class tbmDIR_GROUPMatadata
        {
            /// <summary> 
            /// 系統目錄編號 fnDIR_ID
            /// </summary>
            [Display(Name = "系統目錄編號")]
            [Required]
            public long fnDIR_ID { get; set; }
            /// <summary>
            /// 角色群組 fsGROUP_ID
            /// </summary>
            [Display(Name = "角色群組")]
            [Required]
            public string fsGROUP_ID { get; set; }
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

            [Display(Name = "新增時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }
            [Display(Name = "新增人員")]
            public string fsCREATED_BY { get; set; }
            [Display(Name = "修改時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }
            [Display(Name = "修改人員")]
            public string fsUPDATED_BY { get; set; }
        }
    }
}
