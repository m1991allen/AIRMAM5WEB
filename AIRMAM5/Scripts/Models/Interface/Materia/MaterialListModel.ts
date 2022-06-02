import { CreateMaterialModel } from './CreateMaterialModel';
import { TSMFileStatus } from '../../Enum/TSMFileStatus';

/**我的調用清單列表 */
export interface MaterialListModel extends CreateMaterialModel {
    /**借調編號 */
    MaterialId: number;
    /**檔案狀態 */
    TSMFileStatus: number | TSMFileStatus;
    /**檔案狀態 */
    TSMFileStatusStr: string;
    /**檔案狀態顯示文字 */
    FileCategoryStr: string;
    /**標題 */
    Title: string;
    /**調用日期(建立日期) */
    CreatedDate: string;
    /** 影片長度(時長:原始秒數) */
    VideoMaxTime: string;
    /**起始時間(原始秒數):結束時間(原始秒數):截取時長(原始秒數) */
    ParameterStr: string;
    /** 影片長度(時長:已轉換為timcode格式)用於篩選器和顯示 */
    VideoMaxTimeStr: string;
    /**起始時間 (已轉換為timcode格式)用於篩選器和顯示*/
    MarkInTimeStr: string;
    /**結束時間(已轉換為timcode格式)用於篩選器和顯示 */
    MarkOutTimeStr: string;
    /**截取時長(已轉換為timcode格式)用於篩選器和顯示 */
    MarkDurationStr: string;
    /**版權 */License: string;
    /**版權中文 */LicenseStr: string;
    /**版權是否調用提醒 */IsAlert: boolean;
    /**版權是否調用禁止 */IsForBid: boolean;
    /**版權是否過期*/IsExpired:boolean;
    /**版權提醒訊息 */LicenseMessage:string;
    /**版權到期日*/LicenseEndDate:string;
}
