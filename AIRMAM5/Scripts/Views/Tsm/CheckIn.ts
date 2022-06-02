import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { TabulatorSetting, initSetting } from '../../Models/initSetting';
import { ITsmController, TsmController } from '../../Models/Controller/TsmController';
import { FormValidField } from '../../Models/Const/FormValid';
import { TsmMessageSetting } from '../../Models/MessageSetting';
import { GetPendingTapeModel } from '../../Models/Interface/Tsm/GetPendingTapeModel';
import { SuccessMessage, ErrorMessage } from '../../Models/Function/Message';
import { Logger } from '../../Models/Class/LoggerService';
import { Filter } from '../../Models/Enum/Filter';
import { IResponse } from '../../Models/Interface/Shared/IResponse';
import { TsmCheckInUpdateModel } from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';
import { JsonDateToDate } from '../../Models/Function/Date';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import * as dayjs_ from 'dayjs';
import { StringEnum } from '../../Models/Enum/StringEnum';
/*===============================================================*/
/*宣告變數*/
const dayjs = (<any>dayjs_).default || dayjs_;
const message = TsmMessageSetting;
const valid = FormValidField.Tsm;
const route: ITsmController = new TsmController();
const $CheckInBtn = $('#CheckInBtn'); //上架按鈕
/**回傳Modal性質*/
const prop = (key: keyof GetPendingTapeModel): string => {
    return route.GetProperty<GetPendingTapeModel>(key);
};
/**載入頁面時要先檢查是否有上架作業,若有則禁止上架 */
var checkWork = (function() {
    route
        .CheckInWorks()
        .then(res => {
            const hasWork: boolean = <boolean>res.Data;
            const convertRes = Object.assign(<IResponse>{}, res, { Data: { HasWork: hasWork } });
            Logger.res(route.api.CheckInWorks, '檢查上架作業', convertRes, true);
            res.IsSuccess && !hasWork ? $CheckInBtn.removeClass('disabled') : $CheckInBtn.addClass('disabled');
        })
        .catch(error => {
            Logger.viewres(route.api.CheckInWorks, '檢查上架作業', error, true);
            $CheckInBtn.addClass('disabled');
        });
})();
/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('TapeNumber'), type: Filter.Like, value: word },
        { field: prop('StatusName'), type: Filter.Like, value: word },
        { field: prop('Priority'), type: Filter.Like, value: word },
        { field: prop('WrokId'), type: Filter.Like, value: word },
        { field: prop('CreatedDate'), type: Filter.Like, value: word },
        { field: prop('CreatedBy'), type: Filter.Like, value: word },
        { field: prop('BookingReason'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
const table: ItabulatorService = new tabulatorService(initSetting.TableId, {
    height: TabulatorSetting.height,
    layout: TabulatorSetting.layout,
    ajaxURL: route.api.GetPendingTape,
    ajaxContentType: 'json',
    ajaxConfig: 'GET',
    ajaxParams: {},
    index: prop('TapeNumber'),
    groupBy: prop('TapeNumber'),
    initialSort: [{ column: prop('TapeNumber'), dir: 'asc' }],
    groupStartOpen: false,
    groupToggleElement: 'header',
    groupHeader: [
        function(value, count, data) {
            return `<div class="group-header">磁帶【${value}】：待上架檔案${count}個 </div>`;
        },
    ],
    columns: [
        { title: '磁帶編號', field: prop('TapeNumber'), sorter: 'string', width: 105 },
        { title: '狀態', field: prop('StatusName'), sorter: 'string', width: 110 },
        {
            title: '要求時間',
            field: prop('CreatedDate'),
            sorter: 'string',
            width: 160,
            mutator: function(value, data, type, mutatorParams, cell) {
                //為了篩選器啟用變異,已確認不影響功能列
                if (!IsNULLorEmpty(value)) {
                    const convertDate = JsonDateToDate(value);
                    return IsNULLorEmpty(convertDate) ? '' : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                } else {
                    return StringEnum.Empty;
                }
            },
        },
        { title: '調用人員', field: prop('CreatedBy'), sorter: 'string', width: 150 },
        { title: '轉檔編號', field: prop('WrokId'), sorter: 'number', width: 150 },
        {
            title: '轉檔優先序',
            field: prop('Priority'),
            sorter: 'string',
            width: 100,
            titleFormatter: function() {
                return '轉檔<br>優先序';
            },
        },
        { title: '調用原因', field: prop('BookingReason'), sorter: 'string', minWidth: 120 },
    ],
});

/**上架 */
$CheckInBtn.click(function() {
    $(this).addClass('disabled');
    route
        .CheckIn()
        .then(res => {
            res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
            $(this).removeClass('disabled');
        })
        .catch(error => {
            Logger.viewres(route.api.CheckIn, '上架', error, true);
            $(this).removeClass('disabled');
        });
});

/**接受SignalR postMessage以更新列表 */
window.top.addEventListener(
    'message',
    function(e) {
        if (e.data.eventid === 'UpdateTsmCheckInList') {
            const list = (<TsmCheckInUpdateModel>e.data).list;
            table.GetTable().setData(list);
        }
    },
    false
);
