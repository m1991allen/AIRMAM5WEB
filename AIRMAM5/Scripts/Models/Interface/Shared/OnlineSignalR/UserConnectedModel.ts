/**連線參數 */
export interface UserConnectedModel {
    /**使用者Id */
    UserId: string;
    /**連線hub的Id */
    SignalrConnectionId: string;
    /**群組編號 */
    GroupId: string;
    /**登入帳號 */
    LoginId: string;
    /**登入Log的Id */
    LoginLogId: number;
}
