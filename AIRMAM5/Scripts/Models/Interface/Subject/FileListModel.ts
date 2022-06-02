import { WorkStatus } from '../../Enum/WorkStatus';

/**檔案列表 */
export interface FileListModel {
    /**檔案編號 */
    fsFILE_NO: string;
    /**圖片Url */
    ImageUrl: string;
    /**檔案Url */
    FileUrl: string;
    /**關鍵影格數量 */
    KeyFrameCount: number;
    /**段落描述數量 */
    SegmentCount: number;
    /**標題 */
    Title: string;
    /**是否可置換 */
    IsChange: boolean;
    /**檔案橫向H/直向V */
    fsDIRECTION: string;
    /**轉檔工作狀態 */
    //fsSTATUS: string;
    fsSTATUS: WorkStatus;
    /**檔案寬 */
    fnWIDTH: number;
    /**檔案高 */
    fnHEIGHT: number;
}
