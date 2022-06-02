declare global {
    interface Window {
        /**離開頁面是否需要確認 */
        LeaveConfirm: Array<string>;
        /**已完成更新的workId */
        FinishWorkIds: Array<number>;
        /**暫存iframe找到的進度條元素 */
        TempProgress: Array<Element>;
        /**儀錶板圖表暫存 */
        TempChart: Chart;
        /**堆疊圖表暫存 */
        StackedChart: Chart;
        /**調用量測圖表-主機入庫數_20210520 */
        TempGaugeARC: Chart;
        /**調用量測圖表-主機調用數_20210520 */
        TempGaugeBOOK: Chart;
    }
}


/**調用量測圖表 分類代碼 */
export const gaugeChartType = {
    ARC: 'guageUpload',
    BOOK: 'guageBoooking'
};

window.LeaveConfirm = [];
window.FinishWorkIds = [];
window.TempProgress = [];
window.TempChart = null;
window.StackedChart = null;
window.TempGaugeARC = null;
window.TempGaugeBOOK = null;

/**儲存離開或關閉頁面時要確認的頁面路徑 */
export function SetLeaveConfirm(pageUrl: string) {
    window.top.LeaveConfirm.indexOf(pageUrl) == -1 ? window.top.LeaveConfirm.push(pageUrl) : false;
}
/**移除離開或關閉頁面時要確認的頁面路徑 */
export function RemoveLeaveConfirm(pageUrl: string) {
    window.top.LeaveConfirm = window.top.LeaveConfirm.filter(url => url !== pageUrl);
}