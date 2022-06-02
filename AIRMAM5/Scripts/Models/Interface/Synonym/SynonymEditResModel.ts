import { InfoModel, UpdateInfoModel, CreateInfoModel } from '../Shared/IDate';
import { SynonymEditModel } from './SynonymEditModel';
/**同義詞編輯後回應Record格式 */
export interface SynonymEditResModel extends SynonymEditModel, InfoModel, UpdateInfoModel, CreateInfoModel {
    fsTYPE_NAME: null;
}
