import { RuleCategoryModel } from './RuleCategoryModel';

/**
 * 編輯規則
 */
export interface EditRuleModel extends RuleCategoryModel {
    /**規則名 */
    RuleName: string;
    /**優先序 */
    //Priority: number;
    /**是否啟用 */
    IsEnabled: boolean;
    /**備註 */
    Note: string;
}
