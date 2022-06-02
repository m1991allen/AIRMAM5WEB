using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 列表資料定義 Model。　繼承參考<see cref="RuleTableModel"/>
    /// </summary>
    public class RuleListModel : RuleTableModel
    {
        /// <summary>
        ///  列表資料定義
        /// </summary>
        public RuleListModel()
        {
            this.RuleFilters = new List<RuleListFilterModel>();
        }

        /// <summary>
        /// 規則資料表篩選條件list
        /// </summary>
        public List<RuleListFilterModel> RuleFilters { get; set; }

        /// <summary>
        /// 資料格式轉換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public RuleListModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            this.RuleFilters = new List<RuleListFilterModel>();

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                var val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsRULECATEGORY") { RuleCategory = val.ToString(); }
                if (pp.Name == "fsRULENAME") { RuleName = val.ToString(); }
                if (pp.Name == "fsTABLE") { RuleTable = val.ToString(); }
                if (pp.Name == "fsTABLE_NAME") { TableName = val.ToString(); }

                if (pp.Name == "fbISENABLED")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.RuleEnabled = chk; }
                }
            }

            return this;
        }
    }

}
