import { IsNULLorEmpty } from './Check';
import { PlayerSetting} from '../initSetting';

/**
 * 關鍵影格時間碼轉秒數
 * @param keycodetime 關鍵影格時間碼
 * @return {number} 回傳秒數
 */
export function KeyTimeToSeconds(keycodetime: string): number {
    if (!IsNULLorEmpty(keycodetime)) {
        const second = parseInt(keycodetime.slice(0, 6));
        const mssecond = parseInt(keycodetime.slice(6, 9));
        const totalseconds = second + mssecond / 1000;
        return totalseconds;
    } else {
        return 0;
    }
}

/**
 * 將現在時間轉為秒數==>轉為關鍵影格時間顯示
 * @param seconds 將現在時間轉換成秒數
 * @param type 傳入的秒數單位是毫秒或秒
 * @returns {string} 格式化後的 HH:mm:ss;XX(影格)
 * Notice:
 * (1)定義:NTSC電視系統：1秒鐘＝30格 (格為剪輯單位)，1格＝2圖場
 * (2)Drop Frame Timecode:利用丟時間碼來減少彩色副載波一半掃描頻率，進而減少彩色副載波對黑白視訊所產生的干擾，讓同一視訊中能兼容黑白與彩色視訊載波的一種妥協方法※但是並沒有視訊影格真正被丟棄，而是採用（跳過時間標籤）的方式來計算。
 * (3)如果要減少彩色副載波對黑白視訊所產生的干擾，每小時的影片檔案需要丟棄/跳過的影格為: 60分鐘 *60秒 * (30-29.97)=108格(代表NTSC 525系統,每60分鐘要跳過108(fps)/30(fps/s)=3.6秒)
 */
export function CurrentTimeToTimeCode(seconds: number, type: 'second' | 'millisecond'): string {
    const showTwoDigits = (number: number) => {
        return ('00' + number).slice(-2);
    };
    const totalSeconds = type === 'second' ? seconds : seconds / 1000;
    const fps: number = PlayerSetting.fps; //影片禎數(fps/s)
    const frames = totalSeconds * fps; //這些時間內總共有多少影格(fps)

    let itmp = frames % 107892; //每一小時有多少影格(fps)=60*60*fps=3600*29.97=107892;

    const HH = Math.floor(frames / 107892);
    let MM = Math.floor(itmp / 17982) * 10; //每分鐘有多少影格(fps)=60*fps=60*29.97=1,798.2，所以17982=10分鐘總影格數
    itmp = itmp % 17982; //每10分鐘一循環的剩餘影格數
    if (itmp > 2) {
        MM = MM + Math.floor((itmp - 2) / 1798);
        itmp = (itmp - 2) % 1798;
        itmp = itmp + 2;
    }
    const SS = Math.floor(itmp / 30);
    const FF = Math.floor(itmp % 30);
    /*
     if(HH < 10) HH = "0" + HH;
     if(MM < 10) MM = "0" + MM;
     if(SS < 10) SS = "0" + SS;
     if(FF < 10) FF = "0" + FF;
  */
    return `${showTwoDigits(HH)}:${showTwoDigits(MM)}:${showTwoDigits(SS)};${showTwoDigits(FF)}`;
}
/**
 * 增減關鍵影格
 * @param currentSeconds 目前時間總時長(秒或毫秒)
 * @param type 單位(秒或毫秒)
 * @param duration 區間(一次跳幾格)
 * @param  totalSeconds 影片或聲音總時長
 * @returns {object} { finalTime 目前時間以毫秒回傳, finalTimeCode 目前時間以關鍵影格時間回傳}
 */
export function Framechange(
    currentSeconds: number,
    type: 'second' | 'millisecond',
    duration: number,
    totalSeconds: number
): { finalTime: number; finalTimeCode: string } {
    let currenttotalSeconds = type === 'second' ? currentSeconds : currentSeconds / 1000;
    const fps: number = PlayerSetting.fps; //影片禎數
    currenttotalSeconds =
        currenttotalSeconds < 0 ? 0 : currenttotalSeconds <= totalSeconds ? currenttotalSeconds : totalSeconds;
    currenttotalSeconds += duration * (1 / fps);
    const finaltime = CurrentTimeToTimeCode(currenttotalSeconds, 'second');
    return {
        finalTime: currenttotalSeconds,
        finalTimeCode: finaltime,
    };
}

//COPY FROM:原有轉換碼Scripts/player/video.js

// function showTwoDigits(number) {
//     return ("00" + number).slice(-2);
// }
// function frame2timecode(frames) {
//     var HH = 0;
//     var MM = 0;
//     var SS = 0;
//     var FF = 0;
//     var itmp = 0;

//     //frames = frames - 1;
//     HH = Math.floor(frames / 107892);
//     itmp = frames % 107892;

//     //2011/05/05 by Mike
//     MM = Math.floor(itmp / 17982) * 10;
//     itmp = itmp % 17982;
//     if (itmp > 2)
//     {
//         MM = MM +(Math.floor((itmp - 2) / 1798));
//         itmp = (itmp - 2) % 1798;
//         itmp = itmp + 2;
//     }
//     SS = Math.floor(itmp / 30);
//     FF = Math.floor(itmp % 30);
//     /*
//     if(HH < 10) HH = "0" + HH;
//     if(MM < 10) MM = "0" + MM;
//     if(SS < 10) SS = "0" + SS;
//     if(FF < 10) FF = "0" + FF;
//     */
//     return (showTwoDigits(HH) + ":" + showTwoDigits(MM) + ":" + showTwoDigits(SS) + ";" + showTwoDigits(FF));
// }

// var framechange = function(args){
//     video[0].pause();
//     $('.btnPlay').removeClass('paused');
//     video[0].currentTime += args * (1 / fps);
// };
