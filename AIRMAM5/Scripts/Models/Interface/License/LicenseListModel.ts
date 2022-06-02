import { LicenseCodeModel } from "Models/Interface/License/LicenseIdModel";

/**版權資料列表 */
export interface LicenseListModel extends LicenseCodeModel{
    
    /**版權名稱 */
    LicenseName: string;
    // /**版權備註 */
    // LicenseDesc: string;
    /**授權到期日期, 預設無日期 */
    EndDate: string;
    /**提醒訊息內容 */
    AlertMessage: string;
    /**是否調用提醒 */
    IsBookingAlert: boolean;
    /**是否調用禁止 */
    IsNotBooking: boolean;
    /**是否啟用 */
    IsActive: boolean;
}