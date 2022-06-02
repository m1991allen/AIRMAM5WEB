import { MainSubCodeListModel } from '../../Interface/UserCode/MainSubCodeListModel';
interface TemplateBaseFieldModel {
    /**(樣板)欄位代號 */
    Field: string;
    /**欄位名稱 */
    FieldName: string;
    /**欄位型別 */
    FieldType: string;
    /**欄位描述 */
    FieldDesc: string;
    /**欄位長度 */
    FieldLen: number;
    /**顯示順序 */
    FieldOrder: number;
    /**欄位控制項寬度(內容上限) */
    FieldWidth: number;
    /**是否多行 */
    IsMultiline: boolean;
    /**是否可為NULL */
    IsNullable: boolean;
    /**欄位的實際儲存代碼或值(在載入時會當預設值使用) */
    FieldDef: string;
    /**單選或多選 (0:多選、1:單選) */
    FieldCodeCnt: number;
    /**是否要列為進階搜尋 */
    IsSearch: boolean;
}

/**自訂欄位 Add Field */
export interface TemplateFieldModel extends TemplateBaseFieldModel {
    /**代碼編號 */
    FieldCodeId: string;
    /**控制項類型(目前不使用) */
    FieldCodeCtrl: string;
}

/**繼承用:欄位資訊 */
export interface TemplateFieldRowModel extends TemplateBaseFieldModel {
    /**控制項類型(目前不使用) */
    FieldCodeCtrl: string;
    /**欄位代碼 */
    FieldCodeId: string;
    /**欄位型別 */
    FieldTypeEnum: number;
    /**欄位型別顯示文字 */
    FieldTypeName: string;
    /**樣板Id */
    fnTEMP_ID: number;
    /** */
    CustomerCodeList: Array<MainSubCodeListModel>;
}
