import { UpdateInfoModel, CreateInfoModel } from '../Shared/IDate';

/**新增節點(前端介接) */
export interface CreateNodeModel {
    /**目錄編號 */
    fnDIR_ID: number;
    /**目錄標題名稱(Required) */
    fsNAME: string;
    /**父節點 */
    fnPARENT_ID: number;
    /**目錄描 */
    fsDESCRIPTION: string;
    /**目錄類型: Q=末端節點，可新增主題 */
    fsDIRTYPE: 'Q' | '' | string;
    IsQueue: boolean;
    /**顯示順序 */
    fnORDER: number;
    /**主題檔樣板編號 */
    fnTEMP_ID_SUBJECT: number;
    /**影片檔樣板編號 */
    fnTEMP_ID_VIDEO: number;
    /**聲音檔樣板編號 */
    fnTEMP_ID_AUDIO: number;
    /**圖片檔樣板編號 */
    fnTEMP_ID_PHOTO: number;
    /**文件檔樣板編號 */
    fnTEMP_ID_DOC: number;
    /**目錄管理群組 */
    fsADMIN_GROUP: string;
    /**目錄管理使用者 */
    fsADMIN_USER: string;
    /**fsCODE_ID=DIR002(目錄開放類型) */
    fsSHOWTYPE: string;
}

/**新增節點(傳入給後端) */
export interface CreateNodeRealModel extends CreateNodeModel, UpdateInfoModel, CreateInfoModel {}
