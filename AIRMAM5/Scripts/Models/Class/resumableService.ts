import { IsNULLorEmpty, IsNullorUndefined } from '../Function/Check';
import { ErrorMessage } from '../Function/Message';
import { IResponse } from '../Interface/Shared/IResponse';
import { Logger } from './LoggerService';


export interface IresumableService {
    /**取的所有檔案 */
    files(): ResumableFile[];
    /**設置Query參數 */
    SetQuery(query: { [key: string]: any }): Promise<boolean>;
    /**增加檔案 */
    FileAdded(cusevent: (file?:ResumableFile, event?: DragEvent) => void): void ;
    /**上載特定文件時出了點問題，正在重試上載*/
    FileRetry(uniqueIdentifier: string);
    /**上傳特定文件時發生錯誤*/
    FileError(file, message);
    /**中止上傳文件，並將其從要上傳的文件列表中刪除*/
    FileRemove(uniqueIdentifier: string): void;
    /**中止上傳文件，並刪除所有上傳文件*/
    FileCancel():void;
    /**檔案成功 */
    FileSuccess(cusevent?: (file: ResumableFile, message: IResponse) => void): void;
    /**檔案發生錯誤 */
    FileError(cusevent?: (file: ResumableFile, message: IResponse) => void): void;
    /**上傳進度自定義事件 */
    FileProgress(cusevent?: (file: ResumableFile) => void): void;
    /**中止上傳文件(完全放棄)*/
    FileAbort(uniqueIdentifier: string): void;
    /**
     * 將瀏覽操作分配給一個或多個DOM節點。傳入true以允許選擇目錄（僅適用於Chrome)
     * @param {Array<Element>|Element} domNodes 一個或多個瀏覽操作元素
     * @param {boolean} isDirectory 傳入true以允許選擇目錄（僅適用於Chrome）
     */
    AssignBrowse(domNodes: Array<Element> | Element, isDirectory?: boolean): void;
    /**
     * 將一個或多個DOM節點分配為放置目標
     * @param  {Array<Element>|Element} domNodes 一個或多個瀏覽操作元素
     */
    AssignDrop(domNodes: Array<Element> | Element): void;

    /**監聽來自Resumable.js的事件 */
    On(event: ResumableEvent, callback: Function);
    /**開始或繼續上傳*/
    Upload();
    /**返回介於0和1之間的浮點數，指示所有文件的當前上傳進度*/
    Progress(): number;
    /**
     *特定文件的上傳進度
     * @param {string} uniqueIdentifier 通過ResumableFile對象的唯一標識符查找對象
     * @param {boolean} relative　返回相對於Resumable.js實例中的所有文件的值
     * @returns {number} 進度
     * @memberof IresumableService
     */
    GetFileProgress(uniqueIdentifier: string, relative: boolean): number;
    /**返回一個布爾值，指示實例當前是否正在上傳任何東西*/
    IsUploading(): boolean;
    /**返回一個布爾值，指示實例當前是否已經完成上傳 */
    IsComplete(uniqueIdentifier: string): boolean;
    /**ResumableFile從列表中取消列表中特定對象的上載*/
    RemoveFile(file: ResumableFile): void;
    /**通過ResumableFile對象的唯一標識符查找對象*/
    GetFromUniqueIdentifier(uniqueIdentifier: string):ResumableFile;
    /**返回上傳的總大小（以字節為單位） */
    GetSize(): number;
    /**上載已暫停*/
    Pause(uniqueIdentifier: string): void;
    /**使用相同的回調函數監聽上面列出的所有事件*/
    CatchAll(event, ...event2: any);
    /**返回介於0和1之間的浮點數，指示文件的當前上傳進度。如果relative為is true，則返回相對於Resumable.js實例中的所有文件的值*/
    Progress(relative);
    /**在取消項目之前觸發，允許對上傳文件進行任何處理 */
    //BeforeCancel(cusevent?:Function):void;
    /**重建ResumableFile對象的狀態，包括重新分配塊和XMLHttpRequest實例*/
    Bootstrap(uniqueIdentifier: string): void;
    /**完成上傳時的自定義事件 */
    Complete(cusevent?: Function): void;
}

/**------------------------------------------------
 *瀏覽器是否支援Resumable.js
 * @export
 * @returns {boolean} 是否支援
 -------------------------------------------------*/
export function IsResumableSupport(): boolean {
    return new Resumable({}).support;
}

/**
 * resumablejs上傳檔案服務
 * (建議使用前先確認是否支援,可用　IsResumableSupport()　)
 * @param api 上傳api路徑
 * @param chuckSize 分塊大小()
 * @param simultaneousUploads 平行上傳數量(一次上傳幾個分塊數或檔案)
 * @param fileExt 可接受的副檔名
 * @param timeoutSeconds 前端請求超時時間(秒)
 * @param maxFiles 可以上傳的檔案數量,如果是undefined表示不限制上傳數量
 */
