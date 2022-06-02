/**編輯群組/使用者權限:前端回傳 */
export interface EditAuthInputModel {
    /**系統目錄編號 */
    DirId: number;
    /**角色群組ID */
    GroupId: string;
    /**角色群組 名稱 */
    GroupName: string;
    /**使用者ID */
    UserId: string;
    /**使用者帳號 */
    LoginId: string;
    /**使用者顯示名稱 */
    ShowName: string;
    /**目錄管理權限 Y=直接 , y=繼承 */
    C_ADMIN: 'Y' | 'y';
    /**主題/檔案權限 Y=直接 , y=繼承 */
    C_USER: 'Y' | 'y';
    /**主題 權限內容 */
    LimitSubject: Array<string>;
    /** 影片 權限內容*/
    LimitVideo: Array<string>;
    /**聲音 權限內容 */
    LimitAudio: Array<string>;
    /**圖片 權限內容 */
    LimitPhoto: Array<string>;
    /** 文件 權限內容 */
    LimitDoc: Array<string>;
}
