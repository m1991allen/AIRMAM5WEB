
/**每一行資料所需的主鍵資訊 */
export interface DraftDataItem{
     /**資料主鍵欄位名稱,可能為多個 */
     PKeyCol:Array<string>;
     /**資料主鍵數值,會為多個,順序依造PkeyCol順序 */
     PKeyVal:Array<string>;
}
/**
 * 前端controller parser的第三方系統搜尋結果
 */
export interface DraftResultModel{
     /**資料主鍵欄位名稱,可能為多個 */
     PKeyCol:Array<string>;
    /**資料欄位標題(動態json key) */
    DataTitle:object;
    /** 資料內容(動態json key) */
    DataList:Array<object & DraftDataItem>;
}

/**
 * 後端實際上給的第三方搜尋結果
 */
export interface DraftOriginalResultModel{
      /**資料主鍵欄位,可能為多個以;分隔符號 */
      PKeyCol:string;
      /**資料欄位標題(動態json key) */
      DataTitle:object;
      /** 資料內容(動態json key) */
      DataList:Array<object>;
}

