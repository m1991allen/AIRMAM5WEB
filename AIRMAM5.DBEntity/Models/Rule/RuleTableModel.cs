
namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 規則資料表Model (Master)
    /// </summary>
    public class RuleTableModel
    {
        /// <summary>
        /// 流程類別
        /// </summary>
        public string RuleCategory { get; set; } = string.Empty;

        /// <summary>
        /// 流程規則名稱
        /// </summary>
        public string RuleName { get; set; } = string.Empty;

        /// <summary>
        /// 流程規則啟用否?
        /// </summary>
        public bool RuleEnabled { get; set; }

        /// <summary>
        /// 流程規則資料表
        /// </summary>
        public string RuleTable { get; set; } = string.Empty;

        /// <summary>
        /// 流程規則資料表-中文
        /// </summary>
        public string TableName { get; set; } = string.Empty;
    }

}
