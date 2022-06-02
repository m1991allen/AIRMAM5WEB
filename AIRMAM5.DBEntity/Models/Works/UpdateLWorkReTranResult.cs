using AIRMAM5.DBEntity.Models.Shared;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Works
{
    /// <summary>
    /// (整批)重新轉檔/取消調用 結果資訊
    /// </summary>
    public class UpdateLWorkReTranResult : VerifyResult
    {
        /// <summary>
        /// (整批)重新轉檔 結果資訊
        /// </summary>
        public UpdateLWorkReTranResult() { }

        /// <summary>
        /// (整批)重新轉檔 結果資訊
        /// </summary>
        /// <param name="b"></param>
        /// <param name="m"></param>
        public UpdateLWorkReTranResult(bool b, string m) { IsSuccess = b; Message = m; }

        /// <summary>
        /// Procedure : OK/已處理
        /// </summary>
        public List<string> Processed { get; set; } = new List<string>();

        /// <summary>
        /// Procedure : 未處理無錯誤...(fsSTATUS=1~8不能轉檔)
        /// </summary>
        public List<string> UnProcessed { get; set; } = new List<string>();

        /// <summary>
        /// Procedure 回覆 Error字樣.
        /// </summary>
        public List<string> Failure { get; set; } = new List<string>();
    }

}
