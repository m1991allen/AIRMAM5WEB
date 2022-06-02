import { RuleCategoryModel } from './RuleCategoryModel';
/**子流程啟用或關閉輸入參數 */
export interface RuleFilterActiveModel extends RuleCategoryModel {
    /**規則資料表 */
    Table: string | '*';
    /**條件篩選欄位 */
    Column: string | '*';
    /**是否啟用 */
    IsActive: boolean;
}
