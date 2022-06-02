import { SelectListItem } from '../Shared/ISelectListItem';
import { YesNo } from '../../Enum/BooleanEnum';

/**公告維護基本Model */
export interface AnnMatainBaseModel {
    /**"公告標題"*/
    fsTITLE: string;
    /**"公告內容"*/
    fsCONTENT: string;
    /**"上架日期"*/
    fdSDATE: string;
    /**"下架日期"*/
    fdEDATE: string;
    /**"公告分類"*/
    fsTYPE: string;
    /**排序 */
    fnORDER: number;
    /**公告群組 */
    GroupList: Array<string>;
    /**是否隱藏 */
    fsIS_HIDDEN: YesNo;
    /**發佈單位 */
    fsDEPT: string;
    /**備註" */
    fsNOTE: string;
    /** 發佈單位 選單 */
    AnnDeptList?: Array<SelectListItem>;
    /**公告分類 選單 */
    AnnTypeList?: Array<SelectListItem>;
    /**隱藏選單 */
    HiddenList?: Array<SelectListItem>;
    /**公告群組選單 */
    GroupListItems?: Array<SelectListItem>;
}
