import { IDate } from './Shared/IDate';

export interface LUploadSearchModel extends IDate {
    /**工作狀態 */
    WorkStatus: string;
}
/**
 * 轉檔進度
 */
export interface WorkProgressModel {
    /**轉檔工作編號 */
    fnWORK_ID: number;
    /**轉檔進度% */
    Progress: string; //fsPROGRESS: string;
    /**轉檔狀態代碼 */
    WorkStatus: string;
    /**轉檔狀態顯示名稱 */
    WorkStatusName: string;
    /**狀態代表顏色 */
    StatusColor: string;
    /**轉檔開始時間 _added_on_20200305 */
    WorkSTime: string;
    /**轉檔結束時間 _added_on_20200305 */
    WorkETime: string;
}

/**轉檔結果列表 */
export interface LUploadListModel {
    /**轉檔工作編號 */
    fnWORK_ID: number;
    /**轉檔進度名稱 */
    StatusName: string; //C_sSTATUSNAME: string;
    /**轉檔進度 */
    Progress: string; //fsPROGRESS: string;
    /**開始轉檔時間 */
    fdSTIME: string;
    /**結束轉檔時間 */
    fdETIME: string;
    /**檔案資訊 */
    C_sFILE_INFO: string;
    /**轉檔參數 */
    fsPARAMETERS: string;
    /**轉檔結果 */
    fsRESULT: string;
    /**優先順序 */
    fsPRIORITY: string;
    /**備註 */
    fsNOTE: string;
    /**工作/轉檔類別 */
    fsTYPE: string;
    /**工作/轉檔類別 中文 */
    C_sTYPENAME: string;
    /** 工作/轉檔狀態 */
    WorkStatus: string; //fsSTATUS: string;
    /**建立者 */
    CreatedBy: string;
    /**狀態代表顏色 */
    StatusColor: string;
}
