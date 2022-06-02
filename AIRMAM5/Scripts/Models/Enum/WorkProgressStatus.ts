/**工作進程分類 */
export enum WorkProgressStatus {
    /**	任務已初始化，但尚未安排 */
    Created = 0,
    /**	該任務正在等待激活 */
    WaitingForActivation = 1,
    /**該任務已安排執行，但尚未開始執行 */
    WaitingToRun = 2,
    /*任務正在運行，但尚未完成* */
    Running = 3,
    /**該任務已完成執行，並且正在隱式等待附加的子任務完成 */
    WaitingForChildrenToComplete = 4,
    /**任務成功完成執行 */
    RanToCompletion = 5,
    /**取消工作 */
    Canceled = 6,
    /**	由於未處理的異常，任務已完成 */
    Faulted = 7,
}
/**簡易的工作狀態中文分類 */
export enum WorkProgressEasyChineseStatus {
    尚未開始 = 0,
    正在執行 = 1,
    已完成 = 2,
    取消 = 3,
    錯誤 = 4,
    其他工作狀態 = 5,
}
