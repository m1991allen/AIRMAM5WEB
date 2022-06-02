import { IDate2 } from '../Shared/IDate';
/**
 * 刪除查詢參數
 */
export interface DeleteSearchModel extends IDate2 {
    /**媒體類別(影音圖文) */
    Type: string;
    /**狀態 */
    Status: string;
}
