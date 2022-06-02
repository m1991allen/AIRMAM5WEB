import { IFullTextSearchInput } from '../../Models/Interface/Search/IFullTextSearchInput';
import { MediaType, SearchTypeChineseEnum, SearchTypeEnum } from '../../Models/Enum/MediaType';
import {
    SearchResponseBaseModel,
    SearchCountResponseModel,
    ConditionModel,
    FileStatusResult,
} from '../../Models/Interface/Search/ISearchResponseVideoModel';
import { initSetting, SearchSetting, TSMSetting, TabulatorSetting } from '../../Models/initSetting';
import { ISearchResult } from '../../Models/Interface/Search/ISearchResult';
import { GetImage } from '../../Models/Templete/ImageTemp';
import { GetImageUrl } from '../../Models/Function/Url';
import { TabNameEnum, TabNameChineseEnum } from '../../Models/Enum/TabNameEnum';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { HomoMode } from '../../Models/Enum/HomoMode';
import { SynonymousChineseMode } from '../../Models/Enum/SynonymousMode';
import { AirmamImage } from '../../Models/Const/Image';
import { HttpStatusCode } from '../../Models/Enum/HttpStatusCode';
import { TSMFileStatus } from '../../Models/Enum/TSMFileStatus';
import { ShowModal, ModalTask } from '../../Models/Function/Modal';
import { audioPlayer } from '../../Models/Class/audioPlayer';
import { videoPlayer } from '../../Models/Class/videoPlayer';
import { CreateMaterialModel } from '../../Models/Interface/Materia/CreateMaterialModel';
import { renderBody } from '../../Models/Const/Const.';
import { Color } from '../../Models/Enum/ColorEnum';
import { CurrentTimeToTimeCode } from '../../Models/Function/Frame';
import { PermissionDefinition } from '../../Models/Enum/PermissionDefinition';
import { setCalendar } from '../../Models/Function/Date';
import { AddDynamicNullable, CheckForm } from '../../Models/Function/Form';
import { FormValidField } from '../../Models/Const/FormValid';
import { SearchMessageSetting } from '../../Models/MessageSetting';
import { SubjectPreviewModel } from '../../Models/Interface/Subject/SubjectPreviewModel';
import { UI } from '../../Models/Templete/CompoentTemp';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { GetTsmColor, GetTsmImgUrlByStatus, GetTsmTextByStatus } from '../../Models/Function/TSM';
import { SearchController, ISearchController } from '../../Models/Controller/SearchController';
import { Logger } from '../../Models/Class/LoggerService';
import { Filter } from '../../Models/Enum/Filter';
import { ErrorMessage, InfoMessage, SuccessMessage } from '../../Models/Function/Message';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { GetDropdown } from '../../Models/Function/Element';
import { CreateSelect } from '../../Models/Templete/FormTemp';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { RemoveLeaveConfirm } from '../Shared/_windowParameter';
import { IsImageValid } from '../../Models/Function/Image';
import { SearchViewStyle } from '../../Models/Enum/SearchViewStyle';



/*----------------------------宣告變數-------------------------------- */
/**要不要使用tsm */ const IsUseTsm: boolean = renderBody.getAttribute('data-IsUseTsm') === 'true' ? true : false;
/**要不要開啟非雲端功能 */ const IsNonCloud: boolean =renderBody.getAttribute('data-IsNonCloud') === 'true' ? true : false;
/**暫存已查詢過tsm狀態的檔案結果*/var savefilesResult: Array<FileStatusResult> = [];
/**暫存檔案編號*/var savefileNos: Array<string> = [];

const valid = FormValidField.Search;
const message = SearchMessageSetting;
const $Main = $('#Main');
const $dataTitle = $Main.find("div[name='dataTitle']");
const $dataTab = $Main.find("div[name='dataTab']");
const $Condition = $Main.find("div[name='condition']");
const $List = $Main.find(`div[name='list']`);
const $Preview = $Main.find("div[name='preview']");
const ActiveClass = 'cusactive';
const EditMediaId = '#EditMediaModal';
/**編輯按鈕 */ const $EditBtn: JQuery<HTMLElement> = $("button[name='edit']");
/**暫存創建的videoplayer */ var vtempPlayer: videoPlayer = null;
/**暫存創建的audioplayer */ var atempPlayer: audioPlayer = null;
var tableService: tabulatorService;
/**暫存目前的類型 */
var currentMediaType:MediaType=MediaType.VIDEO;
/**暫存各類別的檢索資料數量*/
var currentSearchCount:SearchCountResponseModel=null;
var Parameter = <IFullTextSearchInput>JSON.parse($Main.attr('data-Parameter'));
var route: ISearchController = new SearchController();
/**回傳Modal性質*/
const prop = (key: keyof ISearchResult): string => {
    return route.GetProperty<ISearchResult>(key);
};
/**圖片放大 */
const lightbox=(src:string)=>{
    const lightbox = document.createElement('div');
    const img = GetImage(src, '預覽圖', ['x-open']);
    lightbox.innerHTML = img;
    lightbox.className = 'x-lightbox';
    lightbox.onclick = function() {
        lightbox.parentNode.removeChild(lightbox);
    };
    document.body.appendChild(lightbox);
};
/*文檔就緒後移除#Main的loading樣式 */
$(document).ready(function() {
    $('.loader').remove();
});
/**
 * 列表篩選
 */
$(document).on('keyup','#wordFilter',function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('CreateDate'), type: Filter.Like, value: word },
        { field: prop('Title'), type: Filter.Like, value: word },
        { field: prop('TSMFileStatusStr'), type: Filter.Like, value: word },
        { field: prop('Duration'), type: Filter.Like, value: word },
        { field: prop('FileType'), type: Filter.Like, value: word },
        { field: prop('fsMATCH'), type: Filter.Like, value: word },
    ];
    tableService.SetFilter(filter);
});
/*Notice:
 * split-paney在瀏覽器大小快速變更時,若不在該頁面上,則右側pane會無法取得相對寬度,導致跑版
 * 必須在瀏覽器大小變更時重置
 */
window.onresize = function() {
    $('div.split-pane').splitPane('lastComponentSize', 50);
};

/**
 * 依照媒體檢索類型取得圖片標籤,影=時間,音=時間,圖=像素,文=檔案副檔名
 * @param data 檢索列表的列資訊
 */
const DurationLabel = (data: ISearchResult): string => {
    switch (data.SearchType) {
        case MediaType.VIDEO:
        case MediaType.AUDIO:
            return `<span>${data.Duration}</span>`;
        case MediaType.PHOTO:
        case MediaType.Doc:
        case MediaType.SUBJECT:
            return `<span>${data.FileType}</span>`;
    }
};

/**
 * 清空右下資訊分頁
 */
const EmptyTabs = (): void => {
    $dataTitle.empty();
    $dataTab.empty();
};

/**
 * 回傳沒有資料的右下資訊分頁顯示畫面
 * @param tab 英文名稱
 * @param iconClass 符號樣式
 */
const ReturnNodataTab = (tab: TabNameEnum, iconClass: string): string => {
    const tabname = getEnumKeyByEnumValue(TabNameChineseEnum, tab);
    const segment = UI.Error.CorrectSegment('查無資料', '建議嘗試使用其他關鍵字', iconClass, Color.紅);
    return `<div class="ui bottom attached active tab" data-tab="${tab}">
            ${segment.outerHTML}
            </div>`;
};
/**
 * 回傳錯誤的右下資訊分頁顯示畫面
 * @param tab 英文名稱
 * @param iconClass 符號樣式
 */
const ReturnErrorTab = (tab: TabNameEnum, iconClass: string): string => {
    const tabname = getEnumKeyByEnumValue(TabNameChineseEnum, tab);
    const segment = UI.Error.CorrectSegment(
        ` Oops,載入${tabname}發生錯誤`,
        '看起來發生了一些狀況...',
        iconClass,
        Color.紅
    );
    return `<div class="ui bottom attached tab" data-tab="${tab}">
            ${segment.outerHTML}
            </div>`;
};

