import { SelectListItem } from '../Shared/ISelectListItem';
/**新增同義詞 */
export interface SynonymCreateModel {
    /**同義詞字串(以;分隔) */
    fsTEXT_LIST: string;
    /**分類 */
    fsTYPE: string;
    /**備註 */
    fsNOTE: string;
    /** */
    SynonymTypeList: Array<SelectListItem>;
}
