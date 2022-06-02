using System;
using System.Net;

namespace AIRMAM5.DBEntity.Models.Shared
{
    /// <summary>
    /// 驗證結果格式
    /// </summary>
    public class VerifyResult
    {
        /// <summary>
        /// false
        /// </summary>
        public VerifyResult() : base()
        {
            IsSuccess = false;
            Message = string.Empty;
            Data = new object();
        }

        /// <summary>
        /// 指定 IsSuccess
        /// </summary>
        /// <param name="b"></param>
        public VerifyResult(bool b)
        {
            IsSuccess = b;
        }

        /// <summary>
        /// 指定 IsSuccess, Message
        /// </summary>
        /// <param name="b"></param>
        /// <param name="m"></param>
        public VerifyResult(bool b, string m)
        {
            IsSuccess = b;
            Message = m;
        }

        /// <summary>
        /// 結果
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 回覆前端資料Model
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 例外訊息
        /// </summary>
        public Exception ErrorException { get; set; }
    }

    /// <summary>
    /// 統一回覆前端格式 HttpStatusCode
    /// </summary>
    public class ResponseResultModel : VerifyResult
    {
        /// <summary>
        /// 初始 false
        /// </summary>
        public ResponseResultModel() : base()
        {
            IsSuccess = false;
            Message = string.Empty;
            StatusCode = HttpStatusCode.OK;
            ResponseTime = string.Format($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        }
        /// <summary>
        /// 指定 IsSuccess, ResponseTime
        /// </summary>
        /// <param name="b"></param>
        public ResponseResultModel(bool b)
        {
            IsSuccess = b;
            ResponseTime = string.Format($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        }
        /// <summary>
        /// 指定 IsSuccess, Message, Records, ResponseTime
        /// </summary>
        /// <param name="b"></param>
        /// <param name="m"></param>
        /// <param name="r">前端參數資料model</param>
        public ResponseResultModel(bool b, string m, object r)
        {
            IsSuccess = b;
            Message = m;
            Records = r;
            ResponseTime = string.Format($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        }
        /// <summary>
        /// 依 VerifyResult Model 指定 值(IsSuccess, Message, Data)
        /// </summary>
        /// <param name="r"></param>
        public ResponseResultModel(VerifyResult r)
        {
            IsSuccess = r.IsSuccess;
            Message = r.Message;
            Data = r.Data;
            ResponseTime = string.Format($"{DateTime.Now:yyyy/MM/dd HH:mm:ss}");
        }

        /// <summary>
        /// HttpStatusCode 狀態碼
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// SEVER回覆時間   yyyy-MM-dd(T)HH:mm:ss, EX: 2019-06-28T18:24:18
        /// </summary>
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        //public DateTime ResponseTime { get; set; } = DateTime.Now;
        public string ResponseTime { get; set; } = string.Empty;

        /// <summary>
        /// 前端參數資料Model
        /// </summary>
        public object Records { get; set; }  = new object();

        ///// <summary>
        ///// 回覆前端資料Model
        ///// </summary>
        //public object Data { get; set; } = new object();
    }

    /// <summary>
    /// API Response格式
    /// </summary>
    public class APIResponse
    {
        //{"fsRESULT":"error","fsMESSAGE":"刪除失敗"}

        public bool IsOk { get; set; }

        public string Message { get; set; }
    }

    #region Search API 回覆格式
    /// <summary>
    /// AIRMAM5.Search.Controllers 回覆格式(原本寫法,已改寫:api有搭配修改回傳格式)
    /// </summary>
    /// <remarks> 範例: {"fsRESULT":"error","fsMESSAGE":"刪除失敗"} </remarks>
    [Obsolete("已改寫: SearchAPIResponse", true)]
    public class SearchAPIReturn
    {
        /// <summary>
        /// 結果: success, error
        /// </summary>
        public string fsRESULT { get; set; }
        /// <summary>
        /// 訊息文字
        /// </summary>
        public string fsMESSAGE { get; set; }
    }
    /// <summary>
    /// AIRMAM5.Search.Lucene.Controllers 回覆格式
    /// </summary>
    /// <remarks> 配合API修改回覆格式，新增。</remarks>
    public class SearchAPIResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; } = false;
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 回傳資訊
        /// </summary>
        public object Data { get; set; } = null;
        /// <summary>
        /// 回傳時間
        /// </summary>
        public string ResponseTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        /// <summary>
        /// 例外訊息
        /// </summary>
        public Exception ErrorException { get; set; } = null;
    }
    /// <summary>
    /// Search API Response Model + API HttpStatusCode 
    /// </summary>
    public class SearchAPIResponseCode : SearchAPIResponse
    {
        public SearchAPIResponseCode() { }
        public SearchAPIResponseCode(SearchAPIResponse m)
        {
            IsSuccess = m.IsSuccess;
            Message = m.Message;
            Data = m.Data;
            ResponseTime = m.ResponseTime;
            ErrorException = m.ErrorException;
        }

        /// <summary>
        /// HttpStatusCode 狀態碼
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
    #endregion

}
