/** 審核調用拒絕原因 */
export enum VerifyRejectReason {
    /**權限不符合，請先經過主管同意 */
    NoPermisson = 1,
    /**未經過申請，須補書面申請或電子郵件 */
    WithoutApplication = 2,
    /**請與片庫聯絡，領取帳號申請單 */
    GetAccountApplicationForm = 3,
    /**其他 */
    Other = 4,
}
