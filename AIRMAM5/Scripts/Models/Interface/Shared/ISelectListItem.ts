/**
 * .Net下拉選單
 */
export interface SelectListItem{
    /**文字 */
    Text:string;
    /**值 */
    Value:string;
    /**選擇 */
    Selected :boolean;
    /**禁用 */
    Disabled:boolean;
    /**群組 */
    Group: SelectListGroup;
  }

/**
 * 下拉群組
 */
 export interface  SelectListGroup{
    /**禁用 */
    Disabled:boolean;
    Name:string;
  }