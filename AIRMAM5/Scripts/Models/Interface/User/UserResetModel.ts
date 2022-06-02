/**使用者帳號還原密碼 */
export interface UserResetModel {
    /**帳號ID */
    id: string;
    /**帳號關閉或激活 */
    active: boolean;
}
