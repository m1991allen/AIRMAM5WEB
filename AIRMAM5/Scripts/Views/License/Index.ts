import { tabulatorService } from '../../Models/Class/tabulatorService';
import {  dayjs, setCalendar } from '../../Models/Function/Date';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { CheckForm } from '../../Models/Function/Form';
import {
    SearchFormId,
    CreateFormId,
    CreateModalId,
    EditFormId,
    EditModalId,
    //sdateId,
    edateId,
} from '../../Models/Const/Const.';
import { EditButton, DetailButton } from '../../Models/Templete/ButtonTemp';
import { LicenseMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { Color } from '../../Models/Enum/ColorEnum';
import { Label } from '../../Models/Templete/LabelTemp';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { ErrorMessage, WarningMessage } from '../../Models/Function/Message';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { LicenseController, ILicenseController } from '../../Models/Controller/LicenseController';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { LicenseListModel } from '../../Models/Interface/License/LicenseListModel';
import { LicenseSearchModel } from 'Models/Interface/License/LicenseSearchModel';
import { Logger } from '../../Models/Class/LoggerService';
import { LicenseCreateModel } from 'Models/Interface/License/LicenseCreateModel';
/*=====================宣告變數====================================*/
var table: ItabulatorService;
var route: ILicenseController = new LicenseController();
const valid = FormValidField.License;
const message = LicenseMessageSetting;
const $licensename: JQuery<HTMLInputElement> = $('#licensename');
const $edate: JQuery<HTMLInputElement> = $(edateId);
const prop = (key: keyof LicenseListModel): string => {
    return route.GetProperty<LicenseListModel>(key);
};

/*************************************日期 */
//SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar(`${SearchFormId} .calendar`, 'date');

/*頁面載入查詢*/
Search({
    name: <string>$licensename.val(),
    edt: <string>$edate.val(),
});

/*表單查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const $form = $(SearchFormId);
    //const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    //if (IsFormValid) {
        const edate = dayjs($edate.val());
        const name = $form.find('#licensename').val();
        Search({
            name: <string>$licensename.val(),
            edt: <string>$edate.val(),
        });
    //}
});

/**表單清空 */
$("button[name='reset']").click(function() {
    //SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    //const $form = $(SearchFormId);
    $edate.val('');
    $('licensename').val('');
});

/**新增版權 */
ModalTask(CreateModalId, false, {
    closable: false,
    onShow: function() {
        //SetDate('#EndDate', dayjs().add(7, 'day'), 'YYYY/MM/DD');
        setCalendar(`${CreateFormId} .calendar`, 'date');
    },
    onApprove: function() {
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            const $form = $(CreateFormId);
            const $ISACTIVE: JQuery<HTMLElement> = $form.find("input[name='switchActive']");
            const $ISBOOKINGALERT: JQuery<HTMLElement> = $form.find("input[name='switchBookingAlert']");
            const $ISNOTBOOKING: JQuery<HTMLElement> = $form.find("input[name='switchNotBooking']");
            let edt: string = <string>$form.find("input[name='EndDate']").val();
            const input:LicenseCreateModel={
                LicenseCode: <string>$form.find("input[name='LicenseCode']").val(),
                LicenseName: <string>$form.find("input[name='LicenseName']").val(),
                LicenseDesc: <string>($form.find("textarea[name='LicenseDesc']").val()),
                EndDate: IsNULLorEmpty(edt) ? "" : dayjs(edt).format('YYYY/MM/DD'),
                Order: Number($form.find("input[name='Order']").val()),
                IsActive: $ISACTIVE.prop('checked') ? true : false,
                IsBookingAlert: $ISBOOKINGALERT.prop('checked') ? true : false,
                IsNotBooking: $ISNOTBOOKING.prop('checked') ? true : false,
                AlertMessage: <string>$form.find("input[name='AlertMessage']").val(),
            }
            if(input.IsBookingAlert && input.IsNotBooking){
                WarningMessage('若設置禁止調用,則無法開啟調用提醒');
                //$ISBOOKINGALERT.parent('.checkbox').checkbox('set unchecked');//尚未確認是否有效
                return false;
            }
            if(input.IsBookingAlert && (IsNULLorEmpty(input.AlertMessage) || IsNullorUndefined(input.AlertMessage))){
                WarningMessage('若開啟調用提醒,則必須輸入提醒訊息的內容');
                $form.find("input[name='AlertMessage']").parent('.field').addClass('error');
                return false;
            }
            route
                .Create(input)
                .then(res => {
                    Logger.res(route.api.Create, '新增版權', res);
                    if (res.IsSuccess) {
                        const data = <LicenseCreateModel>res.Data;
                        table.AddRow(<LicenseListModel>{
                            LicenseCode: data.LicenseCode,
                            LicenseName: data.LicenseName,
                            EndDate: data.EndDate,
                            AlertMessage: data.AlertMessage,
                            IsNotBooking: data.IsNotBooking,
                            IsBookingAlert: data.IsBookingAlert,
                            IsActive: data.IsActive,
                        });
                        $(CreateModalId).modal('hide');
                        $form.trigger('reset');
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.Create, '新增版權', error);
                });
        }
        return false;
    },
    onDeny:function(){
        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
    }
});

