import { BookingSaveBaseModel } from '../Booking/BookingSaveBaseModel';

/**批次調用存檔參數 */
export interface BatchBookingCreateModel extends BookingSaveBaseModel {
    /** 媒資檔案編號s [fsFILE_NOs] (多筆以"^"為分隔符號) */
    FileNos: string;
}