/**沒有資料列表時的呈現 */
const noDataList = (text: string, status: HttpStatusCode): void => {
    const nodata = document.createElement('div');
    const imageurl = GetImageUrl(AirmamImage.NoData).href;
    const image = GetImage(imageurl, '查無資料', ['ui', 'image', 'medium', 'centered']);
    nodata.innerHTML = UI.Error.CorrectSegment(image, text, Color.灰).outerHTML;
    $List.empty().html(nodata.innerHTML);
};
/**插入右下資訊標題 */
const insertInfoTitle = (input: Array<{ tabName: TabNameEnum; tabstr: string }>): void => {
    for (let item of input) {
        const tabLength = $dataTitle.find(`a.item[data-tab='${item.tabName}']`).length;
        if (tabLength > 1) {
            $dataTitle
                .find(`a.item[data-tab='${item.tabName}']`)
                .first()
                .remove();
        } else {
            /*預設基本資料active*/
            if (tabLength == 0) {
                const extraclass = item.tabName === TabNameEnum.BaseMeta ? 'active' : '';
                $dataTitle.append(`<a class="item ${extraclass}" data-tab="${item.tabName}">${item.tabstr}</a>`);
            }
        }
    }
};
/**
 * 插入右下資訊分頁
 * @param input 分頁資訊{分頁元素名,分頁顯示名稱,分頁html}
 */
const insertInfoTab = (input: { tabName: TabNameEnum; tabhtml: string }): void => {
    (async () => {
        await (() => {
            if ($dataTab.find(`.tab[data-tab='${input.tabName}']`).length > 0) {
                $dataTab.find(`.tab[data-tab='${input.tabName}']`).remove();
            }
        })();
        await (() => {
            $dataTab.append(input.tabhtml);
        })();
    })();
};
/**
 * 將查詢條件json轉文字顯示
 * @param input 表單參數
 */
const getConditionStrFromParameter = (input: IFullTextSearchInput): ConditionModel => {
    const interval =
        IsNULLorEmpty(input.clsDATE.fdSDATE) && IsNULLorEmpty(input.clsDATE.fdEDATE)
            ? '沒有指定日期區間'
            : `${input.clsDATE.fdSDATE}~${input.clsDATE.fdEDATE}`;
    const type = ((): string => {
        let gettype: Array<string> = [];
        const types = input.fsINDEX.split(',');
        for (let item of types) {
            gettype.push(`${getEnumKeyByEnumValue(SearchTypeChineseEnum, item)}`);
        }
        return gettype.length == 0 ? '沒有指定媒體類型' : gettype.join('、');
    })();
    const mode = ((): string => {
        const gethomo: Array<string> = [];
        const homo = input.fnHOMO === HomoMode.Open ? '同音' : '';
        const synonymous = getEnumKeyByEnumValue(SynonymousChineseMode, input.fnSEARCH_MODE);
        if (!IsNULLorEmpty(homo)) {
            gethomo.push(homo);
        }
        if (!IsNULLorEmpty(synonymous)) {
            gethomo.push(synonymous);
        }
        return gethomo.join('、');
    })();
    const query = ((): string => {
        const getquery: Array<string> = [];
        for (let item of input.lstCOLUMN_SEARCH) {
            getquery.push(`${item.fsCOLUMN}=${item.fsVALUE}`);
        }
        return getquery.join('&');
    })();
    const res: ConditionModel = {
        SearchType: type,
        SearchMode: mode,
        DateInterval: interval,
        AdvancedQry: query,
    };
    return res;
};
/**檢查要以哪種類型先呈現(零資料略過,並以影音圖文順序判斷,若全無資料則預設以影呈現) */
const checkWhichMediaShowFirst=(m:SearchCountResponseModel):{type:MediaType;searchType:SearchTypeEnum,count:number;isAllzero:boolean;}=>{
  return m.fnVIDEO_COUNT>0?{type:MediaType.VIDEO,searchType:SearchTypeEnum.VIDEO,count:IsNullorUndefined(m.fnVIDEO_COUNT)?0:m.fnVIDEO_COUNT,isAllzero:false}:
         m.fnAUDIO_COUNT>0?{type:MediaType.AUDIO,searchType:SearchTypeEnum.AUDIO,count:IsNullorUndefined(m.fnAUDIO_COUNT)?0:m.fnAUDIO_COUNT,isAllzero:false}:
         m.fnPHOTO_COUNT>0?{type:MediaType.PHOTO,searchType:SearchTypeEnum.PHOTO,count:IsNullorUndefined(m.fnPHOTO_COUNT)?0:m.fnPHOTO_COUNT,isAllzero:false}:
         m.fnDOC_COUNT>0?{type:MediaType.Doc,searchType:SearchTypeEnum.DOC,count:IsNullorUndefined(m.fnDOC_COUNT)?0:m.fnDOC_COUNT,isAllzero:false}:
         {type:MediaType.VIDEO,searchType:SearchTypeEnum.Unknown,count:IsNullorUndefined(m.fnVIDEO_COUNT)?0:m.fnVIDEO_COUNT,isAllzero:true};
}
/**檢查目前頁面上選擇的檢視樣式 */
const ButtonViewStyle={
    get:():SearchViewStyle=>{
        return $('button[name="viewstyle"]').attr('viewstyle') as SearchViewStyle;
    },
    set:(value:SearchViewStyle)=>{
        $('button[name="viewstyle"]').attr('viewstyle',value.toString());
    }
}

/*----------------------------View Task-------------------------------- */
/**
 * 創建查詢區域
 * @param input 檢索內容(筆數,顯示文字)
 */
const condition = (input: SearchResponseBaseModel): void => {
    route
        .Condition(input)
        .then(view => {
            $Main.find("div[name='condition']").html(view);
        })
        .catch(error => {
            Logger.viewres(route.api.Condition, '取得查詢條件區塊', error, false);
            $Main.find("div[name='condition']").html(`
                        <div class="ui small breadcrumb _SearchStyleLinchpin">
                        查詢"<span>關鍵字：未知</span>＂結果：共 <span>０</span>筆
                       <i class="angle right link icon" style="margin: 0 0 0 2px;" id="MoreConditions"></i>     
                      <div class="_Custom" id="MoreC">
                        <div class="_filter">檢索類型：<span>ㄒ</span></div>
                        <div class="_filter"><label>查詢方式：</label> <span>無法讀取</span> </div>
                        <div class="_filter"> 日期區間： <span>無法讀取</span> </div>
                      </div>
                      </div>
                      <div class="ui secondary pointing menu tabs _SearchStyleTabs">
                          <a class="active item" data-type="V">影片(<span>0</span>)</a>
                          <a class="item" data-type="A">音(<span>0</span>)</a>
                          <a class="item" data-type="D">圖(<span>0</span>)</a>
                          <a class="item" data-type="P">文(<span>0</span>)</a>
                      </div>`);
        })
        .then(() => {
            ButtonViewStyle.set(SearchSetting.SetSearchViewStyle);//會影響初始顯示的檢索樣式
            changetab();
            templatelist(checkWhichMediaShowFirst(input.CountData).type);
            exportcsv();

        });
};
/**CSV匯出功能點擊 */
const  exportcsv=()=>{
 const $exportBtn=$Main.find(".button[name='export']");
 $exportBtn.click(function(){
    const $templeteDropdown = GetDropdown('div[name="templeteItem"]','templeteDropdown');
    const mediatype= $Condition.children().find('a.active.item').attr('data-type');
    const mediatypeEnum: MediaType = MediaType[getEnumKeyByEnumValue(MediaType,mediatype)];
    const selectTempId:number=Number($templeteDropdown.dropdown('get value'));
    if(selectTempId.toString() === StringEnum.NoData){
        // InfoMessage('沒有可選擇的樣版,無法匯出');
        $exportBtn.addClass('disabled');
    }else{
        $exportBtn.removeClass('disabled');
        let copyParameter:IFullTextSearchInput=JSON.parse(JSON.stringify(Parameter));/*Notice:一定要深度拷貝才不會影響共用的Paramter暫存參數*/
        copyParameter.fnTEMP_ID=selectTempId;
        route.SearchExportAsync(mediatypeEnum,copyParameter)
        .then(res => {
            res.IsSuccess? SuccessMessage(`匯出工作將開始，請耐心等候，完成後下載連結將顯示在"提醒訊息"中`):ErrorMessage(`匯出工作無法執行,可能是系統端導致，請稍後重試或聯絡系統管理員`);
            Logger.res(route.api.SearchExportAsync,'匯出csv',res,false);
        }).catch(error=>{
            ErrorMessage(`匯出工作無法執行,可能是系統端導致，請稍後重試或聯絡系統管理員`)
            Logger.viewres(route.api.SearchExportAsync,'匯出csv',error,false);
        });
    }
  
});
};
/**從暫存的數量結果取得最大頁數 */
const getMaxPage=()=>{
    switch(currentMediaType){
        case MediaType.VIDEO:
            return Math.ceil(currentSearchCount.fnVIDEO_COUNT/SearchSetting.pageSize);
        case MediaType.AUDIO:
            return Math.ceil(currentSearchCount.fnAUDIO_COUNT/SearchSetting.pageSize);
        case MediaType.PHOTO:
            return Math.ceil(currentSearchCount.fnPHOTO_COUNT/SearchSetting.pageSize);
        case MediaType.Doc:
            return Math.ceil(currentSearchCount.fnDOC_COUNT/SearchSetting.pageSize);
        case MediaType.SUBJECT:
            return Math.ceil(currentSearchCount.fnSUBJECT_COUNT/SearchSetting.pageSize);
    }
}

