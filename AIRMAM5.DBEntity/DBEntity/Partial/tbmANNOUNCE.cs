//using AIRMAM5.DBEntity.Models.Announce;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 系統公告  tbmANNOUNCE
    /// </summary>
    [MetadataType(typeof(tbmANNOUNCEMetadata))]
    public partial class tbmANNOUNCE
    {
        public tbmANNOUNCE()
        {
            fdSDATE = DateTime.Now;
            fdEDATE = DateTime.Now.AddDays(7);
            fdCREATED_DATE = DateTime.Now;
            IsHidden = false;
        }

        /* marked_&_modified_20211007 */
        //public tbmANNOUNCE(AnnounceCreateModel m)
        //{
        //    fnANN_ID = m.AnnounceId;
        //    fsTITLE = m.fsTITLE;
        //    fsCONTENT = m.fsCONTENT;
        //    fdSDATE = m.fdSDATE;
        //    fdEDATE = m.fdEDATE;
        //    fsTYPE = m.fsTYPE;
        //    fnORDER = m.fnORDER;
        //    fsGROUP_LIST = m.GroupList == null ? string.Empty : string.Join(";", m.GroupList);
        //    fsIS_HIDDEN = m.fsIS_HIDDEN;
        //    //IsHidden = m.fsIS_HIDDEN == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fsDEPT = m.fsDEPT;
        //    fsNOTE = m.fsNOTE ?? string.Empty;
        //    fdCREATED_DATE = DateTime.Now;
        //    fsCREATED_BY = string.Empty;
        //    fdUPDATED_DATE = null;
        //    fsUPDATED_BY = string.Empty;
        //    ////公告對象群組
        //    //GroupList = m.GroupList;
        //}

        /// <summary>
        /// 是否隱藏
        /// </summary>
        public bool IsHidden { get; set; }
        ///// <summary>
        ///// 公告群組清單
        ///// </summary>
        //public List<string> GroupList { get; set; } = new List<string>();

        //public tbmANNOUNCE(spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result m)
        //{
        //    fnANN_ID = m.fnANN_ID;
        //    fsTITLE = m.fsTITLE;
        //    fsCONTENT = m.fsCONTENT;
        //    fdSDATE = m.fdSDATE;
        //    fdEDATE = m.fdEDATE;
        //    fsTYPE = m.fsTYPE;
        //    fnORDER = m.fnORDER;
        //    fsGROUP_LIST = m.fsGROUP_LIST;
        //    fsIS_HIDDEN = m.fsIS_HIDDEN;
        //    IsHidden = m.fsIS_HIDDEN == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fsDEPT = m.fsDEPT;
        //    fsNOTE = m.fsNOTE;
        //    fdCREATED_DATE = m.fdCREATED_DATE;
        //    fsCREATED_BY = m.fsCREATED_BY;
        //    fdUPDATED_DATE = m.fdUPDATED_DATE;
        //    fsUPDATED_BY = m.fsUPDATED_BY;
        //    //公告對象群組
        //    GroupList = m.fsGROUP_LIST.Split(new char[] { ';' }).ToList();
        //}

        public class tbmANNOUNCEMetadata
        {
            [Display(Name = "ID")]
            public long fnANN_ID { get; set; }

            [Display(Name = "公告標題")]
            [Required]
            [MaxLength(50, ErrorMessage ="標題字串長度限制50")]
            public string fsTITLE { get; set; }

            [Display(Name = "公告內容")]
            [Required]
            public string fsCONTENT { get; set; }

            [Display(Name = "上架日期")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            [Required]
            public DateTime fdSDATE { get; set; }

            [Display(Name = "下架日期")]
            [DataType(DataType.Text)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public DateTime? fdEDATE { get; set; }

            [Display(Name = "公告分類")]
            public string fsTYPE { get; set; }

            [Display(Name = "排序")]
            public int fnORDER { get; set; }

            /// <summary>
            /// 公告群組
            /// </summary>
            [Display(Name = "公告群組")]
            public string fsGROUP_LIST { get; set; }

            [Display(Name = "是否隱藏")]
            public string fsIS_HIDDEN { get; set; }

            [Display(Name = "發佈單位")]
            public string fsDEPT { get; set; }

            [Display(Name = "備註")]
            public string fsNOTE { get; set; }

            [Display(Name = "建立人員")]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動人員")]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動時間")]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
            public string fsUPDATED_BY { get; set; }
        }

        /// <summary>
        /// 系統公告 資料轉換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public void ConvertGet<T>(T data)
        {
            if (data != null)
            {
                var properties = typeof(T).GetProperties();

                foreach (var pp in properties)
                {
                    var val = pp.GetValue(data) ?? string.Empty;

                    if (pp.Name == "fnANN_ID" || pp.Name == "AnnounceId")
                    { //公告ID
                        if (long.TryParse(val.ToString(), out long idx)) { this.fnANN_ID = idx; }
                    }

                    if (pp.Name == "fsTITLE" || pp.Name == "AnnTitle") { this.fsTITLE = val.ToString(); }
                    if (pp.Name == "fsCONTENT" || pp.Name == "AnnContent") { this.fsCONTENT = val.ToString(); }

                    if (pp.Name == "fdSDATE" || pp.Name == "BegDate")
                    { //上架日期(起)
                        if (DateTime.TryParse(val.ToString(), out DateTime dt)) { this.fdSDATE = dt; }
                    }

                    if (pp.Name == "fdEDATE" || pp.Name == "EndDate")
                    { //上架日期(迄)
                        if (DateTime.TryParse(val.ToString(), out DateTime dt)) { this.fdEDATE = dt; }
                    }

                    if (pp.Name == "fsDEPT" || pp.Name == "Dept") { this.fsDEPT = val.ToString(); }
                    if (pp.Name == "fsTYPE" || pp.Name == "AnnType") { this.fsTYPE = val.ToString(); }

                    if (pp.Name == "fnORDER" || pp.Name == "AnnOrder")
                    {
                        if (int.TryParse(val.ToString(), out int num)) { this.fnORDER = num; }
                    }

                    if (pp.Name == "fsIS_HIDDEN" || pp.Name == "IsHidden")
                    {
                        if (bool.TryParse(val.ToString(), out bool chk))
                        {
                            this.IsHidden = chk;
                            this.fsIS_HIDDEN = chk ? "1" : "0";
                        }
                        this.fsIS_HIDDEN = val.ToString();
                    }

                    if (pp.Name == "GroupList" || pp.Name == "fsGROUP_LIST")
                    {
                        this.fsGROUP_LIST = string.IsNullOrEmpty(val.ToString()) ? string.Empty : string.Join(";", val.ToString());
                    }

                    if (pp.Name == "fsNOTE" || pp.Name == "AnnNote") { this.fsNOTE = val.ToString(); }

                    if (pp.Name == "fdCREATED_DATE" || pp.Name == "CreatedDate")
                    {
                        if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        {
                            this.fdCREATED_DATE = dt;
                        }
                    }
                    if (pp.Name == "fsCREATED_BY" || pp.Name == "CreatedBy") { this.fsCREATED_BY = val.ToString(); }

                    if (pp.Name == "fdUPDATED_DATE" || pp.Name == "UpdatedDate")
                    {
                        if (DateTime.TryParse(val.ToString(), out DateTime dt))
                        {
                            this.fdUPDATED_DATE = dt;
                        }
                    }
                    if (pp.Name == "fsUPDATED_BY" || pp.Name == "UpdatedBy") { this.fsUPDATED_BY = val.ToString(); }
                }
            }

        }
    }
}
