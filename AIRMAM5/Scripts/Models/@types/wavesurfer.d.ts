
   /**
  *    audioprocess: 音頻播放時連續發射。搜尋時也會觸發。
  *    dblclick:雙擊實例時。
  *    destroy:實例被銷毀時。
  *    error: 發生錯誤。回調將收到（字符串）錯誤消息。
  *    finish: 完成播放時。
  *    interaction:與波形有相互作用時。
  *    loading: 使用抓取或拖放加載時連續觸發。回調將以百分比[0..100]接收（整數）加載進度。
  *    mute:靜音更改。回調將收到（布爾）新的靜音狀態。
  *    pause: 音頻暫停時。
  *    play: 播放開始時。
  *    ready: 加載，解碼和繪製音頻後，即可進行音頻播放。使用MediaElement時會在繪製波形之前觸發，請參見waveform-ready。
  *    scroll:移動滾動條時。回調將收到一個ScrollEvent對象。
  *    seek:在尋求。回調將收到（浮動）進度[0..1]。
  *    volume:音量變化時。回調將收到（整數）新音量。
  *    waveform-ready: 使用MediaElement後端時在繪製波形後觸發。如果您使用的是WebAudio後端，則可以使用ready。
  *    zoom: 縮放。回調將收到（整數）minPxPerSec
  */
    type WavesurferEvent ="audioprocess" |"dblclick"|"destroy"|"error" |"finish" |"interaction"|"loading" |"mute"|"pause" |"play" |"ready" |"scroll" |"seek" |"volume"|"waveform-ready" |"zoom";
    type Disposer = () => void;
     interface WavesurferOptions {
       /**
        * 播放音頻的速度。較低的數字較慢。
        * @type: {float}
        * @default:1
        */
       audioRate?: number;
       /**
        * 使用您自己的先前初始化的AudioContext或保留空白。
        */
       audioContext?:object;
       /**
        * 使用您自己的先前初始化的ScriptProcessorNode或保留空白
        */
       audioScriptProcessor?:object;
   
       /**
        * WebAudio，MediaElement或MediaElementWebAudio。MediaElement是不支持的瀏覽器的後備。
        * @default:"WebAudio"
        */
       backend?: "WebAudio" | "MediaElement";
   
       /**
        * 波形容器的背景色
        */
       backgroundColor?: string|CanvasGradient;
       /**
        * 如果未提供，則波形條之間的可選間隔將以舊格式計算。
        */
       barGap?:number;
       /**
        * 波形條的高度。大於1的數字將增加波形條的高度。
        * @default:1
        */
       barHeight?: number | 1;
       /**
        * 使鋼筋變圓的半徑
        * @default:0
        */
       barRadius?:number;
       /**
        * default: none
        * 如果指定，則波形將繪製如下：▇▅ ▁ ▂ ▇ ▃ ▅ ▂
        */
       barWidth?: number;
       /**
        * 調用destroy方法時，關閉所有音頻上下文並使它們無效。
        * @default:false
        */
       closeAudioContext?:boolean;
       /**
        * 應該在其中繪製波形的CSS選擇器或HTML元素。這是唯一必需的參數。
        */
       container: string | Element;
   
       /**
        * 光標的填充顏色，指示播放頭的位置。
        * 	@default: #333
        */
       cursorColor?: string|CanvasGradient;
   
       /**
        * 以像素為單位。
        * @type: {integer}
        * @default: 1
        */
       cursorWidth?: number;
       
       /**
        * 是否填充整個容器或僅根據進行繪製minPxPerSec。
        * @default: true
        */
       fillParent?: boolean;
       
       /**
        * 縮放以獲得更詳細的波形時，使用Web音頻強制解碼音頻。
        * @default:false
        */
       forceDecode?:boolean;
   
       /**
        * 波形的高度。以像素為單位。
        * @type: {integer}
        * @default:128
        */
       height?: number;
   
       /**
        * 通常顯示一個水平滾動條時是否隱藏它。
        * @default: false
        */
       hideScrollbar?: boolean;
   
       /**
        * 在初始化時是否啟用鼠標交互。您以後可以隨時切換此參數。
        * @default: true
        */
       interact?: boolean;
       /**
        * （與區域插件一起使用）啟用選定區域的循環。
        * @default:true
        */
       loopSelection?:boolean;
       /**
        * 單個畫布的最大寬度（以像素為單位），不包括小的重疊量（2 * pixelRatio，四捨五入為下一個偶數整數）。
        * 如果波形的長度大於此值，則將使用其他畫布來渲染該波形，這對於非常大的波形很有用，對於瀏覽器而言，太大的波形可能無法在單個畫布上繪製。此參數僅適用於MultiCanvas渲染器。
        * @type: {integer}
        * @default: 4000
        */
       maxCanvasWidth?: number;
   
       /**
        * 與backend一起使用MediaElement）這將啟用media元素的本機控件。
        */
       mediaControls?:boolean;
       
       /**
        * 'audio'或'video'。僅用於後端MediaElement。
        * @default: "audio"
        */
       mediaType?: "audio" | "video";
   
       /**
        * 音頻每秒的最小像素數。
        * 	@type: {integer}
        *  @default: 50
        */
       minPxPerSec?: number;
   
       /**
        * 如果為true，則通過最大峰值而不是1.0進行歸一化。
        * @default: false
        */
       normalize?: boolean;
   
       /**
        * 使用PeakCache可以提高大波形的渲染速度。
        * @type: {integer}
        * @default:false
        */
       partialRender?:boolean;
       /**
        * 可以設置1為更快的渲染。
        * @default:window.devicePixelRatio
        */
       pixelRatio?: number;
       /**
        * 實例化期間要註冊的一組插件定義。除非將deferInit屬性設置為，否則將直接對其進行初始化true。
        * @default:[]
        */
       plugins?:Array<any>;
   
       /**
        * 光標後面的波形部分的填充色。當progressColor和waveColor相同時，完全不呈現進度波。
        * @default: "#555"
        */
       progressColor?: string|CanvasGradient;
       /**
        * 設置為false在銷毀播放器時將媒體元素保留在DOM中。通過該loadMediaElement方法重用現有媒體元素時，這很有用。
        * @default:true
        */
       removeMediaElementOnDestroy?:boolean;
       /**
        * 可用於注入自定義渲染器。
        * @default:MultiCanvas
        */
       renderer?: string;
       /**
        * 如果設置為true調整波形大小，則調整窗口大小時。默認情況下，此時間會以100ms的超時進行去抖動。如果此參數是數字，則表示該超時。
        * @default:false
        */
       responsive?:boolean|number;
   
       /**
        * 是否以較長的波形滾動容器。否則，波形將縮小到容器寬度（請參閱fillParent）。
        * @default: false
        */
       scrollParent?: boolean;
   
       /**
        * skipForward()和skipBackward() 方法跳過的秒數。
        * @type: {float}
        * @default: 2
        */
       skipLength?: number;
       /**
        * 使用音頻通道的單獨波形進行渲染。
        * @default:false
        */
       splitChannels?:boolean;
   
       /**
        * 光標後的波形填充顏色。
        * 	@default "#999"
        */
       waveColor?: string|CanvasGradient;
       /**
        * @default:{}
        */
       xhr?:object;
       /**
        * @default:true
        * If a scrollbar is present, center the waveform around the progress
        */
       autoCenter?: boolean;
   
   
     }
     interface WaveSurfer {
       /**
        * 取消音頻文件加載過程。
        */
       cancelAjax():void;
   
       /**
        * 刪除事件，元素並斷開Web音頻節點的連接。
        */
       destroy(): void;
   
       /**
        *清除波形，就像加載了零長度音頻一樣。
        */
       empty(): void;
       /**
        * 返回當前已初始化的插件名稱的映射。
        */
       getActivePlugins():any;
       /**
        * 返回波形容器的背景色。
        */
       getBackgroundColor():string;
       
       /**
        * 以秒為單位返回當前進度。
        */
       getCurrentTime(): number;
       /**
        * 返回指示播放頭位置的光標填充顏色
        */
       getCursorColor():string;
       /**
        *  以秒為單位返回音頻剪輯的持續時間
        */
       getDuration(): number;
       /**
        *返回音頻片段的播放速度
        */
       getPlaybackRate(): number;
       /**
        * 返迴光標後面的波形的填充顏色
        */
       getProgressColor():string;
       /**
        * 返回當前音頻片段的音量
        */
       getVolume(): number;
       /**
        *返回當前的靜音狀態
        */
       getMute(): any;
   
       /**
        * 返回當前設置的過濾器數組
        */
       getFilters(): any[];
       /**
        * 返回當前的就緒狀態
        */
       getReady():any;
       /**
        * 返迴光標後的波形填充顏色
        */
       getWaveColor():string;
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
   
       /**
        *如果當前正在播放，則返回，否則返回false
        */
       isPlaying(): boolean;
   
       /**
        * 通過XHR 加載音頻，如果使用後端MediaElement，則解析為Audio元素。
        */
       load(url: string, peaks?: any[], preload?: ['none' | 'metadata' | 'auto']): void;
   
       /**
        加載來自Blob或File對象的音頻
        */
       loadBlob(url: string): void;
   
       /**
        *訂閱事件。有關所有事件的列表，請參見WaveSurfer事件
        */
       on(eventName: WavesurferEvent, callback: (x?: any) => void): Disposer;
   
       /**
        *取消訂閱活動
        */
       un(eventName: WavesurferEvent, callback: (x?: any) => void): void;
       /**
        * 取消訂閱所有事件
        */
       unAll(): void;
   
       /**
        * 停止播放
        */
       pause(): void;
   
       /**
        *從當前位置開始播放。以秒為單位可用於設置要播放的音頻範圍
        */
       play(start?: number, end?: number): void;
   
       /**
        *播放時暫停，播放時暫停
        */
       playPause(): void;
   
       /**
        * 尋求進度和居中視圖[0..1](0 =開始，1 =結束)
        */
       seekAndCenter(progress: number): void;
       /** 
        *尋求進度[0..1](0 =開始，1 =結束)
        */
       seekTo(progress: number): void;
       /**
        * 設置波形容器的背景色
        * @param color 顏色
        */
       setBackgroundColor(color):void;
       /** 
        * 設置指示播放頭位置的光標的填充顏色
        */
       setCursorColor(color):void;
       /**
        * 設置波形的高度
        * @param height 
        */
       setHeight(height):void;
       /**
        *  用於將自己的WebAudio節點插入圖中。請參閱下面的連接過濾器。
        * －－－－－－－－－－－－－－－－－－－－－－－－－－－－
        * 使用WebAudio或MediaElementWebAudio後端，您可以使用方法將自己的Web音頻節點插入圖中setFilter()。例：
        * var lowpass = wavesurfer.backend.ac.createBiquadFilter();
        * wavesurfer.backend.setFilter(lowpass);
        * －－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－－
        */
       setFilter(filters: any[]): void;
   
       /**
        *設置播放速度（0.5是半速，1正常速度，2雙倍速度等等）
        */
       setPlaybackRate(rate: number): void;
   
       /**
        * 將播放音量設置為新值[0..1]（0 =靜音，1 =最大）
        */
       setVolume(newVolume: number): void;
   
       /**
        * 使當前聲音靜音。可以是布爾值，true以使聲音靜音或false取消靜音
        */
       setMute(mute: boolean): void;
       /**
        * 設置光標後面的波形填充顏色
        * @param color 
        */
       setProgressColor(color):void;
       /**
        * 設置光標後的波形填充顏色
        * @param color 
        */
       setWaveColor(color):void;
       /**
        * 從當前位置跳數秒（使用負值向後移動）
        */
       skip(offset: number): void;
   
       /**
        * 倒數skipLength秒
        */
       skipBackward(): void;
   
       /**
        * 跳過skipLength幾秒鐘
        */
       skipForward(): void;
   
       /**
        * 設置接收器ID以更改音頻輸出設備
        * @param deviceId 
        */
       setSinkId(deviceId):void;
   
       /**
        * 停止並開始
        */
       stop(): void;
   
       /**
        * 開啟和關閉音量
        */
       toggleMute(): void;
   
       /**
        * 切換鼠標交互
        */
       toggleInteraction(): void;
   
   
       /**
        * 切換scrollParent
        */
       toggleScroll(): void;
   
       /**
        * 水平放大和縮小波形。該參數是每秒音頻的水平像素數。
        * 它還會更改參數minPxPerSec並啟用該 scrollParent選項。
        */
       zoom(pxPerSec: number): void;
   
        /**
        * Initialise the wave
        * 
        * @example
        * var wavesurfer = new WaveSurfer(params);
        * wavesurfer.init();
        * @return {this}
        */
       init(): void;
   
       /*
        * Factory
        */
       create(options: WavesurferOptions): WaveSurfer;
     }  
   
   declare var WaveSurfer:WaveSurfer;
   declare module "WaveSurfer" {
     export = WaveSurfer;
   }
   
   
   