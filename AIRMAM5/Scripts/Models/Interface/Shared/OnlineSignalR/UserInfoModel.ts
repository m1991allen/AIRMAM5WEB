/**使用者ID,帳號 */
export interface UserIdModel {
    /** 使用者識別Id */
    fsUSER_ID: string;
    /**使用者帳號 */
    fsLOGIN_ID: string;
}

/**使用者登入資訊 */
export interface UserInfoModel extends UserIdModel {
    /**使用者顯示名稱 */
    RealName: string;
    /**隸屬單位ID */
    DeptId: string;
    /**隸屬單位名稱 */
    DeptName: string;
    /**電子郵件 */
    Email: string;
    /**使用者所屬群組/角色 */
    UserRoles: Array<string>;
    /** 登入記錄編號 */
    LoginLogid: number;
}