/**
 * 創建列表區域
 * @param mediaType 媒體類型VADPS
 * @param pageno 頁碼
 * @param clear 是否要清空原來的列表區域
 * @param  totalDataLength 總資料筆數
 */
const list = (pageno: number, totalDataLength: number): Promise<Array<ISearchResult>> => {
    return new Promise(resolve => {
        const list = document.createElement('div');
        const imageurl = GetImageUrl(AirmamImage.NoData).href;
        const image = GetImage(imageurl, '查無資料', ['ui', 'image', 'medium', 'centered']);
        list.className = 'ui divided items _ListMenu';
       // MaxPageNo =totalDataLength > 0 ? Math.ceil(totalDataLength / SearchSetting.pageSize) : SearchSetting.startIndex;
        let IsRowSelected: boolean = false;
        let firstRowData: ISearchResult | false = false;
        let allRowData: Array<ISearchResult> = [];
        Parameter.fnSTART_INDEX = SearchSetting.pageSize * (pageno - 1) + 1;
        tableService = new tabulatorService(
            initSetting.TableId,
            {
                height: TabulatorSetting.height,
                layout: 'fitColumns',
                selectable: 1,
                placeholder: UI.Error.CorrectSegment(image, `查無資料`, Color.灰).outerHTML,
                addRowPos: 'top',
                virtualDom:false,
                initialSort :[ { column :prop('CreateDate') , dir :"desc" }],
                // groupBy:function(data:ISearchResult){
                //     return IsNULLorEmpty(data.CreateDate)?"":data.CreateDate.split(/\//g)[0];
                // },
                // groupStartOpen: true,
                groupToggleElement: 'header',
                // groupHeader:[
                //     function(value,count:number,data:Array<ISearchResult>){
                //         const msg=data.length===0?"":data[0].CreateDate.split(/\//g)[0];
                //         return `<div class="x-license-group"><i class="ui icon flag red"></i>${msg}年度</div>`;
                //     }
                // ],
                pagination: 'remote',
                paginationSize: SearchSetting.pageSize,
                paginationInitialPage: SearchSetting.startIndex,
                paginationButtonCount: 3,
                headerVisible: false /*隱藏標題列*/,
                resizableRows: false,
                resizableColumns:false,
                index: prop('fsFILE_NO'),
                pageLoaded: function(pageno: number) {
                    if (currentMediaType === MediaType.VIDEO) {
                        GetTsmAPI(currentMediaType, allRowData);
                    }
                },
                footerElement:`<div name="filter" class="ui basic compact center aligned segment">
                                  <div class="ui inverted mini grey header">
                                      本頁快速篩選：
                                      <div class="ui inverted transparent input">
                                          <input autocomplete="off" id="wordFilter" type="text" placeholder="請輸入篩選詞彙">
                                      </div>
                                  </div>
                              </div>`,
                ajaxURL: route.api.SearchListAsync,
                ajaxContentType: 'json',
                ajaxConfig: 'POST',
                ajaxParams: <{ mediaType: MediaType; model: IFullTextSearchInput }>{
                    mediaType:currentMediaType,
                    model: {
                        fsKEYWORD: Parameter.fsKEYWORD,
                        fsINDEX: Parameter.fsINDEX,
                        fnSEARCH_MODE: Parameter.fnSEARCH_MODE,
                        fnHOMO: Parameter.fnHOMO,
                        clsDATE: Parameter.clsDATE,
                        lstCOLUMN_ORDER: Parameter.lstCOLUMN_ORDER,
                        fnTEMP_ID: Parameter.fnTEMP_ID,
                        lstCOLUMN_SEARCH: Parameter.lstCOLUMN_SEARCH,
                        fnPAGE_SIZE: Parameter.fnPAGE_SIZE,
                        fnSTART_INDEX: SearchSetting.pageSize * (pageno - 1) + 1
                        //fnSTART_INDEX: Parameter.fnSTART_INDEX,
                    },
                },
                ajaxRequesting: function(url: string, param: { mediaType: MediaType; model: IFullTextSearchInput }) {
                    let pageno = IsNullorUndefined(tableService) ? 1 : tableService.GetTable().getPage();
                    if (pageno !== false) {
                        param.mediaType=currentMediaType;
                        param.model.fnSTART_INDEX = SearchSetting.pageSize * (pageno - 1) + 1;
                    }
                    return true;
                },
                ajaxResponse: function(
                    url: string,
                    param: { mediaType: MediaType; model: IFullTextSearchInput },
                    response
                ) {
                    /*Notice:
                       (1)要重設Parameter中的索引開始筆數給後端
                       (2)啟用遠端分頁,回傳的格式要為{last_page:number,data:any}
                       (3)如果先前已知的暫存各類型筆數中,該類型若筆數為0,則不能回傳資料給使用者
                    */
                    let resData = !IsNullorUndefined(response.Data) ?<Array<ISearchResult>> JSON.parse(JSON.stringify(response.Data)) :<Array<ISearchResult>> [];
                       allRowData = resData;
                    if (resData !== [] && firstRowData === false) {
                        firstRowData = (<Array<ISearchResult>>response.Data)[0];
                    }
                    
                    switch(currentMediaType){
                        case MediaType.VIDEO:
                            if(currentSearchCount.fnVIDEO_COUNT===0){resData=[]; }
                            break;
                        case MediaType.AUDIO:
                            if(currentSearchCount.fnAUDIO_COUNT===0){resData=[]; }
                            break;
                        case MediaType.PHOTO:
                            if(currentSearchCount.fnPHOTO_COUNT===0){ resData=[]; }
                            break;
                        case MediaType.Doc:
                            if(currentSearchCount.fnDOC_COUNT===0){  resData=[];}
                            break;
                    }

                    resolve(resData);
                    return JSON.parse(
                        JSON.stringify({
                            last_page: getMaxPage(),
                            data: resData,
                        })
                    );
                },
                ajaxError:function(xhr,textStatus,errorThrown){
                    $List.html(UI.Error.ErrorSegment(`查無資料[${textStatus}]`,errorThrown).outerHTML);
                    $EditBtn.hide();
                    preview(true);
                    EmptyTabs();
                    insertInfoTab({
                        tabName: TabNameEnum.BaseMeta,
                        tabhtml: ReturnNodataTab(TabNameEnum.BaseMeta, 'search'),
                    });
                },
                rowSelected: function(row: Tabulator.RowComponent) {
                    const rowElement = row.getElement();
                    if (rowElement.className.indexOf(ActiveClass) == -1) {
                        rowElement.classList.add(ActiveClass); //TODO 確認為何無效
                        const rowdata = <ISearchResult>row.getData();
                        const mediatype =currentMediaType;
                        const subjectid = rowdata.fsSUBJECT_ID;
                        const fileno = rowdata.fsFILE_NO;
                        getAuthBySubjectId(mediatype, subjectid); /*判斷使用權限以決定是否隱藏編輯按鈕*/
                        preview(false, {
                            fsSUBJECT_ID: subjectid,
                            SearchType: mediatype,
                            fsFILE_NO: fileno,
                        });
                        new Promise(resolve => {
                            EmptyTabs();
                            resolve('已清空右下分頁');
                        }).then(message => {
                            Logger.log(<string>message);
                            basicdynamicMedia(mediatype, {
                                fsSUBJECT_ID: subjectid,
                                fileNo: fileno,
                                type: mediatype,
                            });
                            updateEditBtnAttribute(subjectid, fileno, mediatype);
                            if (mediatype === MediaType.VIDEO) {
                                keyframe(fileno);
                            }
                            if (mediatype === MediaType.AUDIO) {
                                audioalbuminfo(subjectid, fileno);
                            }
                            if (mediatype == MediaType.VIDEO || mediatype == MediaType.AUDIO) {
                                paragraph(mediatype, fileno);
                            }
                            if (mediatype == MediaType.PHOTO) {
                                photoexifinfo(fileno);
                            }
                            if (mediatype == MediaType.Doc) {
                                documentInfo(fileno);
                            }
                        });
                    }
                },
                rowDeselected: function(row: Tabulator.RowComponent) {
                    row.getElement().classList.remove(ActiveClass);
                },
                rowClick:function(e,row:Tabulator.RowComponent){
                    const target=e.target;
                    const rowdata = <ISearchResult>row.getData();
                   const noImage = GetImageUrl(AirmamImage.NoImage).href;
                    if(target instanceof HTMLImageElement){
                        IsImageValid(rowdata.HeadFrame).then(isok=>{
                            isok?lightbox(rowdata.HeadFrame):lightbox(noImage);
                        });
                    }
                },
                rowFormatter: function(row: Tabulator.RowComponent) {
                    const rowdata = <ISearchResult>row.getData();
                    const rowElement = row.getElement();
                    rowElement.classList.add('ui', 'items');
                    const itemdiv: HTMLDivElement = document.createElement('div');
                    if (firstRowData !== false) {
                        if (row.getIndex() == firstRowData.fsFILE_NO && IsRowSelected === false) {
                            row.select();
                            IsRowSelected = true;
                        }
                        itemdiv.className = 'item';
                        itemdiv.setAttribute('data-fileno', rowdata.fsFILE_NO);
                        itemdiv.setAttribute('data-mediatype', currentMediaType);
                        const image = GetImage(rowdata.HeadFrame, rowdata.Title,[],'lightbox');
                        const savetsmStatus = savefilesResult.filter(item => item.FILE_NO == rowdata.fsFILE_NO);
                        /**如果陣列有tsm資料存在,就直接設為此資料的狀態 */
                        const tsmStatus =
                            savetsmStatus.length > 0
                                ? savetsmStatus[0].FILE_STATUS
                                : currentMediaType != MediaType.VIDEO
                                ? IsNonCloud
                                    ? TSMFileStatus.FileOnDisk
                                    : TSMFileStatus.Online
                                : IsUseTsm
                                ? rowdata.TSMFileStatus
                                : IsNonCloud
                                ? TSMFileStatus.FileOnDisk
                                : TSMFileStatus.Online;
                        const tsmStatusColor = GetTsmColor(<TSMFileStatus>tsmStatus);
                        const tsmStatusStr = GetTsmTextByStatus(tsmStatus);
                        const tsmStatusImg = GetTsmImgUrlByStatus(<TSMFileStatus>tsmStatus);
                        const batchBtn=rowdata.IsExpired?`<button type="button" class="ui _darkGrey mini red button booking" name="addMateria" disabled><i class="stop icon"></i>授權到期</button>`:
                                       !rowdata.IsForBid?`<button type="button" class="ui mini button booking" name="addMateria">${rowdata.IsAlert?'<span class="x-tooltip">'+rowdata.LicenseMessage+'</span>':""}<i class="plus icon"></i>加入借調</button>`:
                                       `<button type="button" class="ui _darkGrey mini red button booking" name="addMateria" disabled><i class="stop icon"></i>禁止借調</button>`;
                        const hashTags=IsNullorUndefined(rowdata.HashTag)?"":
                                       Array.isArray(rowdata.HashTag)?rowdata.HashTag.length===0?"":rowdata.HashTag.filter(x=>x!="").map(x=>`<label class="ui blue x-hashtag label">#`+x+`</label>`).join("")
                                       :rowdata.HashTag.split("^").filter(x=>x!="").map(x=>`<label class="ui blue x-hashtag label">#`+x+`</label>`).join("");//處理自訂標籤樣式
                        /**Notice:要求將<<關鍵字>>變更顏色 */
                        const fsMATCH =IsNULLorEmpty(rowdata.fsMATCH)?"": rowdata.fsMATCH.replace(/<</gm, "<span class='keypoint'>").replace(/>>/gm, '</span>');
                        /**版權是否提醒、訊息內容 */
                        const licenseMSG = rowdata.IsAlert ? `【${rowdata.LicenseMessage}】` : "";
                       switch(ButtonViewStyle.get()){
                           case SearchViewStyle.List:
                               itemdiv.innerHTML=rowElement.innerHTML;
                               break;
                           case SearchViewStyle.Detail:
                           default:
                            itemdiv.innerHTML = `<input type="hidden" value="${rowdata.fsSUBJECT_ID}"><input type="hidden" value="${rowdata.fsFILE_NO}">
                                                 <div class="ui small image _styleImg">  
                                                     ${image} 
                                                     <div class="_timeLong">${DurationLabel(rowdata)}</div>
                                                 </div>
                                                 <div class="content">
                                                      <span class="header">${rowdata.Title}</span>
                                                      <div class="description">${fsMATCH} </div>
                                                      <div class="extra _time">
                                                           <span>資料日期:${rowdata.CreateDate} </span>
                                                           <span class="x-license-label">版權：${IsNULLorEmpty(rowdata.LicenseStr)?"(未設定)":rowdata.LicenseStr}${licenseMSG}</span>
                                                          ${batchBtn}
                                                      </div>
                                                      <div class="extra">
                                                         ${hashTags}
                                                         <div class="_status ui image label" name="fileStatus"><img src="${tsmStatusImg}"> <span class="${tsmStatusColor}"> ${tsmStatusStr}</span></div>
                                                      </div>
                                                 </div>`;
                               break;
                       }
                        rowElement.innerHTML = itemdiv.outerHTML;
                    }
                },
                columns:[
                    {
                        title: '標題',
                        field: prop('Title'),
                        sorter: 'string',
                        width:'100%',
                        formatter:function(cell,formatterParams){
                            const rowdata = <ISearchResult>cell.getRow().getData(); 
                            const hashTags=IsNullorUndefined(rowdata.HashTag)?"":
                                           Array.isArray(rowdata.HashTag)?rowdata.HashTag.length===0?""
                                           :rowdata.HashTag.filter(x=>x!="").map(x=>`<label class="ui tiny blue x-hashtag label">#`+x+`</label>`).join("")
                                           :rowdata.HashTag.split("^").filter(x=>x!="").map(x=>`<label class="ui tiny blue x-hashtag label">#`+x+`</label>`).join("");//處理自訂標籤樣式
                            return `${hashTags}<div style="white-space:break-spaces;">${cell.getValue()}</div>`;
                        }
                    },
                    {
                        title:'資料日期',
                        field:prop('CreateDate'),
                        sorter:'string',
                        width:200,
                        minWidth:200,
                        formatter:function(cell,formatterParams){
                            return `<div style="white-space:break-spaces;">${cell.getValue()}</div>`;
                        }
                    },
                    {
                        title:'版權',
                        field:prop('LicenseStr'),
                        sorter:'string',
                        width:150,
                        minWidth:150,
                        formatter:function(cell,formatterParams){
                            const rowdata = <ISearchResult>cell.getRow().getData();
                            return `<div style="white-space:break-spaces;">${IsNULLorEmpty(rowdata.LicenseStr)?"(未設定)":rowdata.LicenseStr}</div>`;
                        }
                    },
                    {
                        title:'TSM狀態',
                        field:prop('TSMFileStatus'),
                        width:50,
                        minWidth:50,
                        formatter:function(cell,formatterParams){
                            const rowdata = <ISearchResult>cell.getRow().getData();
                            const savetsmStatus = savefilesResult.filter(item => item.FILE_NO == rowdata.fsFILE_NO);
                            /**如果陣列有tsm資料存在,就直接設為此資料的狀態 */
                            const tsmStatus =
                                savetsmStatus.length > 0
                                    ? savetsmStatus[0].FILE_STATUS
                                    : currentMediaType != MediaType.VIDEO
                                    ? IsNonCloud
                                        ? TSMFileStatus.FileOnDisk
                                        : TSMFileStatus.Online
                                    : IsUseTsm
                                    ? rowdata.TSMFileStatus
                                    : IsNonCloud
                                    ? TSMFileStatus.FileOnDisk
                                    : TSMFileStatus.Online;
                            const tsmStatusColor = GetTsmColor(<TSMFileStatus>tsmStatus);
                            const tsmStatusStr = GetTsmTextByStatus(tsmStatus);
                            const tsmStatusImg = GetTsmImgUrlByStatus(<TSMFileStatus>tsmStatus);
                           return `<div class="_status ui image label" name="fileStatus"><img src="${tsmStatusImg}" title="${tsmStatusStr}"></div>`;
                           
                        }
                    },
                    {
                        title:'借調動作',
                        field:prop('IsForBid'),
                        width:115,
                        minWidth:115,
                        formatter:function(cell,formatterParams){
                            const rowdata = <ISearchResult>cell.getRow().getData();
                            const batchBtn=rowdata.IsExpired?`<button type="button" class="ui _darkGrey mini red button booking" name="addMateria" disabled><i class="stop icon"></i>授權到期</button>`:
                                          !rowdata.IsForBid?`<button type="button" class="ui mini button booking" name="addMateria">${rowdata.IsAlert?'<span class="x-tooltip">'+rowdata.LicenseMessage+'</span>':""}<i class="plus icon"></i>加入借調</button>`:
                                           `<button type="button" class="ui _darkGrey mini red button booking" name="addMateria" disabled><i class="stop icon"></i>禁止借調</button>`;
                            return batchBtn;
                        }

                    }
                ]
            },
            false
        );
        //如果有啟用分群功能,就啟用分群設定
        if(SearchSetting.SetSearchGroup=== true){
            tableService.GetTable().setGroupStartOpen(true);
            tableService.GetTable().setGroupBy(function(data:ISearchResult){
                return IsNULLorEmpty(data.CreateDate)?"":data.CreateDate.split(/\//g)[0];
            });
            tableService.GetTable().setGroupHeader([
                function(value,count:number,data:Array<ISearchResult>){
                    const msg=data.length===0?"":data[0].CreateDate.split(/\//g)[0];
                    return `<div class="x-license-group"><i class="ui icon flag red"></i>${msg}年度</div>`;
                }
            ]);
          
        }
    });
};
/**
 * 檢索結果的樣版清單
 * @param mediaType 媒體類型
 * @param templeteId 樣板ID (全文檢索查詢的樣板)
 */
const templatelist = (mediaType: MediaType): void=>{
    const $templeteMenu=   $Main.find('div[name="templeteMenu"]');
    const $templeteItem= $Main.find('div[name="templeteItem"]');
    const $exportBtn =  $Main.find('.button[name="export"]');
    $templeteItem.find('.dropdown').addClass('loading');
    $exportBtn.addClass('disabled');
    const copyParameter:IFullTextSearchInput=JSON.parse(JSON.stringify(Parameter));
    if(SearchSetting.OpenSearchExportButton===false ){
        $templeteItem.remove();
    }else{
        let $newSelectHTML:string="";
        let hasTemplete:boolean=false;
        route.TemplateListAsync(mediaType,copyParameter)
        .then(async res=>{
            Logger.res(route.api.TemplateListAsync,'樣版清單',res,false);
            hasTemplete=(<Array<SelectListItem>>res.Data).length>0 && res.Data !==[];
            const items= hasTemplete?<Array<SelectListItem>>res.Data:[<SelectListItem>{Text:'無樣板',Value:'-1',Selected:true,Disabled:false}];
            if(hasTemplete){
                $exportBtn.removeClass('disabled');  
                const $newSelect=CreateSelect(items,false,'templeteDropdown');
                $newSelectHTML=$newSelect.outerHTML;
            }else{
                $newSelectHTML="";
            }
        })
        .catch(error=>{
            const $newSelect=CreateSelect([<SelectListItem>{Text:'錯誤發生',Value:'-1',Selected:true,Disabled:false}],false,'templeteDropdown');
            $templeteItem.html($newSelect.outerHTML).find('select[name="templeteDropdown"]').dropdown().addClass('disabled');
            Logger.viewres(route.api.TemplateListAsync, '依媒體類型取得檢索結果的樣版列表', error, false);
            $newSelectHTML="",hasTemplete=false;
        }).finally(()=>{
            $templeteMenu.attr('style','display:flex;padding:2px')
            $templeteItem.find('.dropdown').removeClass('loading');
            $templeteItem.html($newSelectHTML).find('select[name="templeteDropdown"]').dropdown().addClass(hasTemplete?'':'disabled');
        });
    }
};

/**
 * 創建預覽區域
 * @param IsNoData 是否沒有資料
 * @param subjectId 主題Id
 * @param mediaType 媒體類型
 * @param fileNo 檔案編號
 */
const preview = (
    IsNoData: boolean,
    input?: {
        fsSUBJECT_ID: string;
        SearchType: MediaType;
        fsFILE_NO: string;
    }
): void => {
    const $preview = $Preview;
    !IsNULLorEmpty(vtempPlayer) ? vtempPlayer.Destory() : false;
    !IsNULLorEmpty(atempPlayer) ? atempPlayer.Destory() : false;
    if (IsNoData) {
        atempPlayer = null;
        vtempPlayer = null;
        $preview.empty().html(
            `<div class="ui placeholder segment" style="background-color:#333 !important;width:100%;height:100%;box-shadow:none;border-bottom: 0.2px solid #2f2f2f;">
                   <div class="ui inverted header">沒有可預覽的檔案</div>
                  </div>`
        );
    } else {
        route
            .Preview(input.fsSUBJECT_ID, input.SearchType, input.fsFILE_NO)
            .then(view => {
                $preview.empty().html(view);
                const embed = $preview.find('.ui.embed');
                const fileurl: string = embed.attr('data-url');
                const imageurl: string = embed.attr('data-placeholder');
                switch (input.SearchType) {
                    case MediaType.VIDEO:
                        vtempPlayer = new videoPlayer(
                            "div[name='preview'] .embed",
                            '#videoMenu',
                            '#fullScreenContainer'
                        );

                        vtempPlayer.Load(fileurl, imageurl);
                        break;
                    case MediaType.AUDIO:
                        atempPlayer = new audioPlayer("div[name='preview'] .embed");
                        atempPlayer.Load(fileurl, imageurl);
                        break;
                    default:
                        break;
                }
            })
            .catch(error => {
                Logger.viewres(route.api.Preview, '載入預覽圖', error, false);
                $preview.empty().html(UI.Error.ErrorSegment('載入預覽圖失敗', '請重新整理頁面或確定網路狀態'));
            });
    }
};
/**
 * 創建基本與動態資料區域
 */
const basicdynamicMedia = (type: MediaType, input: SubjectPreviewModel) => {
    const basictext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.BaseMeta);
    const dynamictext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.DetailMeta);
    switch (type) {
        case MediaType.VIDEO:
            insertInfoTitle([
                {
                    tabName: TabNameEnum.BaseMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.BaseMeta),
                },
                {
                    tabName: TabNameEnum.DetailMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.DetailMeta),
                },
                {
                    tabName: TabNameEnum.KeyFrame,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.KeyFrame),
                },
                {
                    tabName: TabNameEnum.Paragraph,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.Paragraph),
                },
            ]);
            break;
        case MediaType.AUDIO:
            insertInfoTitle([
                {
                    tabName: TabNameEnum.BaseMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.BaseMeta),
                },
                {
                    tabName: TabNameEnum.DetailMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.DetailMeta),
                },
                {
                    tabName: TabNameEnum.albumInfo,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.albumInfo),
                },
                {
                    tabName: TabNameEnum.Paragraph,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.Paragraph),
                },
            ]);
            break;
        case MediaType.PHOTO:
            insertInfoTitle([
                {
                    tabName: TabNameEnum.BaseMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.BaseMeta),
                },
                {
                    tabName: TabNameEnum.DetailMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.DetailMeta),
                },
                {
                    tabName: TabNameEnum.exifInfo,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.exifInfo),
                },
            ]);
            break;
        case MediaType.Doc:
            insertInfoTitle([
                {
                    tabName: TabNameEnum.BaseMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.BaseMeta),
                },
                {
                    tabName: TabNameEnum.DetailMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.DetailMeta),
                },
                {
                    tabName: TabNameEnum.documentInfo,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.documentInfo),
                },
            ]);
            break;
        default:
            insertInfoTitle([
                {
                    tabName: TabNameEnum.BaseMeta,
                    tabstr: getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.BaseMeta),
                },
            ]);
            break;
    }
    route
        .BasicMedia(input)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.BaseMeta,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${basictext}View時發生錯誤,api=${route.api.BasicMedia}`, error);
            insertInfoTab({
                tabName: TabNameEnum.BaseMeta,
                tabhtml: ReturnErrorTab(TabNameEnum.BaseMeta, 'database'),
            });
        });
    route
        .DynamicMedia(input)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.DetailMeta,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${dynamictext}View時發生錯誤,api=${route.api.DynamicMedia}`, error);
            insertInfoTab({
                tabName: TabNameEnum.DetailMeta,
                tabhtml: ReturnErrorTab(TabNameEnum.DetailMeta, 'database'),
            });
        });
};
/**
 * 創建關鍵影格區域
 * @param fileNo 檔案編號
 */
