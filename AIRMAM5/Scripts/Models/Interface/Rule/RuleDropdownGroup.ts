/**規則元素群組 */
export interface RuleDropdownGroup {
    /**規則下拉選單群組的元素Id */
    $RuleInput: JQuery<HTMLElement>;
    /**流程下拉選單Dropdown */
    $CategoryDropdown: JQuery<HTMLElement>;

    /**規則表下拉選單Select */
    $RuleTableSelect: JQuery<HTMLElement>;
    /**欄位下拉選單Select */
    $RuleColumnSelect: JQuery<HTMLElement>;

    /**欄位下拉選單Dropdown */
    $RuleColumnDropdown: JQuery<HTMLElement>;
    /**欄位下拉選單Dropdown*/
    $RuleTableDropdown: JQuery<HTMLElement>;

    /**邏輯值下拉選單Select */
    $RuleOperatorSelect: JQuery<HTMLElement>;
    /**邏輯值下拉選單Dropdown */
    $RuleOperatorDropdown: JQuery<HTMLElement>;

    /**篩選值元素置放區域 */
    $OperatorUI: JQuery<HTMLElement>;
    /**篩選值下拉選單Select */
    $RuleFilterSelect: JQuery<HTMLElement>;
}
