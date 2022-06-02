
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { VerifyBookingMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { VerifyBookingController, IVerifyBookingController } from '../../Models/Controller/VerifyBookingController';
import { VerifyBookingSearchModel } from '../../Models/Interface/VerifyBooking/VerifyBookingSearchModel';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { VerifyBookingListModel } from '../../Models/Interface/VerifyBooking/VerifyBookingListModel';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { VerifyStatus } from '../../Models/Enum/VerifyStatus';
import { Label } from '../../Models/Templete/LabelTemp';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { Color } from '../../Models/Enum/ColorEnum';
import { DetailButton } from '../../Models/Templete/ButtonTemp';
import { SearchFormId, DeleteModalId, DeleteFormId, sdateId, edateId } from '../../Models/Const/Const.';
import { CheckForm } from '../../Models/Function/Form';
import { getIconByMediaType } from '../../Models/Function/Icon';
import { MediaType, ChineseMediaType } from '../../Models/Enum/MediaType';
import { ErrorMessage, WarningMessage } from '../../Models/Function/Message';
import { ModalTask } from '../../Models/Function/Modal';
import { dayjs, setCalendar,SetDate } from '../../Models/Function/Date';
import { MediaDetail } from '../Materia/_Detail';
import { VerifyRejectReason } from '../../Models/Enum/VerifyRejectReason';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';

/*===============================================================*/
/*宣告變數*/
var table: ItabulatorService;
const message = VerifyBookingMessageSetting;
const valid = FormValidField.VerifyBooking;
var route: IVerifyBookingController = new VerifyBookingController();
var selectids: Array<number> = [];
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $UserIdDropdown: JQuery<HTMLElement> = $(SearchFormId)
    .find("select[name='UserId']")
    .closest('.dropdown');
const $ApproveStatusDropdown: JQuery<HTMLElement> = $(SearchFormId)
    .find("select[name='ApproveStatus']")
    .closest('.dropdown');
/**查詢任務 */
const SearchTask = (input: VerifyBookingSearchModel) => {
    lastSearchCondition = input;
    route
        .Search(input)
        .then(res => {
            if (res.IsSuccess) {
                let data = <Array<VerifyBookingListModel>>res.Data;
                Logger.log('查詢結果:', data);
                Search(data);
            } else {
                ErrorMessage(res.Message);
                Search([]);
            }
        })
        .catch(error => {
            Search([]);
            Logger.viewres(route.api.Search, '查詢審核調用列表', error, true);
        });
};
/**指定列表checkbox設置unchecked */
const RowSetUnchecked = (rowId: number | string) => {
    const rowElement = table
        .GetTable()
        .getRow(rowId)
        .getElement();
    $(rowElement)
        .find('.checkbox')
        .checkbox('set unchecked');
};
/**審核拒絕任務 */
const RejectTask = (VerifyIds: Array<number>) => {
    ModalTask(DeleteModalId, true, {
        closable: false,
        onShow: function() {
            $("input[name='verifyreason']").change(function() {
                $Reason.prepend($Reason.text());
            });
        },
        onApprove: function() {
            const $Form = $(DeleteModalId).find('form');
            const $Reason: JQuery<HTMLElement> = $Form.find("textarea[name='Reason']");
            const $ReasonCheckbox: JQuery<HTMLElement> = $Form.find("input[name='VerifyReason']:checked");
            const IsFormValid: boolean = CheckForm(DeleteFormId, valid.Delete);

            if (IsFormValid) {
                const reason =
                    Number($ReasonCheckbox.val()) == VerifyRejectReason.Other
                        ? <string>$Reason.val()
                        : <string>$ReasonCheckbox.siblings('label').html();
                route
                    .Verify(VerifyIds, false, reason)
                    .then(res => {
                        Logger.res(route.api.Verify, '審核調用', res, true);
                        if (res.IsSuccess) {
                            selectids = [];

                            $(DeleteModalId).modal('hide');
                            const data = <Array<VerifyBookingListModel>>res.Data;
                            for (let item of data) {
                                /*Notice!!!:如果該欄位的內容或狀態會更新,就不應讓不同欄位綁定同一個json[key],不然會不知要更新哪個DOM而不更新*/
                                table.ReactivityUpdate(item.WorkId, item);

                                RowSetUnchecked(item.WorkId);
                            }
                        }
                    })
                    .catch(error => {
                        Logger.viewres(route.api.Verify, '審核調用', error, true);
                    });
            }
            return false;
        },
    });
};
/**審核通過任務 */
const PassTask = (VerifyIds: Array<number>) => {
    ModalTask('#ConfirmPassCodeModal', true, {
        closable: false,
        onShow: function() {
            $('#ConfirmPassCodeModal')
                .find(`.content`)
                .html(`確定要將調用編號【${VerifyIds}】設置為過審？`);
        },
        onApprove: function() {
            route
                .Verify(VerifyIds, true, 'OK')
                .then(res => {
                    Logger.res(route.api.Verify, '審核調用', res, true);
                    if (res.IsSuccess) {
                        selectids = [];
                        const data = <Array<VerifyBookingListModel>>res.Data;
                        $('#ConfirmPassCodeModal').modal('hide');
                        for (let item of data) {
                             /*Notice!!!:如果該欄位的內容或狀態會更新,就不應讓不同欄位綁定同一個json[key],不然會不知要更新哪個DOM而不更新*/
                            table.ReactivityUpdate(item.WorkId, item);
                            RowSetUnchecked(item.WorkId);
                        }
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.Verify, '審核調用', error, true);
                });
            return false;
        },
    });
};
/*------------------------------------------------------------*/
/*預設查詢日期,先預設日期再初始化日曆,會在日曆上顯示預設日期*/
SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/**暫存最後一次的查詢條件 */
var lastSearchCondition: VerifyBookingSearchModel = {
    StartDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    UserId: StringEnum.All,
    ApproveStatus: StringEnum.All,
};
/*預載入查詢*/
SearchTask(lastSearchCondition);

/**重載列表 */
$("button[name='reload']").click(function() {
    SearchTask(lastSearchCondition);
});

/**表單查詢 */
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const userId = <string>$UserIdDropdown.dropdown('get value');
    const approvestatus = <string>$ApproveStatusDropdown.dropdown('get value');
    const IsFormValid = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        SearchTask({
            StartDate: <string>$sdate.val(),
            EndDate: <string>$edate.val(),
            UserId: userId,
            ApproveStatus: <VerifyStatus>approvestatus,
        });
    }
});

