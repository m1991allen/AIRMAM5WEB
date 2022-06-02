interface SubjectWindowOption{
}
interface SubjectWindowConfig{
  config:SubjectWindowOption;
}
interface Subjectwindow{
/**取後端給的查詢因子下拉選單 */
GetDropdown:(type:any)=>void;
 /**取得前端所有輸入的查詢因子 */
 CurrentFactors():Array<object>;
 /**新稱條件因子 */
 AddFactor(item:object):boolean;
 /**移除條件因子*/
 RemoveFactor:(removeKey:string,value:any)=>void;
 /**清空所有條件因子 */
 ClearFactor():Array<object>;
  /**重新設置table的資料 */
  SearchTable(rowData:any,input:{subjectId:string;mediatype:string;type:number;},items:Array<any>):void;
 /**清空table */
 ClearTble():void;
}

interface JQueryStatic{
    subjectwindow(options:SubjectWindowOption):SubjectWindowConfig & Subjectwindow;
}