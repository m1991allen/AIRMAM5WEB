import { MediaType } from '../../Enum/MediaType';

/**刪除指定檔案的媒體資料參數 */
export interface DeleteMediaModel {
    /**檔案編號 */
    FileNo: string;
    /** 媒體檔案分類: V, A, P, D */
    FileCategory: MediaType;
    /**刪除原因 */
    Reason: string;
}
