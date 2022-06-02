import { MediaType } from '../../Enum/MediaType';

/**
 *新增,刪除段落描述
 */
export interface ParagraphCUModel {
    /**檔案類型: A聲音, D文件, P圖片, S主題, V影片  */
    FileCategory: MediaType;
    /** 檔案編號 */
    fsFILE_NO: string;
    /**流水號 fnSEQ_NO */
    SeqNo: number | 0;
    /**開始時間(秒) ,ex: 4.521 */
    BegTime: number;
    /** 結束時間(秒) ,ex: 27.879 */
    EndTime: number;
    /**描述 */
    Description: string;
}
