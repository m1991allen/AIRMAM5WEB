using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models
{
    #region _9/13另新增sp,不使用 spGET_L_WORK
    ///// <summary>
    ///// spGET_L_WORK 查詢參數Model
    ///// </summary>
    //public class SP_GetLWork
    //{
    //    public SP_GetLWork() { }

    //    public SP_GetLWork(long workid) { WorkId = workid; }

    //    /// <summary>
    //    /// 工作編號
    //    /// </summary>
    //    public long WorkId { get; set; } = 0;

    //    /// <summary>
    //    /// 群組編號
    //    /// </summary>
    //    public long GroupId { get; set; } = 0;

    //    /// <summary>
    //    /// 工作/轉檔類別 , dbo.tbzCODE.fsCODE_ID = WORK001
    //    /// </summary>
    //    public string WorkType { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 工作/轉檔狀態 , tbzCODE.fsCODE_ID = WORK_TC；tbzCODE.fsCODE_ID = WORK_BK
    //    /// </summary>
    //    public string WorkStatus { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 開始日期 yyyy/MM/dd
    //    /// </summary>
    //    public string BegDate { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 結束日期 yyyy/MM/dd
    //    /// </summary>
    //    public string EndDate { get; set; } = string.Empty;
    //}
    #endregion

    #region 【查詢: 上傳轉檔 Model】
    /*20200831 *///--GetLWorkByTranscodeParam 移至  AIRMAM5.DBEntity.Models.Works
    ///// <summary>
    ///// 取出上傳轉檔主檔資料 【spGET_L_WORK_BY_TRANSCODE】參數
    ///// </summary>
    //public class GetLWorkByTranscodeParam : SearchByDate
    //{
    //    public GetLWorkByTranscodeParam() { }
    //    /// <summary>
    //    /// 指定區間天數。例: 5表示 區間: 前五日~後一日
    //    /// </summary>
    //    public GetLWorkByTranscodeParam(long workid, int days)
    //    {
    //        WorkId = workid;
    //        DateTime dt = DateTime.Now.AddDays(-days);
    //        StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
    //        dt = DateTime.Now.AddDays(+1);
    //        EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
    //    }
    //    /// <summary>
    //    /// 【spGET_L_WORK_BY_TRANSCODE】參數: 指定 [fsWORK_ID]
    //    /// </summary>
    //    /// <param name="workid"></param>
    //    public GetLWorkByTranscodeParam(long workid) { WorkId = workid; StartDate = string.Empty; EndDate = string.Empty; }

    //    /// <summary>
    //    /// 工作編號
    //    /// </summary>
    //    public long WorkId { get; set; }

    //    /// <summary>
    //    /// 工作/轉檔狀態 , fsSTATUS 是 CODE.WORK_TC 這個代碼
    //    /// </summary>
    //    public string WorkStatus { get; set; } = string.Empty;
    //}
    
    /*20200831 *///--GetLWorkByTranscodeParam 移至  AIRMAM5.DBEntity.Models.Works
    ///// <summary>
    ///// (整批)重新轉檔 結果資訊
    ///// </summary>
    //public class UpdateLWorkReTranResult : VerifyResult
    //{
    //    public UpdateLWorkReTranResult() { }

    //    public UpdateLWorkReTranResult(bool b, string m) { IsSuccess = b; Message = m; }

    //    /// <summary>
    //    /// Procedure : OK/已處理
    //    /// </summary>
    //    public List<string> Processed { get; set; } = new List<string>();

    //    /// <summary>
    //    /// Procedure : 未處理無錯誤...(fsSTATUS=1~8不能轉檔)
    //    /// </summary>
    //    public List<string> UnProcessed { get; set; } = new List<string>();

    //    /// <summary>
    //    /// Procedure 回覆 Error字樣.
    //    /// </summary>
    //    public List<string> Failure { get; set; } = new List<string>();
    //}
    
    /*20200831 *///--UploadWorkEditModel 移至  AIRMAM5.DBEntity.Models.Works
    ///// <summary>
    ///// 上傳紀錄內容 Edit Model
    ///// </summary>
    //public class UploadWorkEditModel
    //{
    //    public UploadWorkEditModel() { }

    //    /// <summary>
    //    /// 預存【spGET_L_WORK_BY_TRANSCODE 】資料
    //    /// </summary>
    //    /// <param name="m"></param>
    //    public UploadWorkEditModel(spGET_L_WORK_BY_TRANSCODE_Result m)
    //    {
    //        fnWORK_ID = m.fnWORK_ID;
    //        fsTYPE = m.fsTYPE;
    //        C_sTYPENAME = m.C_sTYPENAME ?? string.Empty;
    //        WorkStatus = m.fsSTATUS;
    //        StatusName = m.C_sSTATUSNAME ?? string.Empty;
    //        StatusColor = m.fsSTATUS_COLOR ?? "grey";
    //        fsPRIORITY = m.fsPRIORITY;
    //        fsRESULT = m.fsRESULT;
    //        fsNOTE = m.fsNOTE ?? string.Empty;
    //        fdSTIME = m.fdSTIME;
    //        fdETIME = m.fdETIME;
    //        C_sFILE_INFO = m.C_sFILE_INFO ?? string.Empty;
    //        fsPARAMETERS = m.fsPARAMETERS ?? string.Empty;
    //        Progress = m.fsPROGRESS;
    //        CreatedBy = string.Format("{0}{1}"
    //            , string.IsNullOrEmpty(m.fsCREATED_BY) ? string.Empty : m.fsCREATED_BY
    //            , string.IsNullOrEmpty(m.fsCREATED_BY_NAME) ? string.Empty : string.Format($"({m.fsCREATED_BY_NAME})"));
    //    }

    //    /// <summary>
    //    /// 轉檔編號 fnWORK_ID
    //    /// </summary>
    //    [Display(Name = "編號")]
    //    public long fnWORK_ID { get; set; }

    //    /// <summary>
    //    /// 工作/轉檔類別 fsTYPE , dbo.tbzCODE.fsCODE_ID = WORK001
    //    /// </summary>
    //    [Display(Name = "轉檔類別")]
    //    public string fsTYPE { get; set; }
    //    /// <summary>
    //    /// 工作/轉檔類別 中文
    //    /// </summary>
    //    public string C_sTYPENAME { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 工作/轉檔狀態 fsSTATUS , tbzCODE.fsCODE_ID = WORK_TC/WORK_BK
    //    /// </summary>
    //    [Display(Name = "狀態")]
    //    public string WorkStatus { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 轉檔狀態 [C_sSTATUSNAME]
    //    /// </summary>
    //    [Display(Name = "轉檔狀態")]
    //    public string StatusName { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 狀態顏色表示
    //    /// </summary>
    //    public string StatusColor { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 進度% [fsPROGRESS]
    //    /// </summary>
    //    [Display(Name = "進度%")]
    //    public string Progress { get; set; }
    //    /// <summary>
    //    /// 優先順序 fsPRIORITY
    //    /// </summary>
    //    [Display(Name = "優先順序")]
    //    //[Range(1, 9)]
    //    public string fsPRIORITY { get; set; }

    //    /// <summary>
    //    /// 轉檔結果 fsRESULT
    //    /// </summary>
    //    [Display(Name = "轉檔結果")]
    //    public string fsRESULT { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 備註 fsNOTE
    //    /// </summary>
    //    [Display(Name = "備註")]
    //    public string fsNOTE { get; set; } = string.Empty;

    //    /// <summary> 
    //    /// 開始轉檔時間 fdSTIME
    //    /// </summary>
    //    [Display(Name = "開始轉檔時間")]
    //    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
    //    public DateTime? fdSTIME { get; set; }
    //    /// <summary>
    //    /// 結束轉檔時間 fdETIME
    //    /// </summary>
    //    [Display(Name = "結束轉檔時間")]
    //    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true, NullDisplayText = "")]
    //    public DateTime? fdETIME { get; set; }

    //    /// <summary>
    //    /// 檔案資訊 C_sFILE_INFO
    //    /// </summary>
    //    [Display(Name = "檔案資訊")]
    //    public string C_sFILE_INFO { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 轉檔參數 fsPARAMETERS
    //    /// </summary>
    //    [Display(Name = "轉檔參數")]
    //    public string fsPARAMETERS { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 建立者
    //    /// </summary>
    //    [Display(Name = "建立者")]
    //    public string CreatedBy { get; set; }
    //}
    
    /*20200831 *///--UploadWorkViewModel 移至  AIRMAM5.DBEntity.Models.Works
    ///// <summary>
    ///// 手動上傳轉檔詳細內容Modal
    ///// </summary>
    //public class UploadWorkViewModel : UploadWorkEditModel
    //{
    //    public UploadWorkViewModel() { }
    //    /// <summary>
    //    /// 預存【spGET_L_WORK_BY_TRANSCODE 】資料
    //    /// </summary>
    //    /// <param name="m"></param>
    //    public UploadWorkViewModel(spGET_L_WORK_BY_TRANSCODE_Result m)
    //    {
    //        fnWORK_ID = m.fnWORK_ID;
    //        fsTYPE = m.fsTYPE;
    //        C_sTYPENAME = m.C_sTYPENAME ?? string.Empty;
    //        WorkStatus = m.fsSTATUS;
    //        StatusName = m.C_sSTATUSNAME ?? string.Empty;
    //        fsPRIORITY = m.fsPRIORITY;
    //        fsRESULT = m.fsRESULT ?? string.Empty;
    //        fsNOTE = m.fsNOTE ?? string.Empty;
    //        fdSTIME = m.fdSTIME;
    //        fdETIME = m.fdETIME;
    //        C_sFILE_INFO = m.C_sFILE_INFO ?? string.Empty;
    //        fsPARAMETERS = m.fsPARAMETERS ?? string.Empty;
    //        Progress = m.fsPROGRESS;
    //        CreatedBy = string.Format("{0}{1}"
    //            , string.IsNullOrEmpty(m.fsCREATED_BY) ? string.Empty : m.fsCREATED_BY
    //            , string.IsNullOrEmpty(m.fsCREATED_BY_NAME) ? string.Empty : string.Format($"({m.fsCREATED_BY_NAME})"));
    //        UpdatedBy = string.Format("{0}{1}"
    //            , string.IsNullOrEmpty(m.fsUPDATED_BY) ? string.Empty : m.fsUPDATED_BY
    //            , string.IsNullOrEmpty(m.fsUPDATED_BY_NAME) ? string.Empty : string.Format($"({m.fsUPDATED_BY_NAME})"));
    //        CreatedTime = m.fdCREATED_DATE;
    //        UpdatedTime = m.fdUPDATED_DATE;
    //    }

    //    ///// <summary>
    //    ///// 進度 %
    //    ///// </summary>
    //    //[Display(Name = "轉檔進度")]
    //    //public string fsPROGRESS { get; set; }

    //    /// <summary>
    //    /// 建立時間
    //    /// </summary>
    //    [Display(Name = "建立時間")]
    //    public DateTime CreatedTime { get; set; }
    //    ///// <summary>
    //    ///// 建立者
    //    ///// </summary>
    //    //[Display(Name = "建立者")]
    //    //public string CreatedBy { get; set; }
    //    /// <summary>
    //    /// 最後異動時間
    //    /// </summary>
    //    [Display(Name = "異動時間")]
    //    public DateTime? UpdatedTime { get; set; }
    //    /// <summary>
    //    /// 最後異動者
    //    /// </summary>
    //    [Display(Name = "異動者")]
    //    public string UpdatedBy { get; set; }
    //}
    #endregion

    #region 【檔案調用/ Model】
    /*20200831*///--GetLWorkByBookingParam 移至  AIRMAM5.DBEntity.Models.Booking
    ///// <summary>
    ///// 取出調用轉檔資料 【spGET_L_WORK_BY_BOOKING】參數
    ///// </summary>
    //public class GetLWorkByBookingParam : GetLWorkByTranscodeParam
    //{
    //    public GetLWorkByBookingParam() { }
    //    /// <summary>
    //    /// 僅指定 工作轉檔編號[fsWORK_ID]
    //    /// </summary>
    //    /// <param name="workid"></param>
    //    /// <param name="loginid"></param>
    //    public GetLWorkByBookingParam(long workid, string loginid)
    //    {
    //        this.WorkId = workid;
    //        this.LoginId = loginid;
    //        StartDate = string.Empty;
    //        EndDate = string.Empty;
    //        WorkStatus = string.Empty;
    //    }

    //    /// <summary>
    //    /// 指定
    //    /// </summary>
    //    /// <param name="days">指定區間天數。例: 5表示 區間: 前五日~後一日 </param>
    //    /// <param name="workid">工作轉檔編號[fsWORK_ID] </param>
    //    /// <param name="loginid">使用者帳號 </param>
    //    /// <param name="status">工作轉檔狀態[fsSTATUS] </param>
    //    public GetLWorkByBookingParam(int days, long workid, string loginid, string status)
    //    {
    //        //預存程序 日期查詢 格式:yyyy/MM/dd
    //        DateTime dt = DateTime.Now.AddDays(-days);
    //        StartDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
    //        dt = DateTime.Now.AddDays(+1);
    //        EndDate = string.Format($"{new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0):yyyy/MM/dd}");
    //        //
    //        this.WorkId = workid;
    //        this.LoginId = loginid;
    //        this.WorkStatus = status;
    //    }
       
    //    /// <summary>
    //    /// 使用者帳號
    //    /// </summary>
    //    public string LoginId { get; set; } = string.Empty;
    //}
    
    /*20200831*///--MyBookingModel 移至  AIRMAM5.DBEntity.Models.Booking
    ///// <summary>
    ///// 我的調用狀態資料Model
    ///// </summary>
    //public class MyBookingModel
    //{
    //    public MyBookingModel() { }
    //    public MyBookingModel(spGET_L_WORK_BY_BOOKING_Result m)
    //    {
    //        BookingId = m.fnBOOKING_ID;
    //        WorkId = m.fnWORK_ID;
    //        Progress = m.fsPROGRESS;
    //        Result = m.fsRESULT;
    //        ArcType = m.C_ARC_TYPE;
    //        ArcTypeName = m.C_ARC_TYPE_NAME;
    //        BookingTypeName = m.fsTYPE_NAME;
    //        WorkStatus = m.fsSTATUS ?? string.Empty;
    //        StatusName = m.fsSTATUS_NAME ?? string.Empty;
    //        StatusColor = m.fsSTATUS_COLOR ?? "grey"; //排程中
    //        Title = m.fsTITLE ?? string.Empty;
    //        BookingDate = string.Format($"{m.fdCREATED_DATE:yyyy/MM/dd HH:mm:ss}");
    //        MarkInTime = m.fdMARKIN ?? string.Empty; ;
    //        MarkOutTime = m.fdMARKOUT ?? string.Empty; ;
    //        NoteStr = m.fsNOTE ?? string.Empty; ;
    //        StartTime = string.Format($"{m.fdSTIME:yyyy/MM/dd HH:mm:ss}"); 
    //        EndTime = string.Format($"{m.fdETIME:yyyy/MM/dd HH:mm:ss}");
    //    }

    //    /// <summary>
    //    /// 調用編號 [fnBOOKING_ID]
    //    /// </summary>
    //    public long BookingId { get; set; } = 0;
    //    /// <summary>
    //    /// 轉檔編號 [fnWORK_ID]
    //    /// </summary>
    //    public long WorkId { get; set; } = 0;

    //    /// <summary>
    //    /// 轉檔進度 %
    //    /// </summary>
    //    [Display(Name = "轉檔進度")]
    //    public string Progress { get; set; }
    //    /// <summary>
    //    /// 調用結果 [fsRESULT]
    //    /// </summary>
    //    public string Result { get; set; }
    //    /// <summary>
    //    /// 檔案類型 [_ARC_TYPE] : 影音圖文
    //    /// </summary>
    //    public string ArcType { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 檔案類型名稱 [_ARC_TYPE_NAME]: fsCODE_ID = 'MTRL001'
    //    /// </summary>
    //    public string ArcTypeName { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 調用類別名稱 [fsTYPE_NAME]: fsCODE_ID = 'WORK001'
    //    /// </summary>
    //    public string BookingTypeName { get; set; } = string.Empty;

    //    /// <summary>
    //    /// 狀態 [fsSTATUS]
    //    /// </summary>
    //    public string WorkStatus { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 狀態說明 [fsSTATUS_NAME] 
    //    /// </summary>
    //    public string StatusName { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 狀態顏色表示
    //    /// </summary>
    //    public string StatusColor { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 標題 [fsTITLE]
    //    /// </summary>
    //    public string Title { get; set; } = string.Empty;
        
    //    /// <summary>
    //    /// 調用日期 [fdCREATED_DATE]
    //    /// </summary>
    //    public string BookingDate { get; set; }

    //    /// <summary>
    //    /// 起始時間 [fdMARKIN] : 已為TimeCode格式
    //    /// </summary>
    //    public string MarkInTime { get; set; }
    //    /// <summary>
    //    /// 結束時間 [fdMARKOUT] : 已為TimeCode格式
    //    /// </summary>
    //    public string MarkOutTime { get; set; }

    //    /// <summary>
    //    /// 調用備註 [fsNOTE]
    //    /// </summary>
    //    public string NoteStr { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 轉檔開始時間 [fdSTIME]
    //    /// </summary>
    //    public string StartTime { get; set; }
    //    /// <summary>
    //    /// 轉檔結束時間 [fdETIME]
    //    /// </summary>
    //    public string EndTime { get; set; }
    //}
    
    /*20200831*///--BookingDetModel 移至  AIRMAM5.DBEntity.Models.Booking
    ///// <summary>
    ///// 調用詳細內容Model
    ///// </summary>
    //public class BookingDetModel : MyBookingModel
    //{
    //    public BookingDetModel() { }
    //    public BookingDetModel(spGET_L_WORK_BY_BOOKING_Result m)
    //    {
    //        BookingId = m.fnBOOKING_ID;
    //        WorkId = m.fnWORK_ID;
    //        Progress = m.fsPROGRESS;
    //        Result = m.fsRESULT;
    //        ArcType = m.C_ARC_TYPE;
    //        ArcTypeName = m.C_ARC_TYPE_NAME;
    //        BookingTypeName = m.fsTYPE_NAME;
    //        WorkStatus = m.fsSTATUS ?? string.Empty;
    //        StatusName = m.fsSTATUS_NAME ?? string.Empty;
    //        Title = m.fsTITLE ?? string.Empty;
    //        BookingDate = string.Format($"{m.fdCREATED_DATE:yyyy/MM/dd HH:mm:ss}");
    //        MarkInTime = m.fdMARKIN ?? string.Empty; ;
    //        MarkOutTime = m.fdMARKOUT ?? string.Empty; ;
    //        NoteStr = m.fsNOTE ?? string.Empty; ;
    //        StartTime = string.Format($"{m.fdSTIME:yyyy/MM/dd HH:mm:ss}");
    //        EndTime = string.Format($"{m.fdETIME:yyyy/MM/dd HH:mm:ss}");
    //        CreatedBy = m.fsCREATED_BY;
    //        CreatedByName = m.fsCREATED_BY_NAME;
    //        CreatedDate = m.fdCREATED_DATE;
    //        UpdatedBy = m.fsUPDATED_BY;
    //        UpdatedByName = m.fsUPDATED_BY_NAME;
    //        UpdatedDate = m.fdUPDATED_DATE;
    //    }

    //    /// <summary>
    //    /// 建立時間 fdCREATED_DATE
    //    /// </summary>
    //    public DateTime CreatedDate { get; set; }
    //    /// <summary>
    //    /// 建立帳號 fsCREATED_BY
    //    /// </summary>
    //    [Display(Name = "建立帳號")]
    //    public string CreatedBy { get; set; }
    //    /// <summary>
    //    /// 最後異動時間 fdUPDATED_DATE
    //    /// </summary>
    //    public DateTime? UpdatedDate { get; set; }
    //    /// <summary>
    //    /// 最後異動帳號 fsUPDATED_BY
    //    /// </summary>
    //    [Display(Name = "最後異動帳號")]
    //    public string UpdatedBy { get; set; }

    //    /// <summary>
    //    /// 建立帳號顯示名稱 fsCREATED_BY_NAME
    //    /// </summary>
    //    [Display(Name = "建立帳號名稱")]
    //    public string CreatedByName { get; set; }

    //    /// <summary>
    //    /// 最後異動帳號顯示名稱 fsUPDATED_BY_NAME
    //    /// </summary>
    //    [Display(Name = "最後異動帳號名稱")]
    //    public string UpdatedByName { get; set; }
    //}
    
    /*20200831*///--BookingViewModel 移至  AIRMAM5.DBEntity.Models.Booking
    ///// <summary>
    ///// 管理調用狀態資料Model 
    ///// </summary>
    //public class BookingViewModel : MyBookingModel
    //{
    //    public BookingViewModel() { }
    //    /// <summary>
    //    /// 預存結果
    //    /// </summary>
    //    /// <param name="m"></param>
    //    public BookingViewModel(spGET_L_WORK_BY_BOOKING_Result m)
    //    {
    //        BookingId = m.fnBOOKING_ID;
    //        WorkId = m.fnWORK_ID;
    //        Progress = m.fsPROGRESS;
    //        Result = m.fsRESULT;
    //        ArcType = m.C_ARC_TYPE;
    //        ArcTypeName = m.C_ARC_TYPE_NAME;
    //        BookingTypeName = m.fsTYPE_NAME;
    //        WorkStatus = m.fsSTATUS ?? string.Empty;
    //        StatusName = m.fsSTATUS_NAME ?? string.Empty;
    //        StatusColor = m.fsSTATUS_COLOR ?? "grey"; //排程中
    //        Title = m.fsTITLE ?? string.Empty;
    //        BookingDate = string.Format($"{m.fdCREATED_DATE:yyyy/MM/dd HH:mm:ss}");
    //        MarkInTime = m.fdMARKIN ?? string.Empty; ;
    //        MarkOutTime = m.fdMARKOUT ?? string.Empty; ;
    //        NoteStr = m.fsNOTE ?? string.Empty; ;
    //        StartTime = string.Format($"{m.fdSTIME:yyyy/MM/dd HH:mm:ss}");
    //        EndTime = string.Format($"{m.fdETIME:yyyy/MM/dd HH:mm:ss}");
    //        Priority = m.fsPRIORITY ?? string.Empty;
    //        CreateBy = m.fsCREATED_BY;
    //    }

    //    /// <summary>
    //    /// 優先權
    //    /// </summary>
    //    public string Priority { get; set; }
    //    /// <summary>
    //    /// 調用者
    //    /// </summary>
    //    public string CreateBy { get; set; }
    //}
    #endregion

    /*20200831*///--LWorkIdModel 移至  AIRMAM5.DBEntity.Models.Works
    ///// <summary>
    ///// 轉檔工作編號 tblWORK.[fnWORK_ID]
    ///// </summary>
    //public class LWorkIdModel
    //{
    //    /// <summary>
    //    /// 轉檔工作編號 fnWORK_ID
    //    /// </summary>
    //    [Display(Name = "工作編號")]
    //    public long fnWORK_ID { get; set; }
    //}

    /*20200831*///--LWorkProgressModel 移至  AIRMAM5.DBEntity.Models.Works
    ///// <summary>
    ///// 轉檔工作編號進度資料
    ///// </summary>
    //public class LWorkProgressModel : LWorkIdModel
    //{
    //    public LWorkProgressModel() { }
    //    public LWorkProgressModel(spGET_L_WORK_MERGE_Result m)//(tblWORK m)
    //    {
    //        fnWORK_ID = m.fnWORK_ID;
    //        Progress = m.fsPROGRESS;
    //        WorkStatus = m.fsSTATUS ?? string.Empty;
    //        WorkStatusName = m.C_sSTATUSNAME ?? string.Empty;
    //        StatusColor = m.C_sSTATUSCOLOR ?? "grey";
    //        WorkSTime = m.fdSTIME == null ? string.Empty : string.Format($"{m.fdSTIME:yyyy-MM-dd HH:mm:ss}");
    //        WorkETime = m.fdETIME == null ? string.Empty : string.Format($"{m.fdETIME:yyyy-MM-dd HH:mm:ss}");
    //    }

    //    /// <summary>
    //    /// 進度 % [fsPROGRESS]
    //    /// </summary>
    //    [Display(Name = "轉檔進度")]
    //    [JsonProperty(PropertyName = "Progress")]
    //    public string Progress { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 轉檔狀態代碼  [fsSTATUS]
    //    /// </summary>
    //    public string WorkStatus { get; set; }
    //    /// <summary>
    //    /// 轉檔狀態代碼中文
    //    /// </summary>
    //    public string WorkStatusName { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 狀態顏色表示
    //    /// </summary>
    //    public string StatusColor { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 轉檔開始時間
    //    /// </summary>
    //    public string WorkSTime { get; set; } = string.Empty;
    //    /// <summary>
    //    /// 轉檔結束時間
    //    /// </summary>
    //    public string WorkETime { get; set; } = string.Empty;
    //}

}
