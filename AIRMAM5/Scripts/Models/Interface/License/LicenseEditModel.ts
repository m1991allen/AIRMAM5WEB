import { LicenseBaseModel } from "./LicenseBaseModel";

/**版權 - 編輯 */
export interface LicenseEditModel extends LicenseBaseModel{
    /**排序 */
    Order: number;
}
