import { EditRuleFilterModel } from './EditRuleFilterModel';
import { EditRuleModel } from './EditRuleModel';

/**
 * 新增規則表
 */
export interface CreateRuleModel {
    /**規則定義表 */
    RuleMaster: EditRuleModel;
    /** 規則定義條件 */
    Filters: Array<EditRuleFilterModel>;
}
