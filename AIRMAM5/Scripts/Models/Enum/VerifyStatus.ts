/**審核狀態 */
export enum VerifyStatus {
    /**待審核 */
    Pending = '_A',
    /**已通過  */
    Passed = '_C',
    /**已拒絕 */
    Rejected = '_R',
    /**無須審核 */
    NoVerify = '_N',
}
/**審核中文狀態 */
export enum VerifyChineseStatus {
    待審核 = '_A',
    已通過 = '_C',
    已拒絕 = '_R',
    無須審核 = '_N',
}
