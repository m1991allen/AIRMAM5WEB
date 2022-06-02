
/**版權資料搜尋參數 */
export interface LicenseSearchModel {
    /**版權名稱 (模糊比對,關鍵字) */
    name: string,
    /**授權到期日期 (可為空值) */
    edt: string,
}