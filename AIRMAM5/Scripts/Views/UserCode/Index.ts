import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { CheckForm } from '../../Models/Function/Form';
import { StringEnum } from '../../Models/Enum/StringEnum';
import {
    SearchFormId,
    CreateFormId,
    CreateModalId,
    EditModalId,
    EditFormId,
    DeleteModalId,
} from '../../Models/Const/Const.';
import { FormValidField } from '../../Models/Const/FormValid';
import { UserCodeMessageSetting } from '../../Models/MessageSetting';
import { UserCodeController, IUserCodeController } from '../../Models/Controller/UserCodeController';
import { SuccessMessage, ErrorMessage } from '../../Models/Function/Message';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { EditButton, DetailButton, CogButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { Label } from '../../Models/Templete/LabelTemp';
import { Color } from '../../Models/Enum/ColorEnum';
import { IsNULLorEmpty, IsNULL } from '../../Models/Function/Check';
import { CodeSearchModel } from '../../Models/Interface/UserCode/CodeSearchModel';
import { CodoResponseModel } from '../../Models/Interface/UserCode/CodoResponseModel';
import { CreateUserSubCodeModel } from '../../Models/Interface/UserCode/CreateUserSubCodeModel';
import { SubCodeListModel } from '../../Models/Interface/UserCode/SubCodeListModel';
import { EditUserSubCodeModel } from '../../Models/Interface/UserCode/EditUserSubCodeModel';
import { SysCodeSearchModel } from '../../Models/Interface/SysCode/SysCodeSearchModel';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
/*================================宣告變數======================================= */
var cogtable: ItabulatorService;
var table: ItabulatorService;
const valid = FormValidField.UserCode;
const message = UserCodeMessageSetting;
const subTableId: string = '#SubTable';
const CogFormId = '#CogForm';
const CogModalId = '#CogModal';
var route: IUserCodeController = new UserCodeController();
/**回傳Modal性質*/
const prop = (key: keyof CodeSearchModel): string => {
    return route.GetProperty<CodeSearchModel>(key);
};
const prop2 = (key: keyof SubCodeListModel): string => {
    return route.GetProperty<SubCodeListModel>(key);
};
/*================================操作======================================= */
/*查詢*/
Search({
    fsCODE_ID: StringEnum.Empty,
    fsTITLE: StringEnum.Empty,
});
/*查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        Search({
            fsCODE_ID: <string>$('#codeID').val(),
            fsTITLE: <string>$('#codename').val(),
        });
    }
});
/**新增自訂代碼 */
ModalTask(CreateModalId, false, {
    closable: false,
    onApprove: function() {
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            route
                .Create({
                    fsCODE_ID: <string>$('#fsCODE_ID').val(),
                    fsTITLE: <string>$('#fsTITLE').val(),
                    IsEnabled: $('#Checkenabled').checkbox('is checked') ? true : false,
                    fsNOTE: <string>$('#fsNOTE').val(),
                })
                .then(res => {
                    Logger.res(route.api.Create, '新增主代碼', res, true);
                    if (res.IsSuccess) {
                        const data = <CodeSearchModel>res.Data;
                        table.AddRow(data);
                        $(CreateFormId).trigger('reset');
                        $(CreateModalId).modal('hide');
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.Create, '新增主代碼', error, true);
                });
        }
        return false;
    },
    onDeny:function(){
        $(CreateFormId).trigger('reset');
    }
}).modal('attach events', "button[name='create']");
/**列表篩選 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fsCODE_ID'), type: Filter.Like, value: word },
        { field: prop('fsTITLE'), type: Filter.Like, value: word },
        { field: prop('C_nCNT_CODE'), type: Filter.Like, value: word },
        { field: prop('fsNOTE'), type: Filter.Like, value: word },
    ];
    const BooleanWord: YesNo | '' = word == '是' ? YesNo.是 : word == '否' ? YesNo.否 : '';
    if (!IsNULLorEmpty(BooleanWord)) {
        filter.push({ field: 'fsIS_ENABLED', type: Filter.Like, value: BooleanWord });
    }
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: SysCodeSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        addRowPos: 'top',
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('fsCODE_ID'),
        columns: [
            { title: '代碼編號', field: prop('fsCODE_ID'), minWidth: 245, sorter: 'string' },
            { title: '代碼名稱', field: prop('fsTITLE'), minWidth: 320, sorter: 'string' },
            { title: '代碼數量', field: prop('C_nCNT_CODE'), sorter: 'string', width: 120 },
            { title: '備註', field: prop('fsNOTE'), sorter: 'string', width: 165 },
            {
                title: '可選',
                field: prop('fsIS_ENABLED'),
                width: 80,
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    const enabled = <YesNo>cell.getValue();
                    switch (enabled) {
                        case YesNo.是:
                            return Label('是', Color.綠);
                        case YesNo.否:
                            return Label('否', Color.紅);
                        default:
                            return enabled;
                    }
                },
            },
            {
                title: '操作',
                field: prop('fsCODE_ID'),
                hozAlign: 'left',
                width: 180,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: string = cell.getValue();
                    const editbtn = EditButton(id, message.Controller);
                    const detailbtn = DetailButton(id, message.Controller);
                    const cogbtn = CogButton(id, message.Controller);
                    const deletebtn = DeleteButton(id, message.Controller);
                    const btngroups: string = editbtn + detailbtn + cogbtn + deletebtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const id: string = cell.getValue();
                    const row = cell.getRow();
                    const rowdata = <CodeSearchModel>row.getData();
                    switch (true) {
                        /*檢視自訂代碼*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.UserCode, Action.ShowDetails, { id: id });
                            break;
                        /*編輯自訂代碼*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: id })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(EditModalId, true, {
                                            closable: false,
                                            onApprove: function() {
                                                const IsFormValid: boolean = CheckForm(EditFormId, valid.Edit);
                                                if (IsFormValid) {
                                                    route
                                                        .Edit({
                                                            fsCODE_ID: id,
                                                            fsTITLE: <string>$('#EditTltle').val(),
                                                            fsIS_ENABLED: $('#CheckEditenabled').checkbox('is checked')
                                                                ? true
                                                                : false,
                                                            fsNOTE: <string>$('#EditNote').val(),
                                                            IsEnabled: $('#CheckEditenabled').checkbox('is checked')
                                                                ? true
                                                                : false,
                                                        })
                                                        .then(res => {
                                                            Logger.res(route.api.Edit, '編輯子代碼', res, true);
                                                            if (res.IsSuccess) {
                                                                const data = <CodoResponseModel>res.Data;
                                                                table.ReactivityUpdate(id, <CodeSearchModel>{
                                                                    fsCODE_ID: data.fsCODE_ID,
                                                                    fsNOTE: data.fsNOTE,
                                                                    C_nCNT_CODE: rowdata.C_nCNT_CODE,
                                                                    fsIS_ENABLED: data.fsIS_ENABLED,
                                                                    fsTITLE: data.fsTITLE,
                                                                });
                                                                $(EditModalId).modal('hide');
                                                            }
                                                        })
                                                        .catch(error => {
                                                            Logger.viewres(route.api.Edit, '編輯主代碼', error, true);
                                                        });
                                                }
                                                return false;
                                            },
                                        });
                                    } else {
                                        ErrorMessage(`編輯主代碼燈箱發生錯誤`);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowEdit, '顯示編輯代碼燈箱', error, true);
                                });
                            break;
                        /*刪除自訂代碼*/
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal<IdModel>(DeleteModalId, route.api.ShowDelete, { id: id })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(DeleteModalId, true, {
                                            closable: false,
                                            onApprove: function() {
                                                route.Delete(id).then(res => {
                                                    if (res.IsSuccess) {
                                                        table.RemoveRow(id);
                                                        SuccessMessage(res.Message);
                                                        $(DeleteModalId).modal('hide');
                                                    } else {
                                                        ErrorMessage(res.Message);
                                                    }
                                                });
                                                return false;
                                            },
                                        });
                                    } else {
                                        ErrorMessage(`刪除代碼燈箱發生錯誤`);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowDelete, '顯示刪除代碼燈箱', error, true);
                                });
                            break;
                        /**設定子檔 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('cog icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'cog':
                            ShowModal<IdModel>(CogModalId, route.api.ShowCog, { id: id })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(CogModalId, true, {
                                            allowMultiple: true,
                                            onShow: function() {
                                                subtableTask();
                                                insertSubCodeTask(id);
                                            },
                                        });
                                    } else {
                                        ErrorMessage(`設定代碼子檔燈箱發生錯誤`);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowCog, '顯示代碼子檔燈箱', error, true);
                                });
                            break;
                    }
                },
            },
        ],
    });
}
/*================================宣告Task======================================= */
/**Task:插入子代碼到table */
const insertSubCodeTask = (MainCodeId: string) => {
    const $CogForm = $(CogFormId);
    $('#createSubCode').click(function(event) {
        event.preventDefault();
        const IsFormValid: boolean = CheckForm(CogFormId, valid.CreateSubCode);
        if (IsFormValid) {
            const cogtableLength: number = cogtable.GetData().length;
            route
                .CreateSubCode({
                    fsTITLE: <string>$CogForm.find("input[name='fsTITLE']").val(),
                    fsCODE_ID: MainCodeId,
                    fsNAME: <string>$CogForm.find("input[name='Code.fsNAME']").val(),
                    fsENAME: <string>$CogForm.find("input[name='Code.fsENAME']").val(),
                    fnORDER: Number($CogForm.find("input[name='Code.fnORDER']").val()),
                    IsEnabled: $('#subCodeEnabled').checkbox('is checked') ? true : false,
                    fsIS_ENABLED: $('#subCodeEnabled').checkbox('is checked') ? YesNo.是 : YesNo.否,
                    fsNOTE: <string>$CogForm.find("textarea[name='Code.fsNOTE']").val(),
                    fsSET: <string>$('#subCodeSetting').val(),
                    fsCODE: <string>$('#subCode').val(),
                })
                .then(res => {
                    if (res.IsSuccess) {
                        SuccessMessage(res.Message);
                        const Records = <CreateUserSubCodeModel>res.Records;
                        cogtable.AddRow(Records);
                        $(CogFormId).trigger('reset');
                        table.ReactivityUpdate(<string>Records.fsCODE_ID, { C_nCNT_CODE: cogtableLength + 1 });
                    } else {
                        ErrorMessage(res.Message);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.CreateCode, '新增自定義子代碼', error, true);
                });
        }
    });
};

