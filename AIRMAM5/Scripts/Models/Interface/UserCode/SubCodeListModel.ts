import { CodeIdModel } from './CodeIdModel';
import { UpdateInfoModel, CreateInfoModel } from '../Shared/IDate';
import { YesNo } from '../../Enum/BooleanEnum';

/**子代碼列表 */
export interface SubCodeListModel extends CodeIdModel, UpdateInfoModel, CreateInfoModel {
    /**子代碼 */
    fsCODE: string;
    /**欄位名稱 */
    fsNAME: string;
    /**欄位英文名稱 */
    fsENAME: string;
    /**欄位排序 */
    fnORDER: number;
    /**代碼備註 */
    fsNOTE: string;
    /**是否啟用 */
    IsEnabled: boolean;
    /**設定 */
    fsSET: string;
    /**是否啟用 */
    fsIS_ENABLED: YesNo;
    /**代碼類別(S=系統代碼,C=自定義代碼) */
    fsTYPE: 'S' | 'C';
}
