import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { FileSetting, initSetting, TabulatorSetting } from '../../Models/initSetting';
import { dayjs, setCalendar } from '../../Models/Function/Date';
import { SetDate } from '../../Models/Function/Date';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { getIconByMediaType } from '../../Models/Function/Icon';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { MediaType, ChineseMediaType } from '../../Models/Enum/MediaType';
import { CheckForm } from '../../Models/Function/Form';
import { Controller } from '../../Models/Enum/Controller';
import { ErrorMessage, SuccessMessage, WarningMessage } from '../../Models/Function/Message';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Action } from '../../Models/Enum/Action';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { EditModalId, EditFormId, sdateId, edateId, SearchFormId } from '../../Models/Const/Const.';
import { IBookingController, BookingController } from '../../Models/Controller/BookingController';
import { GetProgressBarHtml } from '../L_Upload/FileProgress';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { BookingMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { EditButton, DetailButton } from '../../Models/Templete/ButtonTemp';
import { WorkStatus } from '../../Models/Enum/WorkStatus';
import { BookingListModel } from '../../Models/Interface/Booking/BookingListModel';
import { MyBookingSearchModel } from '../../Models/Interface/MyBooking/MyBookingSearchModel';
import { Filter } from '../../Models/Enum/Filter';
import { GetDropdown } from '../../Models/Function/Element';
import { Logger } from '../../Models/Class/LoggerService';
import { WorkProgressModel } from '../../Models/Interface/ILUploadIndex';
import { RefreshBookingMessage } from '../../Models/Interface/Shared/PostMessage/RefreshBookingMessage';
/*=====================宣告變數================================*/
const message = BookingMessageSetting;
const valid = FormValidField.Booking;
const $SEARCHFORM = $('#SearchForm');
const $WorkStatus: JQuery<HTMLElement> = GetDropdown(SearchFormId, 'WorkStatus');
const $WorkId = $('#workid');
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
var route: IBookingController = new BookingController();
var selectids: Array<number> = [];
var timeoutId: null | ReturnType<typeof setTimeout> = null
/**回傳Modal性質*/
const prop = (key: keyof BookingListModel)=>route.GetProperty<BookingListModel>(key);
/**允許更新進度條的工作狀態 */
const allowWorkStatus: Array<string> = [
    WorkStatus.OnSchedule,
    WorkStatus.TransferToAP,
    WorkStatus.GearshiftProgramInit,
    WorkStatus.BookingInTransition,
    WorkStatus.FileToDir,
    WorkStatus.TranscodingComplete,
    WorkStatus.WaitForDownloading,
    WorkStatus.DownloadingFromClound,
    WorkStatus.VerifyPass,
    WorkStatus.VerifyTapeStatus,
    WorkStatus.VerifyOnTape,
    WorkStatus.TapeBacktracking,
];
/**由查詢列表資料取得所有工作Id */
const GetWorks = (data: Array<BookingListModel>): Array<{WorkId:number,BookId:number}>=> {
    if (data.length > 0) {
        return data.filter(x=>!IsNULLorEmpty(x.WorkStatus) && x.WorkStatus !==WorkStatus.TranscodingComplete && allowWorkStatus.indexOf(x.WorkStatus)>-1).map(x=>{return {WorkId:x.WorkId,BookId:x.BookingId};});
    }
    return [];
};
const doWork=(workIds:Array<number>)=>{
    route.Progress(workIds).then(res=>{
        const progresses = <Array<WorkProgressModel>>res.Data;
        if(!IsNullorUndefined(progresses) && progresses.length>0){
            const continueWorkIds: Array<number> =progresses.filter(x=>x.WorkStatus!== WorkStatus.TranscodingComplete && allowWorkStatus.indexOf(x.WorkStatus)>-1).map(x=>x.fnWORK_ID);
            progresses.forEach(x=>{
                table.ReactivityUpdate(x.fnWORK_ID,<BookingListModel>{
                    Progress:x.Progress,
                    WorkStatus:x.WorkStatus,
                    StartTime:x.WorkSTime,
                    EndTime:x.WorkETime,
                    StatusName:x.WorkStatusName,
                    StatusColor:x.StatusColor
                });
            });
            timeoutId= setTimeout(function(){ 
                doWork(continueWorkIds); 
                clearTimeout(timeoutId); 
            }, FileSetting.ProgressUpdateIntervalSeconds * 1000);
          
        }
     
    });
};
/**列表 */
var table: ItabulatorService= new tabulatorService(initSetting.TableId, {
    height: TabulatorSetting.height,
    layout: TabulatorSetting.layout,
    data: [],
    index: prop('WorkId'),
    rowContextMenu: [
        {
            label: "<i class='mouse pointer icon'></i> 當頁全選",
            action: function(e, row) {
                const currentPage=row.getTable().getPage();
                const pageSize=row.getTable().getPageSize();
                if( currentPage!==false){
                    const startIndex=(currentPage-1)*pageSize;
                    const endIndex=currentPage*pageSize-1>row.getTable().getDataCount()?row.getTable().getDataCount():currentPage*pageSize-1;
                    for(let x=startIndex;x<=endIndex;x++){
                       const _row_= row.getTable().getRows()[x];
                       if(!IsNullorUndefined(_row_) && (<BookingListModel>_row_.getData()).WorkStatus!==WorkStatus.VerifyReject){
                          _row_.update(<BookingListModel & {SelectStatus?:boolean}>{SelectStatus:true});
                           _row_.select();
                         if(selectids.indexOf(_row_.getIndex())===-1){ selectids.push(_row_.getIndex());}
                       }
                    }
                }
             
            },
        },
        {
            label: "<i class='window close icon'></i> 當頁取消",
            action: function(e, row) {
                const currentPage=row.getTable().getPage();
                const pageSize=row.getTable().getPageSize();
                if( currentPage!==false){
                    const startIndex=(currentPage-1)*pageSize;
                    const endIndex=currentPage*pageSize-1>row.getTable().getDataCount()?row.getTable().getDataCount():currentPage*pageSize-1;
                    $('input[name="selectall"]').parent().checkbox('set unchecked');
                    for(let x=startIndex;x<=endIndex;x++){
                        const _row_= row.getTable().getRows()[x];
                        if(!IsNullorUndefined(_row_)){
                            _row_.update(<BookingListModel & {SelectStatus?:boolean}>{SelectStatus:false});
                            _row_.deselect();
                            selectids=selectids.filter(id=>id !==_row_.getIndex());
                        }
                    } 
                }
                
            },
        },
        {
            label: "<i class='mouse pointer icon'></i> 所有全選",
            action: function(e, row) {      
                const newData=  (<Array<BookingListModel & {SelectStatus?:boolean}>>row.getTable().getData()).map(item=>{
                    if(selectids.indexOf(item.WorkId)===-1&& item.WorkStatus!==WorkStatus.VerifyReject){ selectids.push(item.WorkId); }
                    item.SelectStatus=true;
                    return item;
                 });
                 row.getTable().selectRow();
                 row.getTable().updateData(newData);
            },
        },
        {
            label: "<i class='window close icon'></i> 所有取消",
            action: function(e, row) {
               selectids=[];
               const newData=  (<Array<BookingListModel & {SelectStatus?:boolean}>>row.getTable().getData()).map(item=>{
                item.SelectStatus=false;
                return item;
               });
                 row.getTable().deselectRow();
                 row.getTable().updateData(newData);              
            },
        }
    ],
    rowFormatter: function(row: Tabulator.RowComponent) {
        const rowdata = <BookingListModel>row.getData();
        const rowElement = row.getElement();
        rowElement.setAttribute('data-workid', rowdata.WorkId.toString());
        if (rowElement.className.indexOf('hastitle') == -1) {
            rowElement.classList.add('hastitle');
            const fakediv: HTMLDivElement = document.createElement('div');
            fakediv.style.paddingTop = '15px';
            fakediv.innerHTML = `——————${rowdata.Title}`;
            rowElement.insertBefore(fakediv, rowElement.firstChild);
        }
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
                const rowdata = <BookingListModel>cell.getRow().getData();
                const id = rowdata.WorkId;
                const isCheck=cell.getValue()===true?'checked="checked"':'';
                const checkbox = `<div class="ui checkbox" data-Id="${id}"><input type="checkbox" name="reconvert" ${isCheck}> <label></label></div>`;
                return rowdata.WorkStatus===WorkStatus.VerifyReject ?"":checkbox;
            },
            cellClick: function(e, cell) {
                const target: HTMLLabelElement | HTMLInputElement | HTMLDivElement | HTMLElement = <any>e.target;
                const row = cell.getRow();
                const rowdata = <BookingListModel&{SelectStatus?:boolean}>row.getData();
                const id = IsNULLorEmpty(rowdata.WorkId) ? 0 : Number(rowdata.WorkId);
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
        {
            title: '類型',
            field: prop('ArcType'),
            sorter: 'string',
            width: 75,
            formatter: function(cell, formatterParams) {
                const type = cell.getValue();
                const icon = getIconByMediaType(<MediaType>type);
                return icon + getEnumKeyByEnumValue(ChineseMediaType, type);
            },
        },
        {
            title: '轉檔進度/轉檔狀態',
            field: prop('Progress'),
            sorter: 'string',
            width: 135,
            titleFormatter: function() {
                return '轉檔進度/<br>轉檔狀態';
            },
            formatter: function(cell, formatterParams) {
                const rowdata = <BookingListModel>cell.getRow().getData();
                const progressbar = GetProgressBarHtml(
                    rowdata.WorkId,
                    rowdata.Progress,
                    rowdata.StatusName,
                    rowdata.StatusColor
                );
                return progressbar;
            },
        },
        { title: '調用類別', field: prop('BookingTypeName'), sorter: 'string', minWidth: 110 },
        {
            title: '調用日期',
            field: prop('BookingDate'),
            sorter: 'string',
            width: 105,
            titleFormatter: function() {
                return '調用<br>日期';
            },
            formatter: function(cell, formatterParams) {
                const cellValue = cell.getValue();
                return cellValue.replace(/\s/g, '<br>');
            },
        },
        {
            title: '優先順序',
            field: prop('Priority'),
            sorter: 'string',
            width: 80,
            hozAlign: 'center',
            titleFormatter: function() {
                return '優先<br>順序';
            },
        },
        { title: '調用者', field: prop('CreateBy'), sorter: 'string', width: 110 },
        { title: '轉檔狀態', field: prop('StatusName'), sorter: 'string', visible: false, download: false },

        // { title: '狀態說明', field: 'StatusName', sorter: 'string' },
        // { title: '標題', field: 'Title', sorter: 'string' },
        {
            title: '調用編號',
            field: prop('BookingId'),
            sorter: 'string',
            width: 90,
            hozAlign: 'center',
            titleFormatter: function() {
                return '調用<br>編號';
            },
        },
        {
            title: '轉檔編號',
            field: prop('WorkId'),
            sorter: 'number',
            width: 100,
            hozAlign: 'center',
            titleFormatter: function() {
                return '轉檔<br>編號';
            },
        },
        {
            title: '剪輯起始時間',
            field: prop('MarkInTime'),
            sorter: 'string',
            width: 105,
            titleFormatter: function() {
                return '剪輯<br>起始時間';
            },
        },
        {
            title: '剪輯結束時間',
            field: prop('MarkOutTime'),
            sorter: 'string',
            width: 105,
            titleFormatter: function() {
                return '剪輯<br>結束時間';
            },
        },
        {
            title: '轉檔開始時間',
            field: prop('StartTime'),
            sorter: 'string',
            width: 105,
            titleFormatter: function() {
                return '轉檔<br>開始時間';
            },
            formatter: function(cell, formatterParams) {
                const cellValue = cell.getValue();
                return cellValue.replace(/\s/g, '<br>');
            },
        },
        {
            title: '轉檔結束時間',
            field: prop('EndTime'),
            sorter: 'string',
            width: 105,
            titleFormatter: function() {
                return '轉檔<br>結束時間';
            },
            formatter: function(cell, formatterParams) {
                const cellValue = cell.getValue();
                return cellValue.replace(/\s/g, '<br>');
            },
        },
        {
            title: '操作',
            field: prop('BookingId'),
            hozAlign: 'left',
            sorter: 'number',
            width: 100,
            formatter: function(cell, formatterParams) {
                const rowdata = <BookingListModel>cell.getRow().getData();
                cell.getElement().classList.add('tabulator-operation');
                const id = cell.getValue();
                const editbtn =rowdata.WorkStatus=== WorkStatus.VerifyReject?"": EditButton(id, message.Controller);
                const detailbtn = DetailButton(id, message.Controller);
                const btngroups: string = editbtn + detailbtn;
                return btngroups;
            },
            cellClick: function(e, cell) {
                const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                const bookingid = cell.getValue();
                const rowdata = <BookingListModel>cell.getRow().getData();
                const workid = Number(rowdata.WorkId);
                switch (true) {
                    /*點擊:檢視*/
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                        DetailModal(Controller.Booking, Action.ShowDetails, { id: workid });
                        break;
                    /*點擊:編輯,設定優先權*/
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                        ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: workid }).then(success => {
                            if (success) {
                                ModalTask(EditModalId, true, {
                                    closable: false,
                                    allowMultiple: true,
                                    onShow: function() {},
                                    onApprove: function() {
                                        const $EDITFORM = $(EditFormId);
                                        const IsFormValid: boolean = CheckForm(EditFormId, valid.Edit);
                                        /*Notcie:David說這部分優先序只能接受1~9(1優先度最高),但不知為何要這樣規範*/
                                        const priority = <string>$EDITFORM.find('#Priority').val() || '1';
                                        if (IsFormValid) {
                                            route
                                                .Edit({
                                                    workid: workid,
                                                    priority: Number(priority),
                                                })
                                                .then(res => {
                                                    if (res.IsSuccess) {
                                                        const record = res.Records;
                                                        SuccessMessage(res.Message);
                                                        table.ReactivityUpdate(workid, {
                                                            Priority: priority,
                                                        });
                                                        $(EditModalId).modal('hide');
                                                    } else {
                                                        ErrorMessage(res.Message);
                                                    }
                                                });
                                        }
                                        return false;
                                    },
                                    onDeny: function() {},
                                });
                            } else {
                                ErrorMessage('設定優先權燈箱發生錯誤');
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
/**查詢任務:創建列表與查詢進度 */
const SearchTask = (input: MyBookingSearchModel) => {
    route
        .Search(input)
        .then(res => {
            if (res.IsSuccess) {
                const data = <Array<BookingListModel &{SelectStatus:boolean;}>>res.Data;
                data.forEach(item=>{item.SelectStatus=false;});
                table.GetTable().setData(data);
                doWork(GetWorks(data).map(x=>x.WorkId));
            } else {
                ErrorMessage(res.Message);
                table.GetTable().setData([]);
            }
        })
        .catch(error => {
            table.GetTable().setData([]);
            ErrorMessage('系統發生錯誤,無法查詢');
            Logger.viewres(route.api.Search, '查詢', error, false);
        });
};

/*預設查詢日期*/
setCalendar('.calendar', 'date');
SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
/*頁面載入查詢*/
SearchTask({
    StartDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    WorkId: 0,
    WorkStatus: StringEnum.All /*初始化查全部狀態*/,
    LoginId: StringEnum.Empty,
});
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    $WorkStatus.dropdown('set selected', StringEnum.All);
    $WorkId.val(StringEnum.Empty);
});
/*表單查詢*/
$SEARCHFORM.submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const sdate = dayjs($sdate.val());
        const edate = dayjs($edate.val());
        const workid = $WorkId.val() || 0;
        const workstatus = $WorkStatus.dropdown('get value');
        if (edate.isBefore(sdate)) {
            $(this).addClass('error');
            ErrorMessage('查詢的結束日期要在起始日期之後');
        } else {
            $(this).removeClass('error');
            SearchTask({
                StartDate: sdate.format('YYYY/MM/DD'),
                EndDate: edate.format('YYYY/MM/DD'),
                WorkId: IsNULLorEmpty(workid) ? 0 : Number(workid),
                WorkStatus: workstatus,
                LoginId: StringEnum.Empty,
            });
        }
    }
});
/**重設借調 */
$("button[name='Rebooking']").click(function() {
    if (selectids.length == 0) {
        WarningMessage('至少選擇一筆資料');
    } else {
        const selects = selectids.join(',');
        const ids: Array<string> = selectids.join(',').split(',');
        ModalTask('#RebookingConfirm', true, {
            closable: false,
            onShow: function() {
                $('#RebookingConfirm')
                    .children('.content')
                    .empty()
                    .html(`確定要重新借調選定的檔案?<p style="word-break:break-all;">【轉檔編號:${selects}】</p>`);
            },
            onApprove: function() {
                route.ReBooking(ids).then(res => {
                    res.Message = res.Message.replace(/!/, '!<br>');
                    res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                    const table_ = table.GetTable();
                    if (res.IsSuccess) {

                        const updateData=ids.map(id=><BookingListModel& {SelectStatus?:boolean}>{
                            StartTime: StringEnum.Empty,
                            EndTime: StringEnum.Empty,
                            SelectStatus:false,
                            WorkId:Number(id)
                        })
                        table.GetTable().updateData(updateData);
                        selectids = []; //將選擇的檔案設置為無
                        window.top.postMessage(<RefreshBookingMessage>{eventid:'refreshBooking'},'/');
                    }
                }).catch(error=>{
                    ErrorMessage(`伺服端重設借調API發生錯誤`);
                });
            },
        });
    }
});

/**取消調用 */
$("button[name='Cancelbooking']").click(function(){
   if(selectids.length===0){
       WarningMessage('至少選擇一筆資料');
   }else{
        const selects = selectids.join(',');
        const ids: Array<string> = selectids.join(',').split(',');
        ModalTask('#CancelbookingConfirm',true,{
            closable:false,
            onShow:function(){
                $('#CancelbookingConfirm')
                .children('.content')
                .empty()
                .html(`確定要取消借調選定的檔案?<p style="word-break:break-all;">【轉檔編號:${selects}】</p>`);
            },
            onApprove:function(){
                route.CancelBooking(ids).then(res=>{
                 res.Message = res.Message.replace(/!/, '!<br>');
                 res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                 const table_ = table.GetTable();
                 if (res.IsSuccess) {
                    selectids = []; //將選擇的檔案設置為無
                    const updateData=ids.map(id=><BookingListModel& {SelectStatus?:boolean}>{
                        StartTime: StringEnum.Empty,
                        EndTime: StringEnum.Empty,
                        SelectStatus:false,
                        WorkId:Number(id)
                    })
                    table.GetTable().updateData(updateData);
                    window.top.postMessage(<RefreshBookingMessage>{eventid:'refreshBooking'},'/');
                 }
                }).catch(error=>{
                     ErrorMessage('伺服端取消借調API發生錯誤');
                });
            }
        });
   }
});

/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('Title'), type: Filter.Like, value: word },
        { field: prop('BookingTypeName'), type: Filter.Like, value: word },
        { field: prop('BookingDate'), type: Filter.Like, value: word },
        { field: prop('Priority'), type: Filter.Like, value: word },
        { field: prop('CreateBy'), type: Filter.Like, value: word },
        { field: prop('BookingId'), type: Filter.Like, value: word },
        { field: prop('WorkId'), type: Filter.Like, value: word },
        { field: prop('MarkInTime'), type: Filter.Like, value: word },
        { field: prop('MarkOutTime'), type: Filter.Like, value: word },
        { field: prop('StartTime'), type: Filter.Like, value: word },
        { field: prop('EndTime'), type: Filter.Like, value: word },
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
    if (!IsNULLorEmpty(CategoryWord)) {
        filter.push({
            field: prop('ArcType'),
            type: Filter.Equal,
            value: CategoryWord,
        });
    }
    table.SetFilter(filter);
});

/**
 * document接收到列表重新查詢指示
 * (送出調用後觸發)
 */
window.addEventListener("message",(event)=>{
     if(event.data.eventid==='refreshBooking'){
        clearTimeout(timeoutId);
        $SEARCHFORM.submit();
     }
});