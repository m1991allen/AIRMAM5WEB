import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { setCalendar } from '../../Models/Function/Date';
import { API } from '../../Models/Const/API';
import { DetailModal } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { dayjs, SetDate } from '../../Models/Function/Date';
import { LLogSearchModel, LLogListModel } from '../../Models/Interface/ILLogIndex';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { CheckForm } from '../../Models/Function/Form';
import { DetailButton } from '../../Models/Templete/ButtonTemp';
import { LLogMessageSetting } from '../../Models/MessageSetting';
import { SearchFormId, edateId, sdateId } from '../../Models/Const/Const.';
import { FormValidField } from '../../Models/Const/FormValid';
import { ErrorMessage } from '../../Models/Function/Message';
import { Filter } from '../../Models/Enum/Filter';
/*===============================================================*/
/*宣告變數*/
const api = API;
const message = LLogMessageSetting;
const valid = FormValidField.L_Log;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $SysAccount: JQuery<HTMLElement> = $('#UserId').closest('.dropdown');
const selfId = <string>$('#SelfAccount').val();
/**回傳Modal性質*/
const prop = (key: keyof LLogListModel): string => {
    return key;
};
//=====================================================================
/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
var table: ItabulatorService;
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
        const searchid = <string>$SysAccount.dropdown('get value');
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
        { field: prop('fnlLOG_ID'), type: Filter.Like, value: word },
        { field: prop('fsTYPE'), type: Filter.Like, value: word },
        { field: prop('fsGROUP'), type: Filter.Like, value: word },
        { field: prop('fsCREATED_BY'), type: Filter.Like, value: word },
        { field: prop('fsDESCRIPTION'), type: Filter.Like, value: word },
        { field: prop('fdCREATED_DATE'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: LLogSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: api.L_Log.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        // groupBy: prop('fsTYPE'),
        // groupToggleElement: 'header',
        // groupHeader: [
        //     function(value, count, getData) {
        //         return `<div class="group-header">${value}( ${count}筆紀錄 )</div>`;
        //     },
        // ],
        columns: [
            { title: 'ID', field: prop('fnlLOG_ID'), width: 90, sorter: 'number', visible: false },
            { title: '類別', field: prop('fsTYPE'), width: 130, sorter: 'string' },
            { title: '群組', field: prop('fsGROUP'), sorter: 'string', visible: false, download: false },
            { title: '帳號', field: prop('fsCREATED_BY'), width: 145, sorter: 'string' },
            { title: '描述', field: prop('fsDESCRIPTION'), sorter: 'string', minWidth: 480 },
            {
                title: '建立時間',
                field: prop('fdCREATED_DATE'),
                width: 170,
                sorter: 'string',
                //Modified:後端調整回傳方法(JsonConvert)日期已格式化
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
                field: prop('fnlLOG_ID'),
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
                    const rowdata = <LLogListModel>cell.getRow().getData();
                    const loginId: number = rowdata.fnlLOG_ID | 0;
                    switch (true) {
                        /*點擊:檢視*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.L_Log, Action.ShowDetails, { id: loginId });
                            break;
                    }
                },
            },
        ],
    });
}
