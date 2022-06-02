/**動作 */
export enum Action {
    /*----------------------回傳頁面用--------------------------*/
    Index2 = 'Index2' /**全文檢索查詢2(new) TODO_2019/12/24 */,
    Index = 'Index',
    ShowEdit = '_Edit',
    ShowDetails = '_Details',
    ShowDelete = '_Delete',
    ShowCreate = '_Create',
    ShowSubCreate = '_SubCreate',
    ShowRedo = '_Redo',
    ShowUpload = '_Upload',
    ShowDeleteMedia = '_DeleteMedia',
    ShowEditMedia = '_EditMedia',
    ShowKeyFrameAdd = '_KeyFrameAdd',
    ShowKeyFrameEdit = '_KeyFrameEdit',
    ShowKeyFrameDelete = '_KeyFrameDelete',
    ShowAddParagraphView = '_ParagraphAdd',
    ShowEditParagraphView = '_ParagraphEdit',
    ShowDeleteParagraphView = '_ParagraphDelete',
    ShowChangeMedia = '_ChangeMedia',
    /*-----------------------系統代碼/自訂代碼用------------------*/
    /**代碼子檔 */
    ShowCog = '_Cog',
    /**新增子代碼 */
    CreateCode = 'CreateCode',
    DeleteCode = 'DeleteCode',
    EditCode = 'EditCode',
    /*-----------------------共用功能------------------*/
    Search = 'Search',
    Edit = 'Edit',
    Details = 'Details',
    Delete = 'Delete',
    Create = 'Create',
    Matain = 'Matain',
    Function = 'Funcs',
    /*-----------------------帳號群組維護------------------*/
    /**功能列表 */
    FuncsSave = 'FuncsSave',
    /**帳號啟用停權*/
    UpdateActive = 'UpdateActive',
    /*還原密碼*/
    RestorePwd = 'RestorePwd',
    /**回復媒體檔案 */
    RestoreARC = 'RestoreARC',
    /**回復媒體檔案 */
    Redo = 'Redo',
    /*重建*/
    Rebuild = 'Rebuild',
    /**寄帳號驗證信 */
    SendEmailVerify = 'SendEmailVerify',
    /**變更電子信箱 */
    ChangeEmail = 'ChangeEmail',
    /**顯示角色群組的帳號資料 */
    ShowAccount = 'ShowAccount',
    /*-----------------------系統目錄------------------*/
    /*刪除節點*/
    ShowDeleteDir = '_DeleteDir',
    /*新增節點*/
    ShowCreateDir = '_CreateDir',
    /**編輯節點 */
    ShowEditDir = '_EditDir',
    /**系統目錄資訊頁面 */
    ShowDirInfo = '_DirInfo',
    /**系統目錄權限 */
    ShowDirAuth = '_DirAuth',
    /**顯示系統目錄編輯頁面 */
    ShowDirAuthEdit = '_DirAuthEdit',

