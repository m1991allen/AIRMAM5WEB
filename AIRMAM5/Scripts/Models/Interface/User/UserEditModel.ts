/**編輯帳號 */
export interface UserEditModel {
    /**被編輯的帳號ID */
    fsUSER_ID: string;
    /**連絡電話 */
    fsPHONE: string;
    /**機密等級 */
    FSecretList: Array<string>;
    /**描述/備註 */
    fsDESCRIPTION: string;
    /**預設備用路徑 */
    fsBOOKING_TARGET_PATH: string;
    /**所屬群組 */
    GroupList: Array<string>;
    /**顯示名稱 */
    fsNAME: string;
    /**英文名稱 */
    fsENAME: string;
    /**職稱 */
    fsTITLE: string;
    /**隸屬單位部門 */
    fsDEPT_ID: string;
    /**電子郵件 */
    fsEMAIL: string;
}
