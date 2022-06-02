/**同義詞編輯參數 */
export interface SynonymEditModel {
    /**同義詞Id */
    fnINDEX_ID: number;
    /**同義詞詞彙(用;符號join) */
    fsTEXT_LIST: string;
    /**備註 */
    fsNOTE: string;
    /**同義詞類別 */
    fsTYPE: string;
    /**原始資料:同義詞詞彙 */
    OrigSynonyms: string;
}
