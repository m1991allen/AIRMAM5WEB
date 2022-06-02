/**調用基本列表屬性 */
export interface BookingBaseModel {
    /**轉檔工作編號 */
    WorkId: number;
    /**轉檔狀態 */
    StatusName: string;
    /**轉檔進度/轉檔狀態 */
    Progress: string;
    /**類型 */
    ArcType: string;
    /**調用類別 */
    BookingTypeName: string;
    /**標題 */
    Title: string;
    /**調用日期 */
    BookingDate: string;
    /**剪輯起始時間: 已為TimeCode格式 */
    MarkInTime: string;
    /**剪輯結束時間: 已為TimeCode格式 */
    MarkOutTime: string;
    /**調用Id */
    BookingId: number;
    /**狀態代表顏色 */
    StatusColor: string;
}
