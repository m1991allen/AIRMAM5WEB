
using AIRMAM5.DBEntity.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 目錄節點之群組/使用者的權限設定資料model。 繼承參考<see cref="DirIdModel"/>
    /// </summary>
    public class DirAuthEditModel : DirIdModel
    {
        /// <summary>
        /// 目錄節點之群組/使用者的權限設定資料model
        /// </summary>
        public DirAuthEditModel()
        {
            AuthOperaList = GetEnums.GetOperationAuthority();
        }

        /* Marked_BY_20211005 */
        ///// <summary>
        /////目錄節點之群組/使用者的權限設定資料
        ///// </summary>
        ///// <param name="">預存回傳資料 <see cref="spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result"/> </param>
        //public DirAuthEditModel(spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result m)
        //{
        //    DirId = m.fnDIR_ID ?? 0;
        //    DataType = m.DATATYPE;
        //    UserId = m.USER_ID ?? string.Empty;
        //    LoginId = m.LOGIN_ID ?? string.Empty;
        //    ShowName = m.USER_NAME ?? string.Empty;
        //    GroupId = m.GROUP_ID ?? string.Empty;
        //    GroupName = m.GROUP_NAME ?? string.Empty;
        //    LimitVideo = m.LIMIT_VIDEO.Split(new char[] { ',' });
        //    LimitAudio = m.LIMIT_AUDIO.Split(new char[] { ',' });
        //    LimitPhoto = m.LIMIT_PHOTO.Split(new char[] { ',' });
        //    LimitDoc = m.LIMIT_DOC.Split(new char[] { ',' });
        //    LimitSubject = m.LIMIT_SUBJECT.Split(new char[] { ',' });
        //    AuthOperaList = GetEnums.GetOperationAuthority();
        //}

        #region >>> 欄位參數
        /// <summary>
        /// 欄位類別 : G群組/U使用者 [DATATYPE]
        /// </summary>
        [Display(Name = "欄位類別")]
        public string DataType { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 使用者帳號
        /// </summary>
        [Display(Name = "使用者")]
        public string LoginId { get; set; } = string.Empty;

        /// <summary>
        /// 帳號顯示名稱
        /// </summary>
        public string ShowName = string.Empty;
        /// <summary>
        /// 角色群組ID
        /// </summary>
        [Display(Name = "角色")]
        public string GroupId { get; set; } = string.Empty;
        /// <summary>
        /// 角色群組名稱
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// 操作權限下拉清單: V,I,U,D,B
        /// </summary>
        public List<SelectListItem> AuthOperaList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 主題 權限 [LIMIT_SUBJECT]
        /// </summary>
        [Display(Name = "主題")]
        [Required]
        public string[] LimitSubject { get; set; }

        /// <summary>
        /// 影片 權限
        /// </summary>
        [Display(Name = "影片")]
        [Required]
        public string[] LimitVideo { get; set; }

        /// <summary>
        /// 聲音 權限
        /// </summary>
        [Display(Name = "聲音")]
        [Required]
        public string[] LimitAudio { get; set; }

        /// <summary>
        /// 圖片 權限
        /// </summary>
        [Display(Name = "圖片")]
        [Required]
        public string[] LimitPhoto { get; set; }

        /// <summary>
        /// 文件 權限
        /// </summary>
        [Display(Name = "文件")]
        [Required]
        public string[] LimitDoc { get; set; }
        #endregion

        /// <summary>
        /// 傳入資料型別<typeparamref name="T"/> 資料格式轉換處理
        /// </summary>
        /// <typeparam name="T">來源資料型別 </typeparam>
        /// <param name="data">來源資料 </param>
        /// <returns></returns>
        public DirAuthEditModel ConvertData<T>(T data )
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();

            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fnDIR_ID" || pp.Name.ToUpper() == "DIRID")
                {
                    if (long.TryParse(val.ToString(), out long idx)) { this.DirId = idx; }
                }

                if (pp.Name == "DATATYPE") { this.DataType = val.ToString(); }
                if (pp.Name == "USER_ID" || pp.Name.ToUpper() == "USERID") { this.UserId = val.ToString(); }
                if (pp.Name == "LOGIN_ID" || pp.Name.ToUpper() == "LOGINID" || pp.Name.ToUpper() == "USERNAME") { this.LoginId = val.ToString(); }
                if (pp.Name == "USER_NAME" || pp.Name.ToUpper() == "USERNAME") { this.ShowName = val.ToString(); }
                if (pp.Name == "GROUP_ID" || pp.Name.ToUpper() == "GROUPID") { this.GroupId = val.ToString(); }
                if (pp.Name == "GROUP_NAME" || pp.Name.ToUpper() == "GROUPNAME") { this.GroupName = val.ToString(); }
                if (pp.Name == "LIMIT_VIDEO" || pp.Name.ToUpper() == "LIMITVIDEO") { this.LimitVideo = val.ToString().Split(new char[] { ',' }); }
                if (pp.Name == "LIMIT_AUDIO" || pp.Name.ToUpper() == "LIMITAUDIO") { this.LimitAudio = val.ToString().Split(new char[] { ',' }); }
                if (pp.Name == "LIMIT_PHOTO" || pp.Name.ToUpper() == "LIMITPHOTO") { this.LimitPhoto = val.ToString().Split(new char[] { ',' }); }
                if (pp.Name == "LIMIT_DOC" || pp.Name.ToUpper() == "LIMITDOC") { this.LimitDoc = val.ToString().Split(new char[] { ',' }); }
                if (pp.Name == "LIMIT_SUBJECT" || pp.Name.ToUpper() == "LIMITSUBJECT") { this.LimitSubject = val.ToString().Split(new char[] { ',' }); }
                if (pp.Name == "DATATYPE") { this.DataType = val.ToString(); }

            }

            this.AuthOperaList = GetEnums.GetOperationAuthority();

            return this;
        }
    }

}
