using AIRMAM5.DBEntity.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Announce
{
    /// <summary>
    /// 公告維護- 新增/編輯Model , 繼承參考 <see cref="AnnounceIdModel"/>
    /// </summary>
    public class AnnounceCreateModel : AnnounceIdModel
    {
        /// <summary>
        /// 公告維護- 新增/編輯Model : 初始值
        /// </summary>
        public AnnounceCreateModel()
        {
            //_serg = new GroupsService();
            fdSDATE = DateTime.Now;
            fdEDATE = DateTime.Now.AddDays(7);
        }

        #region >>> 欄位定義
        /// <summary>
        /// 公告標題
        /// </summary>
        [Display(Name = "公告標題")]
        [Required]
        [MaxLength(50, ErrorMessage = "標題字串長度限制50")]
        public string fsTITLE { get; set; } = string.Empty;
        /// <summary>
        /// 公告內容
        /// </summary>
        [Display(Name = "公告內容")]
        [Required]
        public string fsCONTENT { get; set; } = string.Empty;
        /// <summary>
        /// 上架日期(起)
        /// </summary>
        [Display(Name = "上架日期(起)")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime fdSDATE { get; set; }
        /// <summary>
        /// 上架日期(迄)
        /// </summary>
        [Display(Name = "上架日期(迄)")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? fdEDATE { get; set; }
        /// <summary>
        /// 發佈單位
        /// </summary>
        [Display(Name = "發佈單位")]
        public string fsDEPT { get; set; } = string.Empty;
        /// <summary>
        /// 發佈單位 選單
        /// </summary>
        public List<SelectListItem> AnnDeptList { get; set; }
        /// <summary>
        /// 公告分類
        /// </summary>
        [Display(Name = "公告分類")]
        public string fsTYPE { get; set; } = string.Empty;
        /// <summary>
        /// 公告分類 選單
        /// </summary>
        public List<SelectListItem> AnnTypeList { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int fnORDER { get; set; }
        /// <summary>
        /// 是否隱藏
        /// </summary>
        [Display(Name = "是否隱藏")]
        public string fsIS_HIDDEN { get; set; } = IsTrueFalseEnum.N.ToString();
        /// <summary>
        /// 是否隱藏 選單
        /// </summary>
        public List<SelectListItem> HiddenList { get; set; }

        /// <summary>
        /// 公告群組 fsGROUP_LIST
        /// </summary>
        [Display(Name = "公告群組")]
        public string[] GroupList { get; set; }
        /// <summary>
        /// 公告角色群組選單
        /// </summary>
        public List<SelectListItem> GroupListItems { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        [Display(Name = "備註")]
        public string fsNOTE { get; set; } = string.Empty;
        #endregion

        /* marked_20211001_取消類別中的方法 */
        /// <summary>
        /// 公告維護- 新增/編輯頁, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result"/> 預存回傳值</typeparam>
        public AnnounceCreateModel DataConvert<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnANN_ID")
                    this.AnnounceId = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsTITLE") this.fsTITLE = val.ToString();
                if (info.Name == "fsCONTENT") this.fsCONTENT = val.ToString();
                if (info.Name == "fdSDATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.fdSDATE = dt;
                }
                if (info.Name == "fdEDATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.fdEDATE = dt;
                }

                if (info.Name == "fsTYPE") this.fsTYPE = val.ToString();
                if (info.Name == "fnORDER")
                    this.fnORDER = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsIS_HIDDEN") this.fsIS_HIDDEN = val.ToString();
                if (info.Name == "fsDEPT") this.fsDEPT = val.ToString();
                if (info.Name == "fsNOTE") this.fsNOTE = val.ToString();
                //公告對象群組
                if (info.Name == "fsGROUP_LIST") this.GroupList = val.ToString().Split(new char[] { ';' });
            }

            //this.GroupListItems = new GroupsService().GetUserRoles();
            //this.AnnTypeList = _ser.GetCodeItemList(TbzCodeIdEnum.ANN001.ToString());
            //this.AnnDeptList = _ser.GetCodeItemList(TbzCodeIdEnum.DEPT001.ToString());
            //this.HiddenList = _ser.GetBoolItemList(new string[2] { "隱藏", "顯示" });

            return this;
        }
    }

}
