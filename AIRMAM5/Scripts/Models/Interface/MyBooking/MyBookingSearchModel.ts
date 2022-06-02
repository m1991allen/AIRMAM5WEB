import { IDate2 } from '../Shared/IDate';

/**查詢我的調用狀態 */
export interface MyBookingSearchModel extends IDate2 {
    /**工作狀態 */
    WorkStatus: string;
    /**工作Id */
    WorkId: number;
    /**登入帳號 */
    LoginId: string;
}
