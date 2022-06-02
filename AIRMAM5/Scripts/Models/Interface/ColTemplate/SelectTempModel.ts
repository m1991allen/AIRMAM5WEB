import { TempleteEnum } from '../../Enum/TempleteEnum';

/**選擇新增的樣板種類 */
export interface SelectTempModel {
    /**全新的樣版(NEW/NULL)或既有的樣板(COPY) */
    template: TempleteEnum | null;
    /**樣板Id,如果是NEW就傳0 */
    existtemplate: number;
    /**樣板名稱 */
    fsNAME: string;
    /**樣板分類 */
    fsTABLE: string;
    /**樣版描述 */
    fsDESCRIPTION: string;
    /**進階搜尋 */
    IsSearch: boolean;
}