/**Task:創建子代碼table */
const subtableTask = () => {
    const editableInput = function(cell: Tabulator.CellComponent) {
        const rowElement: HTMLElement = cell.getRow().getElement();
        return rowElement.getAttribute('data-isediting') == 'true' ? true : false;
    };
    const lockIcon = `<i class="icon blue lock"></i><br>`;
    const editIcon = `<br>`;
    const json = JSON.parse(JSON.stringify($('#SaveData').data('json')));
    cogtable = new tabulatorService(subTableId, {
        pagination: 'local',
        addRowPos: 'top',
        data: json,
        index: prop2('fsCODE'),
        columns: [
            {
                title: '子代碼',
                field: prop2('fsCODE'),
                sorter: 'string',
                width: 130,
                titleFormatter: function() {
                    return lockIcon + '子代碼';
                },
            },
            {
                title: '名稱',
                field: prop2('fsNAME'),
                sorter: 'string',
                editor: 'input',
                editable: editableInput,
                validator: 'required',
                width: 130,
                titleFormatter: function() {
                    return editIcon + '名稱';
                },
            },
            {
                title: '英文名稱',
                field: prop2('fsENAME'),
                sorter: 'string',
                editor: 'input',
                width: 120,
                editable: editableInput,
                validator: 'regex:[A-Za-z0-9]',
                titleFormatter: function() {
                    return editIcon + '英文名稱';
                },
            },
            {
                title: '顯示順序',
                field: prop2('fnORDER'),
                sorter: 'number',
                editor: 'number',
                width: 120,
                editable: editableInput,
                validator: 'min:0',
                titleFormatter: function() {
                    return editIcon + '顯示順序';
                },
            },
            {
                title: '備註',
                field: prop2('fsNOTE'),
                sorter: 'string',
                editor: 'input',
                width: 120,
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '備註';
                },
            },
            {
                title: '可選',
                field: prop2('IsEnabled'),
                width: 80,
                sorter: 'boolean',
                formatter: function(cell, formatterParams) {
                    const enabled: boolean = cell.getValue();
                    const labelStr = enabled ? '是' : '否';
                    const color = enabled ? Color.綠 : Color.紅;
                    return Label(labelStr, color);
                },
                editor: 'tickCross',
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '可選';
                },
            },
            {
                title: '設定',
                field: prop2('fsSET'),
                sorter: 'string',
                editor: 'input',
                width: 110,
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '設定';
                },
            },
            {
                title: '操作',
                field: prop2('fsCODE'),
                hozAlign: 'center',
                minWidth: 120,
                titleFormatter: function() {
                    return editIcon + '操作';
                },
                formatter: function(cell, formatterParams) {
                    // cell.getElement().classList.add("tabulator-operation");/*會導致欄位樣式動態抖動*/
                    const id: string = cell.getRow().getData().fsCODE_ID;
                    const editbtn = EditButton(id, message.Controller);
                    const deletebtn = DeleteButton(id, message.Controller);
                    const savebtn = `<button type="button" name="save" data-inverted="" data-tooltip="儲存" data-position="bottom center" class="ui yellow circular icon button" data-Id="${id}">儲存</button>`;
                    const cancelbtn = `<button type="button" name="cancel" data-inverted="" data-tooltip="取消" data-position="bottom center" class="ui circular icon button" data-Id="${id}">取消</button>`;
                    const groupbtn = `<div class="btngroup1">${editbtn}${deletebtn}</div><div class="btngroup2" style="display:none;">${savebtn}${cancelbtn}</div>`;
                    return groupbtn;
                },
                cellClick: function(e: any, cell: Tabulator.CellComponent) {
                    e.preventDefault();
                    const cellElement = cell.getElement();
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = e.target;
                    const row = cell.getRow();
                    const rowElement = row.getElement();
                    const rowdata = <SubCodeListModel>row.getData();
                    const parentCodeId: string = rowdata.fsCODE_ID;
                    const subCodeId: string = rowdata.fsCODE;
                    const cells: Tabulator.CellComponent[] = row.getCells();
                    const btngroup1 = <HTMLDivElement>cellElement.querySelector('.btngroup1');
                    const btngroup2 = <HTMLDivElement>cellElement.querySelector('.btngroup2');
                    switch (true) {
                        /*Task:編輯子代碼 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            rowElement.setAttribute('data-isediting', 'true');
                            rowElement.setAttribute('data-originData', JSON.stringify(rowdata));
                            btngroup1.style.display = 'none';
                            btngroup2.style.display = 'block';
                            //  cogtable.RedrawTable();
                            break;
                        /*Task:取消儲存子代碼 */
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'cancel':
                            rowElement.setAttribute('data-isediting', 'false');
                            btngroup1.style.display = 'block';
                            btngroup2.style.display = 'none';
                            const orginData =
                                <SubCodeListModel>JSON.parse(rowElement.getAttribute('data-originData')) ||
                                <SubCodeListModel>{};
                            for (const CELL of cells) {
                                const CELLOldValue = CELL.getOldValue();
                                if (!IsNULL(CELLOldValue)) {
                                    CELL.setValue(orginData[CELL.getField()], true);
                                }
                            }
                            break;
                        /*Task:儲存子代碼 */
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'save':
                            let parameters = <EditUserSubCodeModel>{};
                            let validerrorField: string[] = [];
                            cells.forEach((item, index) => {
                                const field: string = item.getField();
                                const newValue = item.getValue();
                                item.getElement().className.indexOf('tabulator-validation-fail') > -1
                                    ? validerrorField.push(field)
                                    : false;

                                switch (field) {
                                    case route.GetProperty<SubCodeListModel>('fsCODE'):
                                        parameters.fsCODE_ID = parentCodeId;
                                        parameters.fsCODE = subCodeId;
                                        break;
                                    case route.GetProperty<SubCodeListModel>('fsNAME'):
                                        parameters.fsNAME = IsNULLorEmpty(newValue) ? StringEnum.Empty : newValue;
                                        break;
                                    case route.GetProperty<SubCodeListModel>('fsENAME'):
                                        parameters.fsENAME = IsNULLorEmpty(newValue) ? StringEnum.Empty : newValue;
                                        break;
                                    case route.GetProperty<SubCodeListModel>('fnORDER'):
                                        parameters.fnORDER = IsNULLorEmpty(newValue) ? 99 : Number(newValue);
                                        break;
                                    case route.GetProperty<SubCodeListModel>('fsSET'):
                                        parameters.fsSET = IsNULLorEmpty(newValue) ? StringEnum.Empty : newValue;
                                        break;
                                    case route.GetProperty<SubCodeListModel>('fsNOTE'):
                                        parameters.fsNOTE = IsNULLorEmpty(newValue) ? StringEnum.Empty : newValue;
                                        break;
                                    case route.GetProperty<SubCodeListModel>('IsEnabled'):
                                        parameters.IsEnabled = newValue || newValue == 'true' ? true : false;
                                        parameters.fsIS_ENABLED = newValue || newValue == 'true' ? YesNo.是 : YesNo.否;
                                        break;
                                }
                            });
                            if (validerrorField.length == 0) {
                                route
                                    .EditSubCode({
                                        IsEnabled: parameters.IsEnabled,
                                        fsCODE_ID: parameters.fsCODE_ID,
                                        fsCODE: parameters.fsCODE,
                                        fsNAME: parameters.fsNAME,
                                        fsENAME: parameters.fsENAME,
                                        fsSET: parameters.fsSET,
                                        fnORDER: parameters.fnORDER,
                                        fsTITLE: parameters.fsTITLE,
                                        fsIS_ENABLED: parameters.IsEnabled ? YesNo.是 : YesNo.否,
                                        fsNOTE: parameters.fsNOTE,
                                    })
                                    .then(res => {
                                        if (res.IsSuccess) {
                                            rowElement.setAttribute('data-isediting', 'false');
                                            const CELL = cells[cells.length - 1].getElement();
                                            (<HTMLDivElement>CELL.querySelector('.btngroup1')).style.display = 'block';
                                            (<HTMLDivElement>CELL.querySelector('.btngroup2')).style.display = 'none';
                                            cogtable.ReactivityUpdate(subCodeId, res.Records);
                                            SuccessMessage(res.Message);
                                        } else {
                                            ErrorMessage(res.Message);
                                        }
                                    })
                                    .catch(error => {
                                        Logger.viewres(route.api.EditCode, '編輯子代碼', error, true);
                                    });
                            } else {
                                ErrorMessage('欄位格式錯誤');
                            }

                            break;
                        /*Task:刪除子代碼 */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            const ConfirmDeleteModalId = '#ConfirmDeleteCodeModal';
                            ModalTask(ConfirmDeleteModalId, true, {
                                closable: false,
                                allowMultiple: true,
                                onApprove: function() {
                                    route
                                        .DeleteSubCode({ CodeId: parentCodeId, Code: subCodeId })
                                        .then(res => {
                                            if (res.IsSuccess) {
                                                SuccessMessage(res.Message);
                                                cogtable.RemoveRow(subCodeId);
                                                const cogtableLength: number = cogtable.GetData().length;
                                                table.ReactivityUpdate(parentCodeId, { C_nCNT_CODE: cogtableLength });
                                            } else {
                                                ErrorMessage(res.Message);
                                                cogtable.GetRow(subCodeId).style.border = '1px solid red';
                                            }
                                        })
                                        .catch(error => {
                                            Logger.viewres(route.api.DeleteCode, '刪除子代碼', error, true);
                                        });
                                },
                                onDeny: function() {
                                    $(CogModalId).modal('show');
                                },
                            });
                            break;
                    }
                    return false;
                },
            },
        ],
    });
};
