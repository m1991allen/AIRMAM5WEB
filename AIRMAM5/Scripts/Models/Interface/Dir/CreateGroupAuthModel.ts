import { CreateInfoModel, UpdateInfoModel } from '../Shared/IDate';
/**新增群組權限:前端使用 */
export interface CreateGroupAuthModel {
    /**權限類型 */
    AuthType: 'U' | 'G';
    /**系統目錄編號 */
    fnDIR_ID: number;
    /**角色群組 */
    fsGROUP_ID: string;
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
/**新增群組權限:後端需要 */
export interface CreateGroupRealAuthModel extends CreateGroupAuthModel, CreateInfoModel, UpdateInfoModel {}
