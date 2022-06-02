import { IDate } from './Shared/IDate';

export interface LLogSearchModel extends IDate {
    /**登入的系統帳號 */
    UserId: string;
}
/**操作紀錄列表 */
export interface LLogListModel {
    /**登入編號 */
    fnlLOG_ID: number;
    /**類別 */
    fsTYPE: string;
    /**群組 */
    fsGROUP: string;
    /**創建帳號 */
    fsCREATED_BY: string;
    /**描述 */
    fsDESCRIPTION: string;
    /**建立時間 */
    fdCREATED_DATE: string;
}
