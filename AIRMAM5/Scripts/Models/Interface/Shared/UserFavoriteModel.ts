import { Controller } from '../../Enum/Controller';
import { Action } from '../../Enum/Action';

/**我的最愛 */
export interface UserFavoriteModel {
    /**控制器名稱 */
    ControllerName: Controller;
    /**動作名稱 */
    ActionName: Action;
    /**我的最愛URL */
    FavoriteUrl: string;
    /**功能項目(db)流水號 */
    FuncId: string;
    /**功能項目名稱 */
    FunctionName: string;
    /**功能項目圖示 */
    Icon: string;
}
