import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { TabulatorSetting, initSetting, FileSetting } from '../../Models/initSetting';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { WorkStatus } from '../../Models/Enum/WorkStatus';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { dayjs, setCalendar, SetDate } from '../../Models/Function/Date';
import { Controller } from '../../Models/Enum/Controller';
import { CheckForm } from '../../Models/Function/Form';
import { ChineseMediaType, MediaType } from '../../Models/Enum/MediaType';
import { DetailModal } from '../../Models/Function/Modal';
import { Action } from '../../Models/Enum/Action';
import { ErrorMessage } from '../../Models/Function/Message';
import { getIconByMediaType } from '../../Models/Function/Icon';
import { GetProgressBarHtml} from '../L_Upload/FileProgress';
import { MyBookingController, IMyBookingController } from '../../Models/Controller/MyBookingController';
import { SearchFormId, edateId, sdateId } from '../../Models/Const/Const.';
import { MyBookingMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { DetailButton } from '../../Models/Templete/ButtonTemp';
import { MyBookingListModel } from '../../Models/Interface/MyBooking/MyBookingListModel';
import { MyBookingSearchModel } from '../../Models/Interface/MyBooking/MyBookingSearchModel';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
import { GetDropdown } from '../../Models/Function/Element';
import { WorkProgressModel } from '../../Models/Interface/ILUploadIndex';
/*=====================宣告變數================================*/
const message = MyBookingMessageSetting;
const valid = FormValidField.MyBooking;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $WorkStatus: JQuery<HTMLElement> = GetDropdown(SearchFormId, 'WorkStatus');
const $WorkId = $('#workid');
var route: IMyBookingController = new MyBookingController();
var timeoutId: null | ReturnType<typeof setTimeout> = null
/**回傳Modal性質*/
const prop = (key: keyof MyBookingListModel)=>key;
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
var table:ItabulatorService = new tabulatorService(initSetting.TableId, {
    height: TabulatorSetting.height,
    layout: TabulatorSetting.layout,
    data:[],
    index: prop('WorkId'),
    rowFormatter: function(row: Tabulator.RowComponent) {
        const rowdata = <MyBookingListModel>row.getData();
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
            title: '轉檔進度',
            field: prop('Progress'),
            width: 135,
            sorter: 'string',
            formatter: function(cell, formatterParams) {
                const rowdata = <MyBookingListModel>cell.getRow().getData();
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
            formatter: function(cell, formatterParams) {
                const value: string = cell.getValue();
                return value.replace(/\s+/g, '<br>');
            },
        },
        // { title: '調用備註', field: 'NoteStr', sorter: 'string' },
        {
            title: '調用編號',
            field: prop('BookingId'),
            width: 90,
            sorter: 'number',
            titleFormatter: function() {
                return '調用<br>編號';
            },
        },
        {
            title: '轉檔編號',
            field: prop('WorkId'),
            width: 100,
            sorter: 'number',
            titleFormatter: function() {
                return '轉檔<br>編號';
            },
        },
        // { title: '標題', field: 'Title', sorter: 'string' },
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
                const value: string = cell.getValue();
                return value.replace(/\s+/g, '<br>');
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
                const value: string = cell.getValue();
                return value.replace(/\s+/g, '<br>');
            },
        },
        {
            title: '操作',
            field: prop('BookingId'),
            sorter: 'number',
            hozAlign: 'left',
            width: 75,
            formatter: function(cell, formatterParams) {
                cell.getElement().classList.add('tabulator-operation');
                const rowdata = <MyBookingListModel>cell.getRow().getData();
                const workid = rowdata.WorkId;
                const detailbtn = DetailButton(workid, message.Controller);
                const btngroups: string = detailbtn;
                return btngroups;
            },
            cellClick: function(e, cell) {
                const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                const bookingid = cell.getValue();
                const rowdata = cell.getRow().getData();
                const workid = rowdata.WorkId;
                switch (true) {
                    /*點擊:檢視使用者帳號*/
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                        DetailModal(Controller.MyBooking, Action.ShowDetails, { id: workid });
                        break;
                    default:
                        break;
                }
            },
        },
    ],
});

/**由查詢列表資料取得所有工作Id */
const GetWorks = (data: Array<MyBookingListModel>): Array<{WorkId:number,BookId:number}>=> {
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
                table.ReactivityUpdate(x.fnWORK_ID,<MyBookingListModel>{
                    Progress:x.Progress,
                    WorkStatus:x.WorkStatus,
                    StartTime:x.WorkSTime,
                    EndTime:x.WorkETime,
                    StatusName:x.WorkStatusName,
                    StatusColor:x.StatusColor
                });
            });
            timeoutId= setTimeout(function(){ doWork(continueWorkIds);clearTimeout( timeoutId);  }, FileSetting.ProgressUpdateIntervalSeconds * 1000);
        }
     
    });
};
/**查詢任務:創建列表與查詢進度 */
const SearchTask = (input: MyBookingSearchModel) => {
    route
        .Search(input)
        .then(res => {
            if (res.IsSuccess) {
                const data = <Array<MyBookingListModel>>res.Data;
                table.GetTable().setData(data);
                doWork(GetWorks(data).map(x=>x.WorkId));
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
SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/*頁面載入查詢*/
SearchTask({
    StartDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    WorkId: 0,
    WorkStatus: StringEnum.All /*初始化查全部狀態*/,
    LoginId: '',
});
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    $WorkStatus.dropdown('set selected', StringEnum.All);
    $WorkId.val(StringEnum.Empty);
});
/*表單查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const sdate = dayjs($sdate.val());
        const edate = dayjs($edate.val());
        const workid = $WorkId.val() || 0;
        if (edate.isBefore(sdate)) {
            $(this).addClass('error');
            ErrorMessage('查詢的結束日期要在起始日期之後');
        } else {
            $(this).removeClass('error');
            SearchTask({
                StartDate: sdate.format('YYYY/MM/DD'),
                EndDate: edate.format('YYYY/MM/DD'),
                WorkId: Number(workid),
                WorkStatus: $WorkStatus.dropdown('get value'),
                LoginId: '',
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
        { field: prop('Title'), type: Filter.Like, value: word },
        { field: prop('BookingTypeName'), type: Filter.Like, value: word },
        { field: prop('BookingDate'), type: Filter.Like, value: word },
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
        route.CancelProgress();
        clearTimeout(timeoutId);
        $(SearchFormId).submit();
    }
});