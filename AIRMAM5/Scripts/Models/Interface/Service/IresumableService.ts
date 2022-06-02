export interface IresumableService{
    /**
     * 將瀏覽操作分配給一個或多個DOM節點。
     * 傳入true以允許選擇目錄（僅限Chrome） 
     **/
    assignBrowse(domNodes:any, isDirectory:boolean):void;
    /**
     * 將一個或多個DOM節點指定為放置目標 
     **/
    assignDrop(domNodes:any):void;
    /** 
     * 收聽Resumable.js的活動（見下文）
     **/ 
    on(event:string, callback:Function):void;
    /** 開始或繼續上傳 */
    upload():void;
    /**暫停上傳 */
    pause():void;
    /**取消上傳所有ResumableFile對象並將其從列表中刪除 */
    cancel():void;
    /** 返回0到1之間的浮點數，表示所有文件的當前上載進度*/
    progress():number;
    /**返回一個布爾值，指示實例當前是否正在上傳任何內容 */
    isUploading():boolean;
    /**取消ResumableFile列表中列表上特定對象的上傳 */ 
    removeFile(file:File):void;
    /**ResumableFile通過其唯一標識符查找對象 */
    getFromUniqueIdentifier(uniqueIdentifier):any;
    /**以字節為單位返回上載的總大小 */
    getSize():number;

    /**特定文件已完成 */
    fileSuccess(file:File,successEvent?:Function):boolean;
    /**
     *  上傳特定文件的進度
     * @param file  上傳文件
     * @param progressEvent 自定義的上傳事件
     */ 
    fileProgress(file:File,progressEvent?:Function):void;
    /**
     * 添加了一個新文件
     * @param file 上傳文件
     * @param addEvent 自定義的新增事件
     */
    fileAdded(file:File,addEvent?:Function):boolean;
    /**
     * 上傳特定文件時出現問題，正在重試上傳
     * @param file 上傳文件
     * @param retryTime 重試次數限制
     */
    fileRetry(file:File,retryTime:number):boolean;
    /**
     * 上載特定文件時發生錯誤
     * @param file 上傳文件
     * @param message 顯示訊息
     */
    fileError(file:File, message:string):void;
    /**
     * 上傳完成
     * @param completeEvent 自定義的上傳事件
     */
    complete(completeEvent:Function):void;
    /**
     * 上傳進度
     */
    progress():any;
    /**
     * 發生了錯誤，包括fileError
     * @param message  顯示訊息
     * @param file 上傳文件
     */
    error(message:string, file:File):void;
    /**
     * 上傳暫停
     */
    pause():boolean;
    /**
     * 上傳已取消
     */
    cancel():boolean;
    /**使用相同的回調函數收聽上面列出的所有事件 */
    catchAll(eventName:string,event):void;

}

/**
 * 上傳檔案初始化設定
 */
export interface resumableSetting{
    /**目標URL */
    target: string;
    /**檔案類型 */
    fileType: string;
    /**同時上傳數(默認值=3) */
    simultaneousUploads: 1;
    /**要使用的文件塊多部分POST參數的名稱（默認值：file） */
    fileParameterName:string;
    /**要在包含數據的多部分POST中包含的額外參數。這可以是對像或功能 */
    query?:any;
    /**額外的標頭與數據 */
    headers?:any;
    /**每個上傳的數據塊的大小（以字節為單位1*1024*1024） */
    chunkSize:number;
    /**優先考慮所有文件的第一個和最後一個塊。
     * 如果您可以僅從第一個或最後一個塊確定文件是否對您的服務有效，這可能很方便。
     * 例如，照片或視頻元數據通常位於文件的第一部分，從而可以輕鬆地僅測試第一個塊的支持。
     * （默認值：false）
     * */
    prioritizeFirstAndLastChunk:boolean;
    /**向服務器發出對每個塊的GET請求，以查看它是否已存在。
     * 如果在服務器端實現，即使在瀏覽器崩潰甚至計算機重啟後，這也將允許上傳恢復。
     * （默認值：true）
     * */
    testChunks?:boolean;
    /**覆蓋為每個文件生成唯一標識符的函數。
     * （默認值：null）
     **/
    generateUniqueIdentifier?:string;
    /**
     * 指示在單個會話中可以上載的文件數。
     * 有效值是任何正整數且undefined無限制。
     * （默認值：undefined）
     */
    maxFiles?:number|undefined;
    /**
     * 顯示請在一個時間消息上傳n個文件的功能。
     * （默認：顯示一個警告框，其中包含一次只能輸入一個文件的消息。）
     */
    maxFilesErrorCallback?:Function;    
    
}