import { initSetting } from '../initSetting';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { GetUrl } from '../Function/Url';

/*
 宣告共用 domain
 統一宣告所有API位置
 */
const baseUrl: string = initSetting.APIUrl;

export interface API_Ann {
    /**查詢公告 */
    Search: string;
    /**新增公告 */
    Create: string;
    /**編輯公告 */
    Edit: string;
    /**刪除公告 */
    Delete: string;
    /**View:新增 */
    ShowCreate: string;
    /**View:編輯燈箱 */
    ShowEdit: string;
    /**View:刪除燈箱 */
    ShowDelete: string;
}

/**API:歸檔搬遷 */
export interface API_ArchiveMove {
    /**POST:取節點>主題列表 */
    GetSubjectList: string;
    /**POST:取節點>主題列表 >> 因應"目錄節點Queue"是否啟用需求, 目標目錄節點的主題列表要呼叫另一支預存 */
    GetSubjectList2: string;
    /**POST:取主題>檔案 */
    GetFileList: string;
    /**GET:取來源樹狀 */
    GetDir: string;
    /**POST:取目標樹狀圖(與傳入檔案一樣樣板的樹狀圖) */
    GetTargetDir: string;
    /**POST:檔案搬遷 */
    MoveSave: string;
    /**POST:主題搬遷 */
    SubjMoveSave: string;
    /**POST:樹狀節點搬遷 */
    MoveTreeNode: string;
}
/**API:預編詮釋 */
export interface API_ArcPre {
    /**Json:取預編樣板 */
    GetArcTemplete: string;
    /**JSON:查詢預編詮釋資料 */
    Search: string;
    /**JSON:新增預編詮釋資料 */
    Create: string;
    /**JSON:編輯預編詮釋資料*/
    Edit: string;
    /**JSNO:刪除預編詮釋資料*/
    Delete: string;
    /**View:編輯預編詮釋資料頁面*/
    ShowEdit: string;
    /**View:刪除預編詮釋資料頁面 */
    ShowDelete:string;
    /**View:新增預編詮釋資料頁面*/
    ShowSubCreate: string;
}

export interface API_Booking {
    /**JSON:清單列表 */
    Search: string;
    /**加入調用 */
    AddingBooking: string;
    /**呼叫設定優先權PartialView*/
    ShowEdit: string;
    /**設定優先權 */
    Edit: string;
    /**重設借調 */
    ReBooking: string;
    /**取消借調 */
    BookingCancel: string;
     /**取得檔案進度 */
     GetProgress:string;
}

export interface API_ColTemplate {
    /**樣版列表 */
    Search: string;
    /**View:編輯*/
    ShowEdit: string;
    /**View:刪除 */
    ShowDelete:string;
    /**依選擇類型回傳新增表單 */
    ShowChooseView: string;
    /**顯示代碼子檔 */
    ShowCogView: string;
    /**新增樣版 */
    CreateCopy: string;
    /**編輯樣版 */
    Edit: string;
    /**刪除樣版 */
    Delete: string;
    /**新增樣版欄位 */
    AddField: string;
    /**編輯樣版欄位 */
    EditField: string;
    /**刪除樣版欄位 */
    DeleteField: string;
    /** 樣板選單 */
    GetTemplateList: string;
}
export interface API_Delete {
    Search: string;
    /**View:刪除頁面 */
    ShowDelete: string;
    /**刪除實體檔案 */
    Delete: string;
    /**View:回復媒體檔案頁面 */
    ShowRedo: string;
    /**View:檢視刪除資訊 */
    ShowDetail: string;
    /**回復媒體檔案 */
    Redo: string;
}

export interface API_Dir {
    /**JSON:系統目錄樹狀節點 (id=0，Root directory)*/
    GetDir: string;
    /**系統目錄資訊資訊 */
    ShowInfo: string;
    /**系統目錄權限資訊 */
    ShowAuth: string;
    /**系統目錄編輯資訊 */
    ShowEditedAuth: string;
    /**系統目錄刪除 */
    DeleteOperationAuth: string;
    /**新增節點頁面 */
    ShowCreateNode: string;
    /**編輯節點頁面 */
    ShowEditNode: string;
    /**刪除節點頁面 */
    ShowDeleteNode: string;
    /**新增節點 */
    CreateNode: string;
    /**編輯節點 */
    EditNode: string;
    /**刪除節點 */
    DeleteNode: string;
    /**新增使用者權限 */
    CreateUserAuth: string;
    /**新增群組權限 */
    CreateGroupAuth: string;
    /**編輯使用者權限 */
    EditUserAuth: string;
    /**編輯群組權限 */
    EditGroupAuth: string;
}
export interface API_Group {
    /**查詢群組 */
    Search: string;
    /**新增群組 */
    Create: string;
    /**編輯群組 */
    Edit: string;
    /**刪除群組 */
    Delete: string;
    /**編輯群組開放功能 */
    SaveFunction: string;
    /**呼叫編輯PartialView*/
    ShowEdit: string;
    /**呼叫刪除PartialView*/
    ShowDelete: string;
    /**呼叫功能項目PartialView*/
    ShowFunction: string;
    /**取得 角色群組的帳號資料列表 */
    ShowAccount: string;
}

