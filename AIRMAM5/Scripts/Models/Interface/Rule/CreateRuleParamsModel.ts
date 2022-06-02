import { FilterTableModel } from './FilterTableModel';
import { SelectListItem } from '../Shared/ISelectListItem';

/**流程相關下拉選單資訊*/
export interface CreateRuleParamsModel {
    /**規則類別: 參考代碼"RULE", 調用 BOOKING、入庫 UPLOAD、轉檔 TRANSCODE */
    RuleCategory: string;
    /**規則名稱 */
    RuleName: string;
    /** 套用規則表/ 規則資料表 */
    RuleTable: string;
    /** 備註 */
    Note: string;
    /**套用規則表 */
    TableList: Array<FilterTableModel>;
    /**規則類別 下拉清單 */
    RuleCategoryList: Array<SelectListItem>;
}
