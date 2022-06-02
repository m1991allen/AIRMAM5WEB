/**統計圖表資料內容 */
export interface ChartModel {
    /**月份標籤 */
    Months: Array<string>;
    /**分類與值 */
    BranchData: Array<Branch>;
}
/**分類與值 */
interface Branch {
    /**標籤 */
    LabelStr: string;
    /**數量 */
    Counts: Array<number>;
}


/**統計圖表資料內容 for 今日前10調用者 */
export interface ChartBarModel {
    /**調用使用者標籤 Array */
    UserLabels: Array<string>;
    
    /**調用者調用分類與值_清單(圖表資料 datasets) */
    BookWorkVals: Array<BranchBooking>;
}
/**今日前10調用者.分類與值 */
export interface BranchBooking {
    /**分類 */
    Type: string;
    /**分類名稱 */
    TypeStr: string;
    /**數量 */
    Values: Array<number>;
    /**圖表資料BAR 顏色HEX */
    BarColorHex: string;
}

/** 統計圖表資料 .dataset (圖表資料 datasets) */
export interface ChartDataset {
    barPercentage: number;
    barThickness: number|'flex';
    maxBarThickness: number;
    data: number[]; //[727, 589, 537, 543, 574, 25, 150, 124, 245, 111],
    label: string; //'轉檔調用',
    backgroundColor: string; //HexToRGBA(chartColors.blue, 0.3),
    hoverBackgroundColor: string; //HexToRGBA(chartColors.blue, 0.6),
    borderColor: string; //HexToRGBA(chartColors.blue, 1),
    borderWidth: number
}