const keyframe = (fileNo: string) => {
    const keyframetext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.KeyFrame);
    route
        .KeyFrame(fileNo)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.KeyFrame,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${keyframetext}View時發生錯誤,api=${route.api.KeyFrame}`, error);
            insertInfoTab({
                tabName: TabNameEnum.KeyFrame,
                tabhtml: ReturnErrorTab(TabNameEnum.KeyFrame, 'keyboard'),
            });
        });
};
/**
 * 創建段落描述區域
 * @param mediaType 媒體類型VADP
 * @param fileNo 檔案編號
 */
const paragraph = (mediaType: MediaType, fileNo: string) => {
    const paragraphtext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.Paragraph);
    route
        .Paragraph(mediaType, fileNo)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.Paragraph,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${paragraphtext}View時發生錯誤,api=${route.api.Paragraph}`, error);
            insertInfoTab({
                tabName: TabNameEnum.Paragraph,
                tabhtml: ReturnErrorTab(TabNameEnum.Paragraph, 'list'),
            });
        });
};
/**
 * 創建音頻資訊區域
 * @param subjectId 主題編號
 * @param fileNo 檔案編號
 */
const audioalbuminfo = (subjectId: string, fileNo: string) => {
    const audioalbumtext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.albumInfo);
    route
        .AudioAlbumInfo(subjectId, fileNo)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.albumInfo,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${audioalbumtext}View時發生錯誤,api=${route.api.AudioAlbumInfo}`, error);
            insertInfoTab({
                tabName: TabNameEnum.albumInfo,
                tabhtml: ReturnErrorTab(TabNameEnum.albumInfo, 'music'),
            });
        });
};
/**
 * 創建圖片資訊區域
 * @param fileNo 檔案編號
 */
const photoexifinfo = (fileNo: string) => {
    const photoexiftext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.exifInfo);
    route
        .PhotoExifInfo(fileNo)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.exifInfo,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${photoexiftext}訊View時發生錯誤,api=${route.api.PhotoExifInfo}`, error);
            insertInfoTab({
                tabName: TabNameEnum.exifInfo,
                tabhtml: ReturnErrorTab(TabNameEnum.exifInfo, 'outdent'),
            });
        });
};
/**
 * 創建文件資訊區域
 * @param fileNo 檔案編號
 */
