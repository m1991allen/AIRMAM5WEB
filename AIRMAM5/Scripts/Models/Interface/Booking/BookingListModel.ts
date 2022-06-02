import { BookingBaseModel } from './BookingBaseModel ';

/**管理調用狀態列表 */
export interface BookingListModel extends BookingBaseModel {
    /**狀態  */
    WorkStatus: string;
    /**優先順序 */
    Priority: string;
    /**轉檔開始時間 */
    StartTime: string;
    /**轉檔結束時間 */
    EndTime: string;
    /**調用者 */
    CreateBy: string;
}
