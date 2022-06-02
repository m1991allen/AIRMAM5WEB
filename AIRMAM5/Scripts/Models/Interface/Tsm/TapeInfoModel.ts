/** 架上磁帶資訊*/
export interface TapeInfoModel {
    /**磁帶編號 */
    VOL_ID: string;
    /** 磁帶類型 */
    VOL_TYPE: string;
    /**使用狀態 */
    VOL_USE_STATUS: string;
    /** 已存放資料(GB) */
    USED_GB: number;
    /** 最後讀取日期 */
    READ_DATE: string;
    /**最後寫入日期 */
    WRITE_DATE: string;
    /** 儲存池 */
    POOL_NAME: string;
    /** 讀寫狀態 */
    VOL_RW_STATUS: string;
    /**寫入錯誤次數 */
    WRITE_ERRORS: string;
    /**讀取錯誤次數 */
    READ_ERRORS: string;
}
