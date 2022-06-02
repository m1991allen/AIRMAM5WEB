/** 使用者通知訊息與數量 */
export interface UserNotifyModel {
    /** 使用者userid */
    UserId: string;
    /**未讀訊息數量 */
    UnRead: number;
    /** 訊息清單(僅顯示未刪除) */
    DataList: Array<NotifyDataModel>;
}

export interface NotifyDataModel {
    /** 訊息通知編號 */
    NOTIFY_ID: number;
    /**訊息通知標題 */
    TITLE: string;
    /** 訊息通知內容 */
    CONTENT: string;
    /**是否讀取 */
    IsRead: boolean;
    /**創建日期 */
    CREATED_DATE: string;
    /**創建者 */
    CREATED_BY: string;
}
