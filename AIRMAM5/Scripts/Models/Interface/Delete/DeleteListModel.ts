import { UpdateInfoModel, CreateInfoModel } from '../Shared/IDate';

/**刪除紀錄列表 */
export interface DeleteListModel extends UpdateInfoModel, CreateInfoModel {
    /** 編號 */
    fnINDEX_ID: number;
    /** 檔案編號 */
    fsFILE_NO: string;
    /**類別(V、A、D、P、S) */
    fsTYPE: string;
    /**刪除原因 */
    fsREASON: string;
    /** 狀態 */
    fsSTATUS: string;
    /**狀態顯示文字 */
    C_sSTATUS: string;
    /**類別顯示文字 */
    C_sTYPE: string;
    /**標題 */
    C_sTITLE: string;
    /**建立者 名稱 */
    fsCREATED_BY_NAME: string;
    /**異動者 名稱 */
    fsUPDATED_BY_NAME: string;
}
