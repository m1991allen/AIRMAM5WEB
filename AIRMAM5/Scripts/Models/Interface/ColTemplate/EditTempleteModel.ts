import { ColTemplateSearchModel } from './ColTemplateSearchModel';

/**編輯樣版 */
export interface EditTempleteModel extends ColTemplateSearchModel {
    /**樣板名稱 */
    fsNAME: string;
    /**樣板描述 */
    fsDESCRIPTION: string;
    /**進階搜尋 */
    IsSearch: boolean;
    /**是否進階查詢 */
    fcIS_SEARCH: 'Y' | 'N' | '';
}
