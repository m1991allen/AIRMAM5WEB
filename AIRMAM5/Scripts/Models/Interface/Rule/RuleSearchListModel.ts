import { RuleTableModel, RuleTableInheritModel } from './RuleTableModel';
import { RuleListFilterModel } from './RuleListFilterModel';
import { RuleEnabledStyle } from '../../Enum/RuleEnableStyle';

/**
 * 規則庫查詢結果列表
 */
export interface RuleSearchListModel extends RuleTableModel {
    /**子篩選條件 */
    RuleFilters: Array<RuleListFilterModel>;
}

/**前端轉換的規則庫列表結果 */
export interface RuleFrontEndListModel extends RuleListFilterModel, RuleTableInheritModel {
    /**前端用於確認是否為空Model */
    IsNullModel: boolean;
    /**
     * 前端自定義索引(因為目前資料的索引由多個欄位組成)
     * 索引:RuleCategory_RuleTable_RuleColumn
     */
    RuleIndex: string;
    /**規則啟用樣式 */
    EnabledStyle: RuleEnabledStyle;
}
