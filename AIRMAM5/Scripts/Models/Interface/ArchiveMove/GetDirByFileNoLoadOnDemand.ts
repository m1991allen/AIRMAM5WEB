import { MediaType } from '../../Enum/MediaType';

/** 符合與檔案相同V or A or P or D樣板的節點目錄 */
export interface GetDirByFileNoLoadOnDemand {
    /** 系統目錄編號 */
    DirId: number;
    /**分類: A聲音, D文件, P圖片, S主題, V影片 */
    FileType: MediaType;
    /**檔案編號 */
    FileNo: string;
    /**系統帳號 */
    UserName: string;
    /**關鍵字 */
    KeyWord: string;
}
