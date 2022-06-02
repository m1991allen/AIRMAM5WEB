import { SelectListItem } from '../../Interface/Shared/ISelectListItem';

/** */
export interface MainSubCodeListModel {
    /**主代碼編號 */
    MainCodeId: string;
    /**主代碼名稱 */
    MainCodeName: string;
    /**子代碼列表 */
    SubCodeList: Array<SelectListItem>;
}