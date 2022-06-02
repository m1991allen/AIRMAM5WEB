import { MediaType } from '../../Enum/MediaType';

/** 符合與檔案相同V or A or P or D樣板的節點目錄
 * 
 * 因應"目錄節點Queue"是否啟用需求, 目標目錄節點的主題列表要呼叫另一支預存
*/
export interface GetDirAndSubjectsByDirFilter{
    /** 系統目錄編號 */
    DirId: number;
    /**分類: A聲音, D文件, P圖片, S主題, V影片 */
    FileType: MediaType;
    /**檔案編號 */
    FileNo: string;
}