export interface API_LUpload {
    Search: string;
    /**View:詳細燈箱 */
    ShowDetail: string;
    /**View:編輯燈箱 */
    ShowEdit: string;
    /**取得檔案上傳進度 */
    GetCurrentProgress: string;
    /**編輯 */
    Edit: string;
    /**重轉 */
    ReTran: string;
     /**取得檔案進度 */
     GetProgress:string;
}

export interface API_Materia {
    /**JSON:調用清單列表 */
    Search: string;
    /**View:調用檔案(選單操作) */
    ShowBooking: string;
    /**JSON: 調用檔案*/
    Booking: string;
    /**View:段落剪輯/粗剪 */
    ShowFilmEdit: string;
    /**View:詳細 */
    ShowDetail: string;
    /**JSON:段落剪輯/粗剪 */
    FilmEdit: string;
    /**刪除調用檔案(可複選) */
    DeleteFile: string;
    /**取得多個影片檔案的Tsm狀態 */
    GetTsmStatus: string;
}

export interface API_MyBooking {
    /**JSON:清單列表 */
    Search: string;
    /**View:檢視主題資訊 */
    ShowDetail: string;
    /**取得檔案進度 */
    GetProgress:string;
}

export interface API_Notify {
    Create: string;
    Read: string;
}

export interface API_Report {
    /**查詢報表 */
    Search: string;
}

export interface API_Rule {
    Search: string;
    GetCreateRuleParams: string;
    CreateRule: string;
    ShowSubCreate: string;
    ShowEdit: string;
    ShowDelete: string;
    ShowCategoryEdit: string;
    Add: string;
    Edit: string;
    Delete: string;
    EditCategory: string;
    ActiveCategory: string;
    ActiveSubRule: string;
    GetProcessTableList: string;
}

export interface API_Search {
    /**儲存查詢條件以取得回傳Id */
    SearchParameter: string;
    Search2Page: string;
    /**全文檢索頁面 */
    Index: string;
    /**取得影片檢索條件 */
    VideoSearchParameters: string;
    /**取得聲音檢索條件 */
    AudioSearchParameters: string;
    /**取得圖片檢索條件 */
    PhotoSearchParameters: string;
    /**取得文件檢索條件 */
    DocSearchParameters: string;
    /**依查詢條件取得查詢筆數，檢索參數 */
    // GetCountData:  string;
    SearchCounts: string;
    /**取得查詢條件頁面 */
    Condition: string;
    /**依媒體類型取得檢索結果列表 */
    SearchListAsync: string;
    // ListAsync:  string;
    /**依媒體類型取得檢索結果樣版列表 */
    TemplateListAsync: string;
    /**媒體預覽 */
    Preview: string;
    /**基本資訊頁面 */
    BasicMedia: string;
    /**媒體變動欄位資訊 */
    DynamicMedia: string;
    /**View:關鍵影格 */
    KeyFrame: string;
    /**View:段落描述 */
    Paragraph: string;
    /**View:聲音專輯資訊 */
    AudioAlbumInfo: string;
    /**圖片額外資訊 */
    PhotoExifInfo: string;
    /**文件額外資訊 */
    DocInfo: string;
    /**Top5熱門關鍵字 */
    PopularKeyword: string;
    /**樣板種類 */
    SearchTemplete: string;
    /**依據樣板得到樣板的動態欄位 */
    GetAttriFieldList: string;
    /**取得多個影片檔案的Tsm狀態 */
    GetTsmStatus: string;
    /**JSON:編輯指定檔案的媒體資料 */
    EditMedia: string;
    /**View:編輯MediaData */
    ShowEditMedia: string;
    /**加入借調 */
    AddingBooking: string;
    /**取得使用者在該主題編號對於主題/影/音/圖/文的權限 */
    GetUserSubjAuth: string;
    /**檢索結果匯出 */
    SearchExportAsync: string;
}

