import { IshortDate } from '../Shared/IDate';
import { AnnType } from '../../Enum/AnnEnum';

/**公告維護查詢 */
export interface AnnSearchModel extends IshortDate {
    /**公告分類 */
    type: AnnType | '';
    /**"發佈單位Id"*/
    dept: string;
}
