import { GroupModel } from './GroupModel';

/**新增回傳資訊 */
export interface GroupDataModel extends GroupModel {
    /**使用者群組 */
    tbmUSER_GROUP: string[];
}