/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    $ApproveStatusDropdown.dropdown('set selected', StringEnum.All);
    $UserIdDropdown.dropdown('set selected', StringEnum.All);
});

/**批次過審 */
$("button[name='batchpass']").click(function() {
    if (selectids.length == 0) {
        WarningMessage('至少選擇一筆資料');
        return false;
    } else {
        PassTask(selectids);
    }
});

/**批次不過審 */
const $Reason: JQuery<HTMLElement> = $(DeleteFormId).find("textarea[name='Reason']");
$("button[name='batchreject']").click(function() {
    if (selectids.length == 0) {
        WarningMessage('至少選擇一筆資料');
        return false;
    } else {
        RejectTask(selectids);
    }
});
/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: 'BookingDate', type: Filter.Like, value: word },
        { field: 'BookingLoginId', type: Filter.Like, value: word },
        { field: 'BookingUserName', type: Filter.Like, value: word },
        { field: 'ApproveStatusStr', type: Filter.Like, value: word },
        { field: 'Reason', type: Filter.Like, value: word },
        { field: 'Title', type: Filter.Like, value: word },
        { field: 'MarkInTimeStr', type: Filter.Like, value: word },
        { field: 'MarkOutTimeStr', type: Filter.Like, value: word },
        { field: 'ConfirmLoginId', type: Filter.Like, value: word },
        { field: 'ConfirmTime', type: Filter.Like, value: word },
    ];
    const CategoryWord: MediaType | '' =
        word.indexOf('影') > -1
            ? MediaType.VIDEO
            : word.indexOf('音') > -1 || word.indexOf('聲') > -1
            ? MediaType.AUDIO
            : word.indexOf('圖') > -1
            ? MediaType.PHOTO
            : word.indexOf('文') > -1 || word.indexOf('件') > -1
            ? MediaType.Doc
            : '';
    const mediaStr = getEnumKeyByEnumValue(ChineseMediaType, word);
    if (!IsNULLorEmpty(CategoryWord)) {
        filter.push({
            field: 'MediaType',
            type: Filter.Equal,
            value: CategoryWord,
        });
    }
    if (!IsNULLorEmpty(mediaStr)) {
        filter.push({
            field: 'MediaType',
            type: Filter.Equal,
            value: mediaStr,
        });
    }
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(data: Array<VerifyBookingListModel>) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        addRowPos: 'top',
        index: 'WorkId',
        data: data,
        columns: [
            {
                title: '勾選',
                field: 'ApproveStatus',
                width: 50,
                hozAlign: 'center',
                headerSort: false,
                cellClick: function(e, cell) {
                    const rowdata = <VerifyBookingListModel>cell.getRow().getData();
                    const target: HTMLLabelElement | HTMLInputElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const id = rowdata.WorkId;
                    const $checkbox = cell.getElement().querySelector("input[type='checkbox']");
                    const ischeck: boolean = $checkbox.getAttribute('checked') == 'true' ? true : false;
                    if (target instanceof HTMLLabelElement || target instanceof HTMLInputElement) {
                        if (ischeck) {
                            $checkbox.setAttribute('checked', 'false');
                            selectids = selectids.filter(item => item != id);
                        } else {
                            $checkbox.setAttribute('checked', 'true');
                            if (selectids.indexOf(id) <= -1) {
                                selectids.push(id);
                            }
                        }
                    }
                },
                formatter: function(cell, formatterParams) {
                    const id = cell.getValue();
                    const rowdata = <VerifyBookingListModel>cell.getRow().getData();
                    //const checkbox = `<div class="ui checkbox" data-Id="${id}"><input type="checkbox" name="reconvert"> <label></label></div>`;
                    const checkbox =
                        rowdata.ApproveStatus == VerifyStatus.Pending
                            ? `<div class="ui checkbox" data-Id="${id}"><input type="checkbox" name="reconvert"> <label></label></div>`
                            : '';
                    return checkbox;
                },
            },
            {
                title: '調用資訊',
                field: 'ApproveMemo',
                sorter: 'string',
                minWidth: 350,
                formatter: function(cell, formatterParams) {
                    const rowdata = <VerifyBookingListModel>cell.getRow().getData();
                    const itemdiv: HTMLDivElement = document.createElement('div');
                    const icon = getIconByMediaType(<MediaType>rowdata.MediaType);
                    const bookingDate = IsNULLorEmpty(rowdata.BookingDate)
                        ? ''
                        : dayjs(rowdata.BookingDate).format('YYYY/MM/DD HH:mm:ss');
                    // const statusStr = getEnumKeyByEnumValue(VerifyChineseStatus, rowdata.ApproveStatus);
                    const statusStr = rowdata.ApproveStatusStr;
                    const color =
                        rowdata.ApproveStatus == VerifyStatus.Passed
                            ? Color.綠
                            : rowdata.ApproveStatus == VerifyStatus.Rejected
                            ? Color.紅
                            : rowdata.ApproveStatus == VerifyStatus.Pending
                            ? Color.藍
                            : Color.基本;
                    const mediaStr = getEnumKeyByEnumValue(ChineseMediaType, rowdata.MediaType);
                    itemdiv.className = 'ui items';
                    itemdiv.innerHTML = `<div class="ui item">
                                          <div class="content">
                                            <div class="ui inverted grey header">  ${icon} ${rowdata.Title} </div>
                                            <div class="meta">
                                              ${Label('【調用編號】' + rowdata.BookingId, '')}
                                              ${Label('【調用日期】' + bookingDate, '')} 
                                              ${Label('【調用者】' + rowdata.BookingLoginId + '(' + rowdata.BookingUserName + ')', '')} 
                                            </div>
                                            <div class="description">
                                               ${Label(icon + mediaStr, Color.黑)}  ${Label(statusStr, color)} 
                                               ${Label('【調用原因】' + rowdata.Reason, '')} 
                                               <div class="ui label">【審核備註】<span class="ui tiny ${color} inverted header">
                                               ${rowdata.ApproveMemo}
                                               </span></div>
                                            </div>
                                          </div>
                                        </div>`;
                    return itemdiv.outerHTML;
                },
            },
            {
                title: '剪輯時間',
                field: 'MarkInTimeStr',
                sorter: 'string',
                width: 155,
                formatter: function(cell, formatterParams) {
                    const rowdata = <VerifyBookingListModel>cell.getRow().getData();
                    const markInStr: string = IsNULLorEmpty(rowdata.MarkInTimeStr) ? '無資料' : rowdata.MarkInTimeStr;
                    const markOutStr: string = IsNULLorEmpty(rowdata.MarkOutTimeStr)
                        ? '無資料'
                        : rowdata.MarkOutTimeStr;
                    const markIn = '<div>【開始】' + markInStr + '</div>';
                    const markOut = '<div>【結束】' + markOutStr + '</div>';
                    return `${markIn}${markOut}`;
                },
            },
            {
                title: '審核者',
                field: 'ConfirmLoginId',
                sorter: 'string',
                width: 135,
            },
            { title: '審核日期', field: 'ConfirmTime', sorter: 'string', width: 165 },
            {
                title: '檢視',
                field: 'WorkId',
                sorter: 'number',
                hozAlign: 'center',
                width: 75,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <VerifyBookingListModel>row.getData();
                    const id = rowdata.WorkId;
                    cell.getElement().classList.add('tabulator-operation');
                    const detailbtn = DetailButton(id, message.Controller);
                    const btngroups: string = detailbtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const row = cell.getRow();
                    const rowdata = <VerifyBookingListModel>row.getData();
                    MediaDetail(route.api.ShowDetail, rowdata.WorkId, rowdata.MediaType);
                },
            },
            {
                title: '審核',
                field: 'ApproveStatusStr',
                hozAlign: 'left',
                width: 190,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <VerifyBookingListModel>row.getData();
                    const passbtn = `<button type="button" name="pass" class="ui green  button" data-inverted="" data-tooltip="標為過審" data-position="bottom center"><i class="check icon"></i>過審</button>`;
                    const rejectbtn = `<button type="button" name="reject" class="ui red button" data-inverted="" data-tooltip="標為不過審" data-position="bottom center"><i class="delete icon"></i>不過審</button>`;
                    const verifybtns = `<div class="ui mini buttons">${passbtn} <div class="or" data-text="或"></div>${rejectbtn} </div>`;
                    const btngroups: string = rowdata.ApproveStatus == VerifyStatus.Pending ? verifybtns : '';
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const row = cell.getRow();
                    const rowdata = <VerifyBookingListModel>row.getData();
                    const id = rowdata.WorkId;
                    switch (true) {
                        /**審核通過 */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('check icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'pass':
                            PassTask([id]);
                            break;
                        /**不過審 */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'reject':
                            RejectTask([id]);
                            break;
                        default:
                            break;
                    }
                },
            },
        ],
    });
}
