/**繼承用:欄位資訊 */
interface templateField {
    /**欄位名稱 */
    FieldName: string;
    /**使用欄位 */
    Field: string;
    /**內容型別 */
    FieldType: string;
    /**欄位描述 */
    FieldDesc: string;
    /**欄位長度 */
    FieldLen: number;
    /**內容上限 */
    FieldWidth: number;
    /**是否多行 */
    IsMultiline: boolean;
    /**可為空值 */
    IsNullable: boolean;
    /**預設值 */
    FieldDef: string;
    /**欄位排序 */
    FieldOrder: number;
    /**可進階搜尋 */
    IsSearch: boolean;
    /** 0:多選、1:單選 */
    FieldCodeCnt: number;
}
