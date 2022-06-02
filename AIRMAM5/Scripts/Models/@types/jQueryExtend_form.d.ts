interface JQuery{
    /**啟用form變化編輯追蹤 */
    trackEdit:()=>JQuery<HTMLElement>;
    /**表單是否被編輯 */
    isEdited:()=>JQuery<HTMLElement>;
    /**將表單標記為沒變化 */
    isCancel:()=>JQuery<HTMLElement>;
    /**監聽表單編輯事件 */
    onEdit:(callback?:Function)=>JQuery<HTMLElement>;
    /**監聽表單取消編輯事件 */
    onCancel:(callback?:Function)=>JQuery<HTMLElement>;
}