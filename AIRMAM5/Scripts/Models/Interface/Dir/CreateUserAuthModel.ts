import { UpdateInfoModel, CreateInfoModel } from '../Shared/IDate';

/** 新增使用者權限:前端使用 */
export interface CreateUserAuthModel {
    /**權限類型 */
    AuthType: 'U' | 'G';
    /**系統目錄編號 */
    fnDIR_ID: number;
    /**使用者帳號  */
    fsLOGIN_ID: string;
    /*主題 可用權限* */
    fsLIMIT_SUBJECT: string;
    /**影片 可用權限 */
    fsLIMIT_VIDEO: string;
    /** 聲音 可用權限 */
    fsLIMIT_AUDIO: string;
    /**圖片 可用權限 */
    fsLIMIT_PHOTO: string;
    /** 文件 可用權限 */
    fsLIMIT_DOC: string;
}

/**新增使用者權限:後端使用 */
export interface CreateUserRealAuthModel extends CreateUserAuthModel, UpdateInfoModel, CreateInfoModel {}
