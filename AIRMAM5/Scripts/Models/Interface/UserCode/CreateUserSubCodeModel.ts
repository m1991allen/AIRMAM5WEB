import { CodeIdModel } from './CodeIdModel';
import { CodeNoteModel } from './CodeNoteModel';
import { YesNo } from '../../Enum/BooleanEnum';

/**新增自定義子代碼 */
export interface CreateUserSubCodeModel extends CodeIdModel, CodeNoteModel {
    /**欄位名稱 */
    fsNAME: string;
    /**欄位英文名稱 */
    fsENAME: string;
    /**欄位排序 */
    fnORDER: number;
    /**是否啟用 */
    fsIS_ENABLED: YesNo;
    /**設定 */
    fsSET: string;
    /**子代碼定義編號 */
    fsCODE: string;
}
