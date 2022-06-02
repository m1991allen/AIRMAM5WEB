using AIRMAM5.DBEntity.Models.Enums;

namespace AIRMAM5.DBEntity.Models.Shared
{
    /// <summary>
    /// 產生Serilog 紀錄Model
    /// </summary>
    public class SerilogInputModel
    {
        /// <summary>
        ///Serilog Level 
        /// </summary>
        public SerilogLevelEnum EventLevel { get; set; } = SerilogLevelEnum.Debug;
        /// <summary>
        /// 發生的Controller
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 發生的Function
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 紀錄文字
        /// </summary>
        public string LogString { get; set; } = "";
        /// <summary>
        /// 內容/值
        /// </summary>
        public object Input { get; set; } = new { };
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } = "";
    }
}
