import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { dayjs, setCalendar, SetDate } from '../../Models/Function/Date';
import { API } from '../../Models/Const/API';
import { IDate } from '../../Models/Interface/Shared/IDate';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { sdateId, edateId, SearchFormId } from '../../Models/Const/Const.';
import { FormValidField } from '../../Models/Const/FormValid';
import { CheckForm } from '../../Models/Function/Form';
import { ErrorMessage } from '../../Models/Function/Message';
import { Filter } from '../../Models/Enum/Filter';
import { LTranListModel } from '../../Models/Interface/LTran/LTranListModel';
/*===============================================================*/
/*宣告變數*/
const api = API.L_Tran;
// const message = LTranMessageSetting;
const valid = FormValidField.L_Tran;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
var table: ItabulatorService;
/**回傳Modal性質*/
const prop = (key: keyof LTranListModel): string => {
    return key;
};
//===============================
/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/*頁面載入查詢*/
Search({
    BeginDate: <string>$('#sdate').val(),
    EndDate: <string>$('#edate').val(),
});
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
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
            ErrorMessage('查詢日期的起訖範圍錯誤');
        } else {
            Search({
                BeginDate: <string>$sdate.val(),
                EndDate: <string>$edate.val(),
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
        { field: prop('fnID'), type: Filter.Like, value: word },
        { field: prop('fsTABLE_NAME'), type: Filter.Like, value: word },
        { field: prop('fsFILE_NO'), type: Filter.Like, value: word },
        { field: prop('fsTITLE'), type: Filter.Like, value: word },
        { field: prop('fsACTION'), type: Filter.Like, value: word },
        { field: prop('fsACTION_BY'), type: Filter.Like, value: word },
        { field: prop('fdACTION_DATE'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: IDate) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        columns: [
            { title: '資料ID', field: prop('fnID'), width: 110, sorter: 'number', visible: false },
            { title: '資料表名稱', field: prop('fsTABLE_NAME'), width: 120, sorter: 'string' },
            { title: '資料編號', field: prop('fsFILE_NO'), width: 150, sorter: 'string' },
            { title: '檔案標題', field: prop('fsTITLE'), sorter: 'string', minWidth: 330 },
            { title: '執行動作', field: prop('fsACTION'), width: 110, sorter: 'string' },
            { title: '異動帳號', field: prop('fsACTION_BY'), width: 135, sorter: 'string' },
            {
                title: '異動時間',
                field: prop('fdACTION_DATE'),
                width: 170,
                sorter: 'string',
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
        ],
    });
}
