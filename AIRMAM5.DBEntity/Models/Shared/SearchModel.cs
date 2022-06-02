using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Shared
{
    #region -------------------- Date、DateTime
    /// <summary>
    /// 日期區間 String, 格式:yyyy/MM/dd
    /// </summary>
    public class SearchByDate
    {
        /// <summary>
        /// 初始 區間: 前３日~後一日
        /// </summary>
        public SearchByDate()
        {
            ////預存程序 日期查詢 格式:yyyy/MM/dd
            //DateTime dt = DateTime.Now.AddDays(-3);
            //StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        public SearchByDate(int days)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// 起始日期 yyyy/MM/dd
        /// </summary>
        [Display(Name = "開始時間")]
        public string StartDate { get; set; }

        /// <summary>
        /// 結束日期 yyyy/MM/dd
        /// </summary>
        [Display(Name = "結束時間")]
        public string EndDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// 日期查詢 Date (yyyy-MM-dd)
    /// </summary>
    public class DateSerarchModel
    {
        /// <summary>
        /// 初始 區間: 前三日~後一日
        /// </summary>
        public DateSerarchModel()
        {
            //DateTime dt = DateTime.Now.AddDays(-3);
            //BeginDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }
        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        /// <param name="days"></param>
        public DateSerarchModel(int days)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            BeginDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            dt = DateTime.Now.AddDays(+1);
            EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        /// <summary>
        /// 開始/起始 時間
        /// </summary>
        [Display(Name = "開始時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 結束 時間
        /// </summary>
        [Display(Name = "結束時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// 日期時間查詢 DateTime (yyyy-MM-dd HH:mm:ss)
    /// </summary>
    public class DateTimeSerarchModel
    {
        /// <summary>
        /// 初始 區間: 前３日~後一日
        /// </summary>
        public DateTimeSerarchModel()
        {
            //DateTime dt = DateTime.Now.AddDays(-3);
            //BeginDateTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //dt = DateTime.Now.AddDays(+1);
            //EndDateTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }
        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        /// <param name="days"></param>
        public DateTimeSerarchModel(int days)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            BeginDateTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            dt = DateTime.Now.AddDays(+1);
            EndDateTime = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
        }

        /// <summary>
        /// 開始時間
        /// </summary>
        [Display(Name = "開始時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [DataType(DataType.DateTime)]
        public DateTime BeginDateTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        [Display(Name = "結束時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }
    }
    #endregion

    /// <summary>
    /// 使用者選單+ Date(yyyy-MM-dd) 查詢
    /// </summary>
    public class LoginIdDateSerarchModel : DateSerarchModel
    {
        /// <summary>
        /// 初始 區間: 前3日~後1日、使用者選單。
        /// </summary>
        public LoginIdDateSerarchModel()
        {
            DateTime dt = DateTime.Now.AddDays(-3);
            BeginDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            dt = DateTime.Now.AddDays(+1);
            EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //LoginIdList = _user.GetUsersList();
        }

        /// <summary>
        /// 指定區間天數。例: 3表示 區間: 前三日~後一日
        /// </summary>
        /// <param name="days"></param>
        public LoginIdDateSerarchModel(int days)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            BeginDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            dt = DateTime.Now.AddDays(+1);
            EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //LoginIdList = _user.GetUsersList();
        }

        /// <summary>
        /// 使用者Id = fsUSER_ID
        /// </summary>
        [Display(Name = "系統帳號")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 使用者下拉選單
        /// </summary>
        [Display(Name = "帳號")]
        //public string[] LoginIds { set; get; }
        public List<SelectListItem> LoginIdList { get; set; }
    }

    #region -------------------- 轉檔工作,調用(轉檔工作)狀態
    /// <summary>
    /// 轉檔工作狀態(tblWORK.fsSTATUS)+ Date(yyyy-MM-dd) 查詢
    /// </summary>
    public class WorkStatusDateSerarchModel : DateSerarchModel
    {
        static CodeService _ser = new CodeService();
        List<SelectListItem> _tclist = _ser.GetCodeItemList(TbzCodeIdEnum.WORK_TC.ToString(), true, false, true);

        /// <summary>
        /// 初始 區間: 前三日~後一日
        /// </summary>
        public WorkStatusDateSerarchModel()
        {
            //DateTime dt = DateTime.Now.AddDays(-3);
            //BeginDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            //WorkStatusList = tclist;
        }

        /// <summary>
        /// 指定區間天數。例: 3表示 區間: 前三日~後一日
        /// </summary>
        /// <param name="days"></param>
        public WorkStatusDateSerarchModel(int days=3)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            BeginDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            dt = DateTime.Now.AddDays(+1);
            EndDate = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
            WorkStatusList = _tclist;
        }

        /// <summary>
        /// (轉檔)工作狀態 tblWORK.fsSTATUS
        /// </summary>
        [Display(Name = "工作狀態")]
        public string WorkStatus { get; set; } = string.Empty;

        /// <summary>
        /// (轉檔)工作狀態 下拉選單 tblWORK.fsSTATUS
        /// </summary>
        public List<SelectListItem> WorkStatusList { get; set; } = new List<SelectListItem>();
    }

    /// <summary>
    /// 調用狀態(tblWORK.fsSTATUS),轉檔編號 + String(yyyy/MM/dd)
    /// </summary>
    public class BookingDateSearchModel : SearchByDate
    {
        /// <summary>
        /// 初始 區間: 前三日~後一日
        /// </summary>
        public BookingDateSearchModel()
        {
            ////預存程序 日期查詢 格式:yyyy/MM/dd
            //DateTime dt = DateTime.Now.AddDays(-3);
            //StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }
        
        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        public BookingDateSearchModel(int days=3)
        {
            //預存程序 日期查詢 格式:yyyy/MM/dd
            DateTime dt = DateTime.Now.AddDays(-days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// (轉檔工作)調用狀態 tblWORK.fsSTATUS
        /// </summary>
        [Display(Name = "調用狀態")]
        public string WorkStatus { get; set; } = string.Empty;

        /// <summary>
        /// (轉檔工作)調用狀態 下拉選單 tblWORK.fsSTATUS
        /// </summary>
        public List<SelectListItem> WorkStatusList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 轉檔編號  dbo.tblWORK.[fnWORK_ID]
        /// </summary>
        [Display(Name = "轉檔編號")]
        public string WorkId { get; set; } = string.Empty;
    }
    #endregion

    /// <summary>
    /// 審核(調用)功能頁面-search model
    /// </summary>
    public class VerifyDateSerarchModel : SearchByDate
    {
        /// <summary>
        /// 初始參數為近7日
        /// </summary>
        public VerifyDateSerarchModel()
        {
            //DateTime dt = DateTime.Now.AddDays(-7);
            //StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }
        
        /// <summary>
        /// 依指定天數取回參數
        /// </summary>
        /// <param name="days"></param>
        public VerifyDateSerarchModel(int days=3)
        {
            DateTime dt = DateTime.Now.AddDays(-days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// 審核狀態
        /// </summary>
        [Display(Name = "審核狀態")]
        public string ApproveStatus { get; set; } = string.Empty;

        public List<SelectListItem> ApproveStatusList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 調用者.使用者Id = fsUSER_ID
        /// </summary>
        [Display(Name = "調用者")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 使用者下拉選單
        /// </summary>
        public List<SelectListItem> LoginIdList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 調用的工作轉檔編號(
        /// </summary>
        [Display(Name = "轉檔編號")]
        public int[] WorkIds { get; set; }
    }

    /// <summary>
    /// 刪除紀錄管理 查詢(媒體類別,刪除動作狀態ARC006)
    /// </summary>
    public class DeleteDateSearchModel : SearchByDate
    {
        static CodeService _ser = new CodeService();
        List<SelectListItem> _typelist = _ser.GetCodeItemList(TbzCodeIdEnum.ARC004.ToString(), true, true, true);
        List<SelectListItem> _arc6list = _ser.GetCodeItemList(TbzCodeIdEnum.ARC006.ToString(), true, true, true);

        /// <summary>
        /// 初始 區間: 前三日~後一日
        /// </summary>
        public DeleteDateSearchModel()
        {
            ////預存程序 日期查詢 格式:yyyy/MM/dd
            //DateTime dt = DateTime.Now.AddDays(-3);
            //StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");

            //#region === 下拉選單 ===
            //MediaTypeList = _typelist;// _ser.GetCodeItemList(TbzCodeIdEnum.ARC004.ToString(), true, true, true);
            ////var _arc6 = _ser.GetCodeItemList(TbzCodeIdEnum.ARC006.ToString(), true, true, true);
            //_arc6list.Add(new SelectListItem { Text = " 暫刪除", Value = "-1" });
            //StatusList = _arc6list;
            //#endregion
        }
        
        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        public DeleteDateSearchModel(int days=3)
        {
            //預存程序 日期查詢 格式:yyyy/MM/dd
            DateTime dt = DateTime.Now.AddDays(-days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            #region === 下拉選單 ===
            MediaTypeList = _typelist;
            _arc6list.Add(new SelectListItem { Text = " 暫刪除", Value = "-1" });
            StatusList = _arc6list;
            #endregion
        }

        /// <summary>
        /// 媒體類別下拉清單(TbzCodeIdEnum.ARC004) V,A,P,D
        /// </summary>
        [Display(Name ="類別")]
        public List<SelectListItem> MediaTypeList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 狀態下拉清單(TbzCodeIdEnum.ARC006 刪除動作狀態)
        /// </summary>
        [Display(Name = "狀態")]
        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();
    }

    /// <summary>
    /// 系統報表檔案下載 查詢參數Model (for Index.cshtml)
    /// </summary>
    public class ReportSearchModel : SearchByDate
    {
        static CodeService _ser = new CodeService();
        List<SelectListItem> _rptlist = _ser.GetCodeItemList(TbzCodeIdEnum.REPORT.ToString(), true, false, false);
        List<SelectListItem> _kindlist = new List<SelectListItem>()
        {
            new SelectListItem { Text = " Excel ", Value = "EXCELOPENXML" },
            new SelectListItem(){ Text = " PDF ", Value = "pdf" }
        };
        /// <summary>
        /// 初始 區間: 前三日~後一日
        /// </summary>
        public ReportSearchModel()
        {
            ////預存程序 日期查詢 格式:yyyy/MM/dd
            //DateTime dt = DateTime.Now.AddDays(-3);
            //StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //#region === 下拉選單 ===
            //RptItemList = _rptlist;
            //RptKindList = _kindlist;
            //#endregion
        }

        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        public ReportSearchModel(int days=3)
        {
            //預存程序 日期查詢 格式:yyyy/MM/dd
            DateTime dt = DateTime.Now.AddDays(days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            #region === 下拉選單 ===
            RptItemList = _rptlist;
            RptKindList = _kindlist;
            #endregion
        }

        /// <summary>
        /// 報表種類: excel, pdf
        /// </summary>
        [Display(Name = "報表類別")]
        public string RptKind { get; set; } = string.Empty;
        public List<SelectListItem> RptKindList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 報表名稱: 入庫明細表、入庫統計表 (tbzCODE.fsCode_Id='Report'
        /// </summary>
        [Display(Name = "報表名稱")]
        public string RptItem { get; set; } = string.Empty;
        public List<SelectListItem> RptItemList { get; set; } = new List<SelectListItem>();
    }

    public class ReportParameterModel : SearchByDate
    {
        /// <summary>
        /// 初始 區間: 前三日~後一日
        /// </summary>
        public ReportParameterModel()
        {
            ////預存程序 日期查詢 格式:yyyy/MM/dd
            //DateTime dt = DateTime.Now.AddDays(-3);
            //StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            //dt = DateTime.Now.AddDays(+1);
            //EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
        }

        /// <summary>
        /// 指定區間天數。例: 5表示 區間: 前五日~後一日
        /// </summary>
        public ReportParameterModel(int days=3, string rpt = "")
        {
            //預存程序 日期查詢 格式:yyyy/MM/dd
            DateTime dt = DateTime.Now.AddDays(days);
            StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            dt = DateTime.Now.AddDays(+1);
            EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
            RptItem = rpt;
        }

        /// <summary>
        /// 報表名稱: 入庫明細表、入庫統計表 (tbzCODE.fsCode_Id='Report'
        /// </summary>
        [Display(Name = "報表名稱")]
        public string RptItem { get; set; } = string.Empty;
    }
}