export class resumableService implements IresumableService {
    protected resumable: Resumable;
    constructor(input: {
        api: string;
        chuckSize: number;
        simultaneousUploads: number;
        fileExt: string[];
        timeoutSeconds: number;
        maxFiles: number | undefined;
        fileTypeErrorCallback?:(file, errorCount)=>void;
    }) {
        this.resumable = new Resumable({
            target: input.api,
            chunkSize: input.chuckSize * 1024 * 1024,
            testChunks: false, //true會導致api必須為httpget
            simultaneousUploads: input.simultaneousUploads,
            fileType: input.fileExt,
            xhrTimeout: input.timeoutSeconds * 1000, //(毫秒)
            maxFiles: input.maxFiles < 0 ? undefined : input.maxFiles,
            maxChunkRetries: 10,
            fileTypeErrorCallback: function(file, errorCount) {
                if(!IsNullorUndefined(input.fileTypeErrorCallback)){
                    input.fileTypeErrorCallback(file,errorCount);
                }else{
                    ErrorMessage(`${(<any>file).name}不是可接受的檔案類型(${input.fileExt.join(',')})`);
                }
            },
            maxFilesErrorCallback: (): void => {
                input.maxFiles != undefined ? ErrorMessage(`只能上傳${input.maxFiles}個文件`) : false;
            },
            generateUniqueIdentifier: (): string => {
                const random = Math.random()
                    .toString(36)
                    .substr(2, 16);
                const now = new Date();
                const timespan = now.getHours().toString() + now.getMinutes().toString() + now.getSeconds();
                return random + timespan;
            },
            // permanentErrors:[400, 404, 409, 415, 500, 501]
        });
    }
    files(): ResumableFile[]{
        return this.resumable.files;
    }
    SetQuery(query: { [key: string]: any }): Promise<boolean> {
        Logger.log('----上傳服務設置參數----', query);
        return new Promise(resolve => {
            try {
                this.resumable.opts.query = function() {
                    return query;
                };
                resolve(true);
            } catch (error) {
                resolve(false);
            }
        });
    }
    FileAdded(cusevent: (file?: ResumableFile, event?: DragEvent) => void): void {
        this.resumable.on('fileAdded', function(file:ResumableFile, event: DragEvent) {
            if (!IsNULLorEmpty(cusevent)) {
                cusevent(file, event);
            }
        });
    }
    RemoveFile(file: ResumableFile) {
       this.resumable.removeFile(file);
    }
    FileRemove(uniqueIdentifier: string): void {
        const file = this.GetFromUniqueIdentifier(uniqueIdentifier);
        this.resumable.removeFile(file);
    }
    FileCancel(){
        this.resumable.cancel();
    }
    FileSuccess(cusevent?: (file: ResumableFile, message: IResponse) => void): void {
        this.resumable.on('fileSuccess', function(file: ResumableFile, event: string) {
            if (!IsNULLorEmpty(cusevent)) {
                try {
                    cusevent(file, <IResponse>JSON.parse(event));
                } catch (error) {
                    Logger.error(error);
                }
            }
        });
    }
    FileError(cusevent?: (file: ResumableFile, message: IResponse) => void): void {
        this.resumable.on('fileError', function(file: ResumableFile, event: string) {
            if (!IsNULLorEmpty(cusevent)) {
                try {
                    cusevent(file, <IResponse>JSON.parse(event));
                } catch (error) {
                    Logger.error(error);
                }
            }
        });
    }
    FileProgress(cusevent?: (file: ResumableFile) => void): void {
        this.resumable.on('fileProgress', function(file: ResumableFile) {
            if (typeof cusevent === 'function') {
                try {
                    cusevent(file);
                } catch (error) {
                    Logger.error(error);
                }
            }
        });
    }
    FileRetry(uniqueIdentifier: string) {
        try {
            this.GetFromUniqueIdentifier(uniqueIdentifier).retry();
        } catch (error) {
            Logger.error(error);
        }
    }
    FileAbort(uniqueIdentifier: string): void {
        try {
            this.GetFromUniqueIdentifier(uniqueIdentifier).abort();
        } catch (error) {
            Logger.error(error);
        }
    }
    AssignBrowse(domNodes: Array<Element>, isDirectory?: boolean): void {
        isDirectory = typeof isDirectory === undefined ? true : isDirectory;
        this.resumable.assignBrowse(domNodes, isDirectory);
    }
    AssignDrop(domNodes: Array<Element>): void {
        this.resumable.assignDrop(domNodes);
    }
    On(event: ResumableEvent, callback: Function) {
        try {
            this.resumable.on(event, callback);
        } catch (error) {
            Logger.error(`ResumableService On:${event}發生錯誤`, error);
        }
    }
    Upload(): void {
        this.resumable.upload();
    }
    Pause(): void {
        this.resumable.pause();
    }
    IsComplete(uniqueIdentifier: string): boolean {
        return this.GetFromUniqueIdentifier(uniqueIdentifier).isComplete();
    }
    IsUploading(): boolean {
        return this.resumable.isUploading();
    }
    GetFromUniqueIdentifier(uniqueIdentifier: string): ResumableFile {
        try {
            return this.resumable.getFromUniqueIdentifier(uniqueIdentifier);
        } catch (error) {
            Logger.error(error);
        }
    }
    GetSize(): number {
        return this.resumable.getSize();
    }
    Progress(): number {
        return this.resumable.progress();
    }
    GetFileProgress(uniqueIdentifier: string, relative: boolean): number {
        return this.GetFromUniqueIdentifier(uniqueIdentifier).progress(relative);
    }
    CatchAll(event: any, ...event2: any) {
        throw new Error('Method not implemented.');
    }
    Complete(cusevent?: Function): void {
        this.resumable.on('complete', function() {
            if (typeof cusevent === 'function') {
                cusevent();
            }
        });
    }
    Bootstrap(uniqueIdentifier: string): void {
        this.GetFromUniqueIdentifier(uniqueIdentifier).bootstrap();
    }
}
