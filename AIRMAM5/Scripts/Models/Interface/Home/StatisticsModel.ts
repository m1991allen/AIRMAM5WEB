/** (入庫/調用)統計值 */
export interface StatisticsModel {
    /** 分類: 1今日入庫,2今日調用,3本月入庫,4本月調用 */
    Category: number;
    /**分類: 1今日入庫,2今日調用,3本月入庫,4本月調用 */
    LabelStr: string;
    /** 數量(影/音/圖/文 加總) */
    Counts: number;
}