export interface API_Shared {
    /**調用選項 */
    GetBookingOption: string;
    /**取得我的最愛 */
    GetFavorite: string;
    /**加入我的最愛 */
    AddFavorite: string;
    /**移除我的最愛 */
    DeleteFavorite: string;
    /**關鍵字 */
    PopularKeyword: string;
    /**搜尋樣板 */
    SearchTemplete: string;
    /**儲存檢索搜尋條件 */
    SearchParameter: string;
    /**搜尋頁面導向 */
    Search2Page: string;
    /**取樣版動態欄位 */
    GetAttriFieldList: string;
}
export interface API_Subject {
    /**JSON:節點的主題列表 */
    Search: string;
    /**View:新增主題 */
    ShowCreate: string;
    /**View:編輯主題*/
    ShowEdit: string;
    /**View:刪除主題 */
    ShowDelete:string;
    /**View:檢視主題資訊 */
    ShowDetail: string;
    /**View:編輯MediaData */
    ShowEditMedia: string;
    /**View:刪除MediaData */
    ShowDeleteMedia: string;
    /**View:加關鍵影格 */
    ShowAddKeyFrameView: string;
    /**View:編輯關鍵影格 */
    ShowEditKeyFrameView: string;
    /**View:刪除關鍵影格 */
    ShowDeleteKeyFrameView: string;
    /**媒體預覽 */
    Preview: string;
    /**View:新增段落描述 */
    ShowAddParagraphView: string;
    /**View:編輯段落描述 */
    ShowEditParagraphView: string;
    /**View:刪除段落描述 */
    ShowDeleteParagraphView: string;
    /**View:置換 */
    ShowReplacementView: string;
    /**JSON:新增主題 */
    Create: string;
    /**JSON:編輯主題 */
    Edit: string;
    /**JSON:刪除主題 */
    Delete: string;
    /**JSON:編輯指定檔案的媒體資料 */
    EditMedia: string;
    /**JSON:刪除指定檔案的媒體資料 */
    DeleteMedia: string;
    /**JSON:重轉 */
    Retransfer: string;
    /**JSON:取得預編詮釋資料 */
    GetArcPreList: string;
    /**JSON:取得樹狀節點的目錄權限 */
    GetUserDirAuth: string;
    /**JSON:取得主題的目錄權限 */
    GetUserSubjectAuth: string;
    /**JSON:取得檔案的目錄權限 */
    GetUserFileAuth: string;
    /**JSON:新增關鍵影格 */
    AddKeyFrame: string;
    /**JSON:編輯關鍵影格 */
    EditKeyFrame: string;
    /**JSON:刪除關鍵影格 */
    DeleteKeyFrame: string;
    /**JSON:設置關鍵影格的代表圖 */
    SetHeadFrame: string;

    /**JSON:新增段落描述 */
    AddParagraph: string;
    /**JSON:編輯段落描述 */
    EditParagraph: string;
    /**JSON:刪除段落描述 */
    DeleteParagraph: string;

    /**JSON:上傳View所需資料 */
    ShowUpload: string;
    /**JSON:媒體預覽 */
    ShowViewer: string;
    /**JSON:媒體列表 */
    ShowList: string;
    /**JSON:關鍵影格 */
    ShowKeyFrame: string;
    /**JSON:媒體資料 */
    ShowMetaData: string;
    /**JSON:段落描述 */
    ShowParaDescription: string;
    /**JSON:加入借調 */
    AddingBooking: string;
}
/**第三方系統API(文稿、公文) */
export interface API_SubjectExtend{
    /**取文檔系統的動態條件的因子 */
    DraftDropdown:string;
    /**查詢結果 */
    DraftSearch:string;
    /**設定對應 */
    DraftSetSave:string;
}
/**同義詞API */
export interface API_Synonym {
    /**搜尋同義詞 */
    Search: string;
    /**新增 */
    Create: string;
    /**編輯 */
    Edit: string;
    /**刪除 */
    Delete: string;
    /**重建 */
    Rebuild: string;
    /**View:新增 */
    ShowCreate: string;
    /**View:編輯*/
    ShowEdit: string;
    /**View:刪除*/
    ShowDelete: string;
    /**View:檢視 */
    ShowDetail: string;
}
export interface API_SysCode {
    /**系統代碼列表 */
    Search: string;
    /**設定資訊 */
    ShowCog: string;
    /**新增系統主代碼 */
    Create: string;
    /**編輯系統主代碼 */
    Edit: string;
    /**刪除系統主代碼 */
    Delete: string;
    /**新增系統子代碼 */
    CreateCode: string;
    /**編輯系統子代碼 */
    EditCode: string;
    /**刪除系統子代碼 */
    DeleteCode: string;
    /**View:新增 */
    ShowCreate: string;
    /**View:編輯*/
    ShowEdit: string;
    /**View:刪除*/
    ShowDelete: string;
}
export interface API_UserCode {
    /**自訂代碼列表 */
    Search: string;
    /**設定資訊 */
    ShowCog: string;
    /**新增自訂主代碼 */
    Create: string;
    /**編輯自訂主代碼 */
    Edit: string;
    /**刪除自訂主代碼 */
    Delete: string;
    /**新增自訂子代碼 */
    CreateCode: string;
    /**編輯自訂子代碼 */
    EditCode: string;
    /**刪除自訂子代碼 */
    DeleteCode: string;
    /**View:新增 */
    ShowCreate: string;
    /**View:編輯*/
    ShowEdit: string;
    /**View:刪除*/
    ShowDelete: string;
}