    /**系統目錄樹狀節點 (id=0，Root directory) */
    GetDir = 'GetDir',
    /**系統目錄刪除 */
    DeleteOperationAuth = 'DeleteOperationAuth',
    /**新增節點 */
    CreateDir = 'CreateDir',
    /**編輯節點 */
    EditDir = 'EditDir',
    /**刪除節點 */
    DeleteDir = 'DeleteDir',
    /**新增使用者權限 */
    CreateUserAuth = 'CreateUserAuth',
    /**新增群組權限*/
    CreateGroupAuth = 'CreateGroupAuth',
    /**編輯使用者權限 */
    EditUserAuth = 'EditUserAuth',
    /**編輯群組權限 */
    EditGroupAuth = 'EditGroupAuth',
    /*-------------------上傳進度------------------*/
    GetCurrentProgress = 'GetCurrentProgress',
    /*-----------------------預編詮釋資料用------------------*/
    /**重轉 */
    ReTran = 'ReTran',
    /**---------------------=自訂樣板用---------------------=*/
    /**依選擇類型回傳新增表單 */
    ShowChooseType = '_ChooseType',
    /**??? */
    Copy = 'Copy',
    /**新增樣版欄位 */
    AddField = 'AddField',
    /**編輯樣版欄位 */
    EditField = 'EditField',
    /**刪除樣版欄位 */
    DeleteField = 'DeleteField',
    /**預編詮釋資料列表 */
    GetArcPreList = 'GetArcPreList',
    /*-----------------------主題與目錄-----------------------*/
    /**主題頁面 */
    ShowSubject = '_Subject',
    /**影片頁面 */
    ShowVideo = '_Video',
    /**聲音頁面 */
    ShowAudio = '_Audio',
    /**圖片頁面 */
    ShowPhoto = '_Photo',
    /**文件頁面 */
    ShowDoc = '_Doc',
    /**媒體預覽畫面 */
    ShowViewer = '_Viewer',
    /**媒體列表 */
    ShowList = '_List',
    /**媒體資料 */
    ShowMetaData = '_MetaData',
    /**關鍵影格 */
    ShowKeyFrame = '_KeyFrame',
    /**段落描述 */
    ShowParaDescription = '_ParaDescription',
    /**編輯指定檔案的媒體資料 */
    EditMedia = 'EditMedia',
    /**刪除指定檔案的媒體資料 */
    DeleteMedia = 'DeleteMedia',
    /**關鍵影格設置代表圖 */
    SetHeadFrame = 'SetHeadFrame',
    /**新增關鍵影格 */
    KeyFrameAdd = 'KeyFrameAdd',
    /**編輯關鍵影格 */
    KeyFrameEdit = 'KeyFrameEdit',
    /**刪除關鍵影格 */
    KeyFrameDelete = 'KeyFrameDelete',
    /**重轉 */
    ReTransfer = 'ReTransfer',
    /**新增段落描述 */
    AddParagraph = 'ParagraphAdd',
    /**編輯段落描述 */
    EditParagraph = 'ParagraphEdit',
    /**刪除段落描述 */
    DeleteParagraph = 'ParagraphDelete',
    /**取得樹狀節點的目錄權限 */
    GetUserDirAuth = 'GetUserDirAuth',
    /**取得主題的目錄權限 */
    GetUserSubjAuth = 'GetUserSubjAuth',
    /**取得檔案的目錄權限 */
    GetUserFileAuth = 'GetUserFileAuth',
    /*-------------文檔或公文查詢-------------- */
    /**取文檔系統的搜尋條件因子 */
    DraftDropdown='SubjExtendView',
    /**文檔條件搜尋 */
    DraftSearch='SubjExtendSearch',
    /**文檔設定對應 */
    DraftSetSave='SubjExtendUpdate',
    /*----------------------全文檢索---------------------*/
    /**檢索前,先記錄 搜索參數 以取得查詢Id */
    SearchParameter = 'SearchParameter',
    /**熱門關鍵字 */
    PopularKeywords = 'PopularKeywords',
    /**樣板種類 */
    GetTemplateList = 'GetTemplateList',
    /**依據樣板得到樣板的動態欄位 */
    GetAttriFieldList = 'GetAttriFieldList',
    /**取得查詢筆數，檢索參數*/
    SearchCounts = 'SearchCounts',
    /**查詢條件顯示 */
    ShowCondition = '_Condition',
    /**檢索結果列表 */
    SearchListAsync = 'SearchListAsync',
    // ShowListAsync = '_ListAsync',
    /**檢索結果樣版列表 */
    TemplateListAsync = 'TemplateListAsync',
    /**媒體預覽 */
    ShowPreview = '_Preview',
    /**基本媒體資訊 */
    ShowBasicMedia = '_BasicMedia',
    /**媒體變動欄位資訊 */
    ShowDynamicMedia = '_DynamicMedia',
    /**聲音專輯資訊 */
    ShowAudioAlbumInfo = '_AudioAlbumInfo',
    /**圖片額外資訊 */
    ShowPhotoExifInfo = '_PhotoExifInfo',
    /**文件額外資訊 */
    ShowDocInfo = '_DocInfo',
    /**取得影片類型的檔案的TSM狀態 */
    GetTsmStatus = 'GetTsmStatus',
    /**檢索結果匯至檔案 */
    SearchExportAsync = 'SearchExportAsync',
    /*----------------------調用---------------------*/
    /**加入調用 */
    AddBooking = 'AddBooking',
    /**重設借調 */
    ReBooking = 'ReBooking',
    /**取消借調 */
    BookingCancel ='BookingCancel',
    /*----------------------我的調用清單---------------------*/
    /**調用檔案(選單操作view) */
    ShowBooking = '_Booking',
    Booking = 'Booking',
    ShowFilmEdit = '_FilmEdit',
    /**段落剪輯/粗剪 */
    FilmEdit = 'FilmEdit',
    /*---------------------批次調用---------------------------*/
    MediaFileList = 'MediaFileList',
    /*------------------------訊息通知----------------------*/
    CreateNotify = 'CreateNotify',
    ReadNotify = 'ReadNotify',
    /*------------------------審核調用----------------------*/
    Verify = 'Verify',
    /*----------------------------規則庫------------------*/
    CreateRuleParams = 'CreateRuleParams',
    /**顯示編輯主規則(流程燈箱) */
    ShowCategoryEdit = '_CategoryEdit',
    /**編輯主規則 */
    EditCategory = 'CategoryEdit',
    /**啟用或關閉主規則 */
    ActiveCategory = 'ActiveCategory',
    /**啟用或關閉全站審 */
    ActiveRuleFilter = 'ActiveRuleFilter',
    /**取得子規則的下拉選單 */
    GetProcessTableList = 'GetProcessTableList',
    /**新增子規則 */
    SubCreate = 'SubCreate',

