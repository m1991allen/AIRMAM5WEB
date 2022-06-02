
/**
 * 音頻波形服務規格
 */
export interface IwavesurferService{
    /*-----------------------------　基本相關設定------------------------------*/
        /**通過XHR 加載音頻，如果使用後端MediaElement，則解析為Audio元素。*/
        load(url: string, peaks?: any[], preload?: ['none' | 'metadata' | 'auto']): void;
        /**加載來自Blob或File對象的音頻*/
        loadBlob(url: string): void;
    
        /**從當前位置開始播放。以秒為單位可用於設置要播放的音頻範圍*/
        play(start?: number, end?: number): void;
        /** 停止播放 */
        pause(): void;
        /**播放時暫停，播放時暫停 */
        playPause(): void;
        /**停止並開始*/
        stop(): void;
        /** 開啟和關閉音量*/
        toggleMute(): void;
        /**切換scrollParent*/
        toggleScroll(): void;
        /**水平放大和縮小波形。該參數是每秒音頻的水平像素數。它還會更改參數minPxPerSec並啟用該 scrollParent選項。*/
        zoom(pxPerSec: number): void;
    
        /**如果當前正在播放，則返回，否則返回false */
        isPlaying(): boolean;
        /** 從當前位置跳數秒（使用負值向後移動） */
        skip(offset: number): void;
        /**倒數skipLength秒*/
        skipBackward(): void;
        /** 跳過skipLength幾秒鐘*/
        skipForward(): void;
    
        /**返回當前的就緒狀態*/
        getReady():any;
        /**返回音頻片段的播放速度*/
        getPlaybackRate(): number;
        /**返回當前音頻片段的音量*/
        getVolume(): number;
        /**返回當前的靜音狀態 */
        getMute(): any;
        /** 以秒為單位返回當前進度。*/
        getCurrentTime(): number;
        /**以秒為單位返回音頻剪輯的持續時間 */
        getDuration(): number;
        /** 將播放音量設置為新值[0..1]（0 =靜音，1 =最大） */
        setVolume(newVolume: number): void;
        /**使當前聲音靜音。可以是布爾值，true以使聲音靜音或false取消靜音*/
        setMute(mute: boolean): void;
        /**設置波形的高度 */
        setHeight(height:number):void;
        /**設置播放速度（0.5是半速，1正常速度，2雙倍速度等等） */
        setPlaybackRate(rate: number): void;
    /*-----------------------------顏色相關設定------------------------------*/
    
        /** 返迴光標後面的波形的填充顏色*/
        getProgressColor():string;
       /** 返回波形容器的背景色。 */
        getBackgroundColor():string;
        /** 返回指示播放頭位置的光標填充顏色 */
        getCursorColor():string;
        /**返迴光標後的波形填充顏色 */
        getWaveColor():string;
        /**設置波形容器的背景色 */
        setBackgroundColor(color):void;
        /**設置指示播放頭位置的光標的填充顏色*/
        setCursorColor(color):void;
        /** 設置光標後面的波形填充顏色*/
        setProgressColor(color):void;
        /** 設置光標後的波形填充顏色*/
        setWaveColor(color):void;
       /*-----------------------------其他相關設定------------------------------*/ 
        /**取消音頻文件加載過程。*/
        cancelAjax():void;
        /**刪除事件，元素並斷開Web音頻節點的連接 */
        destroy(): void;
        /**清除波形，就像加載了零長度音頻一樣。*/
        empty(): void;
        /**訂閱事件。有關所有事件的列表，請參見WaveSurfer事件 */
        on(eventName: WavesurferEvent, callback: (x?: any) => void): Disposer;
        /**取消訂閱活動*/
        un(eventName: WavesurferEvent, callback: (x?: any) => void): void;
        /**取消訂閱所有事件*/
        unAll(): void;
        /** 尋求進度和居中視圖[0..1](0 =開始，1 =結束)*/
        seekAndCenter(progress: number): void;
        /**尋求進度[0..1](0 =開始，1 =結束) */
        seekTo(progress: number): void;
        /**設置接收器ID以更改音頻輸出設備*/
        setSinkId(deviceId):void;
        /**用於將自己的WebAudio節點插入圖中 */
        setFilter(filters: any[]): void;
        /**返回當前已初始化的插件名稱的映射 */
        getActivePlugins():any;
       /**返回當前設置的過濾器數組*/
        getFilters(): any[];
        /**切換鼠標交互*/
        toggleInteraction(): void;
        /**
        * 將PCM數據導出到JSON數組中
        * @param length default:1024
        * @param accuracy default:10000
        * @param noWindow default:false
        * @param start default:0
         */
        exportPCM(length?:number, accuracy?:number, noWindow?:boolean, start?:number):JSON;
        /**
         * 將波形圖像作為數據URI或Blob返回
         * @param format 格式
         * @param quality 質量
         * @param type 類型
         */
        exportImage(format:string, quality:number, type:string):URL|Blob|string;
    }
    