/*查詢結果*/
function Search(SearchParams: LicenseSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('LicenseCode'),
        columns: [
            { title: '版權代碼', field: prop('LicenseCode'), width: 110, sorter: 'string', /*visible: false*/ },
            {
                title: '版權名稱',
                field: prop('LicenseName'),
                sorter: 'string',
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const title = <string>cell.getValue();
                    const span: HTMLSpanElement = document.createElement('span');
                    span.innerHTML = title;
                    span.setAttribute('title', title);
                    return span.outerHTML;
                },
            },
            {
                title: '版權到期日',
                field: prop('EndDate'),
                sorter: 'string',
                width: 145,
            },
            {
                title: '啟用',
                field: prop('IsActive'),
                sorter: 'boolean',
                width: 105,
                formatter: function(cell, formatterParams) {
                    const status: boolean | null = cell.getValue();
                    const showtext: string = status ? '啟用' : !status ? '停用' : '未知';
                    const color: Color | '' = status ? Color.綠 : !status ? Color.紅 : StringEnum.Empty;
                    return Label(showtext, color);
                },
            },
            {
                title: '調用提示',
                field: prop('IsBookingAlert'),
                sorter: 'boolean',
                width: 105,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <LicenseListModel>row.getData();
                    const status: boolean | null = cell.getValue();
                    const showtext: string = status ? '提醒' : !status ? '不提醒' : '未知';
                    const color: Color | '' = status ? Color.綠 : !status ? Color.紅 : StringEnum.Empty;
                    return Label(showtext, color, null, rowdata.AlertMessage);
                },
            },
            {
                title: '提醒訊息內容',
                field: prop('AlertMessage'),
                sorter: 'string',
                minWidth: 150,
            },
            {
                title: '調用禁止',
                field: prop('IsNotBooking'),
                sorter: 'boolean',
                width: 105,
                formatter: function(cell, formatterParams) {
                    const status: boolean | null = cell.getValue();
                    const showtext: string = status ? '禁止調用' : !status ? '可調用' : '未知';
                    const color: Color | '' = status ? Color.紅 : !status ? Color.綠 : StringEnum.Empty;
                    return Label(showtext, color);
                },
            },
            {
                title: '操作',
                field: prop('LicenseCode'),
                hozAlign: 'center',
                width: 190,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: number = parseInt(cell.getValue());
                    const editbtn = EditButton(id, message.Controller);
                    const detailbtn = DetailButton(id, message.Controller);
                    //const deletebtn = DeleteButton(id, message.Controller); //不需用'刪除功能鈕'
                    const btngroups: string = editbtn + detailbtn ;//+ deletebtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const rowdata = <LicenseListModel>cell.getRow().getData();
                    const LicenseCode: string = rowdata.LicenseCode;
                    switch (true) {
                        /*點擊:檢視*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(
                                Controller.License,
                                Action.ShowDetails,
                                { code: LicenseCode },
                                { calendar: false, calendarType: 'datetime', dropdown: false }
                            );
                            break;
                            /*點擊:編輯*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: LicenseCode }).then(IsSuccess => {
                                if (IsSuccess) {
                                    const $form = $(EditFormId);
                                    const $ISACTIVE: JQuery<HTMLElement> = $form.find("input[name='switchActive']");
                                    const $ISBOOKINGALERT: JQuery<HTMLElement> = $form.find("input[name='switchBookingAlert']");
                                    const $ISNOTBOOKING: JQuery<HTMLElement> = $form.find("input[name='switchNotBooking']");
                                    route;
                                    ModalTask(EditModalId, true, {
                                        closable: false,
                                        onShow: function() {
                                            setCalendar(`${EditFormId} .calendar`, 'date');
                                        },
                                        onApprove: function() {
                                            const IsFormValid: boolean = CheckForm(EditFormId, valid.Edit);
                                            let edt: string = <string>$form.find("input[name='EndDate']").val();
                                            const input:LicenseCreateModel={
                                                LicenseCode: LicenseCode,
                                                LicenseName: <string>$form.find("input[name='LicenseName']").val(),
                                                LicenseDesc: <string>(
                                                    $form.find("textarea[name='LicenseDesc']").val()
                                                ),
                                                EndDate: IsNULLorEmpty(edt) ? "" : dayjs(edt).format('YYYY/MM/DD'),
                                                Order: Number($form.find("input[name='Order']").val()),
                                                IsActive: $ISACTIVE.prop('checked') ? true : false,
                                                IsBookingAlert: $ISBOOKINGALERT.prop('checked') ? true : false,
                                                IsNotBooking: $ISNOTBOOKING.prop('checked') ? true : false,
                                                AlertMessage: <string>$form.find("input[name='AlertMessage']").val(),
                                            };
                                            if(input.IsBookingAlert && input.IsNotBooking){
                                                WarningMessage('若設置禁止調用,則無法開啟調用提醒');
                                                //$ISBOOKINGALERT.parent('.checkbox').checkbox('set unchecked');//尚未確認是否有效
                                                return false;
                                            }
                                            if(input.IsBookingAlert && (IsNULLorEmpty(input.AlertMessage)||IsNullorUndefined(input.AlertMessage))){
                                                WarningMessage('若開啟調用提醒,則必須輸入提醒訊息的內容');
                                                $form.find("input[name='AlertMessage']").parent('.field').addClass('error');
                                                return false;
                                            }
                                            if (IsFormValid) {
                                                route
                                                    .Edit(input)
                                                    .then(res => {
                                                        Logger.res(route.api.Edit, '編輯版權', res);
                                                        if (res.IsSuccess) {
                                                            const data = <LicenseCreateModel>res.Data;
                                                            table.ReactivityUpdate(LicenseCode, <LicenseListModel>{
                                                                LicenseCode: data.LicenseCode,
                                                                LicenseName: data.LicenseName,
                                                                EndDate: data.EndDate,
                                                                AlertMessage: data.AlertMessage,
                                                                IsBookingAlert: data.IsBookingAlert,
                                                                IsNotBooking: data.IsNotBooking,
                                                                IsActive: data.IsActive,
                                                            });
                                                            $(EditModalId).modal('hide');
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Edit, '編輯版權', error);
                                                    });
                                            }
                                            
                                            return false;
                                        },
                                    });
                                } else {
                                    ErrorMessage('編輯版權燈箱發生問題!');
                                }
                            });
                            break;
                        default:
                            break;
                    }
                },
            },
        ],
    });
}
