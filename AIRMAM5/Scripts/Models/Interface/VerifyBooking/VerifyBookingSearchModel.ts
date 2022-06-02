import { IDate2 } from '../Shared/IDate';
import { VerifyStatus } from '../../Enum/VerifyStatus';

/**審核調用搜尋條件 */
export interface VerifyBookingSearchModel extends IDate2 {
    /**調用者Id */
    UserId: string;
    /**審核狀態 */
    ApproveStatus: '' | '*' | VerifyStatus;
}