export interface API_VerifyBooking {
    Search: string;
    /**JSON:審核 */
    Verify: string;
    /**View:檢視*/
    ShowDetail: string;
    /**View:刪除*/
    ShowDelete: string;
}
export interface API_User {
    /**呼叫編輯PartialView*/
    ShowEdit: string;
    /**呼叫詳細頁面 */
    ShowDetail: string;
    /**呼叫刪除頁面 */
    ShowDelete: string;
    /**搜尋帳號 */
    Search: string;
    /**新增帳號 */
    Create: string;
    /**帳號開放功能 */
    SaveFunction: string;
    /**帳號啟用停權*/
    StopRight: string;
    /**還原密碼 */
    RestorePassword: string;
    /**編輯帳號 */
    EditUser: string;
    /**寄帳號驗證信 */
    SendEmailVerify: string;
    /**變更電子信箱 */
    ChangeEmail: string;
}
export interface API_Tsm {
    /**取得待上架磁帶清單 */
    GetPendingTape: string;
    /**取架上所有磁帶資訊 */
    GetTapeInfoInLib: string;
    /**取納管過的所有磁帶的資訊 */
    GetAllTapeInfo: string;
    /**上架功能 */
    CheckIn: string;
    /**下架功能 */
    CheckOut: string;
    /**查詢是否有上架中工作 */
    CheckInWorks: string;
}

export interface API_BatchBooking {
    /**GET:取來源樹狀 */
    GetDir: string;
    /**取得主題列表 */
    GetSubjectList: string;
    /**取得檔案列表 */
    GetFileList: string;
    /**批次調用儲存 */
    SaveBooking: string;
}

/**版權資料維護 */
export interface API_License {
    /**查詢 */
    Search: string;
    /**新增 */
    Create: string;
    /**編輯公告 */
    Edit: string;
    /**View:新增 */
    ShowCreate: string;
    /**View:編輯燈箱 */
    ShowEdit: string;
}


export const API = {
    Home: {
        BoardHotKey: GetUrl(Controller.Home, Action.BoardHotKey).href,
    },
    L_Login: {
        Search: GetUrl(Controller.L_Login, Action.Search).href,
        /**詳細資訊 */
        ShowDetail: GetUrl(Controller.L_Login, Action.Details).href,
    },
    L_Log: {
        Search: GetUrl(Controller.L_Log, Action.Search).href,
        /**詳細資訊 */
        ShowDetail: GetUrl(Controller.L_Log, Action.Details).href,
    },
    L_Search: {
        Search: GetUrl(Controller.L_Search, Action.Search).href,
        /**詳細資訊 */
        ShowDetail: GetUrl(Controller.L_Search, Action.Details).href,
        /**全文檢索查詢 */
        SearchPage: GetUrl(Controller.Search, Action.Index).href,
        /**全文檢索查詢2(new) TODO_2019/12/24 */
        Search2Page: GetUrl(Controller.Search, Action.Index2).href,
    },
    L_Tran: {
        Search: GetUrl(Controller.L_Tran, Action.Search).href,
        /**詳細資訊 */
        ShowDetail: GetUrl(Controller.L_Tran, Action.Details).href,
    },
};

/**錯誤頁面位置 */
export const ErrorPage = {
    NotFound: GetUrl(Controller.Error, Action.NotFound).href,
    ServerError: GetUrl(Controller.Error, Action.ServerError).href,
    BadRequest: GetUrl(Controller.Error, Action.BadRequset).href,
    Forbidden: GetUrl(Controller.Error, Action.Forbidden).href,
};
