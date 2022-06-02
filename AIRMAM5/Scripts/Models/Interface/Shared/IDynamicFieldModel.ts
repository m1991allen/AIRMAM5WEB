/** (動態欄位)共用model */
export interface IDynamicFieldModel {
    /**樣板Id */
    fnTEMP_ID: number; //TemplateId:number;
    /** (樣板)欄位代號, EX: fsATTRIBUTE1, fsATTRIBUTE2, .... */
    Field: string;
    /** 欄位名稱 */
    FieldName: string;
    /** 欄位型別 (tbzCODE.TEMP002) */
    FieldType: string;
    /**欄位描述 */
    FieldDesc: string;
    /**欄位長度 */
    FieldLen: number;
    /**顯示順序 */
    FieldOrder: number;
    /**內容上限 */
    FieldWidth: number;
    /**是否多行 */
    IsMultiline: boolean;
    /**可為空值 */
    IsNullable: boolean;
    /**預設值或存入的實際值 ,多個值以分號區隔*/
    FieldDef: string;
    /**代碼編號 */
    FieldCodeId: string;
    /**單選或多選 (0:多選、1:單選) */
    FieldCodeCnt: number;
    /**控制項類型 */
    FieldCodeCtrl: string;
    /**	是否要列為進階搜尋 */
    IsSearch: boolean;
    /**欄位值(若為下拉則是顯示中文) */
    FieldValue: string;
}
