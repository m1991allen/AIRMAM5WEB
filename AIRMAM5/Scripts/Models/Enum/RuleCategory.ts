/**規則庫流程類型 */
export enum RuleCategory {
    /**調用 */
    BOOKING = 0,
    /**入庫 */
    UPLOAD = 1,
    /**轉檔 */
    TRANSCODE = 2,
}

/**規則庫流程英文代碼 */
export enum RuleEnglishCategory {
    /**不指定 */
    NotSpecify = '',
    /**調用 */
    BOOKING = 'BOOKING',
    /**入庫 */
    UPLOAD = 'UPLOAD',
    /**轉檔 */
    TRANSCODE = 'TRANSCODE',
}
