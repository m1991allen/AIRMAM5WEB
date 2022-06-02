import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { dayjs, setCalendar, SetDate } from '../../Models/Function/Date';
import { API } from '../../Models/Const/API';
import { DetailModal } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { LLoginSearchModel, LLoginListModel } from '../../Models/Interface/ILLoginIndex';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { CheckForm } from '../../Models/Function/Form';
import { sdateId, edateId, SearchFormId } from '../../Models/Const/Const.';
import { DetailButton } from '../../Models/Templete/ButtonTemp';
import { LLoginMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { ErrorMessage } from '../../Models/Function/Message';
import { Filter } from '../../Models/Enum/Filter';
/*===============================================================*/
/*宣告變數*/
const api = API.L_Login;
const valid = FormValidField.L_Login;
const message = LLoginMessageSetting;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $SysAccount: JQuery<HTMLElement> = $('#SysAccount').closest('.dropdown');
const selfId = <string>$('#SelfAccount').val();
var table: ItabulatorService;
/**回傳Modal性質*/
const prop = (key: keyof LLoginListModel): string => {
    return key;
};
//===================================================
/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
$SysAccount.dropdown('set selected', selfId);
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
        if (edate.isBefore(sdate)) {
            $(this).addClass('error');
            ErrorMessage('登入日期的起訖範圍錯誤');
        } else {
            Search({
                BeginDate: <string>$sdate.val(),
                EndDate: <string>$edate.val(),
                UserId: <string>$SysAccount.dropdown('get value'),
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
        { field: prop('fnLOGIN_ID'), type: Filter.Like, value: word },
        { field: prop('fsLOGIN_ID'), type: Filter.Like, value: word },
        { field: prop('fdSTIME'), type: Filter.Like, value: word },
        { field: prop('fdETIME'), type: Filter.Like, value: word },
        { field: prop('UsageTime'), type: Filter.Like, value: word },
        { field: prop('fsNOTE'), type: Filter.Like, value: word },
        { field: prop('fdUPDATED_DATE'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: LLoginSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        columns: [
            { title: 'ID', field: prop('fnLOGIN_ID'), width: 90, sorter: 'number', visible: false },
            { title: '登入帳號', field: prop('fsLOGIN_ID'), sorter: 'string', width: 135 },
            {
                title: '登入時間',
                field: prop('fdSTIME'),
                sorter: 'string',
                width: 170,
                //Modified_20200420:後端調整回傳方法(JsonConvert)日期已格式化
                // mutator: function(value, data, type, mutatorParams, cell) {
                //     //為了篩選器啟用變異,已確認不影響功能列
                //     const convertDate = JsonDateToDate(value);
                //     const date: string = IsNULLorEmpty(convertDate)
                //         ? ''
                //         : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                //     return date;
                // },
            },
            {
                title: '登出時間',
                field: prop('fdETIME'),
                sorter: 'string',
                width: 170,
                //Modified_20200420:後端調整回傳方法(JsonConvert)日期已格式化
                // mutator: function(value, data, type, mutatorParams, cell) {
                //     //為了篩選器啟用變異,已確認不影響功能列
                //     const convertDate = JsonDateToDate(value);
                //     const date: string = IsNULLorEmpty(convertDate)
                //         ? ''
                //         : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                //     return date;
                // },
            },
            { title: '登入使用時間', field: prop('UsageTime'), sorter: 'string', width: 160 },
            {
                title: '備註',
                field: prop('fsNOTE'),
                sorter: 'string',
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const showValue =
                        cell.getValue() == '未正常登出' ? `<span class="ui red label">未正常登出</span> </td>` : value;
                    return showValue;
                },
            },
            {
                title: '最後異動時間',
                field: prop('fdUPDATED_DATE'),
                sorter: 'string',
                width: 170,
                //Modified_20200420:後端調整回傳方法(JsonConvert)日期已格式化
                // mutator: function(value, data, type, mutatorParams, cell) {
                //     //為了篩選器啟用變異,已確認不影響功能列
                //     const convertDate = JsonDateToDate(value);
                //     const date: string = IsNULLorEmpty(convertDate)
                //         ? ''
                //         : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                //     return date;
                // },
            },
            {
                title: '操作',
                field: prop('fnLOGIN_ID'),
                hozAlign: 'left',
                width: 95,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: number = parseInt(cell.getValue());
                    const detailbtn = DetailButton(id, message.Controller);
                    return detailbtn;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const rowdata = <LLoginListModel>cell.getRow().getData();
                    const loginId: number = rowdata.fnLOGIN_ID | 0;
                    switch (true) {
                        /*點擊:檢視*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.L_Login, Action.ShowDetails, { id: loginId });
                            break;
                    }
                },
            },
        ],
    });
}
