import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { dayjs, setCalendar,SetDate } from '../../Models/Function/Date';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { API } from '../../Models/Const/API';
import { FormValidField } from '../../Models/Const/FormValid';
import { LSearchMessageSetting } from '../../Models/MessageSetting';
import { sdateId, edateId, SearchFormId } from '../../Models/Const/Const.';
import { CheckForm } from '../../Models/Function/Form';
import { ErrorMessage, windowPostMessage } from '../../Models/Function/Message';
import { LSearchSearchModel, LSearchListModel } from '../../Models/Interface/ILSearchIndex';
import { initSetting, TabulatorSetting, SearchSetting } from '../../Models/initSetting';
import { DetailButton } from '../../Models/Templete/ButtonTemp';
import { IFullTextSearchInput } from '../../Models/Interface/Search/IFullTextSearchInput';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { ConditionModel } from '../../Models/Interface/Search/ISearchResponseVideoModel';
import { SearchTypeChineseEnum } from '../../Models/Enum/MediaType';
import { SynonymousChineseMode } from '../../Models/Enum/SynonymousMode';
import { HomoMode } from '../../Models/Enum/HomoMode';
import { Label } from '../../Models/Templete/LabelTemp';
import { Color } from '../../Models/Enum/ColorEnum';
import { Filter } from '../../Models/Enum/Filter';
import { OpenTabMessageModel } from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';

/*===============================================================*/
/*宣告變數*/
const api = API.L_Search;
const valid = FormValidField.L_Search;
const message = LSearchMessageSetting;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $SysAccount: JQuery<HTMLElement> = $('#SysAccount').closest('.dropdown');
const selfId = <string>$('#SelfAccount').val();
var table: ItabulatorService;
/**回傳Modal性質*/
const prop = (key: keyof LSearchListModel): string => {
    return key;
};
//===========================================
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

/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/*頁面載入查詢*/
Search({
    BeginDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    UserId: selfId,
});
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    $SysAccount.dropdown('set selected', selfId);
});
/*查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const sdate = dayjs($sdate.val());
        const edate = dayjs($edate.val());
        let searchid = <string>$SysAccount.dropdown('get value');
        if (edate.isBefore(sdate)) {
            $(this).addClass('error');
            ErrorMessage('查詢日期的起訖範圍錯誤');
        } else {
            Search({
                BeginDate: <string>$sdate.val(),
                EndDate: <string>$edate.val(),
                UserId: searchid,
            });
        }
    }
});
/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fnSRH_ID'), type: Filter.Like, value: word },
        { field: prop('fsCREATED_BY'), type: Filter.Like, value: word },
        { field: prop('fdCREATED_DATE'), type: Filter.Like, value: word },
        { field: prop('fsSTATEMENT'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: LSearchSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: 'fitDataFill',
        ajaxURL: api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        columns: [
            { title: 'ID', field: prop('fnSRH_ID'), width: 90, sorter: 'number', visible: false },
            { title: '帳號', field: prop('fsCREATED_BY'), width: 135, sorter: 'string' },
            {
                title: '查詢日期',
                field: prop('fdCREATED_DATE'),
                sorter: 'string',
                width: 170,
            },
            {
                title: '查詢條件',
                field: prop('fsSTATEMENT'),
                sorter: 'string',
                minWidth: 390,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const json = <IFullTextSearchInput>JSON.parse(value);
                    const convertStr = getConditionStrFromParameter(json);
                    const type = IsNULLorEmpty(convertStr.SearchType) ? '' : Label(convertStr.SearchType, Color.黑);
                    const mode = IsNULLorEmpty(convertStr.SearchMode) ? '' : Label(convertStr.SearchMode, Color.黑);
                    const interval = IsNULLorEmpty(convertStr.DateInterval)
                        ? ''
                        : Label(convertStr.DateInterval, Color.黑);
                    const query = IsNULLorEmpty(convertStr.AdvancedQry) ? '' : Label(convertStr.AdvancedQry, Color.黑);
                    const temp = `${type}${mode}${interval}${query}`;
                    return temp;
                },
            },
            {
                title: '關鍵字',
                field: prop('fsSTATEMENT'),
                sorter: 'string',
                width: 230,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const json = <IFullTextSearchInput>JSON.parse(value);
                    return json.fsKEYWORD;
                },
            },
            {
                title: '操作',
                field: prop('fnSRH_ID'),
                hozAlign: 'left',
                width: 105,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: number = parseInt(cell.getValue());
                    const detailbtn = DetailButton(id, message.Controller);
                    return detailbtn;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const rowdata = <LSearchListModel>cell.getRow().getData();
                    const searchId: number = rowdata.fnSRH_ID | 0;
                    switch (true) {
                        /*點擊:檢視*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            /*新建tab*/
                            const hrefLocation = api.Search2Page + '?id=' + searchId;
                            const statement =rowdata.fsSTATEMENT;
                            windowPostMessage<OpenTabMessageModel>({
                                eventid:'OpenTab',
                                functionId:SearchSetting.FunctionId,
                                tabText:SearchSetting.FrameText,
                                iframeName:SearchSetting.FrameName,
                                src:hrefLocation,
                                description:(<IFullTextSearchInput>JSON.parse(statement)).fsKEYWORD//無效????
                            });
                            break;
                    }
                },
            },
        ],
    });
}
