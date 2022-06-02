import { ArePreMainModel } from './ArePreMainModel';
import { ArePreDynamicField } from './ArePreDynamicField';

/**編輯參數 */
export interface ArePreEditInputModel extends ArePreMainModel {
    /**動態欄位群組 */
    ArcPreAttributes: Array<ArePreDynamicField>;
}
