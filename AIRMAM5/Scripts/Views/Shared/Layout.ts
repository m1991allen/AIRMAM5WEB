import { RightMenu, toggleMenu } from '../../Models/Class/dynamicTabClass';
import { initSetting, ToastrSetting, SearchSetting, RightMenuSetting } from '../../Models/initSetting';
import { IPopularKeywords } from '../../Models/Interface/Search/IPopularKeywords';
import { SharedController } from '../../Models/Controller/SharedController';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { AdvancedQryModel, IFullTextSearchInput } from '../../Models/Interface/Search/IFullTextSearchInput';
import { HomoMode } from '../../Models/Enum/HomoMode';
import { SynonymousMode } from '../../Models/Enum/SynonymousMode';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { SearchTypeEnum } from '../../Models/Enum/MediaType';
import { SecondsToHHMMSS } from '../../Models/Function/Date';
import { HttpStatusCode } from '../../Models/Enum/HttpStatusCode';
import { UserFavoriteModel } from '../../Models/Interface/Shared/UserFavoriteModel';
import { SidebarEnum } from '../../Models/Enum/SidebarEnum';
import {
    DocumentOpenMessageModel,
    DocumentPostMessageModel,
    OpenTabMessageModel,
    TsmMessageModel,
} from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';
import { ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { SearchController } from '../../Models/Controller/SearchController';
import { Action } from '../../Models/Enum/Action';
import { Logger } from '../../Models/Class/LoggerService';
import { GetWebUrl } from '../../Models/Function/Url';
import { RemoveLeaveConfirm } from './_windowParameter';
import { RefreshBookingMessage } from '../../Models/Interface/Shared/PostMessage/RefreshBookingMessage';
import * as dayjs_ from 'dayjs';
import * as relativeTime from 'dayjs/plugin/relativeTime';
import * as zhTW from 'dayjs/locale/zh-tw';
import { HistoryInputModel, HistoryModel } from '../../Models/Interface/Shared/HistoryModel';
import { Guid } from '../../Models/Function/Guid';

dayjs_.extend(relativeTime);
const dayjs = (<any>dayjs_).default || dayjs_;

/*==================頁面處理==============*/
/**點擊:監聽視窗,當點擊tab以外都關閉右鍵選單 */
window.top.addEventListener('click', e => {
    toggleMenu('rightMenu', 'hide');
});

/**
 * 偵測視窗變更或關閉時,提示用戶訊息
 * 新版瀏覽器(瀏覽器會自行覆蓋文字,例如:Chrome:您要離開此網站嗎？您所做的更改可能不會保存。)
 * 舊版瀏覽器(可以由js覆蓋文字,如下顯示)
 * 注意:beforeunload中,chrome不支援非同步ajax(但有時同步會有問題),其他現代瀏覽器不支援同步或非同步ajax
 */
window.addEventListener('beforeunload', function(event) {
    if (window.LeaveConfirm.length > 0) {
        const message: string = '此動作會停止所有片庫執行程序，是否要返回?';
        (event || window.event).returnValue = message; //Gecko + IE
        return '此動作會停止所有片庫執行程序，是否要返回?'; //Webkit, Safari, Chrome
    }
});


/*==================宣告變數==============*/

/*設定toastr位置*/
toastr.options = ToastrSetting;
var counter: number = 1;
const FullTextFormId = '#FullTextForm';
const $TitleTab = $('#TitleTab'); /**分頁標籤容器 */
const $ContentTab = $('#ContentTab'); /**分頁內容容器 */
const $SideBar = $('#Sidebar');
const nav_h = $('nav').outerHeight();
var route = new SharedController();
/*================================semantic ui 初始化事件======================================= */
(():void=>{
    /**隱藏或顯示菜單 */
    const SideBarToggle = () => {
        $('aside') .toggleClass('hide').animate({ width: 'toggle' });
        $('main._mainBody,nav').toggleClass('expand');
    };
   /*響應大區塊比例調整*/
    $('._mainBody').css('top', nav_h + 'px');
    $('._mainBody').css('height', 'calc(100% - ' + nav_h + 'px)');
    /*載入頁面時是否要隱藏菜單 */
    if ($(window).width() <= 760) { SideBarToggle(); }  
    /*響應事件*/
    $('#menutoggle').click(function() { SideBarToggle(); });
    /**所有下拉選單初始化 */
    $('.dropdown').dropdown();
    $('.accordion').accordion({ selector: { trigger: '.title' } });
     /**分頁標籤點擊與右鍵選單事件 */
     $('.tabs.menu')
     .on('mouseover', '.item', function() {
         $(this).tab(); /*動態tab*/
     })
     .on('contextmenu', '.item', function(e) {
         if (!$(this).hasClass(RightMenuSetting.PermanentWindowClass)) {
             const functionId: string = $(this).attr('data-functionid');
             const tabId = $(this).attr('data-tab');
             TAB.rightClick(tabId);
             if (functionId == SearchSetting.FunctionId) {
                 if (!SearchSetting.CanFavorite) {
                     $('#' + RightMenuSetting.MenuId)
                         .find('.item[name="favorite"]')
                         .hide();
                 }
                 RightMenu($(this).attr('data-tab'), RightMenuSetting.MenuId);
             } else {
                 $('#' + RightMenuSetting.MenuId)
                     .find('.item[name="favorite"]')
                     .show();
                 RightMenu($(this).attr('data-tab'), RightMenuSetting.MenuId);
             }
         }
     });
    /**全文檢索查詢視窗顯示事件 */
    $('#SearchDrop').popup({
        inline: true,
        //hoverable: true,
        on: 'click',
        position: 'bottom center',
        delay: {
            show: 300,
            hide: 800,
        },
    });
    /*全文檢索下拉選單 */
    $('#SearchHot').dropdown();
    /**載入我的最愛列表 */
    route
    .GetFavorite()
    .then(res => {
        Logger.log('Layout中載入一次Get我的最愛');
        if (res.IsSuccess) {
            const list = <Array<UserFavoriteModel>>res.Data;
            for (let item of list) {
                CreateFavoriteItem({
                    tabId: TAB.createId(),
                    activeTabName: item.FunctionName,
                    functionId: item.FuncId,
                    hrefUrl: item.FavoriteUrl,
                });
            }
        } else {
            ErrorMessage(res.Message);
        }
    });
})();

/**========分頁相關操作============ */
const TAB=(()=>{
    const CloseTabItem = (tabId:string): void => {
        const $closeTitle=$TitleTab.children(`a[data-tab='${tabId}']`);
        const $closeContent=$ContentTab.children(`div[data-tab='${tabId}']`);
        const prevTabId= $closeTitle.prev().attr('data-tab');
        $closeTitle.remove(),$closeContent.remove();
        TAB.active(prevTabId);
        HISTORY.Delete([tabId]);
    };
    let rightClickTabId="",activeTabId="";
    return {
        /**創建新的分頁Id */
        createId:():string=>{
            const nowToSeconds = Math.round(new Date().getTime() / 1000);
            const tabId = 'tab' + SecondsToHHMMSS(nowToSeconds).replace(/:/g, '') + counter++;
            return tabId;
        },
        /**創建新的分頁 */
        create:(
              title:{html:string,attrs:string[][]},
            content:{html:string,attrs:string[][]}
        ):void=>{
        
            $TitleTab.children('.active.item').removeClass('active'),
            $ContentTab.children('.active.tab').removeClass('active');
            const newTitleTab=document.createElement('a'),
                  newContentTab=document.createElement('div'),
                  closeIcon=document.createElement('span');
            const tabId=title.attrs.find(x=>x[0]==='data-tab')[1];
            newTitleTab.className='item active';
            newTitleTab.innerHTML=title.html;
            for(let attr of title.attrs){
                newTitleTab.setAttribute(attr[0],attr[1]||"");
            }
            newContentTab.className="ui bottom loading attached tab  active _content";
            newContentTab.innerHTML=content.html;
            for(let attr of content.attrs){
                newContentTab.setAttribute(attr[0],attr[1]||"");
            }
            closeIcon.className="ui icon labeled";
            closeIcon.innerHTML=`<i class="icon close"></i>`;       
            closeIcon.addEventListener("click",function callback(e){
                TAB.close(tabId);
                closeIcon.removeEventListener('click', callback);
            },false);
            newTitleTab.appendChild(closeIcon);
            $TitleTab.append(newTitleTab);
            $ContentTab.append(newContentTab);
        },
        /**移除所有分頁active */
        removeAllActive:():void=>{
            $TitleTab.children('.active.item').removeClass('active');
            $ContentTab.children('.active.tab').removeClass('active');
        },
        /**開啟新的tab分頁
         * Notice:iframe中屬性必須設置允許全螢幕,才能讓影片使用全螢幕功能
         */
        open:(functionId: string, tabText: string, iframeName?: string, src?: string,description?:string) => {
            const tabId=TAB.createId();
            const linkHref = IsNULLorEmpty(src) ? '' : src;
            const frameName = IsNULLorEmpty(iframeName) ? '' : `name="${iframeName}"`;
            const frameId = IsNULLorEmpty(iframeName) ? '' : `id="${iframeName}"`;
            const frameSrc = IsNULLorEmpty(src) ? '' : `src='${src}'`;
            const title={
                html:tabText||'新分頁',
                attrs:[
                   ['data-add','0'],
                   ['data-tab',tabId],
                   ['data-functionId',functionId],
                   ['data-href',linkHref||''],
                ]
            };
            const content={
                html:`<iframe  ${frameId} ${frameName} ${frameSrc}  allowfullscreen="true" webkitallowfullscreen="true" mozallowfullscreen="true" onload="$($(this)[0].contentWindow).on('click', function(event) {$('#rightMenu').hide();});$('#${tabId}').removeClass('loading');"></iframe>`,
                attrs:[
                    ['id',tabId],
                    ['data-tab',tabId],
                ]
            };
            TAB.create(title,content);
            HISTORY.Add({
                title:tabText,
                description:description||'',
                type:'link',
                tabId:tabId
            });
        },
        /**關閉分頁 */
        close:(tabId:string):void=>{
            const $tabItem = $TitleTab.children(`a[data-tab="${tabId}"]`).first();
            const pageURL = $tabItem.attr('data-href');
            if (!$tabItem.hasClass(RightMenuSetting.PermanentWindowClass)) {
                if (window.top.LeaveConfirm.indexOf(pageURL) > -1) {
                    ModalTask('#LeaveConfirm', true, {
                        closable: false,
                        onApprove: function() {
                            CloseTabItem(tabId);
                            RemoveLeaveConfirm(pageURL);
                        }
                    });
                } else {
                    CloseTabItem(tabId);
                    RemoveLeaveConfirm(pageURL);
                }
            } else {
                Logger.log('常駐視窗,無法關閉');
            }
        },
        active:(tabId:string):void=>{
            try{
                activeTabId=tabId;
                TAB.removeAllActive();
            }finally{
                $TitleTab.children(`a[data-tab="${tabId}"]`).last().addClass('active');
                $ContentTab.children(`div[data-tab="${tabId}"]`).last().addClass('active');
            }
        },
        /**標註右鍵選擇並回傳該選擇Id,若沒有傳入tabId,則回傳上一次的右鍵選擇Id */
        rightClick:(tabId?:string):string=>{
            if(!IsNullorUndefined(tabId) && !IsNULLorEmpty(tabId)){
                rightClickTabId=tabId;
            }
            return rightClickTabId;
        }

    };
})();
/**========檢索相關操作============= */
const SEARCH=(()=>{
    return {
        /**產生全文檢索的動態欄位dropdown */
        DynamicField:(templeteId: number, search?: boolean): Promise<string> => {
            return new Promise(resolve => {
                route
                    .GetAttriFieldList(templeteId, search)
                    .then(json => {
                        const data = <Array<SelectListItem>>json;
                        if (data.length == 0) {
                            resolve('');
                        } else {
                            const fakediv = document.createElement('div');
                            for (let i = 0; i < data.length; i++) {
                                const item = data[i];
                                fakediv.innerHTML += `<option value="${item.Value}" ${item.Selected ? 'selected' : ''}>
                                                     ${item.Text}
                                                     </option>`;
                            }
                            resolve(`<select class="ui fluid search selection dropdown">${fakediv.innerHTML}</select>`);
                        }
                    })
                    .catch(error => {
                        Logger.error(`全文檢索下拉選單選擇的樣板無法產生動態欄位`);
                        resolve('');
                    });
            });
        },
        /**插入動態欄位Input */
        AddDynamicInput:(saveDropdown: string) => {
            $DynamicFields
                .append(
                    `<div class="fields"><div class="six wide field">
                           ${saveDropdown}
                        </div>
                        <div class="ten wide field">
                            <div class="ui action input">
                                <input type="text">
                                <button class="ui icon yellow button" type="button" name="DynamicField">
                                    <i class="add icon"></i>
                                </button>
                            </div>
                        </div>
                    </div>`
                )
                .children()
                .last()
                .find('.dropdown')
                .dropdown({
                    direction: 'upward',
                    fullTextSearch: true,
                });
        },
        /**
         * 取得全文檢索表單參數
         * @param UseTempleteField 要不要加入樣板欄位篩選的值
         */
        GetFullTextInput:(UseTempleteField: boolean): IFullTextSearchInput => {
            const $Form = $(FullTextFormId);
            const $KeywordInput = $('#SearchHot').children('input');
            const $DateRangeInput = $Form.find('input[name="dateRange"]');
            const SeachtypeValue = (): string => {
                let checkArray: Array<string> = [];
                $("input[name='searchtype']:checked").each(function() {
                    checkArray.push(<string>$(this).val());
                });
                return checkArray.join(',');
            };
            const HomeModeValue: HomoMode = $('#HomoMode').is(':checked') ? HomoMode.Open : HomoMode.Close;
            const DateSort = <string>$Form.find('input[name="datesort"]:checked').val();
            const EngineMode = Number(
                $('#EngineMode')
                    .parent('.dropdown')
                    .dropdown('get value')
            );
            const DateColumnValue = <string>$('#DateType')
                .parent('.dropdown')
                .dropdown('get value');
            const TempleteId = <string>$('#DynamicTemplete')
                .parent('.dropdown')
                .dropdown('get value');
        
            /**如果有勾同義就優先以同義詞,否則以引擎下拉選單為主 */
            const SynonymousModeValue: SynonymousMode = $('#SynonymousMode').is(':checked')
                ? SynonymousMode.Synonymous
                : EngineMode !== 1
                ? SynonymousMode.Common
                : SynonymousMode.Tornado;
            const SearchColumn = (): Array<AdvancedQryModel> => {
                let res: Array<AdvancedQryModel> = [];
                if (UseTempleteField) {
                    for (let input of $DynamicFields.children('.fields').toArray()) {
                        let DropdownField = input.querySelector('.dropdown');
                        let InputField = input.querySelector('.input').querySelector('input');
                        if (!IsNULLorEmpty(InputField.value)) {
                            res.push({
                                /** 是否全文檢索(true:針對欄位全文檢索、false:區間比對~或完全比對"") */
                                fbIS_FULLTEXT:
                                (/[\~]/.test(InputField.value)) || ((InputField.value.indexOf("\"\""))>-1)
                                        ? false
                                        : true /*~表示為區間比對,""為完全比對 */,
                                fsCOLUMN: $(DropdownField).dropdown('get value'),
                                fsVALUE: InputField.value.replace(/\"/g, '^'),
                            });
                        }
                    }
                }
                return res;
            };
            return <IFullTextSearchInput>{
                fsKEYWORD: <string>$KeywordInput.val(),
                fsINDEX: SeachtypeValue(),
                fnSEARCH_MODE: SynonymousModeValue,
                fnHOMO: HomeModeValue,
                clsDATE: {
                    fsCOLUMN: DateColumnValue,
                    fdSDATE: <string>$DateRangeInput.first().val(),
                    fdEDATE: <string>$DateRangeInput.last().val(),
                },
                lstCOLUMN_ORDER: [
                    {
                        fsCOLUMN: DateColumnValue,
                        fsVALUE: DateSort,
                    },
                ],
                fnTEMP_ID: IsNULLorEmpty(TempleteId) ? 0 : Number(TempleteId),
                lstCOLUMN_SEARCH: SearchColumn(),
                fnPAGE_SIZE: SearchSetting.pageSize,
                fnSTART_INDEX: SearchSetting.startIndex,
            };
        },

    };
})();
/**========操作歷程(目前只有"目前視窗功能使用")================= */
const HISTORY=(()=>{
    let historyData=<Array<HistoryModel>>[];
    return {
        /**加入操作歷程*/
        Add:(item:HistoryInputModel)=>{
            historyData.unshift({
                tabId:item.tabId,
                title:item.title||'',
                type:item.type ||'info',
                description:item.description||'',
                guid:Guid(),
                datetime:new Date()
            });
        },
        /**取得操作歷程*/
        Get:():Array<HistoryModel>=>{
            return historyData;
        },
        /**刪除多個操作歷程 */
        Delete(ids:Array<string>){
            historyData=historyData.filter(x=> !ids.includes(x.guid) && (x.tabId !==false && !ids.includes(x.tabId)) );
        }

    };
})();

/**Task:插入我的常用到右側常用功能 */
const CreateFavoriteItem = (input: {
    tabId: string;
    activeTabName: string;
    functionId: string;
    hrefUrl: string;
}): void => {
    const linkArray = !IsNULLorEmpty(input.hrefUrl) ? input.hrefUrl.split('/') : [input.hrefUrl, 'Index'];
    const linkhref =
        input.hrefUrl.indexOf(initSetting.WebUrl) > -1
            ? input.hrefUrl
            : GetWebUrl(<Controller>linkArray[linkArray.length - 2], <Action>linkArray[linkArray.length - 1]).href;
    const favoriteItem: HTMLAnchorElement = document.createElement('a');
    favoriteItem.setAttribute('data-tab', input.tabId);
    favoriteItem.setAttribute('data-functionId', input.functionId);
    favoriteItem.setAttribute('data-href', linkhref);
    favoriteItem.className = 'item';
    favoriteItem.onclick =(event)=> {
        const target = <HTMLElement | HTMLAnchorElement>event.target;
        switch (true) {
            case target instanceof HTMLAnchorElement /*開新分頁*/:
                TAB.removeAllActive();
                const tab = $TitleTab.find(`a.item[data-href='${linkhref}'`);
                const tabSegment = $ContentTab.find(`iframe[src='${linkhref}']`).parent('.tab');
                if (tab.length == 0) {
                    TAB.open(input.functionId, input.activeTabName, '', linkhref);
                } else {
                    tab.last()
                        .addClass('active')
                        .data('add', '0');
                    tabSegment.addClass('active');
                }
                $SideBar.sidebar('hide');
                break;
            case target instanceof HTMLElement /*移除我的最愛*/:
                route.RemoveFavorite({ id: input.functionId }).then(res => {
                    if (res.IsSuccess) {
                        SuccessMessage(res.Message);
                        $(this)
                            .closest('a.item')
                            .remove();
                    } else {
                        ErrorMessage(res.Message);
                    }
                });
                break;
        }
    };
    favoriteItem.innerHTML = `${input.activeTabName}<i class="close icon"></i>`;
    $SideBar.find(`.inside[subIndex='${SidebarEnum.CommonFun}']`).append(favoriteItem);
};

/*================================點擊事件======================================= */
/*點擊:左方側邊選單*/
$('aside .menu .content a.item').click(function(e) {
    $('aside .menu .content a.item')
        .siblings('.active')
        .removeClass('active');
    $(this).addClass('active');
    const hrefArray: Array<string> =
        $(this).attr('data-href') == undefined
            ? []
            : $(this)
                  .attr('data-href')
                  .split('/');
    if (hrefArray.length < 2) {
        ErrorMessage('頁面路徑錯誤!無法開啟');
    } else {
        const linkHref = GetWebUrl(<Controller>hrefArray[hrefArray.length - 2], <Action>hrefArray[hrefArray.length - 1])
            .href;
        const openTab = $TitleTab.children(`a.item[data-href='${linkHref}']`);
        const functionId: string = $(this).attr('id');
        const frameName: string = $(this).attr('data-controller');
        if ($(this).data('mutiple') == false) {
            if (openTab.length == 0) {
                TAB.open(functionId, $(this).text(), frameName, linkHref);
            } else {
                TAB.removeAllActive();
                TAB.active(openTab.attr('data-tab'));
            }
        } else {
            TAB.open(functionId, $(this).text(), frameName, linkHref);
        }
    }
});
/**開啟右上快捷鍵選單,例如:我的調用清單(購物車) */
$("div[name='fastMenu'] a.item").click(function() {
    const controller = $(this).attr('data-controller');
    const action = $(this).attr('data-action');
    const functionId = $(this).attr('id');
    const linkHref = GetWebUrl(<Controller>controller, <Action>action).href;
    const openTab = $TitleTab.children("a.item:contains('" + $(this).text() + "')");
    if ($(this).data('mutiple') == false) {
        if (openTab.length == 0) {
            TAB.open(functionId, $(this).text(), null, linkHref);
        } else {
            TAB.removeAllActive();
            TAB.active(openTab.attr('data-tab'));
        }
    } else {
        TAB.open(functionId, $(this).text(), null, linkHref);
    }
});
/*點擊:開啟右邊選單*/
$('#nav_but').on('click', 'a.item', function() {
    let $navIndex: number = parseInt($(this).attr('index'));
    let $insideBar = $('#Sidebar .inside').eq($navIndex);
    let $insideBarIndex: number = parseInt($insideBar.attr('subIndex'));
    $('#Sidebar .inside').hide();

    if ($navIndex == $insideBarIndex) {
        $insideBar.show();
        $('#Sidebar')
            .sidebar('setting', 'transition', 'push')
            .sidebar('toggle');
        $(`nav a.item[index!='${$navIndex}']`).removeClass('active');
        $(this).addClass('active');
    }
    if($navIndex.toString() === SidebarEnum.OpenWindow){//處理History
       let historys=HISTORY.Get().slice();//需要回傳新陣列而不是參考,不然會影響原有的儲存
       const fragment=document.createDocumentFragment();
       while(historys.length>0){
           const history=historys.pop();
           const fromNow=dayjs(history.datetime).locale(zhTW).fromNow();
           const icon= history.type==='link'?"purple linkify":
                       history.type==='info'?"teal info":
                       history.type==='success'?"green check":
                       history.type==='warning'?"orange exclamation triangle":
                       history.type==='error'?"red exclamation triangle":
           "question circle grey";
           const item=document.createElement('div');
           item.className="item x-history";
           item.innerHTML=`<div class="content">
           <div class="header"><i class="circular small inverted ${icon} icon"></i>${history.title}<p class="time">${fromNow}</p></div>  
           <div class="meta">${history.description}</div></div>`;
           if(history.type==='link' && history.tabId !==false){
               item.onclick=function() { 
                   TAB.active(<string>history.tabId);
                   $SideBar.sidebar('hide');
                }
           }
           fragment.appendChild(item);
       } 
       $insideBar.children('div[name="inside-scroll"]').empty().append(fragment);

    }
});

/*右鍵選單關閉tab */
$(`#rightMenu a.item[name='close']`).click(function(e) {
    toggleMenu('rightMenu', 'hide');
    const tabId=TAB.rightClick();
    TAB.close(tabId);
});

/*加入最愛到工作紀錄 */
$('#rightMenu a.item[name="favorite"]').click(function(e) {
    let activeTabName = $('#TitleTab a.item.active:eq(0)');
    const tabId = activeTabName.attr('data-Tab');
    const functionId = activeTabName.attr('data-functionId');
    const hrefUrl = activeTabName.attr('data-href');
    toggleMenu('rightMenu', 'hide');
    route
        .AddFavorite({
            id: functionId,
        })
        .then(res => {
            if (res.IsSuccess) {
                if (res.StatusCode == HttpStatusCode.Created) {
                    Logger.log('加入最愛到工作紀錄', {
                        tabId: tabId,
                        activeTabName: activeTabName.text(),
                        functionId: functionId,
                        hrefUrl: hrefUrl,
                    });
                    CreateFavoriteItem({
                        tabId: tabId,
                        activeTabName: activeTabName.text(),
                        functionId: functionId,
                        hrefUrl: hrefUrl,
                    });
                }
                SuccessMessage(res.Message);
            } else {
                ErrorMessage(res.Message);
            }
        })
        .catch(error => {
            Logger.viewres(route.api.AddFavorite, '加入我的最愛', error, true);
        });
});

/**最熱門top5 檢索關鍵字 */
$('#SearchHot')
    .click(function() {
        $('._SearchFocus')
            .removeClass('visible')
            .addClass('hidden');
    })
    .find('input')
    .keyup(function(e) {
        e.preventDefault();
        if (e.keyCode === 13 || e.which===13) {
            // $('#FullTextForm').trigger('submit');
            /*依經理與副理建議: 若直接在這裡打關鍵字按下enter => 只檢索關鍵字，不做其他欄位檢索*/
            const input = SEARCH.GetFullTextInput(false);
            if (IsNULLorEmpty(input.fsKEYWORD)) {
                ErrorMessage(`快速檢索請輸入關鍵字!`);
            } else {
                route.SearchIndex(input).then(iframeSrc => {
                    try{
                        console.log('關鍵字Keyup=',input);
                        TAB.open(SearchSetting.FunctionId, SearchSetting.FrameText, SearchSetting.FrameName,iframeSrc,input.fsKEYWORD); 
                    }finally{
                        $(`iframe[name="${SearchSetting.FrameName}"]`)
                        .last()
                        .attr('src', iframeSrc)
                        .show();
                        $('._SearchFocus')
                        .removeClass('visible')
                        .addClass('hidden');
                    }
                });
            }
        } else {
            const word: string = <string>$(this).val();
            const container: HTMLDivElement = document.createElement('div');
            const fragment = document.createDocumentFragment();
            const $SearchFocus = $('._SearchFocus');
            if(word.length>1){
                route
                .PopularKeywords(word)
                .then(json => {
                    const words = <IPopularKeywords[]>json.Data;
                    try {
                        if (words.length == 0) {
                            $SearchFocus.children('p').text(`沒有熱門關鍵字`);
                        } else {
                            $SearchFocus.children('p').text(`熱搜關鍵`);
                            for (let i = 0; i < words.length; i++) {
                                const item: HTMLAnchorElement = document.createElement('a');
                                item.href = '#';
                                item.className = 'item';
                                item.setAttribute('data-value', words[i].word);
                                item.innerHTML = words[i].word;
                                fragment.appendChild(item);
                            }
                            container.append(fragment);
                        }
                    } catch (error) {
                        Logger.error(`介接api:${SearchController.api.PopularKeyword}或渲染發生問題`, error);
                    }
                })
                .then(() => {
                    $SearchFocus
                        .children('div')
                        .empty()
                        .append(`${container.innerHTML}`);
                    $SearchFocus.removeClass('hidden').addClass('visible');
                });
            }
           
        }
    });
/**點擊熱門檢索關鍵字 */
$('#SearchFocus').on('click', '.item', function() {
    const $Input = $('#SearchHot').find('input');
    /**原值*/ const orginword = (<string>$Input.val()).replace(/,/g, '|');
    /**將原值依符號切割為陣列*/ const orginwordArray = orginword.split(/[\s,|]+/gm);
    /**此次搜尋的字詞,不能帶入*/ const wantremove = orginwordArray[orginwordArray.length - 1];
    /**下拉選擇的關鍵字*/ const selectword: string = $(this).text();
    /*將原有的input查詢條件與此次關鍵字結合,以取代原值↓ */
    $Input.val([orginword.substring(0, orginword.length - wantremove.length), selectword].join('')).focus();
    $('._SearchFocus')
        .removeClass('visible')
        .addClass('hidden');
});
/*全文檢索查詢*/
$(FullTextFormId).submit(function(event) {
    event.preventDefault();
    const input =  SEARCH.GetFullTextInput(true);
    if (
        IsNULLorEmpty(input.fsKEYWORD) &&
        IsNULLorEmpty(input.clsDATE.fdSDATE) &&
        IsNULLorEmpty(input.clsDATE.fdEDATE) &&
        input.lstCOLUMN_SEARCH.length <= 0
    ) {
        ErrorMessage(`關鍵字 & 日期區間 & 更多篩選的欄位檢索 至少填入一項`);
    } else {
        (async () => {
            await (() => {
                TAB.open(SearchSetting.FunctionId, SearchSetting.FrameText, SearchSetting.FrameName,input.fsKEYWORD);
            })();
            await (() => {
                route.SearchIndex(input).then(iframeSrc => {
                    $(`iframe[name="${SearchSetting.FrameName}"]`)
                        .last()
                        .attr('src', iframeSrc)
                        .show();
                    $('._SearchFocus')
                        .removeClass('visible')
                        .addClass('hidden');
                }).catch(error=>{
                    ErrorMessage('檢索出了問題，無法開啟頁面');
                });
            })();
        })();
    }
});

/*---------------------------------
   動態欄位
------------------------------------*/
var saveDropdown: string = '';
const $DynamicFields = $('#DynamicField');
const MaxFieldLength: number = parseInt($DynamicFields.attr('data-FieldLength'), 10);
const $DynamicTemplete = $('#DynamicTemplete').parent('.dropdown');
const $DynamicMessage = $('#DynamicMessage');
/**變更樣板 */
$DynamicTemplete.dropdown({
    onChange: function(value: any, text: string, $choice: JQuery<HTMLElement>) {
        $DynamicFields.empty();
        //Tips: 全文檢索時,欄位一定是有開放Search的(null會顯示全部的欄位)
        //DynamicField(Number(value), null).then(dropdown => {
        SEARCH.DynamicField(Number(value), true).then(dropdown => {
            if (dropdown.length == 0) {
                $DynamicFields.append(`<div class="ui mini black message">樣板沒有欄位可選</div>`);
            } else {
                saveDropdown = dropdown;
                SEARCH.AddDynamicInput(dropdown);
            }
        });
    },
});
/**新增動態欄位Input */
$DynamicFields.on('click', 'button[name="DynamicField"]', function(event) {
    event.preventDefault();
    if (MaxFieldLength > $DynamicFields.children('.fields').length) {
        $(this)
            .attr('name', 'DynamicRemove')
            .removeClass('yellow')
            .addClass('red')
            .find('i')
            .addClass('delete')
            .removeClass('add');
            SEARCH.AddDynamicInput(saveDropdown);
        $DynamicMessage.hide();
    } else {
        $DynamicMessage.show();
    }
});
/**移除動態欄位Input */
$DynamicFields.on('click', 'button[name="DynamicRemove"]', function(event) {
    event.preventDefault();
    $(this)
        .closest('.fields')
        .remove();
    $DynamicMessage.hide();
});
/**全文檢索搜尋模式選擇模糊時,同義勾選就取消並禁用 */
$('#EngineMode')
    .parent('.dropdown')
    .dropdown({
        onChange: function(value, text, $selectedItem) {
            const $Checkbox = $('#SynonymousMode').parent('.checkbox');
            const checkValue = Number(value);
            checkValue == SynonymousMode.Tornado
                ? $Checkbox.checkbox('set unchecked').checkbox('set disabled')
                : $Checkbox.checkbox('set enabled');
        },
    });
/**全文檢索的檢索類型勾選變更時,更新可選的檢索樣板種類 */
$(FullTextFormId)
    .find("input[name='searchtype']")
    .change(function() {
        const type = <SearchTypeEnum>$(this).val();
        const ischeck = $(this).is(':checked');
        const options = $('#DynamicTemplete').find(`option[data-searchtype='${type}']`);
        ischeck ? options.prop('disabled', false) : options.prop('disabled', true);
        options.each(function(index: number, element: HTMLElement) {
            const menu = $DynamicTemplete.find('.menu');
            const optionValue = element.getAttribute('value');
            ischeck
                ? menu.append(`<div class="item" data-value="${optionValue}">${element.textContent}</div>`)
                : menu.find(`.item[data-value='${optionValue}']`).remove();
        });
        $DynamicTemplete.dropdown('clear').dropdown('refresh');
    });

/**
 * 父iframe接受傳遞訊息
 * (1) 變化iframe 內部進度條狀態
 * (2) 文件檢視器開關或刪除
 */
window.top.addEventListener('message', receiveMessage, false);
function receiveMessage(e) {
    const origin = e.origin;
    const DocumentViewerId = '#DocumentViewer';
    switch (e.data.eventid) {
        case 'L_Upload':
            const doc = $('#L_Upload')
                .contents()
                .find('body');
            const progress = $(doc)
                .find(initSetting.TableId)
                .find(`.progress[name="progress${e.data.workid}"]`);
            progress.html(e.data.bar);
            break;
        case 'DeleteDocViewer':
            const viewerData = <DocumentPostMessageModel>e.data;
            if (
                !IsNULLorEmpty(viewerData.subjectId) &&
                !IsNULLorEmpty(viewerData.fileNO) &&
                !IsNULLorEmpty(viewerData.fileName)
            ) {
                Logger.log(`文件檢視器接收刪除通知`);
                // initSetting.ShowLog && InfoMessage('文件檢視器關閉');
                route
                    .DeleteViewerFile({
                        api: viewerData.api,
                        fileName: viewerData.fileName,
                        fileNO: viewerData.fileNO,
                        subjectId: viewerData.subjectId,
                    })
                    .then(res => {
                        Logger.res(viewerData.api, 'viewer刪除api失敗回應', res, false);
                    })
                    .catch(error => {
                        Logger.viewres(viewerData.api, 'viewer刪除api失敗回應', error, false);
                    });
            } else {
                Logger.log(`文件檢視器無法刪除`);
            }
            break;
        case 'OpenDocViewer':
            const href = <DocumentOpenMessageModel>e.data.href;
            Logger.log(`文件檢視器接收開啟訊息,位置:${href}`);
            // initSetting.ShowLog && InfoMessage('開啟文件檢視器');
            ModalTask(DocumentViewerId, true, {
                closable: false,
                detachable: true,
                transition: 'horizontal flip',
                duration: 1000,
                onShow: function() {
                    $(DocumentViewerId)
                        .find('.content')
                        .html(`<iframe  src="${href}" name="${Controller.DocViewer}" border="0"></iframe>`);
                    // $(DocumentViewerId)
                    // .find('.content')
                    // .html(`<object type="text/html"  data="${href}" name="${Controller.DocViewer}"></object>`);
                },
                onApprove: function() {},
                onDeny: function() {},
                onHide: function() {
                    $(DocumentViewerId)
                        .find('.content')
                        .empty();
                },
            });

            break;
        case 'CloseDocViewer':
            Logger.log(`文件檢視器接收關閉訊息`);
            $(DocumentViewerId).modal('hide');
            break;
        case 'UpdateTsmStatus':
            Logger.log(`接收Tsm更新訊息`, e.data);
            const tsmData = <TsmMessageModel>e.data;
            const tsmiframeURL = GetWebUrl(Controller.Materia, Action.Index).href;
            const tsmifrmaes = <NodeListOf<HTMLIFrameElement>>(
                document.body.querySelectorAll(`iframe[src='${tsmiframeURL}']`)
            );
            Array.from(tsmifrmaes, (tsmiframe: HTMLIFrameElement, index: number) => {
                const tsmbody = tsmiframe.contentWindow.document.body;
                const table = tsmbody.querySelector(`${initSetting.TableId}`);
                $(table)
                    .find(`.tabulator-row[data-fileno='${tsmData.fileno}']>div[tabulator-field='TSMFileStatus']`)
                    .html(tsmData.labelHTML);
            });
            break;
       
        case 'refreshBooking':
            [Controller.MyBooking,Controller.Booking].forEach(controller=>{
                const iframeURL = GetWebUrl(controller, Action.Index).href;
                const iframes:NodeListOf<HTMLIFrameElement>= window.top.document.body.querySelectorAll(`iframe[src='${iframeURL}']`);
                Array.from(iframes).forEach(iframe=>{
                    const win=iframe.contentWindow;
                    win.postMessage(<RefreshBookingMessage>{eventid:'refreshBooking'},'/');
                });
            });
            break;
        case 'OpenTab':
            const tabdata =<OpenTabMessageModel>e.data;
            TAB.open(tabdata.functionId,tabdata.tabText, tabdata.iframeName,tabdata.src,tabdata.description);
            break;
        default:
            break;
    }
}