const documentInfo = (fileNo: string) => {
    const documenttext = getEnumKeyByEnumValue(TabNameChineseEnum, TabNameEnum.documentInfo);
    route
        .DocInfo(fileNo)
        .then(view => {
            insertInfoTab({
                tabName: TabNameEnum.documentInfo,
                tabhtml: view,
            });
        })
        .catch(error => {
            Logger.error(`載入${documenttext}View時發生錯誤,api=${route.api.PhotoExifInfo}`, error);
            insertInfoTab({
                tabName: TabNameEnum.documentInfo,
                tabhtml: ReturnErrorTab(TabNameEnum.documentInfo, 'file'),
            });
        });
};
/**
 * 取得該主題的編輯權限
 * @param type 媒體種類
 * @param subjectId  主題編號
 */
const getAuthBySubjectId = (type: MediaType, subjectId: string) => {
    route.GetSubjectFunctionAuth(type, subjectId).then(json => {
        Logger.log(type + '權限:' + JSON.stringify(json));
        const data = json;
        const subjAuth = <Array<PermissionDefinition>>data.LimitSubject.split(','); //const allowAuth = <Array<PermissionDefinition>>data.LimitAuth.split(',');
        subjAuth.indexOf(PermissionDefinition.U) > -1 ? $EditBtn.show() : $EditBtn.hide();
    });
};
/**
 * 點擊影音圖文分頁
 */
