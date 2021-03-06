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

/*==================????????????==============*/
/**??????:????????????,?????????tab??????????????????????????? */
window.top.addEventListener('click', e => {
    toggleMenu('rightMenu', 'hide');
});

/**
 * ??????????????????????????????,??????????????????
 * ???????????????(??????????????????????????????,??????:Chrome:??????????????????????????????????????????????????????????????????)
 * ???????????????(?????????js????????????,????????????)
 * ??????:beforeunload???,chrome??????????????????ajax(???????????????????????????),????????????????????????????????????????????????ajax
 */
window.addEventListener('beforeunload', function(event) {
    if (window.LeaveConfirm.length > 0) {
        const message: string = '?????????????????????????????????????????????????????????????';
        (event || window.event).returnValue = message; //Gecko + IE
        return '?????????????????????????????????????????????????????????????'; //Webkit, Safari, Chrome
    }
});


/*==================????????????==============*/

/*??????toastr??????*/
toastr.options = ToastrSetting;
var counter: number = 1;
const FullTextFormId = '#FullTextForm';
const $TitleTab = $('#TitleTab'); /**?????????????????? */
const $ContentTab = $('#ContentTab'); /**?????????????????? */
const $SideBar = $('#Sidebar');
const nav_h = $('nav').outerHeight();
var route = new SharedController();
/*================================semantic ui ???????????????======================================= */
(():void=>{
    /**????????????????????? */
    const SideBarToggle = () => {
        $('aside') .toggleClass('hide').animate({ width: 'toggle' });
        $('main._mainBody,nav').toggleClass('expand');
    };
   /*???????????????????????????*/
    $('._mainBody').css('top', nav_h + 'px');
    $('._mainBody').css('height', 'calc(100% - ' + nav_h + 'px)');
    /*???????????????????????????????????? */
    if ($(window).width() <= 760) { SideBarToggle(); }  
    /*????????????*/
    $('#menutoggle').click(function() { SideBarToggle(); });
    /**??????????????????????????? */
    $('.dropdown').dropdown();
    $('.accordion').accordion({ selector: { trigger: '.title' } });
     /**??????????????????????????????????????? */
     $('.tabs.menu')
     .on('mouseover', '.item', function() {
         $(this).tab(); /*??????tab*/
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
    /**???????????????????????????????????? */
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
    /*???????????????????????? */
    $('#SearchHot').dropdown();
    /**???????????????????????? */
    route
    .GetFavorite()
    .then(res => {
        Logger.log('Layout???????????????Get????????????');
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

/**========??????????????????============ */
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
        /**??????????????????Id */
        createId:():string=>{
            const nowToSeconds = Math.round(new Date().getTime() / 1000);
            const tabId = 'tab' + SecondsToHHMMSS(nowToSeconds).replace(/:/g, '') + counter++;
            return tabId;
        },
        /**?????????????????? */
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
        /**??????????????????active */
        removeAllActive:():void=>{
            $TitleTab.children('.active.item').removeClass('active');
            $ContentTab.children('.active.tab').removeClass('active');
        },
        /**????????????tab??????
         * Notice:iframe????????????????????????????????????,????????????????????????????????????
         */
        open:(functionId: string, tabText: string, iframeName?: string, src?: string,description?:string) => {
            const tabId=TAB.createId();
            const linkHref = IsNULLorEmpty(src) ? '' : src;
            const frameName = IsNULLorEmpty(iframeName) ? '' : `name="${iframeName}"`;
            const frameId = IsNULLorEmpty(iframeName) ? '' : `id="${iframeName}"`;
            const frameSrc = IsNULLorEmpty(src) ? '' : `src='${src}'`;
            const title={
                html:tabText||'?????????',
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
        /**???????????? */
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
                Logger.log('????????????,????????????');
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
        /**????????????????????????????????????Id,???????????????tabId,?????????????????????????????????Id */
        rightClick:(tabId?:string):string=>{
            if(!IsNullorUndefined(tabId) && !IsNULLorEmpty(tabId)){
                rightClickTabId=tabId;
            }
            return rightClickTabId;
        }

    };
})();
/**========??????????????????============= */
const SEARCH=(()=>{
    return {
        /**?????????????????????????????????dropdown */
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
                        Logger.error(`???????????????????????????????????????????????????????????????`);
                        resolve('');
                    });
            });
        },
        /**??????????????????Input */
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
         * ??????????????????????????????
         * @param UseTempleteField ???????????????????????????????????????
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
        
            /**???????????????????????????????????????,????????????????????????????????? */
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
                                /** ??????????????????(true:???????????????????????????false:????????????~???????????????"") */
                                fbIS_FULLTEXT:
                                (/[\~]/.test(InputField.value)) || ((InputField.value.indexOf("\"\""))>-1)
                                        ? false
                                        : true /*~?????????????????????,""??????????????? */,
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
/**========????????????(????????????"????????????????????????")================= */
const HISTORY=(()=>{
    let historyData=<Array<HistoryModel>>[];
    return {
        /**??????????????????*/
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
        /**??????????????????*/
        Get:():Array<HistoryModel>=>{
            return historyData;
        },
        /**???????????????????????? */
        Delete(ids:Array<string>){
            historyData=historyData.filter(x=> !ids.includes(x.guid) && (x.tabId !==false && !ids.includes(x.tabId)) );
        }

    };
})();

/**Task:??????????????????????????????????????? */
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
            case target instanceof HTMLAnchorElement /*????????????*/:
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
            case target instanceof HTMLElement /*??????????????????*/:
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

/*================================????????????======================================= */
/*??????:??????????????????*/
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
        ErrorMessage('??????????????????!????????????');
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
/**???????????????????????????,??????:??????????????????(?????????) */
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
/*??????:??????????????????*/
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
    if($navIndex.toString() === SidebarEnum.OpenWindow){//??????History
       let historys=HISTORY.Get().slice();//????????????????????????????????????,??????????????????????????????
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

/*??????????????????tab */
$(`#rightMenu a.item[name='close']`).click(function(e) {
    toggleMenu('rightMenu', 'hide');
    const tabId=TAB.rightClick();
    TAB.close(tabId);
});

/*??????????????????????????? */
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
                    Logger.log('???????????????????????????', {
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
            Logger.viewres(route.api.AddFavorite, '??????????????????', error, true);
        });
});

/**?????????top5 ??????????????? */
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
            /*????????????????????????: ????????????????????????????????????enter => ?????????????????????????????????????????????*/
            const input = SEARCH.GetFullTextInput(false);
            if (IsNULLorEmpty(input.fsKEYWORD)) {
                ErrorMessage(`??????????????????????????????!`);
            } else {
                route.SearchIndex(input).then(iframeSrc => {
                    try{
                        console.log('?????????Keyup=',input);
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
                            $SearchFocus.children('p').text(`?????????????????????`);
                        } else {
                            $SearchFocus.children('p').text(`????????????`);
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
                        Logger.error(`??????api:${SearchController.api.PopularKeyword}?????????????????????`, error);
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
/**??????????????????????????? */
$('#SearchFocus').on('click', '.item', function() {
    const $Input = $('#SearchHot').find('input');
    /**??????*/ const orginword = (<string>$Input.val()).replace(/,/g, '|');
    /**?????????????????????????????????*/ const orginwordArray = orginword.split(/[\s,|]+/gm);
    /**?????????????????????,????????????*/ const wantremove = orginwordArray[orginwordArray.length - 1];
    /**????????????????????????*/ const selectword: string = $(this).text();
    /*????????????input????????????????????????????????????,?????????????????? */
    $Input.val([orginword.substring(0, orginword.length - wantremove.length), selectword].join('')).focus();
    $('._SearchFocus')
        .removeClass('visible')
        .addClass('hidden');
});
/*??????????????????*/
$(FullTextFormId).submit(function(event) {
    event.preventDefault();
    const input =  SEARCH.GetFullTextInput(true);
    if (
        IsNULLorEmpty(input.fsKEYWORD) &&
        IsNULLorEmpty(input.clsDATE.fdSDATE) &&
        IsNULLorEmpty(input.clsDATE.fdEDATE) &&
        input.lstCOLUMN_SEARCH.length <= 0
    ) {
        ErrorMessage(`????????? & ???????????? & ??????????????????????????? ??????????????????`);
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
                    ErrorMessage('???????????????????????????????????????');
                });
            })();
        })();
    }
});

