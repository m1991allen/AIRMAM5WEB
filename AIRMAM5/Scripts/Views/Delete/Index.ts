import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { dayjs, JsonDateToDate, setCalendar } from '../../Models/Function/Date';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { ModalTask, ShowModal } from '../../Models/Function/Modal';
import { SetDate } from '../../Models/Function/Date';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { CheckForm } from '../../Models/Function/Form';
import { MediaType, ChineseMediaType } from '../../Models/Enum/MediaType';
import { FileRecycleStatus } from '../../Models/Enum/FileRecycleStatus';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { RecycleButton, DeleteButton, DetailButton } from '../../Models/Templete/ButtonTemp';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { SearchFormId, DeleteModalId } from '../../Models/Const/Const.';
import { DeleteMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { DeleteController, IDeleteController } from '../../Models/Controller/DeleteController';
import { getIconByMediaType } from '../../Models/Function/Icon';
import { Label } from '../../Models/Templete/LabelTemp';
import { Color } from '../../Models/Enum/ColorEnum';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { MediaDetail } from '../Materia/_Detail';
import { DeleteSearchModel } from '../../Models/Interface/Delete/DeleteSearchModel';
import { DeleteListModel } from '../../Models/Interface/Delete/DeleteListModel';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
/*===============================================================*/
/*宣告變數*/
const message = DeleteMessageSetting;
const valid = FormValidField.Delete;
const sdateId = '#StartDate';
const edateId = '#EndDate';
const RedoModalId = '#RedoModal';
const $Type: JQuery<HTMLElement> = $('#Type').closest('.dropdown');
const $Status: JQuery<HTMLElement> = $('#Status').closest('.dropdown');
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const route: IDeleteController = new DeleteController();
/**回傳Modal性質*/
const prop = (key: keyof DeleteListModel): string => {
    return route.GetProperty<DeleteListModel>(key);
};
var table: ItabulatorService;
//================================================
/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/*頁面載入查詢*/
Search({
    StartDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    Status: StringEnum.Empty,
    Type: StringEnum.Empty,
});
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-3, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    $Type.dropdown('set selected', StringEnum.All);
    $Status.dropdown('set selected', StringEnum.All);
});
/*查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        Search({
            StartDate: <string>$sdate.val(),
            EndDate: <string>$edate.val(),
            Status: <string>$('#Status').dropdown('get value'),
            Type: <string>$('#Type').dropdown('get value'),
        });
    }
});
/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fnINDEX_ID'), type: Filter.Like, value: word },
        { field: prop('fsFILE_NO'), type: Filter.Like, value: word },
        { field: prop('C_sTITLE'), type: Filter.Like, value: word },
        { field: prop('fsREASON'), type: Filter.Like, value: word },
        { field: prop('fsCREATED_BY'), type: Filter.Like, value: word },
        { field: prop('fdCREATED_DATE'), type: Filter.Like, value: word },
        { field: prop('fdUPDATED_DATE'), type: Filter.Like, value: word },
    ];
    const TypeWord: MediaType | '' =
        word.indexOf('影') > -1
            ? MediaType.VIDEO
            : word.indexOf('音') > -1 || word.indexOf('聲') > -1
            ? MediaType.AUDIO
            : word.indexOf('圖') > -1
            ? MediaType.PHOTO
            : word.indexOf('文') > -1 || word.indexOf('件') > -1
            ? MediaType.Doc
            : word.indexOf('主') > -1 || word.indexOf('題') > -1
            ? MediaType.SUBJECT
            : '';
    const StatusWord =
        word.indexOf('已刪除') > -1
            ? FileRecycleStatus.已刪除
            : word.indexOf('已還原') > -1
            ? FileRecycleStatus.已還原
            : word.indexOf('暫刪除') > -1
            ? FileRecycleStatus.暫刪除
            : '-1';
    if (!IsNULLorEmpty(TypeWord)) {
        filter.push({ field: prop('fsTYPE'), type: Filter.Equal, value: TypeWord });
    }
    if (StatusWord !== '-1') {
        filter.push({ field: prop('fsSTATUS'), type: Filter.Equal, value: StatusWord });
    }
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: DeleteSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('fnINDEX_ID'),
        addRowPos: 'top',
        columns: [
            { title: 'ID', field: prop('fnINDEX_ID'), width: 70, sorter: 'number' },
            { title: '媒體檔編號', field: prop('fsFILE_NO'), sorter: 'string', width: 150 },
            {
                title: '類別',
                field: prop('fsTYPE'),
                width: 85,
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    const type = cell.getValue();
                    const icon = getIconByMediaType(<MediaType>type);
                    return icon + getEnumKeyByEnumValue(ChineseMediaType, type);
                },
            },
            {
                title: '名稱',
                field: prop('C_sTITLE'),
                minWidth: 90,
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    const title = <string>cell.getValue();
                    const span: HTMLSpanElement = document.createElement('span');
                    span.innerHTML = title;
                    span.setAttribute('title', title);
                    return span.outerHTML;
                },
            },
            {
                title: '刪除時間',
                field: prop('fdCREATED_DATE'),
                width: 160,
                sorter: 'string',
                mutator: function(value, data, type, mutatorParams, cell) {
                    //為了篩選器啟用變異,已確認不影響功能列
                    let convertDate = JsonDateToDate(value);
                    let date: string = IsNULLorEmpty(convertDate)
                        ? ''
                        : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                    return date;
                },
            },
            { title: '刪除原因', field: prop('fsREASON'), minWidth: 110, sorter: 'string' },
            {
                title: '刪除者',
                field: prop('fsCREATED_BY'),
                width: 90,
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    let value: string = cell.getValue();
                    const rowdata = <DeleteListModel>cell.getRow().getData();
                    const createByName: string = IsNULLorEmpty(rowdata.fsCREATED_BY_NAME)
                        ? value
                        : value + '(' + rowdata.fsCREATED_BY_NAME + ')';
                    return createByName;
                },
            },
            {
                title: '狀態',
                field: prop('fsSTATUS'),
                hozAlign: 'center',
                width: 80,
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    const rowdata = <DeleteListModel>cell.getRow().getData();
                    const status: string = rowdata.fsSTATUS.replace(/\s/g, ''); /*因為暫刪除可能會回傳""或" "*/
                    const statusStr = rowdata.C_sSTATUS;
                    const color: Color =
                        status == FileRecycleStatus.已刪除
                            ? Color.紅
                            : status == FileRecycleStatus.已還原
                            ? Color.藍
                            : Color.黑;
                    return Label(statusStr, color);
                },
            },
            {
                title: '清空/還原時間',
                field: prop('fdUPDATED_DATE'),
                width: 145,
                sorter: 'string',
                mutator: function(value, data, type, mutatorParams, cell) {
                    //為了篩選器啟用變異,已確認不影響功能列
                    let convertDate = JsonDateToDate(value);
                    let date: string = IsNULLorEmpty(convertDate)
                        ? ''
                        : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                    return date;
                },
            },
            {
                title: '操作',
                field: prop('fnINDEX_ID'),
                hozAlign: 'center',
                width: 150,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const rowdata = <DeleteListModel>cell.getRow().getData();
                    const id: number = rowdata.fnINDEX_ID;
                    const filestatus: string = rowdata.fsSTATUS.trim(); /*因為暫刪除可能會回傳""或" "*/
                    const IsOtherStatus: boolean =
                        FileRecycleStatus.已刪除 !== filestatus && FileRecycleStatus.已還原 !== filestatus;
                    const detailbtn = IsOtherStatus ? DetailButton(id, message.Controller) : '';
                    const recyclebtn = IsOtherStatus ? RecycleButton(id, message.Controller) : '';
                    const deletebtn = IsOtherStatus ? DeleteButton(id, message.Controller) : '';
                    const btngroups: string = detailbtn + recyclebtn + deletebtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const row = cell.getRow();
                    const rowdata = <DeleteListModel>row.getData();
                    const indexId: number = rowdata.fnINDEX_ID | 0;
                    const type = <MediaType>rowdata.fsTYPE;
                    switch (true) {
                        /*點擊:刪除*/
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal<IdModel>(DeleteModalId, route.api.ShowDelete, { id: indexId })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(DeleteModalId, true, {
                                            closable: false,
                                            onApprove: function() {
                                                route
                                                    .Delete(indexId, type, rowdata.fsFILE_NO)
                                                    .then(res => {
                                                        if (res.IsSuccess) {
                                                            const data = <DeleteListModel>res.Data;
                                                            SuccessMessage(res.Message);
                                                            table.ReactivityUpdate(indexId, data);
                                                            cell.getElement().innerHTML = '';
                                                        } else {
                                                            ErrorMessage(res.Message);
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Delete, '刪除動作', error, true);
                                                    });
                                            },
                                        });
                                    } else {
                                        ErrorMessage('無法顯示刪除燈箱');
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowDelete, '顯示刪除燈箱', error, true);
                                });
                            break;
                        /*點擊:還原*/
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('recycle icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'recycle':
                            ShowModal<IdModel>(RedoModalId, route.api.ShowRedo, { id: indexId })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(RedoModalId, true, {
                                            closable: false,
                                            onApprove: function() {
                                                route
                                                    .Recycle(indexId)
                                                    .then(res => {
                                                        if (res.IsSuccess) {
                                                            SuccessMessage(res.Message);
                                                            table.ReactivityUpdate(indexId, res.Data);
                                                            cell.getElement().innerHTML = '';
                                                        } else {
                                                            ErrorMessage(res.Message);
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Redo, '還原動作', error, true);
                                                    });
                                            },
                                        });
                                    } else {
                                        ErrorMessage('無法顯示還原燈箱');
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowRedo, '無法顯示還原燈箱', error, true);
                                });
                            break;
                        /**檢視詳細 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            const IndexId: number = Number(cell.getValue());
                            MediaDetail(route.api.ShowDetail, IndexId, <MediaType>rowdata.fsTYPE);
                            break;
                    }
                },
            },
        ],
    });
}