const changetab = (): void => {
    $Condition
        .children()
        .find('a.item')
        .click(async function() {
            $(this)
                .addClass(`active ${ActiveClass}`)
                .siblings()
                .removeClass(`active ${ActiveClass}`);
            Parameter.fnSTART_INDEX = 1;
            currentMediaType= MediaType[getEnumKeyByEnumValue(MediaType, $(this).attr('data-type'))];
            templatelist(currentMediaType);
            tableService.GetTable().setPage("first");           
        });
};
/**更新編輯按鈕的資料屬性 */
const updateEditBtnAttribute = (subjectId: string, fileNo: string, mediaType: MediaType): void => {
    $("button[name='edit']")
        .attr({
            'data-subjectId': subjectId,
            'data-fileno': fileNo,
            'data-mediatype': mediaType,
        })
        .removeClass('disabled');
};
/*----------------------------實作流程-------------------------------- */
/*查詢條件顯示隱藏*/
$(document).on('click', '#MoreConditions', function() {
    $('#MoreC').fadeToggle('fast').children('i').toggleClass('left').toggleClass('right');
});
/*點擊事件:加入借調 */
$(document).on('click', 'button[name="addMateria"]', function(event) {
    event.preventDefault();
    const item = $(this).closest('.item');
    const input: CreateMaterialModel = {
        FileCategory: <MediaType>item.attr('data-mediatype'),
        FileNo: item.attr('data-fileno'),
        MaterialDesc: IsNULLorEmpty(item.attr('data-description')) ? '' : item.attr('data-description'),
        MaterialNote: IsNULLorEmpty(item.attr('data-note')) ? '' : item.attr('data-note'),
        ParameterStr: '0;0;0',
        //ParameterStr: IsNULLorEmpty(item.attr('data-duration')) ? '0;0;0' : item.attr('data-duration'),
    };
    route
        .AddBooking([input])
        .then(res => {
            Logger.res(route.api.AddingBooking, '全文檢索的加入借調', res, false);
            res.IsSuccess
                ? $(this)
                      .addClass('disabled')
                      .text('已加入清單')
                : $(this).text('無調用權限');
        })
        .catch(error => {
            Logger.viewres(route.api.AddingBooking, '全文檢索的加入借調', error, false);
        });
});