    /*-----------------------歸檔搬遷---------------------*/
    GetSubjFiles = 'GetSubjFiles',
    GetTargetDir = 'GetTargetDir',
    /**檔案搬移 */
    MoveSave = 'MoveSave',
    /**主題搬移 */
    SubjMoveSave = 'SubjMoveSave',
    /**樹狀節點搬移 */
    MoveTreeNode = 'MoveTreeNode',
    GetSubjectsByDirFilter = 'GetSubjectsByDirFilter',
    /*-----------------------儀錶板------------------*/
    BoardHotKey = 'BoardHotKey',
    /*--------------------磁帶管理---------------------- */
    /**取架上所有磁帶資訊 */
    GetTapeInfoInLib = 'GetTapeInfoInLib',
    /**取納管過的所有磁帶的資訊 */
    GetAllTapeInfo = 'GetAllTapeInfo',
    /**取得待上架磁帶清單 */
    GetPendingTape = 'GetPendingTape',
    /**上架功能 */
    CheckIn = 'TapeCheckIn',
    /**下架功能 */
    CheckOut = 'TapeCheckOut',
    /**查詢是否有上架中工作 */
    CheckInWorks = 'TapeCheckInWorks',
    /**上架頁面Action */
    IndexCheckIn = 'IndexCheckIn',
    /**下架頁面Action */
    IndexCheckOut = 'IndexCheckOut',
    /*--------------------共用---------------------- */
    GetBookingOption = 'GetBookingOption',
    /**取得我的最愛 */
    GetFavorite = 'GetFavorite',
    /**加入我的最愛 */
    AddFavorite = 'AddFavorite',
    /** 移除我的最愛*/
    DelFavorite = 'DelFavorite',
    /**登出 */
    LogOff = 'LogOff',
    /**登入 */
    Login = 'Login',
    /*----------------------=錯誤---------------------*/
    /**404*/
    NotFound = 'NotFound',
    /**500*/
    ServerError = 'ServerError',
    /**400 */
    BadRequset = 'BadRequest',
    /**403,405 */
    Forbidden = 'Forbidden',
}
