import { LicenseBaseModel } from "./LicenseBaseModel";

/**版權 - 新增 */
export interface LicenseCreateModel extends LicenseBaseModel{
    /**排序 */
    Order: number;
}