/**頁面初始化載入事件 */
((): void => {
    $('div.split-pane').splitPane(); //視窗拉取套件
    /**查詢各類型的筆數後,決定加載類型的頁面*/
    route.SearchCounts(Parameter).then(async res => {
        const data = <SearchCountResponseModel>(res.IsSuccess ? res.Data : {fnSUBJECT_COUNT:0,fnVIDEO_COUNT:0, fnAUDIO_COUNT:0,fnPHOTO_COUNT:0,fnDOC_COUNT:0});
        currentSearchCount=data;
        const CountData = data;
        condition({
            CountData: data,
            SearchParam: Parameter,
            ConditionStr: getConditionStrFromParameter(Parameter),
            MetaDataList: [],
        });
        const check=checkWhichMediaShowFirst(CountData);
        currentMediaType=check.type;//暫存給後續資料使用
        if(check.isAllzero) {
            $EditBtn.hide();
            noDataList(`列表查詢關鍵字【${Parameter.fsKEYWORD} 】時找到0筆資料`, HttpStatusCode.NoContent);
            preview(true);
            EmptyTabs();
            insertInfoTab({
                tabName: TabNameEnum.BaseMeta,
                tabhtml: ReturnNodataTab(TabNameEnum.BaseMeta, 'search'),
            });
        }else{
            const listData = await list(1,check.count);
             if (listData.length == 0) {
               $EditBtn.hide();
               preview(true);
            }
        }
    });
})(); //TODO　預覽頁面已撥放未停止者會錯誤

/**右下方分頁切換 */
$dataTitle.on('click', 'a.item', function() {
    const tabname = $(this).attr('data-tab');
    $(this)
        .addClass('active')
        .siblings()
        .removeClass('active');
    $dataTab.children('.tab.active').removeClass('active');
    $dataTab.children(`.tab[data-tab='${tabname}']`).addClass('active');
});

/**取得不重複的影片檔案編號 */
const GetFileNosFromList = (lists: Array<ISearchResult>): Array<string> => {
    const filenoArray: Array<string> = [];
    for (let list of lists) {
        filenoArray.indexOf(list.fsFILE_NO) == -1 && list.SearchType === MediaType.VIDEO
            ? filenoArray.push(list.fsFILE_NO)
            : Logger.log('檢索中重複的檔案編號:' + list.fsFILE_NO);
    }
    return filenoArray;
};

/**TsmPAI處理,目前只適用影片類型 */
const GetTsmAPI = (mediatype: MediaType, lists: Array<ISearchResult>) => {
    if (IsUseTsm) {
        const videoNos = GetFileNosFromList(lists);
        const videoLength: number = videoNos.length;
        /*測試後能較容易取得Tsm狀態的檔案個數*/
        const bestGetTsmLength: number = TSMSetting.bestGetTsmLength;
        let arrayOfArrays: Array<Array<string>> = [];
        const maxCreateArrayLength = Math.ceil(videoNos.length / bestGetTsmLength);
        //Step1:取得所有影片編號,並且切割成多個長度小於等於8的陣列
        for (let i = 0; i < maxCreateArrayLength; i++) {
            const start = i * bestGetTsmLength;
            const end = (i + 1) * bestGetTsmLength < videoLength ? (i + 1) * bestGetTsmLength : videoLength;
            const array = videoNos.slice(start, end);
            arrayOfArrays.push(array);
        }
        //Step2:分批次取得Tsm狀態
        if (arrayOfArrays.length > 0) {
            const $ListElement = $List;
            for (let array of arrayOfArrays) {
                route
                    .GetTmsStatus(MediaType.VIDEO, array)
                    .then(res => {
                        Logger.res(route.api.GetTsmStatus, '取得TSM狀態', res, false);
                        const filesResult = res.Data;
                        if (!IsNULLorEmpty(filesResult) && filesResult.TsmFileStatus.length > 0) {
                            /**暫存每次的檔案Tsm查詢結果 */
                            savefilesResult = savefilesResult.concat(filesResult.TsmFileStatus);
                            for (let fileResult of filesResult.TsmFileStatus) {
                                savefileNos.push(fileResult.FILE_NO);
                                const statusStr = GetTsmTextByStatus(fileResult.FILE_STATUS);
                                tableService.ReactivityUpdate(fileResult.FILE_NO,{
                                   fsSTATUS:fileResult.FILE_STATUS,
                                   TSMFileStatus:fileResult.FILE_STATUS,
                                   TSMFileStatusStr:statusStr
                                });
                                // const statusStr = GetTsmTextByStatus(fileResult.FILE_STATUS);
                                // const tsmStatusImg = GetTsmImgUrlByStatus(fileResult.FILE_STATUS);
                                // const tsmColor = GetTsmColor(fileResult.FILE_STATUS);
                                // const item = $ListElement
                                //     .find(`.item[data-fileno='${fileResult.FILE_NO}']`)
                                //     .find('div[name="fileStatus"]');
                                // item.removeClass('error');
                                // switch(ButtonViewStyle.get()){
                                //     case SearchViewStyle.List:
                                //         item.html(`<img src="${tsmStatusImg}" title="${statusStr}">`);
                                //         break;
                                //     case SearchViewStyle.Detail:
                                //     default:
                                //         item.html(`
                                //         <img src="${tsmStatusImg}">
                                //         <span class="${tsmColor}"> ${statusStr}</span>
                                //    `);
                                //         break;
                                // }
                            }
                            tableService.GetTable().redraw();
                        } else {
                            for (let fileNo of array) {
                                const item = $ListElement
                                    .find(`.item[data-fileno='${fileNo}']`)
                                    .find('div[name="fileStatus"]');
                                item.addClass('error');
                                switch(ButtonViewStyle.get()){
                                    case SearchViewStyle.List:
                                        item.html(`<img src="${ GetImageUrl(AirmamImage.TSM_OnNodata).href}" title="TSM查詢逾時">`);
                                        break;
                                    case SearchViewStyle.Detail:
                                    default:
                                        item.html(
                                            `<img src="${
                                                GetImageUrl(AirmamImage.TSM_OnNodata).href
                                            }"><span class="red">TSM查詢逾時</span>`
                                        );
                                        break;
                                }

                            }
                        }
                    })
                    .catch(error => {
                        for (let no of array) {
                            const item = $ListElement.find(`.item[data-fileno='${no}']`).find('div[name="fileStatus"]');
                            item.addClass('error');
                            switch(ButtonViewStyle.get()){
                                case SearchViewStyle.List:
                                    item.html(`<img src="${ GetImageUrl(AirmamImage.TSM_OnNodata).href}" title="TSM查詢逾時">`);
                                    break;
                                case SearchViewStyle.Detail:
                                default:
                                    item.html(
                                        `<img src="${
                                            GetImageUrl(AirmamImage.TSM_OnNodata).href
                                        }"><span class="red">TSM查詢逾時</span>`
                                    );
                                    break;
                            }
                        }
                        Logger.viewres(route.api.GetTsmStatus, '取得Tsm狀態', error, false);
                    });
            }
        }
    }
};
/**處理檢索結果的顯示切換 */
$(document).on('click','button[name="viewstyle"]',function(){
    $(this).children('i').toggleClass('list ol').toggleClass('th list');
    const selectRows=tableService.GetTable().getSelectedRows();
    switch(ButtonViewStyle.get()){
        case SearchViewStyle.List: //List to Detail
            ButtonViewStyle.set(SearchViewStyle.Detail);
            break;
        case SearchViewStyle.Detail: //Detail to List
        default:
            ButtonViewStyle.set(SearchViewStyle.List);
            break;
    }
    tableService.GetTable().redraw(true);
    if(selectRows.length>0){
        
        const fileno = selectRows[0].getIndex();
        const row = tableService.GetTable().getRow(fileno);
        tableService.GetTable().scrollToRow(row, "center", true);;
    }
   
});
/**編輯媒體資料 */
$EditBtn.click(function() {
    const subjectId = $(this).attr('data-subjectId');
    const fileno = $(this).attr('data-fileno');
    const mediaType = <MediaType>$(this).attr('data-mediatype');
    ShowModal<{ subjid: string; type: string; fileno: string }>('#EditMediaModal', route.api.ShowEditMedia, {
        subjid: subjectId,
        fileno: fileno,
        type: mediaType,
    })
        .then(IsSuccess => {
            if (IsSuccess) {
                ModalTask(EditMediaId, true, {
                    closable: false,
                    detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                    observeChanges: true,
                    context: '#left-component',
                    onShow: function(this: JQuery<HTMLElement>) {
                        $(EditMediaId)
                        .find('.dropdown:not(.x-hashtag)')
                        .dropdown({
                            fullTextSearch: 'exact',
                            match: 'text',
                        });
                        $(EditMediaId).find('.ui.dropdown.x-hashtag')
                        .dropdown({
                            allowAdditions: true,
                            keys: {
                                delimiter: 54
                            },
                            onChange:function(value, text, $selectedItem){
                                const newValue=value.replace(/,/g,"^");
                                $("input[name='HashTag']").val(newValue);
                            }
                        });
                        setCalendar(`${EditMediaId} .calendar`, 'date');
                        $(EditMediaId).css({'width':'calc(100% - 80px)'})
                            .find('.checkbox')
                            .checkbox()
                            .checkbox('set checked');
                    },
                    onApprove: function() {
                        const EditMediaFormId = '#EditMediaForm';
                        const $Form = $(EditMediaFormId);
                        /**
                         * Notice:動態驗證條件,注意深淺拷貝問題
                         *深拷貝初始的驗證條件,因為編輯的驗證會隨選單進行初始化,所以要由此去進行擴增
                         */
                        const copyValid = Object.assign({}, valid.Edit);
                        const validObject = AddDynamicNullable(EditMediaFormId, copyValid);
                        const IsFormValid = CheckForm(EditMediaFormId, Object.assign(validObject,valid.Edit));
                        if (IsFormValid) {
                            route
                                .EditMedia($Form.serialize())
                                .then(res => {
                                    /*注意:回傳的Data是一筆陣列 */
                                    Logger.res(route.api.EditMedia, '編輯媒體資料', res, true);
                                    RemoveLeaveConfirm(window.location.href);
                                    if (res.IsSuccess) {
                                        const data=IsNullorUndefined(res.Data)?<ISearchResult>{}:<ISearchResult>res.Data[0];
                                        $(EditMediaId).modal('hide');
                                        //更新列表資料
                                        const row=tableService.GetTable().getRow(fileno);
                                        row.update(<ISearchResult>{
                                            Title:data.Title,
                                            LicenseCode:data.LicenseCode,
                                            IsAlert:data.IsAlert,
                                            IsForBid:data.IsForBid,
                                            LicenseMessage:data.LicenseMessage,
                                            LicenseStr:data.LicenseStr,
                                            HashTag:data.HashTag
                                        });
                                        //刷新基本資料分頁
                                        basicdynamicMedia(mediaType, {
                                            fsSUBJECT_ID: subjectId,
                                            fileNo: fileno,
                                            type: mediaType,
                                        });
                                        
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.EditMedia, '編輯媒體資料', error, true);
                                });
                        }

                        return false;
                    },
                });
            } else {
            }
        })
        .catch(error => {
            Logger.viewres(route.api.ShowEditMedia, '顯示編輯媒體資料燈箱', error, true);
        });
});

