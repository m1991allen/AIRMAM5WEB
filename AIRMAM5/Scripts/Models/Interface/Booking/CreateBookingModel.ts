import { BookingSaveBaseModel } from './BookingSaveBaseModel';

/** 調用紀錄新增 */
export interface CreateBookingModel extends BookingSaveBaseModel {
    /**調用編號s (多筆以"^"為分隔符號) */
    MaterialIds?: string;
}
