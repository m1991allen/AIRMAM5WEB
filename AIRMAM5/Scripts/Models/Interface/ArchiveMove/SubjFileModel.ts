import { MediaType } from '../../Enum/MediaType';

/**主題下的單一檔案資訊 */
export interface SubjFileModel {
    /**分類: A聲音, D文件, P圖片, S主題, V影片 */
    FileType: MediaType;
    /**檔案編號 */
    FileNo: string;
    /** 檔案標題  */
    FileTitle: string;
    /** 預覽圖(URL) */
    ImageUrl: string;
    /**樣版編號 */
    TempId: number;
}
