import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { initSetting, TabulatorSetting, TSMSetting } from '../../Models/initSetting';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { TSMFileStatus } from '../../Models/Enum/TSMFileStatus';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { ChineseMediaType, MediaType } from '../../Models/Enum/MediaType';
import { CurrentTimeToTimeCode } from '../../Models/Function/Frame';
import { ShowModal, ModalTask } from '../../Models/Function/Modal';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { ErrorMessage, SuccessMessage, WarningMessage, windowPostMessage } from '../../Models/Function/Message';
import { IMateriaController, MateriaController } from '../../Models/Controller/MateriaController';
import { CreateModalId, CreateFormId, renderBody } from '../../Models/Const/Const.';
import { SharedController } from '../../Models/Controller/SharedController';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { IsFileValid } from '../../Models/Function/Image';
import { GetImage } from '../../Models/Templete/ImageTemp';
import { AirmamImage } from '../../Models/Const/Image';
import { FileStatusResult } from '../../Models/Interface/Search/ISearchResponseVideoModel';
import { videoPlayer } from '../../Models/Class/videoPlayer';
import { Label } from '../../Models/Templete/LabelTemp';
import * as dayjs_ from 'dayjs';
import { FormValidField } from '../../Models/Const/FormValid';
import { MateriaMessageSetting } from '../../Models/MessageSetting';
import { DeleteButton, FilmButton, DetailButton } from '../../Models/Templete/ButtonTemp';
import { BatchDeleteResponseModel } from '../../Models/Interface/Materia/BatchDeleteResponseModel';
import { MaterialListModel } from '../../Models/Interface/Materia/MaterialListModel';
import { CreateMaterialModel } from '../../Models/Interface/Materia/CreateMaterialModel';
import { CreateBookingModel } from '../../Models/Interface/Booking/CreateBookingModel';
import { MediaDetail } from './_Detail';
import { GetTsmColor, GetTsmImgUrlByStatus, GetTsmTextByStatus } from '../../Models/Function/TSM';
import { Color } from '../../Models/Enum/ColorEnum';
import { Filter } from '../../Models/Enum/Filter';
import {  TsmMessageModel } from '../../Models/Interface/DocumentViewer/DocumentPostMessageModal';
import { Logger } from '../../Models/Class/LoggerService';
import { RefreshBookingMessage } from '../../Models/Interface/Shared/PostMessage/RefreshBookingMessage';

/*----------------------------宣告變數-------------------------------- */
/**要不要使用tsm */
const IsUseTsm: boolean = renderBody.getAttribute('data-IsUseTsm') === 'true' ? true : false;
/**要不要開啟非雲端功能 */
const IsNonCloud: boolean = renderBody.getAttribute('data-IsNonCloud') === 'true' ? true : false;
/**要不要開啟段落調用功能 */
const IsSupportPartial: boolean = renderBody.getAttribute('data-IsSupportPartial') === 'true' ? true : false;
/**引入日期套件 */
const dayjs = (<any>dayjs_).default || dayjs_;
const FilmModalId = '#FilmModal';
const message = MateriaMessageSetting;
const valid = FormValidField.Materia;
var table: ItabulatorService;
var route: IMateriaController = new MateriaController();
var selectids: Array<number> = [];
var selectfilenos: Array<string> = [];
var selectworkids: Array<number> = [];
var savefilesResult: Array<FileStatusResult> = [];
/**回傳Modal性質*/
const prop = (key: keyof MaterialListModel): string => {
    return key;
};
//=========================================
route.Search().then(data => {
    /**頁面初始載入 */
    Search(data);
    /**開始執行tsm狀態查詢 */
    GetTsmTask(data);
    Logger.log(`系統是否使用tsm:${IsUseTsm}`);
});

