import { IDate } from './Shared/IDate';

/**檢索紀錄查詢 */
export interface LSearchSearchModel extends IDate {
    /**登入的系統帳號 */
    UserId: string;
}
/**檢索紀錄列表 */
export interface LSearchListModel {
    /**檢索Id */
    fnSRH_ID: number;
    /**帳號 */
    fsCREATED_BY: string;
    /**查詢日期 */
    fdCREATED_DATE: string;
    /**查詢條件 */
    fsSTATEMENT: string;
}
