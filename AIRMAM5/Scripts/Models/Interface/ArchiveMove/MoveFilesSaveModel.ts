import { MediaType } from '../../Enum/MediaType';

/**檔案搬移存檔 */
export interface MoveFilesSaveModel {
    /** 搬移目的地的主題編號 */
    TargetSubjId: string;
    /** 媒資類別: A聲音, D文件, P圖片, S主題, V影片 */
    FileType: MediaType;
    /** 搬移的檔案編號 Array */
    MoveFileNos: Array<string>;
}
