/**Html5 Video 錯誤定義 */
export enum VideoNetworkState {
    /**暫無資料 另外，readyState是HAVE_NOTHING */
    NETWORK_EMPTY = 0,
    /**1個HTMLMediaElement處於活動狀態，並且已選擇資源，但未使用網絡。 */
    NETWORK_IDLE = 1,
    /**瀏覽器正在下載HTMLMediaElement數據。 */
    NETWORK_LOADING = 2,
    /**找不到HTMLMediaElement src */
    NETWORK_NO_SOURCE = 3,
}
