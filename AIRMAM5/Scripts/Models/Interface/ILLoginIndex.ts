import { IDate, CreateInfoModel, UpdateInfoModel, InfoModel } from './Shared/IDate';

/**登入登出查詢 */
export interface LLoginSearchModel extends IDate {
    /**登入的系統帳號 */
    UserId: string;
}

/**登入登出紀錄列表 */
export interface LLoginListModel extends CreateInfoModel, UpdateInfoModel, InfoModel {
    /**帳號Id */
    fnLOGIN_ID: number;
    /**帳號名稱(使用者姓名) */
    fsLOGIN_NAME: string;
    /**帳號 */
    fsLOGIN_ID: string;
    /**登入時間 */
    fdSTIME: string;
    /**登出時間 */
    fdETIME: string;
    /**登入使用時間 */
    UsageTime: string;
    /**備註 */
    fsNOTE: string;
}
