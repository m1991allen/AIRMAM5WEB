/**調用儲存基本參數 */
export interface BookingSaveBaseModel {
    /** 調用原因/樣板Id */
    ResonId: number | -1;
    /* 調用原因/樣板 */
    ResonStr: string;
    /**調用描述 */
    DescStr: string;
    /**轉出格式:影片 */
    ProfileVideo: string;
    /**轉出格式:聲音 */
    ProfileAudio: string;
    /**浮水印  */
    WaterMark: string;
    /**調用路徑  */
    PathStr: string;
}
