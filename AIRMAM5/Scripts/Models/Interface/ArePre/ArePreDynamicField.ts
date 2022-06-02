/**動態欄位 (直接繼承動態欄位的共用modal)*/
export interface ArePreDynamicField {
    /**樣板Id */
    fnTEMP_ID: number;
    /** fsFIELD (樣板)欄位代號, EX: fsATTRIBUTE1, fsATTRIBUTE2, .... */
    Field: string;
    /**欄位名稱 */
    FieldName: string;
    /**欄位值 */
    FieldValue: string;
}
