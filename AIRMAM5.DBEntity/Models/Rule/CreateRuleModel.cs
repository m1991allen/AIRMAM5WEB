using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 新建規則(主表+篩選條件表) POST
    /// </summary>
    public class CreateRuleModel
    {
        /// <summary>
        /// 規則定義表(tbmRULE) MODEL
        /// </summary>
        public EditRuleModel RuleMaster { get; set; }

        /// <summary>
        /// 規則定義條件(tbmRULE_FILTER) MODEL
        /// </summary>
        public List<EditRuleFilterModel> Filters { get; set; }
    }

}
