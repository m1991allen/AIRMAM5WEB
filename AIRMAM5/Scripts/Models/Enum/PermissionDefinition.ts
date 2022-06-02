/**可開啟的權限功能定義 */
export const enum PermissionDefinition {
    /**檢視 */
    V = 'V',
    /**新增(重轉、置換) */
    I = 'I',
    /**修改(修改、批次修改) */
    U = 'U',
    /**刪除 */
    D = 'D',
    /**調用 */
    B = 'B',
}
