/**
 * 轉檔狀態
 * 地端複製調用 => 00 => 01 => 10 => 21 => 90
 * 雲端複製調用 => 00 => 01 => 10 => 30 => 31 => 90
 * 地端轉檔調用 => 00=> 01 => 10 => 20 => 90
 * 雲端轉檔調用 => 00=> 01 => 10 => 20 => 30 => 31 => 90
 */
export enum WorkStatus {
    /*---------------------共用------------------- */
    /**排程中 */
    OnSchedule = '00',
    /**取出至AP */
    TransferToAP = '01',
    /**轉檔程序初始化 */
    GearshiftProgramInit = '10',
    /**轉檔、調用程序完成 */
    TranscodingComplete = '90',
    /**取不到媒體檔資訊取消排程 */
    CancelSchedule = 'C0',
    /**錯誤：更新資料庫 */
    ErrorUpdateDataBase = 'E0',
    /**錯誤：檔案不存在 */
    ErrorFileNotExist = 'E1',
    /**錯誤：刪除檔案 */
    ErrorDeleteFile = 'E2',
    /**錯誤：建立目錄 */
    ErrorBuildDirectory = 'E3',
    /**錯誤：轉檔異常 */
    ErrorTransition = 'E4',
    /**取不到媒體檔*/
    ErrorE6 = 'E6',
    /**加密檔案不存在 */
    ErrorE7 = 'E7',
    /**下載失敗 */
    ErrorE8 = 'E8',
    /*--------------------入庫專用----------------*/
    /**確認檔案狀態中 */
    ConfirmFileStatus = '_C',
    /**片庫審核中 */
    FilmVerify = '_V',
    /**轉檔中 */
    InTransition = '11',
    /**高解轉檔中 */
    HighResolution = '20',
    /**低解轉檔中 */
    LowResolution = '21',
    /**關鍵影格擷取中 */
    KeyFrameFetching = '22',
    /**語音辨識中 */
    SpeechRecognition='23',
    /**人臉辨識中 */
    FaceRecognition='24',
    /**更新轉檔資訊 */
    UpdateConversionInfo = '30',
    /**錯誤：片庫審核不通過 */
    ErrorFilmVerifyFail = 'E5',
    /*------------------調用專用 --------------------*/
    /**審核過 */
    VerifyPass = '_C',
    /**審核拒絕*/
    VerifyReject = '_R',
    /**確認磁帶審核狀態 */
    VerifyTapeStatus = '_S',
    /**確認磁帶上架 */
    VerifyOnTape = '_T',
    /**磁帶檔案回溯中 */
    TapeBacktracking = '_U',
    /**等待審核*/
    WaitForVerify = '_A',
    /**調用轉檔中 */
    BookingInTransition = '20',
    /**派送借調轉檔檔案送至目的路徑 */
    FileToDir = '21',
    /**等待下載中 */
    WaitForDownloading = '30',
    /**調用檔案下載中 */
    DownloadingFromClound = '31',
}
