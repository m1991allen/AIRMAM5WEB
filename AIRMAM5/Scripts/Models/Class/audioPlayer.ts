import { IwavesurferService } from '../Interface/Service/IwavesurferService';
import { wavesurferService } from './wavesurferService';
import { initSetting, PlayerSetting } from '../initSetting';
import { GetImageUrl } from '../Function/Url';
import { AirmamImage } from '../Const/Image';
import { CurrentTimeToTimeCode } from '../Function/Frame';
import { IsFileValid } from '../Function/Image';
import { IsNULLorEmpty } from '../Function/Check';
import { applyFill } from '../Function/Range';
import { GetImage } from '../Templete/ImageTemp';
import { audioPlayerAbstractClass } from './abstractClass';
import { Logger } from './LoggerService';

/**音頻player */
export class audioPlayer extends audioPlayerAbstractClass {
    private selector: string;
    private player: IwavesurferService;
    private totalTime: number = 0;
    private init(selector: string): wavesurferService {
        return new wavesurferService({
            container: selector,
            scrollParent: true,
            responsive: true,
            height: 270 /*因為預覽區域與video共用關係,用影片的固定高度270px*/,
            barHeight: 1,
            barWidth: 2,
            mediaType: 'audio',
            // waveColor: LinearGradientCanvas(),
            backgroundColor: '#212121',
            progressColor: '#2196F3',
            cursorColor: '#fff',
            waveColor: '#888',
            autoCenter: true,
            xhr: {
                cache: 'default',
                mode: 'cors',
                method: 'GET',
                credentials: 'same-origin',
                redirect: 'follow',
                referrer: 'client',
            },
        });
    }
    private control(): HTMLElement {
        const imageUrl = GetImageUrl(AirmamImage.AudioPreview).href;
        const panel = document.createElement('div');
        panel.className = 'cusgrid';
        panel.innerHTML = `<div class="cusrow">
                                <div class="cuscolumn">
                                     <span name="currentTimer">00:00:00;00</span>
                                </div>
                                <div class="cuscolumn">
                                    <div class="range-slider">
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
                                <button type="button" class="ui black button disabled" name="speed" data-isspeed="0" title="播放速度" >3X</button>
                                <button type="button" class="ui black button disabled" name="redo" title="重播" ><i class="icon redo"></i></button>
                                <button type="button" class="ui black button disabled" name="forward" title="往後一個影格" ><i class="icon step forward"></i></button>
                                <button type="button" class="ui blue button disabled" name="play" title="播放/暫停" ><i class="icon play"></i></button>
                                <button type="button" class="ui black button disabled" name="backward" title="往前一個影格" data-position="bottom left"><i class="icon step backward"></i></button>    
                                </div>
                                <div class="cuscolumn">
                                   <div class="range-slider">
                                       <input name="volumnrange" class="range-slider__range" type="range" value="100" min="0" max="100" step="1">
                                       <span class="range-slider__value">100</span>
                                    </div>
                                </div>
                            </div>`;
        return panel;
    }
    constructor(selector: string) {
        super();
        this.selector = selector;
        const container = document.querySelector(selector);
        this.player = this.init(selector);
        (async () => {
            const _player = this.player;
            const _this = this;
            await (() => {
                container.appendChild(this.control());
            })();
            await (() => {
                const playbtn = <HTMLButtonElement>container.querySelector("button[name='play']");
                const frameForwardbtn = <HTMLButtonElement>container.querySelector("button[name='forward']");
                const frameBackwardbtn = <HTMLButtonElement>container.querySelector("button[name='backward']");
                const mutebtn = <HTMLButtonElement>container.querySelector("button[name='mute']");
                const redobtn = <HTMLButtonElement>container.querySelector("button[name='redo']");
                const speedbtn = <HTMLButtonElement>container.querySelector("button[name='speed']");
                const currenttimer = <HTMLSpanElement>container.querySelector("span[name='currentTimer']");
                const totaltimer = <HTMLSpanElement>container.querySelector("span[name='totalTimer']");
                const volumnrange = <HTMLInputElement>container.querySelector("input[name='volumnrange']");
                const progressrange = <HTMLInputElement>container.querySelector("input[name='range']");
                const volumnlabel = <HTMLSpanElement>container.querySelector('.range-slider__value');
                /*就緒時才更新總時長*/
                _player.on('ready', function() {
                    _this.totalTime = _player.getDuration();
                    totaltimer.innerText = CurrentTimeToTimeCode(_player.getDuration(), 'second');
                    for (let button of Array.from(container.querySelectorAll('button'))) {
                        button.classList.remove('disabled');
                    }
                });
                _player.on('play', function() {
                    playbtn.innerHTML = `<i class="icon pause"></i>`;
                });
                _player.on('pause', function() {
                    playbtn.innerHTML = `<i class="icon play"></i>`;
                });
                /*更新目前撥放時間 */
                _player.on('audioprocess', function() {
                    const currentime = _player.getCurrentTime();
                    currenttimer.innerText = CurrentTimeToTimeCode(_player.getCurrentTime(), 'second');
                    let progress = Number((currentime / _this.totalTime).toFixed(4));
                    progress = progress > 1 ? 1 : progress < 0 ? 0 : progress;
                    const rangeValue = Math.ceil(progress * 100);
                    progressrange.value = rangeValue.toString();
                    applyFill(progressrange, rangeValue/100);
                });
                _player.on('finish', function() {
                    _player.pause();
                    _player.seekTo(0);
                    applyFill(progressrange, 0);
                });
                /**拖拉更新目前撥放時間 */
                _player.on('seek', function() {
                    if (_player.isPlaying()) {
                        _player.pause();
                    }
                    const currentime = _player.getCurrentTime();
                    currenttimer.innerText = CurrentTimeToTimeCode(currentime, 'second');
                    let progress = Number((currentime / _this.totalTime).toFixed(4));
                    progress = progress > 1 ? 1 : progress < 0 ? 0 : progress;
                    progressrange.value = Math.ceil(progress * 100).toString();
                });
                /*撥放暫停 */
                playbtn.onclick = function() {
                    _player.playPause();
                };
                /*靜音 */
                mutebtn.onclick = function() {
                    if (_player.getMute()) {
                        _player.setMute(false);
                        mutebtn.innerHTML = '<i class="icon volume up"></i>';
                        volumnlabel.innerText = volumnrange.value;
                        applyFill(volumnrange, Number(volumnrange.value)/100);
                    } else {
                        _player.setMute(true);
                        mutebtn.innerHTML = '<i class="icon volume off"></i>';
                        volumnlabel.innerText = '0';
                        applyFill(volumnrange, 0);
                    }
                };
                /*增加一個影格 */
                frameForwardbtn.onclick = function() {
                    const fps: number = PlayerSetting.fps; //影片禎數
                    try {
                        if (_player.isPlaying()) {
                            _player.pause();
                        }
                        const currentTime = _player.getCurrentTime() + 1 * (1 / fps);
                        let progress = currentTime / _this.totalTime;
                        progress = progress > 1 ? 1 : progress < 0 ? 0 : progress;
                        currenttimer.innerText = CurrentTimeToTimeCode(currentTime, 'second');
                        const progressrangeValue = Math.ceil(Number(progress.toFixed(4)) * 100);
                        progressrange.value = progressrangeValue.toString();
                        _player.seekAndCenter(progress);
                        applyFill(progressrange, progressrangeValue);
                    } catch (error) {
                        Logger.error(`音增加一個關鍵影格ERROR`, error);
                    }
                };
                /*減少一個影格 */
                frameBackwardbtn.onclick = function() {
                    const fps: number = PlayerSetting.fps; //影片禎數
                    try {
                        if (_player.isPlaying()) {
                            _player.pause();
                        }
                        const currentTime = _player.getCurrentTime() - 1 * (1 / fps);
                        let progress = currentTime / _this.totalTime;
                        progress = progress > 1 ? 1 : progress < 0 ? 0 : progress;
                        currenttimer.innerText = CurrentTimeToTimeCode(currentTime, 'second');
                        const progressrangeValue = Math.ceil(Number(progress.toFixed(4)) * 100);
                        progressrange.value = progressrangeValue.toString();
                        _player.seekAndCenter(progress);
                        applyFill(progressrange, progressrangeValue);
                    } catch (error) {
                        Logger.error(`音減少一個關鍵影格ERROR`, error);
                    }
                };
                /*重新撥放 */
                redobtn.onclick = function() {
                    _player.seekTo(0);
                    progressrange.value = '0';
                    currenttimer.innerText = CurrentTimeToTimeCode(0, 'second');
                    applyFill(progressrange, 0);
                };
                /**3倍速或正常速度播放 */
                speedbtn.onclick = function() {
                    const isspeed = speedbtn.getAttribute('data-isspeed') == '0' ? false : true;
                    if (isspeed) {
                        _player.setPlaybackRate(1);
                        speedbtn.innerText = '1X';
                        speedbtn.setAttribute('data-isspeed', '0');
                    } else {
                        _player.setPlaybackRate(3);
                        speedbtn.innerText = '3X';
                        speedbtn.setAttribute('data-isspeed', '1');
                    }
                };
                /**進度條拖拉 */
                progressrange.onmousedown = function() {
                    _player.isPlaying() ? _player.pause() : false;
                };
                progressrange.oninput = function() {
                    _player.isPlaying() ? _player.pause() : false;
                    try {
                        const progress = Number(progressrange.value) / 100;
                        const current = progress * _this.totalTime;
                        _player.seekTo(progress);
                        currenttimer.innerText = CurrentTimeToTimeCode(current, 'second');
                        applyFill(progressrange, Number(progressrange.value)/100);
                    } catch (error) {
                        Logger.error('audioplayer進度條變更時發生問題,原因:', error);
                    }
                };
                /**聲音拖拉 */
                volumnrange.onchange = function() {
                    const volumn = Number(volumnrange.value) / 100;
                    _player.setVolume(volumn);
                };
                volumnrange.oninput = function() {
                    applyFill(volumnrange, Number(volumnrange.value)/100);
                    volumnlabel.textContent = volumnrange.value;
                };
            })();
        })();
    }
    /**載入檔案 */
    public async Load(fileURL: string, posterURL: string) {
        const _player = this.player;
        const container = document.querySelector(this.selector);
        const currenttimer = <HTMLSpanElement>container.querySelector("span[name='currentTimer']");
        const totaltimer = <HTMLSpanElement>container.querySelector("span[name='totalTimer']");
        const range = <HTMLInputElement>container.querySelector("input[name='range']");
        range.value = '0';
        applyFill(range, 0);
        // const poster = (await IsImageValid(posterURL)) ? posterURL : GetImageUrl(AirmamImage.AudioPreview).href;
        if (!IsNULLorEmpty(fileURL)) {
            await (() => {
                _player.isPlaying() ? _player.stop() : false;
            })();
            await (() => {
                IsFileValid(fileURL).then(IsSuccess => {
                    if (IsSuccess) {
                        _player.load(fileURL);
                    } else {
                        document.querySelector(this.selector).innerHTML = GetImage(posterURL);
                        initSetting.ShowLog && console.error(`聲音${fileURL}檔案發生錯誤`);
                    }
                });
            })();
        } else {
            const container = document.querySelector(this.selector);
            container.innerHTML = GetImage(posterURL);
            for (let button of Array.from(container.querySelectorAll('button'))) {
                button.classList.add('disabled');
            }
        }
    }
    public GetCurrentTime(): number {
        const current = this.player.getCurrentTime();
        return current;
    }
    public GetTotoalTime(): number {
        const duration = this.totalTime;
        return duration;
    }
    public GetTotoalTimeCode(): string {
        const duration = this.totalTime;
        const timecode = CurrentTimeToTimeCode(duration, 'second');
        return timecode;
    }
    public GetCurrentTimeCode(): string {
        const seconds = this.player.getCurrentTime();
        const timecode = CurrentTimeToTimeCode(seconds, 'second');
        return timecode;
    }
    public SeekTo(progress: number): void {
        this.player.seekTo(progress);
        if (!this.player.isPlaying()) {
            this.player.play();
        }
    }
    public Pause(): void {
        this.player.pause();
    }
    public Destory(): void {
        this.player.destroy();
    }
    public SetCurrentTime(currentSeconds: number): void {
        const progress = currentSeconds / this.totalTime;
        this.player.seekAndCenter(progress);
    }
}
