import { RuleCategoryModel } from './RuleCategoryModel';

/**
 *  編輯 規則定義條件
 */
export interface EditRuleFilterModel extends RuleCategoryModel {
    /**目標資料表 */
    TargetTable: string;
    /**篩選欄位 */
    FilterField: string;
    /**運算子: 0包含 include、1不包含 exclude、2等於 equal、3介於 BETWEEN、.... */
    Operator: string;
    /** 篩選值: 分號(;)為分隔符號 */
    FilterValue: string;
    /**資料型別 */
    FieldType: string;
    /** 優先序 */
    Priority: number;
    /** 是否啟用 */
    IsEnabled: boolean;
    /**備註 */
    Note: string;
    /** 條件邏輯: AND, OR */
    WhereClause: 'AND' | 'OR';
}
