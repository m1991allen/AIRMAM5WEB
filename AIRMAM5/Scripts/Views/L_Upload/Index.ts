import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { dayjs, setCalendar,SetDate } from '../../Models/Function/Date';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { FileSetting, initSetting, TabulatorSetting } from '../../Models/initSetting';
import { LUploadSearchModel, LUploadListModel, WorkProgressModel } from '../../Models/Interface/ILUploadIndex';
import { GetProgressBarHtml } from './FileProgress';
import { FormValidField } from '../../Models/Const/FormValid';
import { LUploadMessageSetting } from '../../Models/MessageSetting';
import { sdateId, edateId, SearchFormId, EditModalId, EditFormId } from '../../Models/Const/Const.';
import { ErrorMessage, WarningMessage, SuccessMessage } from '../../Models/Function/Message';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { CheckForm } from '../../Models/Function/Form';
import { EditButton, DetailButton } from '../../Models/Templete/ButtonTemp';
import { LUploadController, ILUploadController } from '../../Models/Controller/LUploadController';
import { WorkStatus } from '../../Models/Enum/WorkStatus';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
/*=====================宣告變數================================*/
const message = LUploadMessageSetting;
const valid = FormValidField.L_Upload;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $WorkStatus: JQuery<HTMLElement> = $('#WorkStatus').closest('.dropdown');
const $ReconvertModal: JQuery<HTMLElement> = $('#ReconvertModal');
/**回傳Modal性質*/
const prop = (key: keyof LUploadListModel): string => key;
/**允許更新進度條的工作狀態 */
const allowWorkStatus: Array<string> = [
    WorkStatus.OnSchedule,
    WorkStatus.TransferToAP,
    WorkStatus.GearshiftProgramInit,
    WorkStatus.TranscodingComplete,
    WorkStatus.ConfirmFileStatus,
    WorkStatus.FilmVerify,
    WorkStatus.InTransition,
    WorkStatus.HighResolution,
    WorkStatus.LowResolution,
    WorkStatus.KeyFrameFetching,
    WorkStatus.SpeechRecognition,
    WorkStatus.FaceRecognition,
    WorkStatus.UpdateConversionInfo,
];
var selectids: Array<string> = [];
var route: ILUploadController = new LUploadController();
var timeoutId: null | ReturnType<typeof setTimeout> = null
var table: ItabulatorService=new tabulatorService(initSetting.TableId, {
    height: TabulatorSetting.height,
    layout: TabulatorSetting.layout,
    data: [],
    index: prop('fnWORK_ID'),
    rowFormatter: function(row: Tabulator.RowComponent) {
        const rowdata = <LUploadListModel>row.getData();
        row.getElement().setAttribute('data-workid', rowdata.fnWORK_ID.toString());
        return row;
    },
    columns: [
        {
            title: '勾選',
            field: 'SelectStatus',
            width: 50,
            sorter: 'number',
            hozAlign: 'center',
            headerSort: false,
            formatter: function(cell, formatterParams) {
                const rowdata = <LUploadListModel>cell.getRow().getData();
                const id = rowdata.fnWORK_ID;
                const isCheck=cell.getValue()===true?'checked="checked"':'';
                const checkbox = `<div class="ui checkbox" data-Id="${id}"><input type="checkbox" name="reconvert" ${isCheck}> <label></label></div>`;
                return rowdata.WorkStatus===WorkStatus.VerifyReject ?"":checkbox;
            },
            cellClick: function(e, cell) {
                const target: HTMLLabelElement | HTMLInputElement | HTMLDivElement | HTMLElement = <any>e.target;
                const row = cell.getRow();
                const rowdata = <LUploadListModel&{SelectStatus?:boolean}>row.getData();
                const id = IsNULLorEmpty(rowdata.fnWORK_ID) ? '0': rowdata.fnWORK_ID.toString();
                const ischeck:boolean =rowdata.SelectStatus===true;
                if (target instanceof HTMLLabelElement || target instanceof HTMLInputElement) {
                    if (ischeck) {
                        $('input[name="selectall"]').parent().checkbox('set unchecked');
                        row.update(<{SelectStatus?:boolean}>{SelectStatus:false});
                        selectids = selectids.filter(item => item != id);
                    } else {
                        row.update(< {SelectStatus?:boolean}>{SelectStatus:true});
                        if (selectids.indexOf(id) <= -1) {  selectids.push(id); }
                    }
                }
            }
        },
        { title: 'ID', field: prop('fnWORK_ID'), width: 90, sorter: 'number' },
        {
            title: '轉檔進度/轉檔狀態',
            titleFormatter: function() {
                return '轉檔進度/<br>轉檔狀態';
            },
            field: prop('Progress'),
            width: 130,
            sorter: 'string',
            formatter: function(cell, formatterParams) {
                const rowdata = <LUploadListModel>cell.getRow().getData();
                const progressbar = GetProgressBarHtml(
                    rowdata.fnWORK_ID,
                    rowdata.Progress,
                    rowdata.StatusName,
                    rowdata.StatusColor
                );
                return progressbar;
            },
        },
        {
            title: '開始轉檔時間',
            field: prop('fdSTIME'),
            sorter: 'string',
            width: 160,
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
            title: '結束轉檔時間',
            field: prop('fdETIME'),
            sorter: 'string',
            width: 160,
        },
        { title: '檔案資訊', field: prop('C_sFILE_INFO'), minWidth: 200, sorter: 'string' },
        { title: '轉檔參數', field: prop('fsPARAMETERS'), sorter: 'string', visible: false },
        { title: '轉檔結果', field: prop('fsRESULT'), sorter: 'string', visible: false },
        {
            title: '優先順序',
            field: prop('fsPRIORITY'),
            width: 75,
            sorter: 'string',
            titleFormatter: function() {
                return '優先<br>順序';
            },
        },
        { title: '備註', field: prop('fsNOTE'), sorter: 'string', visible: false },
        { title: '建立者', field: prop('CreatedBy'), sorter: 'string', width: 125 },
        {
            title: '操作',
            field: prop('fnWORK_ID'),
            hozAlign: 'left',
            width: 110,
            formatter: function(cell, formatterParams) {
                cell.getElement().classList.add('tabulator-operation');
                const id: number = parseInt(cell.getValue());
                const editbtn = EditButton(id, message.Controller);
                const detailbtn = DetailButton(id, message.Controller);
                const btngroups: string = editbtn + detailbtn;
                return btngroups;
            },
            cellClick: function(e, cell) {
                const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                const workId: number = parseInt(cell.getValue());
                switch (true) {
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                        ShowModal(EditModalId, route.api.ShowEdit, { id: workId })
                            .then(IsSuccess => {
                                if (IsSuccess) {
                                    const $form = $(EditFormId);
                                    const $priority: JQuery<HTMLElement> = $form.find("input[name='fsPRIORITY']");
                                    const $note: JQuery<HTMLElement> = $form.find("textarea[name='fsNOTE']");
                                    ModalTask(EditModalId, true, {
                                        closable: false,
                                        onApprove: function() {
                                            const IsFormValid: boolean = CheckForm(EditFormId, valid.Edit); //added_20200504
                                            if (IsFormValid) {
                                                //added_20200504
                                                route
                                                    .Edit(workId, Number($priority.val()), <string>$note.val())
                                                    .then(res => {
                                                        Logger.res(route.api.Edit, '入庫編輯', res, true);
                                                        if (res.IsSuccess) {
                                                            table.ReactivityUpdate(workId, {
                                                                fsPRIORITY: Number($priority.val()),
                                                                fsNOTE: <string>$note.val(),
                                                            });
                                                            $(EditModalId).modal('hide');
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Edit, '入庫編輯', error, true);
                                                    });
                                            }

                                            return false;
                                        },
                                    });
                                } else {
                                    ErrorMessage('入庫編輯燈箱發生錯誤');
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.ShowEdit, '顯示編輯燈箱', error, true);
                            });

                        break;
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                        DetailModal(Controller.L_Upload, Action.ShowDetails, { id: workId });
                        break;
                    default:
                        break;
                }
            },
        },
    ],
});
/**由查詢列表資料取得所有工作Id */
const GetWorkIds = (data: Array<LUploadListModel>):  Array<number>=> {
    if (data.length > 0) {
        return data.filter(x=>!IsNULLorEmpty(x.WorkStatus) && x.WorkStatus !==WorkStatus.TranscodingComplete && allowWorkStatus.indexOf(x.WorkStatus)>-1).map(x=>x.fnWORK_ID);
    }
    return [];
};
const doWork=(workIds:Array<number>)=>{
    route.Progress(workIds).then(res=>{
        const progresses = <Array<WorkProgressModel>>res.Data;
        if(!IsNullorUndefined(progresses) && progresses.length>0){
            const continueWorkIds: Array<number> =progresses.filter(x=>x.WorkStatus!== WorkStatus.TranscodingComplete && allowWorkStatus.indexOf(x.WorkStatus)>-1).map(x=>x.fnWORK_ID);
            progresses.forEach(x=>{
                table.ReactivityUpdate(x.fnWORK_ID,<LUploadListModel>{
                  fdETIME:x.WorkETime,
                  fdSTIME:x.WorkSTime,
                  StatusColor:x.StatusColor,
                  Progress:x.Progress,
                  WorkStatus:x.WorkStatus,
                  StatusName:x.WorkStatusName
                });
            });
            timeoutId= setTimeout(function(){ doWork(continueWorkIds);clearTimeout( timeoutId);  }, FileSetting.ProgressUpdateIntervalSeconds * 1000);
        }
     
    });
};
/**查詢任務:創建列表與查詢進度 */
const SearchTask = (input: LUploadSearchModel) => {
    route
        .Search(input)
        .then(res => {
            if (res.IsSuccess) {
                const data = <Array<LUploadListModel>>res.Data;
                table.GetTable().setData(data);
                doWork(GetWorkIds(data));
            } else {
                ErrorMessage(res.Message);
                table.GetTable().setData([]);
            }
        })
        .catch(error => {
            table.GetTable().setData([]);
            Logger.viewres(route.api.Search, '查詢', error, true);
        });
};
/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-1* initSetting.SearchRangeDay, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/*頁面載入查詢*/
SearchTask({
    BeginDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    WorkStatus: StringEnum.All,
});
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    $WorkStatus.dropdown('set selected', StringEnum.All);
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
            SearchTask({
                BeginDate: <string>$sdate.val(),
                EndDate: <string>$edate.val(),
                WorkStatus: $WorkStatus.dropdown('get value'),
            });
        }
    }
});

