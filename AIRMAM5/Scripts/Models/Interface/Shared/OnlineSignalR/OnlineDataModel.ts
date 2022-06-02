/**在線人數與清單 */
export interface OnlineDataModel {
    /**在線人數 */
    Number: number;
    /**在線使用者清單 */
    DataList: Array<OnlineMembersModel>;
}
/**在線使用者清單 */
export interface OnlineMembersModel {
    /**登入記錄編號 */
    LoginLogid: number;
    /** 使用者ID */
    UserId: string;
    /**使用者帳號 */
    UserName: string;
    /**上線時間 */
    LoginDTime: string;
    /**上次離線時間,如果是1991??? */
    EndDTime: string;
    /**註解 */
    Note: string;
    /**最後異動時間 */
    UpdateDTime: string;
}
