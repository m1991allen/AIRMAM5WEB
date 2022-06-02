import { SubjFileModel } from './SubjFileModel';

/** 主題檔案列表 */
export interface SubjectFilesViewModel {
    /**主題編號 */
    SubjectId: string;
    /**節點編號 */
    DirId: number;
    /**影片檔案 */
    VideoFiles: Array<SubjFileModel>;
    /**聲音檔案 */
    AudioFiles: Array<SubjFileModel>;
    /**圖片檔案 */
    PhotoFiles: Array<SubjFileModel>;
    /**文件檔案 */
    DocFiles: Array<SubjFileModel>;
}
