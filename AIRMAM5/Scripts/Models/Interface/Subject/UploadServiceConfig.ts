/**上傳服務初始化所需參數 */
export interface UploadServiceConfig {
    /**上傳api路徑 */
    uploadPath: string;
    /**分塊大小 */
    chuckSize: number;
    /**平行上傳數量 */
    simultaneousUploads: number;
    /**前端等候超時時間 */
    TimeoutSec: number;
    /**各媒體類別可接受的副檔名 */
    AcceptExtension: Array<AcceptExtension>;
}

/**各媒體類別可接受的副檔名 */
export interface AcceptExtension {
    /**媒體類型 */
    MediaType: string;
    /**可接受的副檔名 */
    FileExtension: string;
}
