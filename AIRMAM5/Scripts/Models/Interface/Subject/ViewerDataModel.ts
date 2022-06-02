import { MediaType } from '../../Enum/MediaType';

/**檔案預覽圖資訊 */
export interface ViewerDataModel {
    /**檔案路徑 */
    FileUrl: string;
    /**圖片路徑 */
    ImageUrl: string;
    /**主題檔案位置,路徑 */
    SubjectPath: string;
    /**檔案編號 */
    fsFILE_NO: string;
    /**檔案類型 */
    FileType: 'A' | 'D' | 'S' | 'V' | 'P' | MediaType;
}
