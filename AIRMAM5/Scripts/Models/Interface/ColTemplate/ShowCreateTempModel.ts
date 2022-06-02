import { ColTemplateSearchModel } from './ColTemplateSearchModel';

/**新增樣版顯示Modal所需欄位 */
export interface ShowCreateTempModel extends ColTemplateSearchModel {
    /**樣板名稱 */
    fsNAME: string;
    /**樣版描述 */
    fsDESCRIPTION: string;
    /**是否進階查詢 */
    IsSearch: boolean;
}
