/**線上人數回應資訊 */
export interface OnlinePersonModel {
    Number: number;
    DataList: Array<PersonInfo>;
}

/**帳戶資訊 */
interface PersonInfo {
    /**帳號Id */
    LoginLogid: number;
    /**使用者Id */
    UserId: string;
    /**帳號 */
    UserName: string;
    /**登入時間 */
    LoginDTime: string;
    /**結束時間,例如:1900-01-01 00:00:00 */
    EndDTime: string;
    /**備註*/
    Note: string;
    /**更新時間 */
    UpdateDTime: string;
}