/**重載列表 */
$("button[name='reload']").click(function() {
    route.Search().then(data => {
        /**頁面初始載入 */
        Search(data);
        /**開始執行tsm狀態查詢 */
        GetTsmTask(data);
    });
});
/**批次新增調用 */
$("button[name='addbooking']").click(function() {
    const ids = selectids;
    if (ids.length == 0) {
        WarningMessage('至少選擇一個檔案!');
    } else {
        ShowModal<{ ids: string }>(CreateModalId, route.api.ShowBooking, { ids: ids.join('^') }).then(success => {
            if (success) {
                const $form = $(CreateFormId);
                ModalTask(CreateModalId, true, {
                    closable: false,
                    onShow: function() {
                        $form.find('.dropdown').dropdown();
                        $('#ddlReson').change(function() {
                            const reasonId = Number($(this).val());
                            SharedController.GetBookingOption(reasonId)
                                .then(data => {
                                    const selectTemp = (
                                        list: Array<SelectListItem>,
                                        name: string,
                                        labelStr: string
                                    ): string => {
                                        if (list.length == 0) {
                                            return '';
                                        } else {
                                            let options = '';
                                            for (let i = 0; i < list.length; i++) {
                                                const item = list[i];
                                                options += `<option value="${item.Value}">${item.Text}</option> `;
                                            }
                                            return `<div class="field path">
                                                     <label for="${name}">${labelStr}</label>
                                                     <select class="ui inverted dropdown black label" id="${name}" name="${name}">${options}</select>
                                                   </div>`;
                                        }
                                    };
                                    const pathSelect = selectTemp(data.PathList, 'PathStr', '調用路徑');
                                    const videoSelect = selectTemp(data.VideoProfileList, 'ProfileV', '轉出格式-影片');
                                    const audioSelect = selectTemp(data.AudioProfileList, 'ProfileA', '轉出格式-聲音');
                                    const waterSelect = selectTemp(data.WatermarkList, 'WatermarkStr', '浮水印');
                                    const templete: string = `${pathSelect}${videoSelect}${audioSelect}${waterSelect}`;
                                    $('#OptionArea')
                                        .empty()
                                        .append(templete);
                                })
                                .then(() => {
                                    $form.find(".dropdown[name!='WatermarkStr']").dropdown();
                                    if ($("select[name='WatermarkStr']").length > 0) {
                                        $("select[name='WatermarkStr']").dropdown();
                                        //  /*下列註解為: 若浮水印選擇非"無"(00)，而是有浮水印的，則影片樣板不可選擇"原檔複製"(COPYFILE) */
                                        // $("select[name='WatermarkStr']").dropdown({
                                        //     onChange: function(value, text, $choice) {
                                        //         const $ProfileV = $form
                                        //             .find("select[name='ProfileV']")
                                        //             .parent('.dropdown');
                                        //         const copyfileItem = $ProfileV.dropdown('get item', 'COPYFILE');
                                        //         const otherfileItem = copyfileItem.siblings().first();
                                        //         if (value == '00') {
                                        //             $ProfileV
                                        //                 .dropdown('set selected', otherfileItem.attr('data-value'))
                                        //                 .dropdown('refresh');
                                        //             copyfileItem.addClass('disabled');
                                        //         } else {
                                        //             copyfileItem.removeClass('disabled');
                                        //         }
                                        //     },
                                        // });
                                    }
                                });
                        });
                        const filenoLabels = selectfilenos.map(function(fileno) {
                            return `<label class="ui mini black label">${fileno}</label>`;
                        });
                        $('#showFileNos')
                            .empty()
                            .html(filenoLabels.join(''));
                    },
                    onApprove: function() {
                        const $ddlReson = $form.find('#ddlReson');
                        const $ProfileV = $form.find("select[name='ProfileV']").parent('.dropdown');
                        const $ProfileA = $form.find("select[name='ProfileA']").parent('.dropdown');
                        const $WatermarkStr = $form.find("select[name='WatermarkStr']").parent('.dropdown');
                        const $PathStr = $form.find("select[name='PathStr']").parent('.dropdown');
                        route
                            .Booking({
                                ResonStr: <string>$ddlReson.children('option:selected').text(),
                                ResonId: Number($ddlReson.children('option:selected').val()),
                                DescStr: <string>$('#txtDescStr').val(),
                                ProfileVideo: $ProfileV.length > 0 ? <string>$ProfileV.dropdown('get value') : '',
                                ProfileAudio: $ProfileA.length > 0 ? <string>$ProfileA.dropdown('get value') : '',
                                WaterMark: $WatermarkStr.length > 0 ? <string>$WatermarkStr.dropdown('get value') : '',
                                PathStr: $PathStr.length > 0 ? <string>$PathStr.dropdown('get value') : '',
                                MaterialIds: ids.join('^'),
                            })
                            .then(res => {
                                Logger.res(route.api.Booking, '批次新增調用', res);
                                if (res.IsSuccess) {
                                    window.top.postMessage(<RefreshBookingMessage>{eventid:'refreshBooking'},'/');
                                    const record = <CreateBookingModel>(
                                        (<{ model: object; urnm: string }>res.Records).model
                                    );
                                    selectids = [];
                                    selectfilenos = [];
                                    const MaterialIds = record.MaterialIds.split(',') || [];
                                    for (let id of MaterialIds) {
                                        RowSetUnchecked(id);
                                    }
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.Booking, '批次新增調用', error);
                            });
                    },
                });
            } else {
                Logger.viewres(route.api.ShowBooking, '新增調用燈箱', '');
            }
        });
    }
});

