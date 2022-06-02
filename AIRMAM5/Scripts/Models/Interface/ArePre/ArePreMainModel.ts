/**查詢結果table column */
export interface ArePreMainModel {
    /**預編Id */
    fnPRE_ID: number; //ArcPreId:number;
    /**預編名稱 */
    fsNAME: string; //ArcPreName:string;
    /**類型  */
    fsTYPE: string; //ArcPreType:string;
    /**類型名稱 */
    fsTYPE_NAME: string; //ArcPreTypeName:string;
    /**樣板 */
    fnTEMP_ID: number; //ArcPreTempId:number;
    /**樣板名稱 */
    fsTEMP_NAME: string; //ArcPreTempName:string;
    /**標題 */
    fsTITLE: string; //ArcPreTitle:string;
    /**描述 */
    fsDESCRIPTION: string; //Description:string;
    /**自訂標籤, ★資料欄位為字串型態, ^為分隔符號。#符號剔除不存入資料欄位。 */
    fsHashTag: string;
    /**自訂標籤 陣列 */
    HashTag: Array<string>;
}
