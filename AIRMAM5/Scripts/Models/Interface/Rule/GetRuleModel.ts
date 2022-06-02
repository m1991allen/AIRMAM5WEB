/**取得子規則 */
export interface GetRuleModel {
    /**流程代碼,例如:BOOKING、UPLOAD... */
    category: string;
    /**規則表代碼 */
    table: string;
    /**欄位代碼 */
    field: string;
}
