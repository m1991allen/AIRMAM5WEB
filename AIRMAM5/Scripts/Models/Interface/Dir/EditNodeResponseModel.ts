import { CreateInfoModel, UpdateInfoModel, InfoModel } from '../Shared/IDate';
import { CreateNodeModel } from './CreateNodeModel';

/**編輯節點回應資訊 */
export interface EditNodeResponseModel extends CreateInfoModel, UpdateInfoModel, InfoModel, CreateNodeModel {
    /**主題樣板名稱 */
    C_SUBJECT_NAME: string;
    /**影片樣板名稱*/
    C_VIDEO_NAME: string;
    /**聲音樣板名稱*/
    C_AUDIO_NAME: string;
    /**圖片樣板名稱*/
    C_PHOTO_NAME: string;
    /**文件樣板名稱*/
    C_DOC_NAME: string;
    /**群組清單名稱*/
    C_sGROUP_NAME_LIST: string;
    /**使用者清單名稱 */
    C_sUSER_NAME_LIST: string;
    /**目錄路徑 */
    C_sDIR_PATH: string;
}
