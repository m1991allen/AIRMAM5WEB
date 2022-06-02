import { SelectListItem } from '../Shared/ISelectListItem';

/** 規則資料表欄位 資訊 */
export interface FieldInfo {
    /**欄位名 ex: fsFILE_NO */
    Column;
    /**欄位說明 */
    Desc: string;
    /**資料型別 */
    Type: string;
    /**資料來源是否為代碼(選單) */
    IsCode: boolean;
    /**代碼選單資料 */
    ListItem: Array<SelectListItem>;
    /** 代碼選單是否為複選 */
    IsMultiple: boolean;
    /**規則資料表欄位可選用的運算子選單 */
    ListOperator: Array<SelectListItem>;
}
