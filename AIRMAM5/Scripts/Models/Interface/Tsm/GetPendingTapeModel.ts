/** 取得待上架磁帶資料 */
export interface GetPendingTapeModel {
    /**磁帶編號 */
    TapeNumber: string;
    /** 狀態 */
    StatusName: string;
    /**要求日期 */
    CreatedDate: string;
    /**調用人員 */
    CreatedBy: string;
    /**轉檔編號 */
    WrokId: number;
    /**優先序號 */
    Priority: string;
    /**調用原因 */
    BookingReason: string;
}
