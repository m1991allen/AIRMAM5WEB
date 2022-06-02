import { MediaType } from '../../Enum/MediaType';

export interface BatchDeleteResponseModel {
    /**調用Id */
    fnMATERIAL_ID: number;
    /**標記帳號 */
    fsMARKED_BY: string;
    /**媒體類型 */
    fsTYPE: MediaType;
    /**檔案編號 */
    fsFILE_NO: string;
    /**描述 */
    fsDESCRIPTION: string;
}
