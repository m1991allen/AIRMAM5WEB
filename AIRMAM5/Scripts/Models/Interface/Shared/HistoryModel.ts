export interface HistoryInputModel{
    /**分頁Id */
    tabId:string|false;
    /**顯示標題 */
    title:string;
    /**描述 */
    description:string;
    /**訊息種類 */
    type:'link'|'success'|'info'|'error'|'warning';
}

export interface HistoryModel extends HistoryInputModel{
   /**唯一辨識碼 */
    guid:string;
    /**訊息時間 */
    datetime:Date;
}