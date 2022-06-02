import { GroupInputModel } from './GroupInputModel';
import { CreateInfoModel, UpdateInfoModel } from '../Shared/IDate';

/**群組資訊 */
export interface GroupModel extends GroupInputModel, CreateInfoModel, UpdateInfoModel {
    /**群組Id */
    fsGROUP_ID: string;

    /**群組類型*/
    fsTYPE: string;
    /**???  Identity 自動產生後的欄位,
     * Tips: dbo.tbmGROUPS.[Discriminator] ='ApplicationRole' / 'IdentityRole'  AppRoleManager 才會讀得到.
     */
    Discriminator: string;
}
