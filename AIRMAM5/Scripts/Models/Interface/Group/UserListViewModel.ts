/**使用者列表 */
export interface UserListViewModel {
    /**使用者識別Id */
    fsUSER_ID: string;
    /**使用者帳號 */
    fsLOGIN_ID: string;
    /**使用者姓名 */
    fsNAME: string;
    /** 隸屬單位ID */
    fsDEPT_ID: string;
    /** 隸屬單位 */
    C_sDEPTNAME: string;
    /** 帳號狀態: 1啟用 / 0不啟用 */
    fsIS_ACTIVE: boolean;
    /** 描述/備註 */
    fsDESCRIPTION: string;
    /**電子郵件 */
    fsEMAIL: string;
    /**電子郵件是否驗證 */
    fsEmailConfirmed: boolean;
    /** 電子郵件是否驗證文字 */
    EmailConfirmedStr: string;
}
