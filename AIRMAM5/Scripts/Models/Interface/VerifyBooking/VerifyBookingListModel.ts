import { MediaType } from '../../Enum/MediaType';

export interface VerifyBookingListModel {
    // /**審核Id */
    // VerifyId: number;
    /**轉檔工作ID */
    WorkId: number;
    /**調用編號 */
    BookingId: string;
    /**調用日期 */
    BookingDate: string;
    /**調用者顯示名稱 // 使用者帳號 */
    BookingLoginId: string;//LoginId: string;
    /**調用者顯示名稱 // 使用者名稱 */ 
    BookingUserName: string;//Name: string;
    /**審核狀態(代碼 WORK_APPROVE) // 調用狀態 */
    ApproveStatus: string;//VerifyStatus: string;//VerifyStatus: number;
    /**審核狀態(中文) // 調用狀態顯示文字 */
    ApproveStatusStr: string;//VerifyStatusStr: string;
    /**媒體類型: V,A,P,D */
    MediaType: MediaType;
    /**調用原因 */
    Reason: string;
    /**預借檔案標題 */
    Title: string;
    /**剪輯起始時間 */
    MarkInTimeStr: string;//MarkInTime: string;
    /**剪輯結束時間 */
    MarkOutTimeStr: string;//MarkOutTime: string;
    /**審核者 */
    ConfirmLoginId: string; //ConfirmLoginId: string;
    /**審核日期 */
    ConfirmTime: string;//ConfirmTime: string;
    /**審核結果備註 */
    ApproveMemo:string;
}
