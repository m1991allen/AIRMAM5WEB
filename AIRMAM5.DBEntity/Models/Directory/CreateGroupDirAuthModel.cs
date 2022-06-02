using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 系統目錄: 新增目錄 角色權限
    /// </summary>
    public class CreateGroupDirAuthModel
    {
        /// <summary>
        /// 系統目錄: 新增目錄 角色權限
        /// </summary>
        public CreateGroupDirAuthModel() { }

        ///// <summary>  Marked_BY_20210902 加入DI而調整
        ///// 系統目錄: 新增目錄 角色權限
        ///// </summary>
        ///// <param name="dirid">目錄ID </param>
        //public CreateGroupDirAuthModel(long dirid)
        //{
        //    //var groupSer = new GroupsService();
        //    var _ddlOpera = GetEnums.GetOperationAuthority();
        //    var _ddlrole = new GroupsService().GetRolesByDirId(dirid);
        //    this.fnDIR_ID = dirid;
        //    this.RoleList = _ddlrole;
        //    this.OperationList = _ddlOpera;
        //}

        #region >>> 參數/屬性
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
        #endregion

        /// <summary>
        /// 帳號資料選單(取出 系統目錄未設定過權限的帳號)
        /// </summary>
        public List<SelectListItem> RoleList { get; set; }

        /// <summary>
        /// 主題/影/音/圖/文 可用操作選單
        /// </summary>
        public List<SelectListItem> OperationList { get; set; }
    }
}
