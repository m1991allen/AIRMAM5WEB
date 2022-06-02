/**顯示Table */
export interface ColTemplateTableModel {
    /**使用欄位 */
    fsFIELD: string;
    /**排序 */
    fnORDER: number;
    /**欄位名稱 */
    fsFIELD_NAME: string;
    /**內容型別*/
    fsFIELD_TYPE: string;
    /**是否為空值(必要) */
    IsNullable: boolean;
    /**內容上限 */
    fnFIELD_LENGTH: number;
    /**預設值 */
    fsDEFAULT: string;
    /**進階檢索 */
    IsSearch: boolean;
}