/**點擊影格事件*/
$('#bottom-component2').on('click', '.ui.card img', function(event) {
    event.preventDefault();
    const video: HTMLVideoElement = document.querySelector('video');
    const duration = video.duration;
    const $rangeSlider: HTMLInputElement = document.querySelector("input[type='range']");
    const $currentTimer: HTMLSpanElement = document.querySelector("span[name='currentTimer']");
    const keycodetime = $(this)
        .closest('.card')
        .attr('data-time');
    const second = parseInt(keycodetime.slice(0, 6));
    const mssecond = parseInt(keycodetime.slice(6, 9));
    const totalseconds = second + mssecond / 1000;
    video.currentTime = totalseconds;
    $rangeSlider.value = ((totalseconds * 100) / duration).toString();
    $currentTimer.innerHTML = CurrentTimeToTimeCode(totalseconds, 'second');
});
/**點擊段落描述 */
$('#bottom-component2').on('click', '._StyleDescription', function(event) {
    event.preventDefault();
    const typeDiv = $Preview.find('div[name="video"]') || $Preview.find('div[name="audio"]') || false;
    if (!typeDiv) {
        ErrorMessage('媒體發生錯誤，無法跳轉到該段落');
        return false;
    }
    const type = typeDiv.attr('name') === 'video' ? MediaType.VIDEO : MediaType.AUDIO;
    const $rangeSlider: HTMLInputElement = document.querySelector("input[type='range']");
    const $currentTimer: HTMLSpanElement = document.querySelector("span[name='currentTimer']");
    const keycodeSeconds = Number($(this).attr('data-begtime')) || 0;
    const duration = type === MediaType.VIDEO ? vtempPlayer.GetTotoalTime() : atempPlayer.GetTotoalTime();
    type === MediaType.VIDEO ? vtempPlayer.SetCurrentTime(keycodeSeconds) : atempPlayer.SetCurrentTime(keycodeSeconds);
    $rangeSlider.value = ((keycodeSeconds * 100) / duration).toString();
    $currentTimer.innerHTML = CurrentTimeToTimeCode(keycodeSeconds, 'second');
});
/**顯示/隱藏關鍵影格資訊 */
$dataTab.on('click', "button[name='cardInfo']", function() {
    const cards = $dataTab
        .find(`.tab[data-tab='${TabNameEnum.KeyFrame}']`)
        .children()
        .find('.cards');
    cards.find('.card .content,.card .description').slideToggle();
    $(this)
        .children('i')
        .toggleClass('slash');
});

/**圖片上一張切換按鈕 */
$Preview.on('click', 'button[name="preImg"]', function() {
    const table_ = tableService.GetTable();
    const tableHolder = document.querySelector(initSetting.TableId).querySelector('.tabulator-tableHolder');
    const selectedRows = table_.getSelectedRows();
    const prevRow =selectedRows===undefined||selectedRows.length===0?false: selectedRows[0].getPrevRow();
    if (prevRow !== false) {
        selectedRows[0].deselect();
        prevRow.select();
        const pretop = prevRow.getElement().offsetTop;
        tableHolder.scrollTop = pretop;
        $(EditMediaId).modal('hide').modal('destroy');
    } else {
        const nowPage = table_.getPage();
        if (nowPage !== false && nowPage > 1) {
            table_.setPage(nowPage - 1).then(function() {
                const lastRow = table_.getRows()[table_.getRows().length - 1];
                selectedRows[0].deselect();
                lastRow.select();
                const lasttop = lastRow.getElement().offsetTop;
                tableHolder.scrollTop = lasttop;
            });
            $(EditMediaId).modal('hide').modal('destroy');
        } else {

            InfoMessage(`<span class="icon camera"></span>已經是第一張囉!`);
            tableHolder.scrollTop = 0;
        }
    }
});

/**圖片下一張切換按鈕 */
$Preview.on('click', 'button[name="nextImg"]', function() {
    const table_ = tableService.GetTable();
    const tableHolder = document.querySelector(initSetting.TableId).querySelector('.tabulator-tableHolder');
    const selectedRows = table_.getSelectedRows();
    const nextRow =selectedRows===undefined||selectedRows.length===0?false:selectedRows[0].getNextRow();
    if (nextRow !== false) {
        selectedRows[0].deselect();
        nextRow.select();
        const pretop = nextRow.getElement().offsetTop;
        tableHolder.scrollTop = pretop;
        $(EditMediaId).modal('hide').modal('destroy');
    } else {
        const nowPage = table_.getPage();
        const maxPage = table_.getPageMax();
        if (nowPage !== false && nowPage < maxPage) {
            table_.setPage(nowPage + 1).then(function() {
                const firstRow = table_.getRows()[0];
                selectedRows[0].deselect();
                firstRow.select();
                const pretop = firstRow.getElement().offsetTop;
                tableHolder.scrollTop = pretop;
            });
            $(EditMediaId).modal('hide').modal('destroy');
        } else {
            InfoMessage(`<span class="icon camera"></span>已經是最後一張囉!`);
        }
    }
});
/**圖片燈箱 */
$Preview.on('click', 'img[name="lightbox"]', function() {
    const src = $(this).attr('src');
    lightbox(src);
});

