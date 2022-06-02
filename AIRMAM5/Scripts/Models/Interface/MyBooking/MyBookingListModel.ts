import { BookingBaseModel } from '../Booking/BookingBaseModel ';

/**我的調用狀態列表 */
export interface MyBookingListModel extends BookingBaseModel {
    /**檔案類型名稱 */
    ArcTypeName: string;
    /**狀態  */
    WorkStatus: string;
    /** 調用備註 */
    NoteStr: string;
    /**轉檔開始時間 */
    StartTime: '';
    /**轉檔結束時間 */
    EndTime: '';
    /**調用結果 */
    Result: '';
}
