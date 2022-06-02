/**主題與檔案後端傳入之設定參數 */
export interface SubjectUploadConfig {
    /**登入UserName */
    LoginId: string;
    /**平行上傳的檔案數 */
    SimultaneousUploads: number;
    /**上傳目的 */
    TargetUrl: string;
    /**暫存檔案夾名 */
    TempFolder: string;
    /**上傳檔案的緩衝(buffer) */
    UploadFileBuffer: number;
    /**檔案上傳時,前端等候時間(秒) */
    TimeoutSec: number;
}