/*---------------------------------
   ????????????
------------------------------------*/
var saveDropdown: string = '';
const $DynamicFields = $('#DynamicField');
const MaxFieldLength: number = parseInt($DynamicFields.attr('data-FieldLength'), 10);
const $DynamicTemplete = $('#DynamicTemplete').parent('.dropdown');
const $DynamicMessage = $('#DynamicMessage');
/**???????????? */
$DynamicTemplete.dropdown({
    onChange: function(value: any, text: string, $choice: JQuery<HTMLElement>) {
        $DynamicFields.empty();
        //Tips: ???????????????,????????????????????????Search???(null????????????????????????)
        //DynamicField(Number(value), null).then(dropdown => {
        SEARCH.DynamicField(Number(value), true).then(dropdown => {
            if (dropdown.length == 0) {
                $DynamicFields.append(`<div class="ui mini black message">????????????????????????</div>`);
            } else {
                saveDropdown = dropdown;
                SEARCH.AddDynamicInput(dropdown);
            }
        });
    },
});
/**??????????????????Input */
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
/**??????????????????Input */
$DynamicFields.on('click', 'button[name="DynamicRemove"]', function(event) {
    event.preventDefault();
    $(this)
        .closest('.fields')
        .remove();
    $DynamicMessage.hide();
});
/**???????????????????????????????????????,?????????????????????????????? */
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
/**??????????????????????????????????????????,????????????????????????????????? */
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
 * ???iframe??????????????????
 * (1) ??????iframe ?????????????????????
 * (2) ??????????????????????????????
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
                Logger.log(`?????????????????????????????????`);
                // initSetting.ShowLog && InfoMessage('?????????????????????');
                route
                    .DeleteViewerFile({
                        api: viewerData.api,
                        fileName: viewerData.fileName,
                        fileNO: viewerData.fileNO,
                        subjectId: viewerData.subjectId,
                    })
                    .then(res => {
                        Logger.res(viewerData.api, 'viewer??????api????????????', res, false);
                    })
                    .catch(error => {
                        Logger.viewres(viewerData.api, 'viewer??????api????????????', error, false);
                    });
            } else {
                Logger.log(`???????????????????????????`);
            }
            break;
        case 'OpenDocViewer':
            const href = <DocumentOpenMessageModel>e.data.href;
            Logger.log(`?????????????????????????????????,??????:${href}`);
            // initSetting.ShowLog && InfoMessage('?????????????????????');
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
            Logger.log(`?????????????????????????????????`);
            $(DocumentViewerId).modal('hide');
            break;
        case 'UpdateTsmStatus':
            Logger.log(`??????Tsm????????????`, e.data);
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
