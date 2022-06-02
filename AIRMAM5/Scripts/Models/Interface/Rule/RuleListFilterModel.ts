import { RuleCategoryModel } from './RuleCategoryModel';

/**
 *  規則資料表欄位條件(篩選)設定
 */
export interface RuleListFilterModel extends RuleCategoryModel {
    /**規則資料表 */
    RuleTable: string;
    /**資料表欄位 */
    RuleColumn: string;
    /** 欄位名稱 */
    ColumnName: string;
    /** 運算子: 0包含 Include、1不包含 Exclude、等於 Equal、2介於 Beetween、.... */
    Operator: string;
    /**運算子顯示名稱 */
    OperatorStr: string;
    /**篩選值: 分號(;)為分隔符號 */
    FilterValue: string;
    /**優先序 */
    Priority: number;
    /**是否啟用 */
    IsEnabled: boolean;
    /** 條件邏輯: AND, OR */
    WhereClause: 'And' | 'OR' | string;
    /**該規則索引(由category_table_column組成) */
    FilterKey: string;
}
