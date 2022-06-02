using AIRMAM5.DBEntity.DBEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 新增/編輯目錄 資料View Model。 繼承參考<see cref="tbmDIRECTORIES"/>
    /// </summary>
    public class DirectoryEditModel : tbmDIRECTORIES
    {
        /// <summary>
        /// 新增/編輯目錄 View 
        /// <para> 選單沒有 "-全部-" 選項 </para>
        /// </summary>
        public DirectoryEditModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 目錄管理群組 (複選)
        /// </summary>
        [Display(Name = "目錄管理群組")]
        public string[] DirGroupsAry { get; set; } = new string[] { };

        /// <summary>
        /// 目錄管理帳號 (複選)
        /// </summary>
        [Display(Name = "目錄管理帳號")]
        public string[] DirUsersAry { get; set; } = new string[] { };

        /// <summary>
        /// 目錄維護是否啟用 末節點Queue (default: true), 20201116
        /// </summary>
        public bool UsingQueue { get; set; } = true;
        #endregion

        #region >>> 欄位參數: 下拉選單
        public List<SelectListItem> UserList { get; set; }

        public List<SelectListItem> GroupList { get; set; }

        /// <summary>
        /// 樣板資料(未分類) List<spGET_TEMPLATE_Result>
        /// </summary>
        public List<spGET_TEMPLATE_Result> TemplateList { get; set; }

        /// <summary>
        /// 目錄開放類型 
        /// </summary>
        public List<SelectListItem> DirShowTypeList { get; set; }
        #endregion

        /// <summary>
        ///  新增/編輯目錄 資料View Model, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m">預存回傳資料集合 <see cref="spGET_DIRECTORIES_Result"/> </param>
        /// <returns></returns>
        public DirectoryEditModel FormatConversion<T>(T m)
        {
            var _properties = typeof(T).GetProperties();

            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fnDIR_ID") { this.fnDIR_ID = string.IsNullOrEmpty(_val.ToString()) ? 0 : long.Parse(_val.ToString()); }
                if (p.Name == "fsNAME") { this.fsNAME = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }
                if (p.Name == "fnPARENT_ID") { long.TryParse(_val.ToString(), out long v); this.fnPARENT_ID = v; }

                if (p.Name == "fsDESCRIPTION") { this.fsDESCRIPTION = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }
                if (p.Name == "fsDIRTYPE")
                {
                    this.fsDIRTYPE = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); 
                    this.IsQueue = this.fsDIRTYPE.ToUpper() == "Q" ? true : false;
                }
                
                if (p.Name == "fnORDER") { int.TryParse(_val.ToString(), out int v); this.fnORDER = v; }

                if (p.Name == "fnTEMP_ID_SUBJECT") { int.TryParse(_val.ToString(), out int v); this.fnTEMP_ID_SUBJECT = v; }
                if (p.Name == "fnTEMP_ID_VIDEO") { int.TryParse(_val.ToString(), out int v); this.fnTEMP_ID_VIDEO = v; }
                if (p.Name == "fnTEMP_ID_AUDIO") { int.TryParse(_val.ToString(), out int v); this.fnTEMP_ID_AUDIO = v; }
                if (p.Name == "fnTEMP_ID_PHOTO") { int.TryParse(_val.ToString(), out int v); this.fnTEMP_ID_PHOTO = v; }
                if (p.Name == "fnTEMP_ID_DOC") { int.TryParse(_val.ToString(), out int v); this.fnTEMP_ID_DOC = v; }

                if (p.Name == "fsADMIN_GROUP") 
                {
                    this.fsADMIN_GROUP = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString();
                    this.DirGroupsAry = this.fsADMIN_GROUP.Split(new char[] { ';' });
                }
                if (p.Name == "fsADMIN_USER") 
                {
                    this.fsADMIN_USER = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString();
                    this.DirUsersAry = this.fsADMIN_USER.Split(new char[] { ';' });
                }

                if (p.Name == "fsSHOWTYPE") { this.fsSHOWTYPE = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }

                if (p.Name == "fdCREATED_DATE") { DateTime.TryParse(_val.ToString(), out DateTime dt); this.fdCREATED_DATE = dt; }
                if (p.Name == "fsCREATED_BY") { this.fsCREATED_BY = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }
                //if (p.Name == "fsCREATED_BY_NAME") { this.fsCREATED_BY = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }

                if (p.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(_val.ToString(), out DateTime dt); this.fdUPDATED_DATE = dt;
                    if (string.IsNullOrEmpty(_val.ToString())) this.fdUPDATED_DATE = null; else this.fdUPDATED_DATE = dt;
                }
                if (p.Name == "fsUPDATED_BY") { this.fsUPDATED_BY = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }
                //if (p.Name == "fsUPDATED_BY_NAME") { this.fsUPDATED_BY = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString(); }
            }

            return this;
        }

    }

}
