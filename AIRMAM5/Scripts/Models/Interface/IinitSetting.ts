import { SearchViewStyle } from '../Enum/SearchViewStyle';
import { Controller } from '../Enum/Controller';

export interface IConfigJson {
    /**是否為debug模式 */
    DebugMode: boolean;
    /**是否顯示log */
    ShowLog: boolean;
    /**所有資源的基本路徑(如noImage) */
    WebUrl: string;
    /**所有API的基本路徑 */
    APIUrl: string;
    /**SignalR路徑 */
    SignalRUrl: string;
    /**一頁顯示幾列 */
    paginationSize: number;
    /**最佳取得Tsm狀態分批數量 */
    bestGetTsmLength: number;
    /**signalR斷線後,幾秒後重連 */
    ErrorTryInterval: number;
    /**SignalR斷線最大嘗試次數 */
    MaxTryTime: number;
    /**是否啟用全文檢索的匯出功能 */
    OpenSearchExportButton:boolean;
    /**是否啟用儀錶板的路數作業,前10大調用圖表 */
    OpenDashBoardGuageChart:boolean;
    /**是否啟用文稿系統 */
    OpenDocumentSystem:boolean;
    /**是否啟用公文系統 */
    OpenOfficeDocumentSystem:boolean;
     /**
     * 全文檢索結果的顯示樣式,有'list'或'detail'兩種顯示
     * list =新聞部要的純文字列表(以標題為主)
     * detail= 有詳細資料,檢索內容和圖片的列表
     */
    SetSearchViewStyle:SearchViewStyle;
    /**全文檢索結果的顯示是否要進行分群,true=需要分群(年度),false=不要分群 */
    SetSearchGroup:boolean;
}

/**一般環境設定 */
export interface IinitSetting {
    /**是否為debug模式,作為跑假資料或 */
    DebugMode: boolean;
    /**是否顯示log */
    ShowLog: boolean;
    /**所有資源的基本路徑(如noImage) */
    WebUrl: string;
    /**所有API的基本路徑 */
    APIUrl: string;
    /**SignalR路徑 */
    SignalRUrl: string;
    /**所有Table的Id */
    TableId: string;
    /**查詢範圍天數(表單查詢日期範圍時) */
    SearchRangeDay:number;
}
/**提示訊息設定 */
export interface IToastrSetting {
    /**是否顯示提示關閉按鈕 */
    closeButton: boolean;
    debug: boolean;
    newestOnTop: boolean;
    /**是否顯示提示進度條 */
    progressBar: boolean;
    positionClass: string;
    preventDuplicates: boolean;
    /**提示點擊事件 */
    onclick: null;
    showDuration: number;
    hideDuration: number;
    timeOut: number;
    extendedTimeOut: number;
    showEasing: string;
    hideEasing: string;
    showMethod: string;
    hideMethod: string;
}
/**列表設定 */
export interface ITabulatorSetting {
    /**table高度 */
    height: string | number;
    /**table rwd樣式 */
    layout: 'fitDataFill' | 'fitData' | 'fitColumns';
    /**由本地或遠端伺服器進行分頁 */
    pagination: 'local' | 'remote';
    /**一頁顯示幾列 */
    paginationSize: number;
    placeholder: string;
    /**頁碼input輸入樣式 */
    pagenoInputClassName: string;
    /**頁碼Go button樣式 */
    pagenoBtnClassName: string;
    /**可選行 */
    selectable: boolean | number | 'highlight';
    /**依客戶端本地語言顯示 */
    locale: boolean;
    /**可選擇的頁碼下拉選單 */
    paginationSizeSelector: Array<number>;
    /**語言 */
    langs: {
        /**ajax時的顯示文字 */
        ajax: {
            /**table 加載中的樣式 */
            loading: string;
            /**table 發生錯誤時的樣式 */
            error: string;
        };
        /**頁碼樣式 */
        pagination: {
            /**分頁按鈕樣式 */
            page_size: string;
            /**首頁樣式 */
            first: string;
            /**首頁tooltip */
            first_title: string;
            /**尾頁樣式 */
            last: string;
            /**尾頁tooltip */
            last_title: string;
            /**前一頁樣式 */
            prev: string;
            /**前一頁tooltip */
            prev_title: string;
            /**下一頁樣式 */
            next: string;
            /**下一頁tooltip */
            next_title: string;
        };
    };
}

export interface IRightMenuSetting {
    /**分頁右鍵選單Id */
    MenuId: string;
    /**常駐視窗的class,不能開啟關閉分頁與加入常用 */
    PermanentWindowClass: string;
}
export interface IPlayerSetting {
    /**影片禎數*/
    fps: number;
}
export interface IFileSetting {
    /**進度條更新時間(秒) */
    ProgressUpdateIntervalSeconds: number;
    /**當發生錯誤時,最多可以重新嘗試幾次 */
    ErrorCount: number;
}

export interface ITSMSetting {
    /**最佳取得Tsm狀態分批數量 */
    bestGetTsmLength: number;
    /**我的調用清單iframe name屬性 */
    MateriaFrame: Controller;
}

/**SignalR設定 */
export interface ISignalRSetting {
    /**幾秒後重新連線 */
    ErrorTryInterval: number;
    /**最大嘗試次數 */
    MaxTryTime:number;
}
/**全文檢索設定 */
export interface ISearchSetting{
    /**全文檢索的功能Id */
    FunctionId:string;
    /**全文檢索的辨識Iframe Name */
    FrameName:string;
    /**全文檢索分頁開啟時的文字 */
    FrameText:string;
    /**是否開啟加入我的最愛功能 */
    CanFavorite:boolean;
    /**每頁筆數 */
    pageSize:number;
    /**起始頁碼 */
    startIndex:number;
    /**是否可使用全文檢索功能 */
    OpenSearchExportButton:boolean;
    /**
     * 全文檢索結果的顯示樣式,有'list'或'detail'兩種顯示
     * list =新聞部要的純文字列表(以標題為主)
     * detail= 有詳細資料,檢索內容和圖片的列表
     */
    SetSearchViewStyle:SearchViewStyle;
    /**全文檢索結果的顯示是否要進行分群,true=需要分群(年度),false=不要分群 */
    SetSearchGroup:boolean;
}