/**批次刪除調用 */
$("button[name='batchdelete']").click(function() {
    const ids = selectids;
    const filenos = selectfilenos;
    const BatchDeleteConfirmId = '#BatchDeleteConfirm';
    if (ids.length == 0) {
        WarningMessage('至少選擇一個檔案!');
    } else {
        ModalTask(BatchDeleteConfirmId, true, {
            closable: false,
            onShow: function() {
                const filenoLabels = filenos.map(function(fileno) {
                    return `<label class="ui mini black label">${fileno}</label>`;
                });
                $(BatchDeleteConfirmId)
                    .find('.content')
                    .empty()
                    .html(
                        `<div style="word-wrap:break-word;">即將刪除借調檔案；<br>
                        ${filenoLabels.join('')}</div>`
                    );
            },
            onApprove: function() {
                route
                    .DeleteFile(ids)
                    .then(res => {
                        Logger.res(route.api.DeleteFile, '批次刪除調用', res);
                        const data = <Array<BatchDeleteResponseModel>>res.Data;
                        if (res.IsSuccess) {
                            selectids = [];
                            selectfilenos = [];
                            for (let item of data) {
                                table.RemoveRow(item.fnMATERIAL_ID);
                            }
                        }
                    })
                    .catch(error => {
                        Logger.viewres(route.api.DeleteFile, '批次刪除調用', error);
                    });
            },
        });
    }
});
/**
 * 列表篩選
 * //TODO timecode和JSONDATE 轉換
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('Title'), type: Filter.Like, value: word },
        { field: prop('CreatedDate'), type: Filter.Like, value: word },
        { field: prop('FileNo'), type: Filter.Like, value: word },
        { field: prop('VideoMaxTimeStr'), type: Filter.Like, value: word },
        { field: prop('MarkInTimeStr'), type: Filter.Like, value: word },
        { field: prop('MarkOutTimeStr'), type: Filter.Like, value: word },
        { field: prop('MarkDurationStr'), type: Filter.Like, value: word },
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
            field: 'FileCategory',
            type: Filter.Equal,
            value: CategoryWord,
        });
    }
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(data: Array<MaterialListModel>) {
    table = new tabulatorService(
        initSetting.TableId,
        {
            height: TabulatorSetting.height,
            layout: TabulatorSetting.layout,
            data: data,
            index: prop('MaterialId'),
            addRowPos: 'top',
            rowFormatter: function(row: Tabulator.RowComponent) {
                const rowdata = <MaterialListModel>row.getData();
                row.getElement().setAttribute('data-fileno', rowdata.FileNo);
                if(rowdata.IsExpired || rowdata.IsForBid){
                    row.getElement().style.background = 'rgba(255,0,0,0.35)';
                }
            },
            columns: [
                {
                    title: '勾選',
                    field: prop('MaterialId'),
                    width: 50,
                    sorter: 'number',
                    hozAlign: 'center',
                    headerSort: false,
                    cellClick: function(e, cell) {
                        const rowdata = <MaterialListModel>cell.getRow().getData();
                        const fileno = rowdata.FileNo;
                        const target: HTMLLabelElement | HTMLInputElement | HTMLDivElement | HTMLElement = <any>(
                            e.target
                        );
                        const id = rowdata.MaterialId;
                        const $checkbox = cell.getElement().querySelector('.checkbox');
                        const $checkboxinput = cell.getElement().querySelector("input[type='checkbox']");
                        const ischeck: boolean = $checkboxinput.getAttribute('checked') == 'true' ? true : false;
                        if (target instanceof HTMLLabelElement || target instanceof HTMLInputElement) {
                            if (ischeck) {
                                $checkbox.classList.remove('checked');
                                $checkboxinput.setAttribute('checked', 'false');
                                selectids = selectids.filter(item => item != id);
                                selectfilenos = selectfilenos.filter(
                                    (item, index) => index !== selectfilenos.lastIndexOf(fileno)
                                );
                            } else {
                                $checkbox.classList.add('checked');
                                $checkboxinput.setAttribute('checked', 'true');
                                if (selectids.indexOf(id) <= -1) {
                                    selectids.push(id);
                                    selectfilenos.push(fileno);
                                }
                            }
                        }
                    },
                    formatter: function(cell, formatterParams) {
                        const rowdata = <MaterialListModel>cell.getRow().getData();"";
                        const id = cell.getValue();
                        const checkbox = `<div class="ui checkbox" data-Id="${id}"><input type="checkbox" name="reconvert" title="${rowdata.LicenseMessage}"> <label></label></div>`;
                        return  rowdata.IsExpired|| rowdata.IsForBid?"":checkbox;
                    },
                },
                {
                    title: '檔案類型',
                    field: prop('FileCategory'),
                    width: 80,
                    sorter: 'string',
                    titleFormatter: function() {
                        return '檔案<br/>類型';
                    },
                    formatter: function(cell, formatterParams) {
                        const cellValue = cell.getValue();
                        return getEnumKeyByEnumValue(ChineseMediaType, cellValue);
                    },
                },
                {
                    title: '檔案狀態',
                    field: prop('TSMFileStatus'),
                    sorter: 'number',
                    width: 80,
                    titleFormatter: function() {
                        return '檔案<br/>狀態';
                    },
                    formatter: function(cell, formatterParams) {
                        const row = cell.getRow();
                        const rowdata = <MaterialListModel>row.getData();
                        /*
                     影:如果沒有使用tsm就顯示在磁碟,否則就依照api內容更新
                     音圖文:永遠顯示在磁碟,與tsm無關
                    */
                        if (rowdata.FileCategory == MediaType.VIDEO) {
                            const saveTsmStatus = savefilesResult.filter(item => item.FILE_NO == rowdata.FileNo);
                            const filestatus = !IsUseTsm
                                ? TSMFileStatus.FileOnDisk
                                : saveTsmStatus.length > 0
                                ? saveTsmStatus[0].FILE_STATUS
                                : TSMFileStatus.Nodata;
                            const statusColor = GetTsmColor(filestatus);
                            const statusImage = GetImage(GetTsmImgUrlByStatus(filestatus));
                            const statusStr = GetTsmTextByStatus(filestatus);
                            // const statusStr = getEnumKeyByEnumValue(ChineseTSMFileStatus, filestatus);
                            return Label(statusImage, statusColor, ['image', 'tansparent'], statusStr);
                        } else {
                            const filestatus = IsNonCloud ? TSMFileStatus.FileOnDisk : TSMFileStatus.Online;
                            const statusColor = GetTsmColor(filestatus);
                            const statusImage = GetImage(GetTsmImgUrlByStatus(filestatus));
                            const statusStr = GetTsmTextByStatus(filestatus);
                            // const statusStr = getEnumKeyByEnumValue(ChineseTSMFileStatus, filestatus);
                            return Label(statusImage, statusColor, ['image', 'tansparent'], statusStr);
                        }
                    },
                },
                {
                    title:'版權',
                    field:prop('LicenseStr'),
                    sorter:'string',
                    width:120,
                    formatter:function(cell,fromatterParams){
                        const rowdata = <MaterialListModel>cell.getRow().getData();
                        return `<span class="x-license-label">${cell.getValue()}</span>`;
                    }
                },
                {
                    title: '標題',
                    field: prop('Title'),
                    sorter: 'string',
                    minWidth: 170,
                    formatter: function(cell, formatterParams) {
                        const title = <string>cell.getValue();
                        const rowdata = <MaterialListModel>cell.getRow().getData();
                       return `<span title="${title}">${title}</span><span class="x-license-message">${rowdata.LicenseMessage}</span>`;
                    },
                },
                {
                    title: '調用日期',
                    field: prop('CreatedDate'),
                    sorter: 'string',
                    width: 105,
                    formatter: function(cell, formatterParams) {
                        const cellValue: string = cell.getValue();
                        const time = IsNULLorEmpty(cellValue)
                            ? ''
                            : dayjs(cellValue).format('YYYY/MM/DD <br /> HH:mm:ss');
                        return time;
                    },
                },
                {
                    title: '影片總長度(時長)',
                    field: prop('VideoMaxTimeStr'),
                    // field: 'VideoMaxTime',
                    sorter: 'string',
                    width: 100,
                    titleFormatter: function() {
                        return '影片<br>總長度';
                    },
                    // formatter: function(cell, formatterParams) {
                    //     const totalSeconds = Number(cell.getValue()) || 0;
                    //     const timecode = IsNULLorEmpty(totalSeconds)
                    //         ? '00:00:00;00'
                    //         : CurrentTimeToTimeCode(totalSeconds, 'second');
                    //     return timecode;
                    // },
                },
                {
                    title: '起始時間',
                    field: prop('MarkInTimeStr'),
                    //field: 'ParameterStr',
                    sorter: 'string',
                    width: 105,
                    // formatter: function(cell, formatterParams) {
                    //     const cellValue: string = cell.getValue();
                    //     if (!IsNULLorEmpty(cellValue) && cellValue !== undefined) {
                    //         const array = cellValue.split(';') || [];
                    //         const begintime = array[0] !== undefined ? Number(array[0]) : 0;
                    //         const endtime = array[1] !== undefined ? Number(array[1]) : 0;
                    //         const timecode =
                    //             begintime == 0 && endtime == 0
                    //                 ? StringEnum.Empty
                    //                 : CurrentTimeToTimeCode(begintime, 'second');
                    //         return timecode;
                    //     } else {
                    //         return StringEnum.Empty;
                    //     }
                    // },
                },
                {
                    title: '結束時間',
                    field: prop('MarkOutTimeStr'),
                    // field: 'ParameterStr',
                    sorter: 'string',
                    width: 105,
                    // formatter: function(cell, formatterParams) {
                    //     const cellValue: string = cell.getValue();
                    //     if (!IsNULLorEmpty(cellValue) && cellValue !== undefined) {
                    //         const array = cellValue.split(';') || [];
                    //         const begintime = array[0] !== undefined ? Number(array[0]) : 0;
                    //         const endtime = array[1] !== undefined ? Number(array[1]) : 0;
                    //         const timecode =
                    //             begintime == 0 && endtime == 0
                    //                 ? StringEnum.Empty
                    //                 : CurrentTimeToTimeCode(endtime, 'second');
                    //         return timecode;
                    //     } else {
                    //         return StringEnum.Empty;
                    //     }
                    // },
                },
                {
                    title: '擷取時長',
                    field: prop('MarkDurationStr'),
                    // field: 'ParameterStr',
                    sorter: 'string',
                    width: 105,
                    // formatter: function(cell, formatterParams) {
                    //     const cellValue: string = cell.getValue();
                    //     const rowdata = <MaterialListModel>cell.getRow().getData();
                    //     if (!IsNULLorEmpty(cellValue) && cellValue !== undefined) {
                    //         const maxduration = Number(rowdata.VideoMaxTime);
                    //         const array = cellValue.split(';') || [];
                    //         const begintime = array[0] !== undefined ? Number(array[0]) : 0;
                    //         const endtime = array[1] !== undefined ? Number(array[1]) : 0;
                    //         const diff = begintime === 0 && endtime === 0 ? maxduration : endtime - begintime;
                    //         const timecode =
                    //             begintime === 0 && endtime === 0 ? '' : CurrentTimeToTimeCode(diff, 'second');
                    //         return timecode;
                    //     }
                    //     return StringEnum.Empty;
                    // },
                },
                //{ title: '調用備註', field: 'MaterialNote', sorter: 'string' },
                //{ title: '檔案編號', field: 'FileNo', sorter: 'string' },
                {
                    title: '操作',
                    field: prop('MaterialId'),
                    sorter: 'number',
                    hozAlign: 'left',
                    width: 150,
                    formatter: function(cell, formatterParams) {
                        cell.getElement().classList.add('tabulator-operation');
                        const rowdata = <MaterialListModel>cell.getRow().getData();
                        const type: MediaType = rowdata.FileCategory;
                        const detailbtn = DetailButton(rowdata.MaterialId, message.Detail); //added
                        const editbtn = IsSupportPartial ? FilmButton(rowdata.MaterialId, '') : '';
                        const deletebtn = DeleteButton(rowdata.MaterialId, message.Controller);
                        const btngroups: string =
                            type == MediaType.VIDEO ? detailbtn + editbtn + deletebtn : detailbtn + deletebtn;
                        return btngroups;
                    },
                    cellClick: function(e, cell) {
                        const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                        const MaterialId: number = Number(cell.getValue());
                        const row = cell.getRow();
                        const rowdata: MaterialListModel = row.getData();
                        const array = rowdata.ParameterStr.split(';') || [];
                        const begintime = array[0] !== undefined ? Number(array[0]) : 0;
                        const endtime = array[1] !== undefined ? Number(array[1]) : 0;
                        switch (true) {
                            /*點擊:檢視*/
                            case target instanceof HTMLElement &&
                                (<HTMLElement>target).className.indexOf('list icon') > -1:
                            case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                                $(FilmModalId).length > 0 ? $(FilmModalId).remove() : false;
                                MediaDetail(route.api.ShowDetail, MaterialId, rowdata.FileCategory);
                                break;
                            /**剪裁 */
                            case target instanceof HTMLElement &&
                                (<HTMLElement>target).className.indexOf('film icon') > -1:
                            case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                                ShowModal<IdModel>(FilmModalId, route.api.ShowFilmEdit, { id: MaterialId }).then(
                                    success => {
                                        if (success) {
                                            let CustListData: Array<CreateMaterialModel> = [];
                                            const $CreateFilmId = '#CreateFilm';
                                            const embed = $($CreateFilmId).find('.ui.embed');
                                            const player = new videoPlayer(
                                                '#CreateFilm .ui.embed',
                                                '#videoMenu',
                                                '#CreateFilm #fullScreenContainer'
                                            );
                                            ModalTask(FilmModalId, true, {
                                                closable: false,
                                                allowMultiple: true,
                                                onShow: function() {
                                                    $(FilmModalId)
                                                        .children('i.close')
                                                        .click(function() {
                                                            player.Destory();
                                                        });
                                                    const fileUrl = embed.attr('data-url');
                                                    const filePlaceholder = embed.attr('data-placeholder');
                                                    if (IsNULLorEmpty(fileUrl)) {
                                                        ErrorMessage('無法取得檔案,請重新整理頁面後再試');
                                                        embed.html(
                                                            GetImage(AirmamImage.VideoPreview, '影片預覽圖', [
                                                                'ui',
                                                                'image',
                                                            ])
                                                        );
                                                        $(FilmModalId).modal('hide');
                                                    } else {
                                                        IsFileValid(fileUrl).then(success => {
                                                            const $BeginInput = $('#BeginTime');
                                                            const $EndInput = $('#EndTime');
                                                            const $SetBegin = $('#SetBegin');
                                                            const $SetEnd = $('#SetEnd');
                                                            const $AddList = $('#AddList');
                                                            const $CutList = $('#CutList');
                                                            if (success) {
                                                                const timerange_ =
                                                                    begintime !== 0 || endtime !== 0
                                                                        ? { start: begintime, end: endtime }
                                                                        : null;
                                                                player.Load(fileUrl, '', timerange_);
                                                                $SetBegin.click(function() {
                                                                    const current = player.GetCurrentTime() || 0;
                                                                    const timecode = CurrentTimeToTimeCode(
                                                                        current,
                                                                        'second'
                                                                    );
                                                                    $BeginInput.val(timecode);
                                                                    $BeginInput.attr('data-Beg', current.toFixed(3));
                                                                });
                                                                $SetEnd.click(function() {
                                                                    const current = player.GetCurrentTime() || 0;
                                                                    const timecode = CurrentTimeToTimeCode(
                                                                        current,
                                                                        'second'
                                                                    );
                                                                    $EndInput.val(timecode);
                                                                    $EndInput.attr('data-End', current.toFixed(3));
                                                                });
                                                                $AddList.click(function() {
                                                                    const BeginTime =
                                                                        Number($BeginInput.attr('data-Beg')) || 0;
                                                                    const EndTime =
                                                                        Number($EndInput.attr('data-End')) || 0;
                                                                    const duration = EndTime - BeginTime;
                                                                    if (duration == 0) {
                                                                        WarningMessage('剪裁的時間長度不可為0');
                                                                    } else {
                                                                        if (BeginTime > EndTime) {
                                                                            WarningMessage('起始時間應該小於結束時間');
                                                                        } else {
                                                                            const ParameterStr = [
                                                                                BeginTime,
                                                                                EndTime,
                                                                                duration.toFixed(3),
                                                                            ];
                                                                            const itemno = CustListData.length;
                                                                            $CutList.append(`<div class="item" data-no="${itemno}" data-Beg="${BeginTime}">
                                                                                      <div class="left floated fluid content">
                                                                                        <i class="video floated icon"></i>
                                                                                        ${$BeginInput.val()}~${$EndInput.val()}
                                                                                        <div class="right floated content">
                                                                                        <button type="button" name="removeList" class="ui red button">移除</button>
                                                                                       </div>
                                                                                      </div>   
                                                                                    </div>`);
                                                                            CustListData.push({
                                                                                FileCategory: rowdata.FileCategory,
                                                                                FileNo: rowdata.FileNo,
                                                                                MaterialDesc:
                                                                                    rowdata.MaterialDesc || '',
                                                                                MaterialNote:
                                                                                    rowdata.MaterialNote || '',
                                                                                ParameterStr: ParameterStr.join(';'),
                                                                            });
                                                                        }
                                                                    }
                                                                });
                                                                $CutList.on(
                                                                    'click',
                                                                    "button[name='removeList']",
                                                                    function() {
                                                                        const item = $(this).closest('.item');
                                                                        const datano = Number(item.attr('data-no'));
                                                                        const wantremove = CustListData[datano];
                                                                        item.remove();
                                                                        CustListData = CustListData.filter(
                                                                            item => item !== wantremove
                                                                        );
                                                                    }
                                                                );
                                                                $CutList.on(
                                                                    'click',
                                                                    '.item>.content:not(.right)',
                                                                    function() {
                                                                        const video: HTMLVideoElement = document
                                                                            .getElementById(
                                                                                FilmModalId.replace('#', '')
                                                                            )
                                                                            .querySelector('video');
                                                                        const currentSecond = Number(
                                                                            $(this)
                                                                                .parent()
                                                                                .attr('data-Beg')
                                                                        );
                                                                        video.currentTime = currentSecond;
                                                                    }
                                                                );
                                                            } else {
                                                                $SetBegin.hide();
                                                                $SetEnd.hide();
                                                                $AddList.hide();
                                                                embed.html(GetImage(filePlaceholder));
                                                            }
                                                        });
                                                    }
                                                },
                                                onApprove: function() {
                                                    if (CustListData.length > 0) {
                                                        route.FilmEdit(CustListData).then(res => {
                                                            if (res.IsSuccess) {
                                                                SuccessMessage(res.Message);
                                                                CustListData = [];
                                                                const data = <Array<CreateMaterialModel>>res.Data;
                                                                for (let item of data) {
                                                                    table.AddRow(item);
                                                                }
                                                                $(FilmModalId).modal('hide');
                                                                player.Destory();
                                                            } else {
                                                                ErrorMessage(res.Message);
                                                            }
                                                        });
                                                    } else {
                                                        WarningMessage('至少剪輯一筆資料');
                                                    }

                                                    return false;
                                                },
                                                onDeny: function() {
                                                    if (CustListData.length > 0) {
                                                        ModalTask('#FileEditConfirm', true, {
                                                            closable: false,
                                                            allowMultiple: true,
                                                            onApprove: function() {
                                                                $(FilmModalId).modal('hide');
                                                            },
                                                            onDeny: function() {},
                                                        });
                                                        return false;
                                                    }
                                                    player.Destory();
                                                },
                                            });
                                        } else {
                                            Logger.viewres(route.api.ShowFilmEdit, '段落剪輯燈箱發生錯誤', '');
                                        }
                                    }
                                );
                                break;
                            /**刪除 */
                            case target instanceof HTMLElement &&
                                (<HTMLElement>target).className.indexOf('delete icon') > -1:
                            case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                                const DeleteConfirmId = '#DeleteConfirm';
                                ModalTask(DeleteConfirmId, true, {
                                    closable: false,
                                    onShow: function() {
                                        $(DeleteConfirmId)
                                            .find('.content')
                                            .empty()
                                            .html(
                                                `確定要刪除借調清單【${rowdata.MaterialId}】-檔案【${rowdata.FileNo}】？`
                                            );
                                    },
                                    onApprove: function() {
                                        route.DeleteFile([rowdata.MaterialId]).then(res => {
                                            Logger.res(route.api.DeleteFile, '刪除借調清單', res);
                                            if (res.IsSuccess) {
                                                table.RemoveRow(rowdata.MaterialId);
                                            }
                                        });
                                    },
                                });
                                break;
                            default:
                                break;
                        }
                    },
                },
            ],
        },
        true
    );
}
/**指定列表checkbox設置unchecked */
const RowSetUnchecked = (rowId: number | string) => {
    const rowElement = table
        .GetTable()
        .getRow(rowId)
        .getElement();
    const $checkbox = $(rowElement).find('.checkbox');
    const $checkboxinput = $(rowElement).find('input[type="checkbox"]');
    $checkbox.checkbox('set unchecked');
    $checkboxinput.removeAttr('checked');
};
/**取得不重複的影片檔案編號 */
const GetFileNosFromList = (lists: Array<MaterialListModel>): Array<string> => {
    const filenoArray: Array<string> = [];
    for (let list of lists) {
        filenoArray.indexOf(list.FileNo) == -1 && list.FileCategory === MediaType.VIDEO
            ? filenoArray.push(list.FileNo)
            : false;
    }
    return filenoArray;
};

