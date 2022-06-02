using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Shared
{
    /// <summary>
    /// 資料表 建立/最後異動 帳號與日期
    /// </summary>
    public class TableUserDateModel
    {
        public TableUserDateModel() : base()
        {
            CreatedBy = string.Empty;
            CreatedDate = DateTime.Now;
            UpdatedBy = string.Empty;
            UpdatedDate = null;
        }

        /// <summary>
        /// 建立時間 fdCREATED_DATE
        /// </summary>
        [Display(Name = "新增時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 建立帳號 fsCREATED_BY
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// 最後異動時間 fdUPDATED_DATE
        /// </summary>
        [Display(Name = "修改時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// 最後異動帳號 fsUPDATED_BY
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedBy { get; set; }

        ///// <summary>
        ///// 建立時間 Table.[fdCREATED_DATE] - String : 'yyyy/MM/dd HH:mm:ss'
        ///// </summary>
        //public string CreatedTime { get; set; } = string.Empty;
        ///// <summary>
        ///// 最後異動時間 Table.[fdUPDATED_DATE] - String : 'yyyy/MM/dd HH:mm:ss'
        ///// </summary>
        //[Display(Name = "修改時間")]
        //public string UpdatedTime { get; set; } = string.Empty;
    }

    /// <summary>
    /// 資料表 建立/最後異動 帳號+日期+帳號顯示名稱 
    /// </summary>
    public class TableUserDateByNameModel : TableUserDateModel
    {
        public TableUserDateByNameModel() { }

        public TableUserDateByNameModel(string insuser, DateTime insdt, string insnm, string upduser, DateTime? upddt, string updnm)
        {
            CreatedBy = insuser;
            CreatedDate = insdt;
            CreatedByName = insnm;
            UpdatedBy = upduser;
            UpdatedDate = upddt;
            UpdatedByName = updnm;
        }

        /// <summary>
        /// 建立帳號顯示名稱 fsCREATED_BY_NAME
        /// </summary>
        [Display(Name = "新增人員")]
        public string CreatedByName { get; set; } = string.Empty;

        /// <summary>
        /// 最後異動帳號顯示名稱 fsUPDATED_BY_NAME
        /// </summary>
        [Display(Name = "修改人員")]
        public string UpdatedByName { get; set; } = string.Empty;
    }
}
