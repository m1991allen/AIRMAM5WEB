import { RuleCategoryModel } from './RuleCategoryModel';
/**規則表底層 */
export interface RuleTableInheritModel extends RuleCategoryModel {
    /**主規則名稱 */
    RuleName: string;
    /**主規則啟用 */
    RuleEnabled: boolean;
    /**主規則表名稱 */
    RuleTable: string;
    /**主規則表名稱-中文 */
    TableName: string;
}

/**規則表 */
export interface RuleTableModel extends RuleCategoryModel, RuleTableInheritModel {}
