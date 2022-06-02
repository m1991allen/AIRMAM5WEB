import { NotifyCategory } from '../../Enum/NotifyCategory';
import { SelectListItem } from '../Shared/ISelectListItem';
/*新增訊息通知* */
export interface NotifyCreateModel {
    /**標題 */
    Title: string;
    /**內容 */
    Content: string;
    /**類別 */
    Category: NotifyCategory;
    /**通知對象(人or群組or部門,測試暫用userid) */
    NoticeTo: string;
}

/**新增訊息通知 */
export interface NotifyCreateRealModel extends NotifyCreateModel {
    /**類別下拉選單 */
    CategoryList: Array<SelectListItem>;
    /**有效日期 */
    ExpireDate: string;
}