/**取得Tsm狀態 */
const GetTsmTask = (lists: Array<MaterialListModel>): void => {
    if (IsUseTsm) {
        const videoNos = GetFileNosFromList(lists);
        const videoLength: number = videoNos.length;
        /*測試後能較容易取得Tsm狀態的檔案個數*/
        const bestGetTsmLength: number = TSMSetting.bestGetTsmLength;
        let arrayOfArrays: Array<Array<string>> = [];
        const maxCreateArrayLength = Math.ceil(videoNos.length / bestGetTsmLength);
        //Step1:取得所有影片編號,並且切割成多個長度小於等於8的陣列
        for (let i = 0; i < maxCreateArrayLength; i++) {
            const start = i * bestGetTsmLength;
            const end = (i + 1) * bestGetTsmLength < videoLength ? (i + 1) * bestGetTsmLength : videoLength;
            const array = videoNos.slice(start, end);
            arrayOfArrays.push(array);
        }
        //Step2:分批次取得Tsm狀態
        if (arrayOfArrays.length > 0) {
            for (let array of arrayOfArrays) {
                route
                    .GetTmsStatus(MediaType.VIDEO, array)
                    .then(res => {
                        Logger.res(route.api.GetTsmStatus, '取得TSM狀態', res, false);
                        const filesResult = res.Data;
                        if (!IsNULLorEmpty(filesResult) && filesResult.TsmFileStatus.length > 0) {
                            /**暫存每次的檔案Tsm查詢結果 */
                            savefilesResult = savefilesResult.concat(filesResult.TsmFileStatus);
                            for (let fileResult of filesResult.TsmFileStatus) {
                                const filestatus = fileResult.FILE_STATUS;
                                const statusColor = GetTsmColor(filestatus);
                                const statusImage = GetImage(GetTsmImgUrlByStatus(filestatus));
                                const statusStr = GetTsmTextByStatus(filestatus);
                                const label = Label(statusImage, statusColor, ['image', 'tansparent'], statusStr);
                                /*推通知訊息給Layout.ts*/
                                windowPostMessage<TsmMessageModel>({
                                    eventid: 'UpdateTsmStatus',
                                    fileno: fileResult.FILE_NO,
                                    labelHTML: label,
                                });
                            }
                        }
                    })
                    .catch(error => {
                        Logger.viewres(route.api.GetTsmStatus, '取得TSM狀態', error, false);
                        const filestatus = TSMFileStatus.Processing;
                        const statusImage = GetImage(GetTsmImgUrlByStatus(filestatus));
                        const statusStr = '查詢失敗';
                        const label = Label(statusImage, Color.紅, ['image', 'tansparent'], statusStr);
                        for (let fileno in array) {
                            /*推通知訊息給Layout.ts*/
                            windowPostMessage<TsmMessageModel>({
                                eventid: 'UpdateTsmStatus',
                                fileno: fileno,
                                labelHTML: label,
                            });
                        }
                    });
            }
        }
    }
};
