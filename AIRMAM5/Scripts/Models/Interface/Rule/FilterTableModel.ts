import { FieldInfo } from './FieldInfo';

export interface FilterTableModel {
    /** 資料表名稱 ex: dbo.tbmGROUPS */
    TableName: string;
    /** 資料表說明 */
    TableDesc: string;
    /**規則資料表欄位屬性清單 */
    Properties: Array<FieldInfo>;
}
