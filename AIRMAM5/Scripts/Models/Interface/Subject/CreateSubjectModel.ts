import { SelectListItem } from '../Shared/ISelectListItem';
import { IDynamicFieldModel } from '../Shared/IDynamicFieldModel';

/**新增主題參數 */
export interface CreateSubjectModel {
    /**樣板Id */
    DirId: number;
    /**系統目錄Id */
    TemplateId: number;
    /**標題 */
    Title: string;
    /**描述 */
    Description: string;
    /**預編詮釋資料樣板Id */
    SubjectPreId: number;
    /**下拉清單: 主題-預編詮釋 */
    ArcPreList?: Array<SelectListItem>;
    /**主題(所在目錄節點)樣版欄位  (變動欄位) */
    ArcPreAttributes?: Array<IDynamicFieldModel>;
}
