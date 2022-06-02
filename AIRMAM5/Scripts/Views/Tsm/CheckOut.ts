import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { TsmMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { CheckForm } from '../../Models/Function/Form';
import { SearchFormId } from '../../Models/Const/Const.';
import { Filter } from '../../Models/Enum/Filter';
import { TabulatorSetting, initSetting } from '../../Models/initSetting';
import { ITsmController, TsmController } from '../../Models/Controller/TsmController';
import { TapeInfoModel } from '../../Models/Interface/Tsm/TapeInfoModel';
import { WarningMessage, SuccessMessage, ErrorMessage } from '../../Models/Function/Message';
import { ModalTask } from '../../Models/Function/Modal';
import { Logger } from '../../Models/Class/LoggerService';
import { IsNULLorEmpty } from '../../Models/Function/Check';
/*===============================================================*/
/*宣告變數*/
const message = TsmMessageSetting;
const valid = FormValidField.Tsm;
const route: ITsmController = new TsmController();
var table: ItabulatorService;
var selectids: Array<string> = [];
/**下架按鈕 */
const $CheckOutBtn = $('#CheckOutBtn');
/**下架確認燈箱 */
const ConfirmCheckoutModalId = '#ConfirmCheckoutModal';
const $ConfirmCheckoutModal = $(ConfirmCheckoutModalId);
/**批次可下架的最大數量 */
const MaxCheckOutCount = Number($('input[name="MaxCheckOutCount"]').val()) || 1;
/**回傳Modal性質*/
const prop = (key: keyof TapeInfoModel): string => {
    return route.GetProperty<TapeInfoModel>(key);
};
/*頁面載入查詢*/
Search('Lib');
/**表單清空 */
$("button[name='reset']").click(function() {
    $('input[name="ChckoutType"][value="Lib"]').prop('checked', true);
});
/*查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const type: 'Lib' | 'All' = $('input[name="ChckoutType"]:checked').val() === 'Lib' ? 'Lib' : 'All';
        Search(type);
    }
});
/**依據選擇類型,禁用或啟用下架按鈕 */
$('input[name="ChckoutType"]').on('change', function() {
    const type: 'Lib' | 'All' = $('input[name="ChckoutType"]:checked').val() === 'Lib' ? 'Lib' : 'All';
    type == 'All' ? $CheckOutBtn.addClass('disabled') : $CheckOutBtn.removeClass('disabled');
});
/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('VOL_ID'), type: Filter.Like, value: word },
        { field: prop('VOL_TYPE'), type: Filter.Like, value: word },
        { field: prop('VOL_USE_STATUS'), type: Filter.Like, value: word },
        { field: prop('USED_GB'), type: Filter.Like, value: word },
        { field: prop('READ_DATE'), type: Filter.Like, value: word },
        { field: prop('WRITE_DATE'), type: Filter.Like, value: word },
        { field: prop('POOL_NAME'), type: Filter.Like, value: word },
        { field: prop('VOL_RW_STATUS'), type: Filter.Like, value: word },
        { field: prop('WRITE_ERRORS'), type: Filter.Like, value: word },
        { field: prop('READ_ERRORS'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: 'Lib' | 'All') {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: SearchParams === 'Lib' ? route.api.GetTapeInfoInLib : route.api.GetAllTapeInfo,
        ajaxContentType: 'json',
        ajaxConfig: 'GET',
        // ajaxParams: {},
        index: prop('VOL_ID'),
        initialSort: [
            { column: prop('VOL_ID'), dir: 'asc' },
            // { column: prop('POOL_NAME'), dir: 'asc' },
        ],
        groupStartOpen: false,
        groupToggleElement: 'header',
        groupHeader: [
            function(value, count, data) {
                return `<div class="group-header">【${value}】共 ${count} 捲磁帶</div>`;
            },
        ],
        groupBy: function(data: TapeInfoModel) {
            /*Notice:以磁帶編號前面字母為群組區分*/
            const tapeID = data.VOL_ID;
            const tapeGroup: Array<string> = tapeID.split(/([^A-Za-z])/).filter(text => text);
            return tapeGroup[0];
        },
        rowFormatter: function(row) {
            const rowdata = <TapeInfoModel>row.getData();
            if (
                (!IsNULLorEmpty(rowdata.WRITE_ERRORS) && rowdata.WRITE_ERRORS !== '0') ||
                (!IsNULLorEmpty(rowdata.READ_ERRORS) && rowdata.READ_ERRORS !== '0')
            ) {
                row.getElement().style.background = 'rgba(255,0,0,0.35)';
            }
        },
        columns: [
            {
                title: '勾選',
                field: prop('VOL_ID'),
                width: 50,
                sorter: 'string',
                hozAlign: 'center',
                headerSort: false,
                cellClick: function(e, cell) {
                    const id = cell.getValue();
                    const $checkbox = cell.getElement().querySelector('.checkbox');
                    const $checkboxinput = cell.getElement().querySelector("input[type='checkbox']");
                    const ischeck: boolean = $checkboxinput.getAttribute('checked') == 'true' ? true : false;
                    if (ischeck) {
                        $checkbox.classList.remove('checked');
                        $checkboxinput.setAttribute('checked', 'false');
                        selectids = selectids.filter(item => item != id);
                    } else {
                        $checkbox.classList.add('checked');
                        $checkboxinput.setAttribute('checked', 'true');
                        if (selectids.indexOf(id) <= -1) {
                            selectids.push(id);
                        }
                    }
                },
                formatter: function(cell, formatterParams) {
                    return `<div class="ui checkbox"><input type="checkbox"> <label></label></div>`;
                },
            },
            { title: '磁帶編號', field: prop('VOL_ID'), width: 105, sorter: 'string' },
            {
                title: '使用狀態',
                field: prop('VOL_USE_STATUS'),
                width: 85,
                sorter: 'string',
                titleFormatter: function() {
                    return `使用<br>狀態`;
                },
            },
            {
                title: '使用狀態',
                field: prop('VOL_USE_STATUS'),
                width: 75,
                sorter: 'string',
                titleFormatter: function() {
                    return `狀態<br>燈號`;
                },
                formatter: function(cell, formatterParams) {
                    const status: 'FULL' | 'FILLUING' | 'EMPTY' | string = cell.getValue();
                    switch (status) {
                        case 'FULL':
                            return `<span class="ui red empty circular huge label"></span>`;
                        case 'FILLING':
                            return `<span class="ui green empty circular huge label"></span>`;
                        case 'EMPTY':
                            return `<span class="ui empty circular huge inverted label"></span>`;
                        default:
                            return status;
                    }
                },
            },
            { title: '讀寫狀態', field: prop('VOL_RW_STATUS'), sorter: 'string', width: 110 },
            {
                title: '已存放資料量(GB)',
                width: 130,
                titleFormatter: function() {
                    return `已存放<br>資料量(GB)`;
                },
                field: prop('USED_GB'),
                sorter: 'number',
            },
            { title: '儲存池', field: prop('POOL_NAME'), sorter: 'string', minWidth: 100 },
            { title: '最後讀取', field: prop('READ_DATE'), sorter: 'string', width: 160 },
            { title: '最後寫入', field: prop('WRITE_DATE'), sorter: 'string', width: 160 },
            {
                title: '讀取錯誤',
                field: prop('READ_ERRORS'),
                sorter: 'string',
                width: 80,
                titleFormatter: function() {
                    return '讀取<br>錯誤';
                },
            },
            {
                title: '寫入錯誤',
                field: prop('WRITE_ERRORS'),
                sorter: 'string',
                width: 80,
                titleFormatter: function() {
                    return '寫入<br>錯誤';
                },
            },
        ],
    });
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
/**下架 */
$CheckOutBtn.click(function() {
    if (selectids.length == 0) {
        WarningMessage('至少勾選一筆欲下架的內容!');
    } else {
        ModalTask(ConfirmCheckoutModalId, true, {
            closable: false,
            onShow: function() {
                $ConfirmCheckoutModal.find('.content').html(`※注意，確定將磁帶【${selectids.join(',')}】下架嗎?`);
            },
            onApprove: function() {
                if (selectids.length > MaxCheckOutCount) {
                    WarningMessage(`最多只能選擇${MaxCheckOutCount}筆以進行下架提交`);
                } else {
                    route
                        .BatchCheckOut(selectids)
                        .then(res => {
                            if (res.IsSuccess) {
                                SuccessMessage(res.Message);
                                // const data = <Array<TapeInfoModel>>res.Data;
                                for (let tapeid of selectids) {
                                    RowSetUnchecked(tapeid);
                                }
                                $ConfirmCheckoutModal.modal('hide');
                                selectids = [];
                            } else {
                                ErrorMessage(res.Message);
                            }
                        })
                        .catch(error => {
                            Logger.viewres(route.api.CheckOut, '批次磁帶下架', error, true);
                        });
                }
                return false;
            },
            onDeny: function() {},
        });
    }
});