/**重新轉換 */
$('#ReconvertBtn').click(function() {
    if (selectids.length <= 0) {
        WarningMessage('至少選擇一筆資料');
    } else {
        $ReconvertModal.find('.content > div.segment:eq(0)').remove();
        $ReconvertModal
            .find('.content')
            .append(
                `<div class="ui inverted segment" style="overflow-wrap: break-word;">目前選擇的ID=【${selectids.join(
                    ','
                )}】</div>`
            );
        $ReconvertModal.modal('show');
    }
});
$ReconvertModal.modal({
    onDeny: function() {
        Logger.log('取消重轉');
    },
    onApprove: function() {
        route.Retran(selectids).then(res => {
            if (res.IsSuccess) {
                SuccessMessage(res.Message);
                const updateData= selectids.map(id=><LUploadListModel&{SelectStatus?:boolean}>{
                    fnWORK_ID:Number(id),
                    fdETIME: StringEnum.Empty,
                    fdSTIME: StringEnum.Empty,
                    SelectStatus:false
                })
                table.GetTable().updateData(updateData);
                clearTimeout(timeoutId);
                $(SearchFormId).submit();
                selectids = [];
            } else {
                ErrorMessage(res.Message);
            }
        });
    },
});
/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fnWORK_ID'), type: Filter.Like, value: word },
        { field: prop('StatusName'), type: Filter.Like, value: word },
        { field: prop('fdSTIME'), type: Filter.Like, value: word },
        { field: prop('fdETIME'), type: Filter.Like, value: word },
        { field: prop('C_sFILE_INFO'), type: Filter.Like, value: word },
        { field: prop('fsPRIORITY'), type: Filter.Like, value: word },
        { field: prop('CreatedBy'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});

