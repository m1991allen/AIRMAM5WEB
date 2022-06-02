using AIRMAM5.DBEntity.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Announce
{
    class AnnounceDetailModel : AnnouncePublicViewModel { }

    /// <summary>
    /// 公告檢視頁 ViewModel ,　繼承參考 <see cref="AnnouncePublicViewModel"/>
    /// </summary>
    public class AnnounceDetailViewModel : AnnouncePublicViewModel
    {
        /// <summary>
        /// 公告檢視頁。　繼承參考 <see cref="AnnouncePublicViewModel"/>
        /// </summary>
        public AnnounceDetailViewModel() { }

        #region >>> 欄位定義
        /// <summary>
        /// 是否隱藏
        /// </summary>
        [Display(Name = "是否隱藏")]
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// 發佈對象/群組角色
        /// </summary>
        [Display(Name = "發佈對象群組")]
        public string PostTo { get; set; } = string.Empty;

        /// <summary>
        /// 建立者
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "新增時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// 修改時間
        /// </summary>
        [Display(Name = "修改時間")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? UpdatedTime { get; set; }
        #endregion

        /* marked_20211001_取消類別中的方法 */
        /// <summary>
        /// 公告檢視頁 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result"/> 預存回傳值</typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public new AnnounceDetailViewModel DataConvert<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();
            string insuser = null, insusernm = null, upduser = null, updusernm = null;

            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnANN_ID")
                    this.AnnounceId = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsTITLE") this.AnnTitle = val.ToString();
                if (info.Name == "fsCONTENT") this.AnnContent = val.ToString();
                if (info.Name == "fdSDATE") this.AnnSdate = DateTime.Parse(val.ToString());
                if (info.Name == "fdEDATE") this.AnnEdate = DateTime.Parse(val.ToString());
                if (info.Name == "fsTYPE") this.AnnType = val.ToString();
                if (info.Name == "fsTYPE_NAME") this.AnnTypeName = val.ToString();
                if (info.Name == "fsDEPT_NAME") this.AnnPublishDept = val.ToString();

                if (info.Name == "fsNOTE") this.AnnNote = val.ToString();
                if (info.Name == "fsIS_HIDDEN")
                {
                    this.IsHidden = string.IsNullOrEmpty(val.ToString())
                        ? false
                        : (val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false);
                }

                if (info.Name == "fsCREATED_BY") insuser = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") insusernm = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime insDate);
                    this.CreatedTime = insDate;
                }

                if (info.Name == "fsUPDATED_BY") upduser = val.ToString();
                if (info.Name == "fsUPDATED_BY_NAME") updusernm = val.ToString();
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime updDate);
                    if (string.IsNullOrEmpty(val.ToString())) this.UpdatedTime = null; else this.UpdatedTime = updDate;
                }

                if (info.Name == "fsGROUP_LIST")
                {
                    //GroupsService _serg = new GroupsService(_serilogService);
                    //this.PostTo = string.Join(",",
                    //            _serg.GroupListItemSelected(val.ToString())
                    //                    .Where(x => x.Selected == true).Select(x => x.Text).ToList());
                    this.PostTo = val.ToString(); // ← ↑ modified_20210902
                }
            }

            this.CreatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(insuser) ? string.Empty : insuser
                    , string.IsNullOrEmpty(insusernm) ? string.Empty : string.Format($"({insusernm})"));
            this.UpdatedBy = string.Format("{0}{1}"
                    , string.IsNullOrEmpty(upduser) ? string.Empty : upduser
                    , string.IsNullOrEmpty(updusernm) ? string.Empty : string.Format($"({updusernm})"));

            return this;
        }
    }

}
