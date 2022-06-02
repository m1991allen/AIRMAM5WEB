/**規則表邏輯定義 */
export enum RuleOperator {
    /**包含 */
    include = 0,
    /**不包含 */
    exclude = 1,
    /**等於*/
    equal = 2,
    /**介於 */
    between = 3,
}
/**規則表邏輯中文定義 */
export enum ChineseRuleOperator {
    包含 = 'include',
    不包含 = 'exclude',
    等於 = 'equal',
    介於 = 'between',
}
