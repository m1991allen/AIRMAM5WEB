import { IsNULLorEmpty, IsNullorUndefined } from '../Function/Check';
import { CurrentTimeToTimeCode } from '../Function/Frame';
import { IsImageValid } from '../Function/Image';
import { PlayerSetting } from '../initSetting';
import { RangeSlider, applyFill } from '../Function/Range';
import { AirmamImage } from '../Const/Image';
import { GetImageUrl } from '../Function/Url';
import { VideoNetworkState } from '../Enum/VideoNetworkState';
import { videoPlayerAbstractClass } from './abstractClass';
import { Logger } from './LoggerService';

/**影片Player */
export class videoPlayer extends videoPlayerAbstractClass {
    private selector: string;
    private menuselector: string;
    private totalTime: number = 0;
    private video: HTMLVideoElement;
    private posterURL: string;
    private rangeTime?: { start: number; end: number };
    private control(): HTMLElement {
        const panel = document.createElement('div');
        panel.className = 'cusgrid';
        panel.innerHTML = `<div class="cusrow">
                                <div class="cuscolumn">
                                     <span name="currentTimer">00:00:00;00</span>
                                </div>
                                <div class="cuscolumn">
                                    <div class="range-slider range-slider--video">
                                       <input name="range" class="range-slider__range" type="range" value="0" min="0" max="100" step="1">
                                       <div class="ticks" id="steplist"></div>
                                    </div>
                                </div>
                                <div class="cuscolumn">
                                     <span name="totalTimer">00:00:00;00</span>
                                </div>
                            </div>
                            <div class="cusrow">
                                <div class="ui icon small buttons cuscolumn">
                                    <button type="button" class="ui black button disabled" name="mute" title="音量" ><i class="icon volume up"></i></button>
                                    <button type="button" class="ui black button disabled" name="forward" title="往後一個影格" ><i class="icon step forward"></i></button>
                                    <button type="button" class="ui blue button disabled" name="play" title="播放/暫停" ><i class="icon play"></i></button>
                                    <button type="button" class="ui black button disabled" name="backward" title="往前一個影格" data-position="bottom left"><i class="icon step backward"></i></button>
                                </div>
                                <div class="cuscolumn">
                                   <div class="range-slider range-slider--volumn">
                                       <input name="volumnrange" class="range-slider__range" type="range" value="100" min="0" max="100" step="1">
                                       <span class="range-slider__value">100</span>
                                    </div>
                                </div>
                                <div class="ui icon small buttons cuscolumn">
                                    <button type="button" class="ui black button disabled" name="redo" title="重播" ><i class="icon redo"></i></button>
                                    <button type="button" class="ui black button disabled" name="speed" data-isspeed="0" title="播放速度" >3X</button>
                                    <button type="button" class="ui black button disabled" name="picture" title="子母畫面" ><i class="icon window restore outline"></i></button>
                                    <button type="button" class="ui black button disabled" name="fullscreen" title="全螢幕" ><i class="icon expand"></i></button>
                                </div>
                            </div>`;
        return panel;
    }
    /**
     * 初始化所需參數
     * @param selector 影片容器
     * @param menuselector 影片控制選單容器
     */
    constructor(selector: string, menuselector: string, fullscreenselector?: string) {
        super();
        const container = document.querySelector(selector);
        const menu = IsNULLorEmpty(menuselector)
            ? document.querySelector(selector)
            : document.querySelector(menuselector);
        const controlPanel = this.control();
        const video: HTMLVideoElement = document.createElement('video');
        video.crossOrigin = 'anonymous';
        this.video = video;
        this.selector = selector;
        this.menuselector = menuselector;
        container.appendChild(video);
        menu.appendChild(controlPanel);
        const playbtn: HTMLButtonElement = controlPanel.querySelector("button[name='play']");
        const frameForwardbtn: HTMLButtonElement = controlPanel.querySelector("button[name='forward']");
        const frameBackwardbtn: HTMLButtonElement = controlPanel.querySelector("button[name='backward']");
        const mutebtn: HTMLButtonElement = controlPanel.querySelector("button[name='mute']");
        const redobtn: HTMLButtonElement = controlPanel.querySelector("button[name='redo']");
        const speedbtn: HTMLButtonElement = controlPanel.querySelector("button[name='speed']");
        const fullscreenbtn: HTMLButtonElement = controlPanel.querySelector("button[name='fullscreen']");
        const picturebtn: HTMLButtonElement = controlPanel.querySelector("button[name='picture']");
        const currentTimer: HTMLSpanElement = controlPanel.querySelector("span[name='currentTimer']");
        const totaltimer: HTMLSpanElement = controlPanel.querySelector("span[name='totalTimer']");
        const volumnrange: HTMLInputElement = controlPanel.querySelector("input[name='volumnrange']");
        const range: HTMLInputElement = controlPanel.querySelector("input[name='range']");
        const rangeTick: HTMLDivElement = controlPanel.querySelector('#steplist');
        const volumnlabel: HTMLSpanElement = controlPanel.querySelector('.range-slider__value');
        /**更新進度條 */
        const updateSlider_ = (videoCurrentTime: number) => {
            const radio = Number((videoCurrentTime / video.duration).toFixed(4)); //撥放比例
            range.setAttribute('value', Number(radio * 100).toString()); //check
            applyFill(range, radio);
            currentTimer.innerText = CurrentTimeToTimeCode(videoCurrentTime, 'second');
        };
        RangeSlider();
        /*檢查子母畫面有沒有支援,否則隱藏子母畫面按鈕*/
        const isPIPSupport: boolean = (<any>document).pictureInPictureEnabled || !(<any>video).disablePictureInPicture;
        const isFirefox = navigator.userAgent.toLowerCase().indexOf('firefox') > -1; //目前火狐沒有呼叫的子母畫面api
        if (!isPIPSupport || isFirefox) {
            picturebtn.style.display = 'none';
        }
        /**加載元數據（如維度和持續時間）時運行的腳本 */
        video.onloadedmetadata = () => {
            for (let button of Array.from(menu.querySelectorAll('button'))) {
                button.classList.remove('disabled');
            }
            applyFill(volumnrange,Number(volumnrange.value)/100);
            const duration = video.duration;
            this.totalTime = duration;
            totaltimer.innerText = CurrentTimeToTimeCode(duration, 'second');
            if (!IsNullorUndefined(this.rangeTime)) {
                const startTimeCode = CurrentTimeToTimeCode(this.rangeTime.start, 'second');
                const endTimeCode = CurrentTimeToTimeCode(this.rangeTime.end, 'second');
                const leftMarginRatio = this.rangeTime.start / duration;
                const rightMarginRatio = (duration - this.rangeTime.end) / duration;
                rangeTick.style.marginLeft = `${leftMarginRatio * 100}%`;
                rangeTick.style.marginRight = `${rightMarginRatio * 100}%`;
                rangeTick.classList.add('slash');
                updateSlider_(this.rangeTime.start);
                rangeTick.innerHTML = `<span class="tick" data-time="${startTimeCode}"></span><span class="tick" data-time="${endTimeCode}"></tick>
                                       <style>
                                       input.range-slider__range[name="range"]:before{ width: calc(${range.clientWidth}px * ${leftMarginRatio}); }
                                       input.range-slider__range[name="range"]:after{ width: calc(${range.clientWidth}px * ${rightMarginRatio});}
                                       </style>`;
                window.addEventListener('resize', () => {
                    rangeTick.innerHTML = `<span class="tick" data-time="${startTimeCode}"></span><span class="tick" data-time="${endTimeCode}"></tick>
                                             <style>
                                             input.range-slider__range[name="range"]:before{ width: calc(${range.clientWidth}px * ${leftMarginRatio}); }
                                             input.range-slider__range[name="range"]:after{ width: calc(${range.clientWidth}px * ${rightMarginRatio});}
                                             </style>`;
                });
            } else {
                rangeTick.innerHTML = '';
                updateSlider_(0);
            }
        };

        /**在發生不良情況時運行的腳本 */
        video.onerror = (e: Event) => {
            switch (video.networkState) {
                case VideoNetworkState.NETWORK_EMPTY:
                    Logger.error(`video error:尚未初始化(NETWORK_EMPTY)`);
                    break;
                case VideoNetworkState.NETWORK_IDLE:
                    video.poster = GetImageUrl(AirmamImage.VideoNetworkError).href;
                    Logger.error(`video error:視頻有效且已選擇資源，但未使用網絡(NETWORK_IDLE)`);
                    break;
                case VideoNetworkState.NETWORK_LOADING:
                    Logger.error(`video error:瀏覽器正在下載數據(NETWORK_LOADING)`);
                    break;
                case VideoNetworkState.NETWORK_NO_SOURCE:
                    video.poster = GetImageUrl(AirmamImage.VideoNotfoundError).href;
                    Logger.error(`video error:未找到視頻來源(NETWORK_NO_SOURCE)`);
                    break;
                default:
                    video.poster = GetImageUrl(AirmamImage.VideoError).href;
                    Logger.error(`video error code:${video.networkState}`);
                    break;
            }
        };
        /**撥放 */
        video.onplay = () => {
            video.poster = this.posterURL;
            playbtn.innerHTML = `<i class="icon pause"></i>`;
        };
        /**撥放暫停 */
        video.onpause = () => {
            playbtn.innerHTML = `<i class="icon play"></i>`;
        };
        /*更新目前撥放時間 */
        video.ontimeupdate = () => {
            if (
                !IsNullorUndefined(this.rangeTime) &&
                (video.currentTime < this.rangeTime.start || video.currentTime > this.rangeTime.end)
            ) {
                video.currentTime = this.rangeTime.start;
            }
            updateSlider_(video.currentTime);
        };
        /**撥放結束 */
        video.onended = () => {
            video.currentTime = IsNullorUndefined(this.rangeTime) ? 0 : this.rangeTime.start;
            video.pause();
            updateSlider_(video.currentTime);
        };
        /*撥放暫停 */
        playbtn.onclick = () => {
            !video.paused ? video.pause() : video.play();
        };
        /*靜音 */
        mutebtn.onclick = () => {
            if (video.muted) {
                video.muted = false;
                mutebtn.innerHTML = '<i class="icon volume up"></i>';
                volumnlabel.innerText = volumnrange.value;
                applyFill(volumnrange, Number(volumnrange.value)/100);
            } else {
                video.muted = true;
                mutebtn.innerHTML = '<i class="icon volume off"></i>';
                volumnlabel.innerText = '0';
                applyFill(volumnrange, 0);
            }
        };
        /*往前一個影格 */
        frameForwardbtn.onclick = () => {
            const fps: number = PlayerSetting.fps; //影片禎數
            try {
                if (!video.paused) {
                    video.pause();
                }
                video.currentTime += 1 * (1 / fps);
                updateSlider_(video.currentTime);
            } catch (error) {
                Logger.error('往前一個影格錯誤:', error);
            }
        };
        /*往後一個影格 */
        frameBackwardbtn.onclick = () => {
            const fps: number = PlayerSetting.fps; //影片禎數
            try {
                if (!video.paused) {
                    video.pause();
                }
                video.currentTime += -1 * (1 / fps);
                updateSlider_(video.currentTime);
            } catch (error) {
                Logger.error('往後一個影格錯誤:', error);
            }
        };
        /*重新撥放 */
        redobtn.onclick = () => {
            try {
                const start = IsNullorUndefined(this.rangeTime) ? 0 : this.rangeTime.start;
                video.currentTime = start;
                updateSlider_(start);
            } catch (error) {
                Logger.error('重新撥放錯誤:', error);
            }
        };
        /**全螢幕 */
        fullscreenbtn.onclick = () => {
            const fullscreencontainer = IsNULLorEmpty(fullscreenselector)
                ? container
                : document.querySelector(fullscreenselector);
            try {
                if ((<any>document).fullscreenElement == fullscreencontainer || document.fullscreen) {
                    menu.classList.add('fullscreen');
                    if (document.exitFullscreen) {
                        document.exitFullscreen();
                    } else if ((<any>document).msExitFullscreen) {
                        (<any>document).msExitFullscreen();
                    } else if ((<any>document).mozCancelFullScreen) {
                        (<any>document).mozCancelFullScreen();
                    } else if ((<any>document).webkitExitFullscreen) {
                        (<any>document).webkitExitFullscreen();
                    }
                    fullscreenbtn.innerHTML = '<i class="icon expand"></i>';
                } else {
                    menu.classList.add('fullscreen');
                    if (fullscreencontainer.requestFullscreen) {
                        fullscreencontainer.requestFullscreen();
                    } else if ((<any>fullscreencontainer).webkitRequestFullScreen) {
                        (<any>fullscreencontainer).webkitRequestFullScreen();
                    } else if ((<any>fullscreencontainer).mozRequestFullScreen) {
                        (<any>fullscreencontainer).mozRequestFullScreen();
                    }
                    fullscreenbtn.innerHTML = '<i class="icon compress"></i>';
                }
            } catch (error) {
                Logger.error('全螢幕功能發生錯誤', error);
                fullscreenbtn.classList.add('disabled');
            }
        };
        /**當使用esc或任何觸發全螢幕事件*/
        document.onkeydown = function(this, event) {
            if (event.key === 'Escape' || event.keyCode === 27) {
                menu.classList.remove('fullscreen');
            }
        };
        /**子母畫面 */
        picturebtn.onclick = () => {
            picturebtn.classList.add('disabled');
            try {
                if (!(<any>document).pictureInPictureElement) {
                    if (
                        (<any>video).webkitSupportsPresentationMode &&
                        typeof (<any>video).webkitSetPresentationMode === 'function'
                    ) {
                        (<any>video).webkitSetPresentationMode(
                            (<any>video).webkitPresentationMode === 'picture-in-picture'
                                ? 'inline'
                                : 'picture-in-picture'
                        );
                    } else {
                        (<any>video).requestPictureInPicture();
                    }
                } else {
                    (<any>document).exitPictureInPicture();
                }
                picturebtn.classList.remove('disabled');
            } catch (error) {
                picturebtn.style.display = 'none';
                Logger.error('呼叫子母畫面發生錯誤', error);
            }
        };
        /**3倍速或正常速度播放 */
        speedbtn.onclick = function() {
            const isFasterSpeed = speedbtn.getAttribute('data-isspeed') !== '0';
            if (isFasterSpeed) {
                video.playbackRate = 1;
                speedbtn.innerText = '1X';
                speedbtn.setAttribute('data-isspeed', '0');
            } else {
                video.playbackRate = 3;
                speedbtn.innerText = '3X';
                speedbtn.setAttribute('data-isspeed', '1');
            }
        };
        /**進度條拖拉 */
        range.onmousedown = () => {
            !video.paused ? video.pause() : false;
        };
        /**進度條拖拉 */
        range.oninput = () => {
            !video.paused ? video.pause() : false;
            try {
                const hasRangeTime = !IsNullorUndefined(this.rangeTime);
                const realDuration = hasRangeTime ? this.rangeTime.end - this.rangeTime.start : video.duration;
                const skipTime = hasRangeTime ? this.rangeTime.start : 0;
                const current = skipTime + (Number(range.value) / Number(range.getAttribute('max'))) * realDuration;
                video.currentTime = current;
            } catch (error) {
                Logger.error('videoplayer進度條變更時發生問題,原因:', error);
            }
        };

        /**聲音拖拉 */
        volumnrange.onchange = () => {
            const volume = Number(volumnrange.value) / 100;
            video.volume = volume;
        };
        volumnrange.oninput = () => {
            applyFill(volumnrange, Number(volumnrange.value)/100);
        };
    }
    /**載入檔案 */
    public async Load(fileURL: string, posterURL: string, rangeTime?: { start: number; end: number }) {
        const container = document.querySelector(this.selector);
        const OKImage = (await IsImageValid(posterURL)) ? posterURL : GetImageUrl(AirmamImage.NoImage).href;
        this.rangeTime = rangeTime;
        const menu = IsNULLorEmpty(this.menuselector)
            ? document.querySelector(this.selector)
            : document.querySelector(this.menuselector);
        const volumnrange = <HTMLInputElement>menu.querySelector("input[name='volumnrange']");
        const volumnlabel = <HTMLSpanElement>menu.querySelector(".range-slider__value");
        const playbtn = <HTMLButtonElement>menu.querySelector("button[name='play']");
        const mutebtn=<HTMLButtonElement>menu.querySelector("button[name='mute']");
        playbtn.innerHTML = `<i class="icon play"></i>`;

        //設置影片
        const IsFileURLEmpty = IsNULLorEmpty(fileURL);
        this.posterURL = OKImage;

        const video = container.querySelector('video') || document.createElement('video');
        if (container.querySelector('video')) {
            container.appendChild(video);
        }
        !video.paused ? video.pause() : false;
        video.src = fileURL;
        video.setAttribute('poster', OKImage);
        video.currentTime = IsNullorUndefined(rangeTime) ? 0 : rangeTime.start; //Notice:初始時間要依造前端需求的參數
        // 設置音量
        if(!video.muted){
            video.volume=Number(volumnrange.value)/100;
            volumnlabel.innerText=`${volumnrange.value}`;
            applyFill(volumnrange, Number(volumnrange.value)/100);
            mutebtn.innerHTML=`<i class="icon volume up"></i>`;
        }else{
            volumnlabel.innerText='0';
            applyFill(volumnrange, 0);
            mutebtn.innerHTML=`<i class="icon volume off"></i>`;
        }
        video.load();
        if (IsFileURLEmpty || IsNullorUndefined(video)) {
            for (let button of Array.from(menu.querySelectorAll('button'))) {
                button.classList.add('disabled');
            }
        }
    }
    public GetCurrentTime(): number {
        const current = this.video.currentTime;
        return current;
    }
    public GetTotoalTime(): number {
        const duration = this.video.duration;
        return duration;
    }
    public GetTotoalTimeCode(): string {
        const duration = this.video.duration;
        const timecode = CurrentTimeToTimeCode(duration, 'second');
        return timecode;
    }
    public GetCurrentTimeCode(): string {
        const seconds = this.video.currentTime;
        const timecode = CurrentTimeToTimeCode(seconds, 'second');
        return timecode;
    }
    public SeekTo(progress: number): void {
        this.video.currentTime = this.video.duration * progress;
        if (this.video.paused) {
            this.video.play();
        }
    }
    public Pause(): void {
        this.video.pause();
    }
    public Destory(): void {
        this.video.pause();
        if ((<any>document).pictureInPictureElement) {
            (<any>document).exitPictureInPicture();
        }
    }
    public SetCurrentTime(currentSeconds: number): void {
        this.video.currentTime = currentSeconds;
    }
    public SetPoster(imgURL: string) {
        if (IsNullorUndefined(imgURL) || imgURL.length == 0) {
            return false;
        }
        this.video.poster = imgURL;
    }
}
