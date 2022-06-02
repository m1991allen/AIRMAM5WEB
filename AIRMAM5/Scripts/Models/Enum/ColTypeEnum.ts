/**
 * 自訂欄位 資料型態
 */
export enum ColFieldType {
    文字 = 'NVARCHAR',
    數字 = 'INTEGER',
    日期 = 'DATETIME',
    自訂代碼 = 'CODE',
}
/*自訂欄位 英文資料型態* */
export enum ColFieldCodeType{
   NVARCHAR= 'NVARCHAR',
   INTEGER='INTEGER',
   DATETIME='DATETIME',
   CODE= 'CODE'
}
/**
 * 自訂欄位 選擇型態
 */
export enum ColFieldSelectType {
    /**多選 */
    Mutiple = 0,
    /**單選 */
    Single = 1,
}
