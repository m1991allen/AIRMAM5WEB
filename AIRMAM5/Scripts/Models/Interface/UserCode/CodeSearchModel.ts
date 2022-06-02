import { CodeIdModel } from './CodeIdModel';
import { YesNo } from '../../Enum/BooleanEnum';

/**主檔代碼列表 */
export interface CodeSearchModel extends CodeIdModel {
    /**主代碼名稱 */
    fsTITLE: string;
    /**代碼數量 */
    C_nCNT_CODE: number;
    /**備註 */
    fsNOTE: string;
    /**是否啟用 */
    fsIS_ENABLED: YesNo;
}
