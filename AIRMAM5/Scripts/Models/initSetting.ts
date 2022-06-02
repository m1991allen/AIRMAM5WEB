import {
    IinitSetting,
    IToastrSetting,
    ITabulatorSetting,
    ITSMSetting,
    IFileSetting,
    IRightMenuSetting,
    IPlayerSetting,
    IConfigJson,
    ISignalRSetting,
    ISearchSetting,
} from './Interface/IinitSetting';
import { Controller } from './Enum/Controller';
var _json = require('../config.setting.json');

const setting: { [key: string]: IConfigJson } = _json;

/**
 *一般環境共用設定
 */
export const initSetting: IinitSetting = {
    /**是否為Debug模式 */
    DebugMode: setting[process.env.NODE_ENV].DebugMode,
    /**是否顯示console.log */
    ShowLog: setting[process.env.NODE_ENV].ShowLog,
    WebUrl: setting[process.env.NODE_ENV].WebUrl,
    APIUrl: setting[process.env.NODE_ENV].APIUrl,
    SignalRUrl: setting[process.env.NODE_ENV].SignalRUrl,
    TableId: '#Table',
    SearchRangeDay:7
};
/**儀錶板設定 */
export const DashBoardSetting={
    ShowGuageChart:setting[process.env.NODE_ENV].OpenDashBoardGuageChart
};

/**文件系統設定 */
export const DocumentSetting={
/**是否顯示文稿設定按鈕 */
 ShowDocSystemButton:setting[process.env.NODE_ENV].OpenDocumentSystem,
 /**是否顯示公文設定按鈕 */
 ShowOfficeDocSystemButton:setting[process.env.NODE_ENV].OpenOfficeDocumentSystem
};

/**提示訊息共用設定 */
export const ToastrSetting: IToastrSetting = {
    closeButton: false,
    debug: false,
    newestOnTop: false,
    progressBar: false,
    positionClass: 'toast-bottom-right',
    preventDuplicates: false,
    onclick: null,
    showDuration: 300,
    hideDuration: 1000,
    timeOut: 5000,
    extendedTimeOut: 1000,
    showEasing: 'swing',
    hideEasing: 'linear',
    showMethod: 'fadeIn',
    hideMethod: 'fadeOut',
};
/**列表共用設定 */
export const TabulatorSetting: ITabulatorSetting = {
    height: '100%',
    layout: <'fitColumns'>'fitColumns',
    pagination: <'local'>'local',
    selectable: 1,
    placeholder: "<div class='NoDateImg'><i class='file alternate icon'></i><p>查無資料</p></div>",
    pagenoInputClassName: 'ui inverted transparent input',
    pagenoBtnClassName: 'ui yellow button',
    locale: true,
    paginationSize: setting[process.env.NODE_ENV].paginationSize,
    paginationSizeSelector: [10, 20, 50, 100],
    langs: {
        ajax: {
            loading: '搜尋中',
            error: '發生錯誤',
        },
        pagination: {
            page_size: '顯示',
            first: "<i class='angle double left icon'></i>",
            first_title: '首頁',
            last: "<i class='angle double right icon'></i>",
            last_title: '尾頁',
            prev: "<i class='angle left icon'></i>",
            prev_title: '上一頁',
            next: "<i class='angle right icon'></i>",
            next_title: '下一頁',
        },
    },
};
/**全文檢索設定 */
export const SearchSetting:ISearchSetting = {
    /**全文檢索的功能Id */
    FunctionId: 'D0004',
    /**全文檢索的辨識Iframe Name */
    FrameName: 'searchframe',
    /**全文檢索分頁開啟時的文字 */
    FrameText: '全文檢索',
    /**是否開啟加入我的最愛功能 */
    CanFavorite: false,
    /**每頁筆數 */
    pageSize: 20,
    /**起始頁碼 */
    startIndex: 1,
    /**是否可使用全文檢索功能 */
    OpenSearchExportButton:setting[process.env.NODE_ENV].OpenSearchExportButton,
    /**
     * 全文檢索結果的顯示樣式,有'list'或'detail'兩種顯示
     * list =新聞部要的純文字列表(以標題為主)
     * detail= 有詳細資料,檢索內容和圖片的列表
     */
    SetSearchViewStyle:setting[process.env.NODE_ENV].SetSearchViewStyle,
    /**全文檢索結果的顯示是否要進行分群,true=需要分群(年度),false=不要分群 */
    SetSearchGroup:setting[process.env.NODE_ENV].SetSearchGroup,
};
/**分頁右鍵選單設定 */
export const RightMenuSetting: IRightMenuSetting = {
    /**分頁右鍵選單Id */
    MenuId: 'rightMenu',
    /**常駐視窗的class,不能開啟關閉分頁與加入常用 */
    PermanentWindowClass: 'permanent',
};

/**播放器設定 */
export const PlayerSetting: IPlayerSetting = {
    /**影片禎數*/
    fps: 29.97,
};

/**檔案設定 */
export const FileSetting: IFileSetting = {
    /**進度條更新時間(秒) */
    ProgressUpdateIntervalSeconds: 10,
    /**當發生錯誤時,最多可以重新嘗試幾次 */
    ErrorCount: 3,
};
/**TSM設定 */
export const TSMSetting: ITSMSetting = {
    /**最佳取得Tsm狀態分批數量 */
    bestGetTsmLength: setting[process.env.NODE_ENV].bestGetTsmLength,
    /**我的調用清單iframe name屬性 */
    MateriaFrame: Controller.Materia,
};

/**SignalR設定 */
export const SignalRSetting: ISignalRSetting = {
    ErrorTryInterval: setting[process.env.NODE_ENV].ErrorTryInterval,
    MaxTryTime:setting[process.env.NODE_ENV].MaxTryTime,
};
