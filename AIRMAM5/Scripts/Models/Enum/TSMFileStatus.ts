/**檔案狀態 */
export enum TSMFileStatus {
    /**無資料 */
    Nodata = -1,
    /**檔案在磁帶 */
    ArchiveOnTape = 0,
    /**檔案在磁碟 */
    FileOnDisk = 1,
    /**檔案錯誤 */
    FileError = 2,
    /**處理中 */
    Processing = 3,
    /**檔案不存在 */
    FileNotExist = 4,
    /**檔案在線 */
    Online = 5,
    /**檔案離線 */
    Offline = 6,
    /**檔案深度離線 */
    OfflineDeep = 7,
}
/**檔案中文狀態 */
export enum ChineseTSMFileStatus {
    無資料 = -1,
    檔案在磁帶 = 0,
    檔案在磁碟 = 1,
    檔案錯誤 = 2,
    處理中 = 3,
    檔案不存在 = 4,
    檔案在線 = 5,
    檔案離線 = 6,
    檔案深度離線 = 7,
}
