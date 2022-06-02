/**自訂欄位樣板維護查詢 */
export interface ColTemplateSearchModel {
    /**樣板Id */
    fnTEMP_ID: number;
    /**樣板分類:提供使用的目的資料表 (代碼: fsCODE_ID='TEMP001') */
    fsTABLE: string;
}
