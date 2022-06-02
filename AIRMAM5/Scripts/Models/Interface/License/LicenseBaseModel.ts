import { LicenseCodeModel } from "./LicenseIdModel";

/**版權主要資料欄位 */
export interface LicenseBaseModel extends LicenseCodeModel{
    
    /**版權名稱 */
    LicenseName: string;
    /**版權備註 */
    LicenseDesc: string;
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