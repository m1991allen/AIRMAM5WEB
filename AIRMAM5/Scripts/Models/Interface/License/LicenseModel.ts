
import { CreateInfoModel, UpdateInfoModel } from '../Shared/IDate';
import { LicenseBaseModel } from './LicenseBaseModel';

/**版權資料 */
export interface LicenseModel extends LicenseBaseModel, CreateInfoModel, UpdateInfoModel{

}
