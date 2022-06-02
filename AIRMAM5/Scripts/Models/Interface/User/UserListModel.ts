/**帳號顯示列表查詢結果 */
export interface UserListModel {
    /** 使用者識別Id */
    fsUSER_ID: string;
    /**使用者帳號 */
    fsLOGIN_ID: string;
    /**使用者姓名 */
    fsNAME: string;
    /**隸屬單位 */
    fsDEPT_ID: string;
    /**隸屬單位 */
    C_sDEPTNAME: string;
    /** 帳號啟用狀態 */
    fsIS_ACTIVE: boolean;
    /** 描述/備註 */
    fsDESCRIPTION: string;
    /**電子郵件 */
    fsEMAIL: string;
    /**電子郵件是否驗證 true/false */
    fsEmailConfirmed: boolean;
    /**電子郵件是否驗證 文字*/
    EmailConfirmedStr: string;
}
