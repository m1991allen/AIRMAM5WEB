import { SelectListItem } from '../Shared/ISelectListItem';

/**自訂代碼主選單與子選單 */
export interface MainCodeListModel {
    MainCodeId: string;
    MainCodeName: string;
    SubCodeList: Array<SelectListItem>;
}
