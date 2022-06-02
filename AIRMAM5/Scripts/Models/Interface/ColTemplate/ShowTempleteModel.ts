import { ColTemplateSearchModel } from './ColTemplateSearchModel';

/**樣板查詢結果 */
export interface ShowTempleteModel extends ColTemplateSearchModel {
    //,templateFieldTime{
    /**樣板Id */
    fnTEMP_ID: number;
    /**樣板分類:提供使用的目的資料表 */
    fsTABLE: string;
    /**樣板名稱 */
    fsNAME: string;
    /**樣版分類 */
    C_sTABLENAME: string;
    /**樣版描述 */
    fsDESCRIPTION: string;
    /**是否進階查詢 */
    fcIS_SEARCH: string;
}
