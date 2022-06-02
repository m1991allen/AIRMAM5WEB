/**input[type="range"]初始化 */
export function RangeSlider(): void {
    const sliders = document.querySelectorAll('.range-slider');
    Array.prototype.forEach.call(sliders, slider => {
        slider.querySelector('input').addEventListener('input', event => {
            const spanelement = slider.querySelector('span');
            if (spanelement !== null && typeof spanelement !== undefined) {
                spanelement.innerHTML = event.target.value;
            }
            applyFill(event.target, 0);
        });
        applyFill(slider.querySelector('input'), 0);
    });
}

/**
 * 對input[type="range"]進行背景填充
 * @param slider 滑拉input range
 * @param setValue 進度(最小為0,最大為1)
 */
export function applyFill(slider, setValue: number) {
    if(setValue<0 || setValue >1){
        throw new Error("applyFill設定值不在範圍內，無法填充輸入顏色");
    }
    const rangeColor = { fill: '#0288D1', background: '#BDBDBD' };
    setValue = setValue || slider.min || 0;
    const percentage = setValue * 100;
    const bg = `linear-gradient(90deg, ${rangeColor.fill} ${percentage}%, ${rangeColor.background} ${percentage +
        0.1}%)`;
    slider.style.background = bg;
}
/**
 * 影片螢幕截圖
 * @param videoElement 影片
 * @param scale 縮放比例
 */
export function getScreenshot(videoElement: HTMLVideoElement, canvas: HTMLCanvasElement): string {
    const w = videoElement.videoWidth;
    const h = videoElement.videoHeight;
    const ratio = videoElement.videoWidth / videoElement.videoHeight;
    const ctx = canvas.getContext('2d');
    ctx.drawImage(videoElement, 0, 0, w, h);
    return canvas.toDataURL('image/png');
}
