/**群組/使用者權限列表顯示資料 */
export interface ShowAuthListModel {
    /**系統目錄編號 */
    fnDIR_ID: number;
    /**角色群組ID */
    GROUP_ID: string;
    /**角色群組 名稱 */
    GROUP_NAME: string;
    /**使用者ID */
    USER_ID: string;
    /**使用者帳號 */
    LOGIN_ID: string;
    /**使用者顯示名稱 */
    USER_NAME: string;
    /**目錄管理權限 Y=直接 , y=繼承 */
    C_ADMIN: 'Y' | 'y';
    /**主題/檔案權限 Y=直接 , y=繼承 */
    C_USER: 'Y' | 'y';
    /**主題 權限內容 */
    LIMIT_SUBJECT: string;
    /** 影片 權限內容*/
    LIMIT_VIDEO: string;
    /**聲音 權限內容 */
    LIMIT_AUDIO: string;
    /**圖片 權限內容 */
    LIMIT_PHOTO: string;
    /** 文件 權限內容 */
    LIMIT_DOC: string;
    /**節點路徑 */
    C_sDIR_PATH: string;
    /**欄位類別:G=群組,U=使用者 */
    DATATYPE: 'U' | 'G' | string;
    /**???? */
    fnPARENT_ID: number;
}
