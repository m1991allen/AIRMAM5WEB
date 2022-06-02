import { CodeIdModel } from './CodeIdModel';
import { UpdateInfoModel, CreateInfoModel, InfoModel } from '../Shared/IDate';
import { YesNo } from '../../Enum/BooleanEnum';

/**代碼回應資訊 */
export interface CodoResponseModel extends CodeIdModel, UpdateInfoModel, CreateInfoModel, InfoModel {
    /*代碼標題*/
    fsTITLE: string;
    /**????? */
    fsTBCOL: string;
    /**???? */
    C_sIS_ENABLED: YesNo;
    /**?????? */
    C_nCNT_CODE: number;
    /**代碼備註 */
    fsNOTE: string;
    /**是否啟用 */
    fsIS_ENABLED: YesNo;
    /**代碼類別(S=系統代碼,C=自定義代碼) */
    fsTYPE: 'S' | 'C';
}
