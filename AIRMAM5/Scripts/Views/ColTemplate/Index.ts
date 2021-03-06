import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { ColFieldType, ColFieldSelectType } from '../../Models/Enum/ColTypeEnum';
import { Action } from '../../Models/Enum/Action';
import { TempleteEnum } from '../../Models/Enum/TempleteEnum';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { ChineseMediaType } from '../../Models/Enum/MediaType';
import { CreateFormId, DeleteModalId, EditFormId, EditModalId } from '../../Models/Const/Const.';
import { IColTemplateController, ColTemplateController } from '../../Models/Controller/ColTemplateController';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { CheckForm } from '../../Models/Function/Form';
import { GetDropdown, GetSelect } from '../../Models/Function/Element';
import { FormValidField } from '../../Models/Const/FormValid';
import { ColTempleteMessageSetting } from '../../Models/MessageSetting';
import { EditButton, DetailButton, CogButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { IsNULLorEmpty, IsNULL, IsNullorUndefined } from '../../Models/Function/Check';
import { Label } from '../../Models/Templete/LabelTemp';
import { Color } from '../../Models/Enum/ColorEnum';
import { ShowCreateTempModel } from '../../Models/Interface/ColTemplate/ShowCreateTempModel';
import { ShowTempleteModel } from '../../Models/Interface/ColTemplate/ShowTempleteModel';
import { ColTemplateSearchModel } from '../../Models/Interface/ColTemplate/ColTemplateSearchModel';
import { EditTempleteModel } from '../../Models/Interface/ColTemplate/EditTempleteModel';
import { EditTempFieldModel } from '../../Models/Interface/ColTemplate/EditTempFieldModel';
import { TemplateFieldModel, TemplateFieldRowModel } from '../../Models/Interface/ColTemplate/TemplateFieldModel';
import { Filter } from '../../Models/Enum/Filter';
import { CreateSelect } from '../../Models/Templete/FormTemp';
import { MainCodeListModel } from '../../Models/Interface/ColTemplate/MainCodeListModel';
import * as dayjs_ from 'dayjs';
import { Logger } from '../../Models/Class/LoggerService';
import { StringEnum } from '../../Models/Enum/StringEnum';
/*=====================????????????===============================*/
const dayjs = (<any>dayjs_).default || dayjs_;
const route: IColTemplateController = new ColTemplateController();
var table: ItabulatorService;
var cogtable: ItabulatorService;
var tableLastSelectRow: Tabulator.RowComponent;
const message = ColTempleteMessageSetting;
const valid = FormValidField.ColTemplete;
const SubTableId: string = '#SubTable';
const SaveDataId: string = '#SaveData';
const ChooseTypeFormId: string = '#ChooseTypeForm';
const ChooseTypeAreaId = '#ChooseTypeArea';
const $SelectDropdown = GetDropdown(CreateFormId, 'fsTABLE');
const lockIcon = `<i class="icon blue lock"></i><br>`;
const editIcon = `<br>`;
/**??????Modal??????*/
const prop = (key: keyof ShowTempleteModel): string => {
    return route.GetProperty<ShowTempleteModel>(key);
};
const prop2 = (key: keyof TemplateFieldRowModel): string => {
    return route.GetProperty<TemplateFieldRowModel>(key);
};
/*=====================??????===============================*/
/*??????????????????????????????
>NEW:?????????[????????????]??????,??????[????????????]?????????
>COPY:?????????[????????????]?????????,??????[????????????]??????,?????????????????????[????????????]
*/
$('.templetetype.checkbox').checkbox({
    onChange: function(this) {
        const type: 'new' | 'copy' = <string>$(this).val() == 'new' ? 'new' : 'copy';
        if (type == 'new') {
            $SelectDropdown.removeClass('disabled');
            $('.templetelayout').checkbox('set disabled');
        } else {
            if (!$SelectDropdown.hasClass('disabled')) {
                $SelectDropdown.addClass('disabled');
            }
            if ($('.checkbox.templetelayout.checked').length > 0) {
                $SelectDropdown.dropdown(
                    'set selected',
                    $('.checkbox.templetelayout.checked').attr('data-templatetype')
                );
            }
            $('.templetelayout').checkbox('set enabled');
        }
    },
});
/**????????????????????????,????????????????????????[????????????] */
$('.templetelayout.checkbox').checkbox({
    onChange: function(this) {
        $(this).checkbox('set checked');
        $SelectDropdown.dropdown(
            'set selected',
            $(this)
                .parent('.checkbox')
                .attr('data-templatetype')
        );
    },
});
/*????????????,?????????????????????tempId,????????????????????????*/
$('#CreateTempBtn').click(function() {
    const $createForm = $(CreateFormId);
    ModalTask('#SelectTempleteModal', true, {
        closable: false,
        onApprove: function() {
            const isnew: boolean = $('#newtemplate').checkbox('is checked');
            if (!isnew) {
                if ($("input[name='templetelayout']:checked").length <= 0) {
                    ErrorMessage('???????????????????????????!');
                    return false;
                }
            }
            const IsFormValid: boolean = CheckForm(CreateFormId, valid.CreateTemp);
            if (IsFormValid) {
                const existtemplate: number = isnew ? 0 : Number($("input[name='templetelayout']:checked").val());
                const template: TempleteEnum = isnew ? TempleteEnum.NEW : TempleteEnum.COPY;
                const isSearch: boolean = $createForm.find(".checkbox[name='issearch']").checkbox('is checked');
                route
                    .CreateCopyTemp({
                        template: template,
                        existtemplate: existtemplate,
                        fsNAME: <string>$createForm.find("input[name='fsNAME']").val(),
                        fsTABLE: $SelectDropdown.dropdown('get value'),
                        fsDESCRIPTION: <string>$createForm.find("textarea[name='fsDESCRIPTION']").val(),
                        IsSearch: isSearch,
                    })
                    .then(res => {
                        if (res.IsSuccess) {
                            //????????????---------------------------------
                            $createForm.trigger('reset');
                            const createParam: ShowCreateTempModel = <ShowCreateTempModel>(<any>res.Data);
                            const modalId = '#CogModal';
                            ShowModal(modalId, route.api.ShowCogView, { id: createParam.fnTEMP_ID }).then(show => {
                                if (show) {
                                    if ($(modalId).length > 0) {
                                        ModalTask(modalId, true, {
                                            closable: false,
                                            onVisible: function() {
                                                AddNewField(ChooseTypeAreaId, createParam.fnTEMP_ID);
                                            },
                                        });
                                        ChangeViewByType('#ChooseType', createParam.fnTEMP_ID, ChooseTypeAreaId);
                                        subtableTask(SubTableId, SaveDataId, createParam.fnTEMP_ID);
                                    }
                                } else {
                                    ErrorMessage('?????????????????????????????????????????????????????????');
                                }
                            });
                            //------------------------------------------
                            table.AddRow(<ShowTempleteModel>{
                                fnTEMP_ID: createParam.fnTEMP_ID,
                                fsTABLE: createParam.fsTABLE,
                                fsNAME: createParam.fsNAME,
                                C_sTABLENAME: getEnumKeyByEnumValue(ChineseMediaType, createParam.fsTABLE),
                                fsDESCRIPTION: createParam.fsDESCRIPTION,
                                fcIS_SEARCH: createParam.IsSearch ? YesNo.??? : YesNo.???,
                            });
                        } else {
                            ErrorMessage(res.Message);
                        }
                    })
                    .catch(error => {
                        Logger.viewres(route.api.CreateCopy, '?????????????????????', error, true);
                    });
            }
            return false;
        },
        onDeny:function(){
            $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
        }
    });
});
/**???????????? */
$('#ReloadBtn').click(function(){
    Search({ fnTEMP_ID: 0, fsTABLE: '' });
});
/*???????????? */
Search({ fnTEMP_ID: 0, fsTABLE: '' });
/**???????????? */
$(initSetting.TableId).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fnTEMP_ID'), type: Filter.Like, value: word },
        { field: prop('fsNAME'), type: Filter.Like, value: word },
        { field: prop('C_sTABLENAME'), type: Filter.Like, value: word },
        { field: prop('fsDESCRIPTION'), type: Filter.Like, value: word },
    ];
    const BooleanWord: YesNo | '' = word == '???' ? YesNo.??? : word == '???' ? YesNo.??? : '';
    if (!IsNULLorEmpty(BooleanWord)) {
        filter.push({ field: prop('fcIS_SEARCH'), type: Filter.Like, value: BooleanWord });
    }
    table.SetFilter(filter);
});
$(document).on('keyup',`${SubTableId} #wordFilter`,function(){
    const word = <string>$(this).val();
    const filter = [
        { field: prop2('FieldName'), type: Filter.Like, value: word },
        { field: prop2('FieldDef'), type: Filter.Like, value: word },
    
    ];
    cogtable.SetFilter(filter);
});

/**
 * ????????????
 * @param SearchParams
 */
function Search(SearchParams: ColTemplateSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        addRowPos: 'top',
        index: prop('fnTEMP_ID'),
        columns: [
            { title: '??????', field: prop('fnTEMP_ID'), width: 90, sorter: 'number', visible: false },
            { title: '????????????', field: prop('fsNAME'), minWidth: 350, sorter: 'string' },
            { title: '????????????', field: prop('C_sTABLENAME'), width: 125, sorter: 'string' },
            {
                title: '???????????????',
                field: prop('fcIS_SEARCH'),
                sorter: 'string',
                width: 125,
                formatter: function(cell, formatterParams) {
                    const enabled: string = cell.getValue();
                    switch (enabled) {
                        case YesNo.???:
                            return `<span class="ui green label">???</span>`;
                        case YesNo.???:
                            return `<span class="ui red label">???</span>`;
                        default:
                            return `<span class="ui red label">${cell.getValue()}</span>`;
                    }
                },
            },
            { title: '????????????', field: prop('fsDESCRIPTION'), sorter: 'string', width: 200 },
            {
                title: '??????',
                field: prop('fnTEMP_ID'),
                hozAlign: 'left',
                width: 210,
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
                    const tempId: number = Number(cell.getValue());
                    switch (true) {
                        /**Task:?????? */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: tempId })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(EditModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $(this).find('select.dropdown').dropdown().dropdown('refresh');
                                            },
                                            onApprove: function() {
                                                const $editform = $(EditModalId).find(EditFormId);
                                                route
                                                    .EditTemp({
                                                        fnTEMP_ID: tempId,
                                                        fsTABLE:<string>$editform.find("input[name='fsTABLE']").val(),
                                                        fsNAME: <string>$editform.find("input[name='fsNAME']").val(),
                                                        fsDESCRIPTION: <string>(
                                                            $editform.find("textarea[name='fsDESCRIPTION']").val()
                                                        ),
                                                        IsSearch: $editform
                                                            .find(".checkbox[name='issearch']")
                                                            .checkbox('is checked')
                                                            ? true
                                                            : false,
                                                        fcIS_SEARCH: $editform
                                                            .find(".checkbox[name='issearch']")
                                                            .checkbox('is checked')
                                                            ? YesNo.???
                                                            : YesNo.???,
                                                    })
                                                    .then(res => {
                                                        if (res.IsSuccess) {
                                                            const data = <EditTempleteModel>res.Data;
                                                            const updatedata: ShowTempleteModel = {
                                                                fnTEMP_ID: data.fnTEMP_ID,
                                                                fsTABLE: data.fsTABLE,
                                                                fsNAME: data.fsNAME,
                                                                C_sTABLENAME: getEnumKeyByEnumValue(
                                                                    ChineseMediaType,
                                                                    data.fsTABLE
                                                                ),
                                                                fsDESCRIPTION: data.fsDESCRIPTION,
                                                                fcIS_SEARCH: data.fcIS_SEARCH,
                                                            };
                                                            table.ReactivityUpdate(tempId, updatedata);
                                                            SuccessMessage(res.Message);
                                                        } else {
                                                            ErrorMessage(res.Message);
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Edit, '????????????', error, true);
                                                    });
                                            },
                                        });
                                    } else {
                                        ErrorMessage(`??????????????????????????????`);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowEdit, '??????????????????', error, true);
                                });
                            break;
                        /**Task:?????? */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.ColTemplate, Action.ShowDetails, { id: tempId });
                            break;
                        /**?????? */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal(DeleteModalId, route.api.ShowDelete, { id: tempId }).then(success=>{
                                if(success){
                                    ModalTask(DeleteModalId,true,{
                                        closable:false,
                                        onApprove:function(){
                                            route.DeleteTemp(tempId)
                                            .then(res => {
                                                if (res.IsSuccess) {
                                                    SuccessMessage(res.Message);
                                                    table.RemoveRow(tempId);
                                                } else {
                                                    ErrorMessage(res.Message);
                                                    table.GetRow(tempId).style.border = '1px solid red';
                                                }
                                            })
                                            .catch(error => {
                                                Logger.viewres(route.api.Delete, '????????????', error, true);
                                            });
                                        }
                                    });
                                }else{
                                    Logger.viewres(route.api.ShowDelete, '??????????????????','', true);
                                }
                            }).catch(error=>{
                                Logger.viewres(route.api.ShowDelete, '??????????????????','', true);
                            });
                            break;
                        /**?????? */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('cog icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'cog':
                            const modalId = '#CogModal';
                            tableLastSelectRow = cell.getRow();
                            ShowModal(modalId, route.api.ShowCogView, { id: tempId }).then(show => {
                                if (show) {
                                    if ($(modalId).length > 0) {
                                        ModalTask(modalId, true, {
                                            closable: false,
                                            onVisible: function() {
                                                AddNewField(ChooseTypeAreaId, tempId);
                                            },
                                        });
                                        subtableTask(SubTableId, SaveDataId, tempId);
                                        ChangeViewByType('#ChooseType', tempId, ChooseTypeAreaId);
                                    }
                                } else {
                                    ErrorMessage('?????????????????????????????????????????????????????????');
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

/**
 * Task1:???????????????table
 * @param TableId ?????????table???Id
 * @param SaveDataId ??????Modal JSON?????????HTMLElement???Id
 * @param templateId ??????Id
 */
const subtableTask = (TableId: string, SaveDataId: string, templateId: number) => {
    const editableInput = function(cell: Tabulator.CellComponent) {
        return cell
            .getRow()
            .getElement()
            .getAttribute('data-isediting') == 'true'
            ? true
            : false;
    };
    const json = JSON.parse(JSON.stringify($(SaveDataId).data('json')));
    cogtable = new tabulatorService(TableId, {
        pagination: 'local',
        layout: 'fitColumns',
        height: '65vh',
        addRowPos: 'top',
        data: json,
        index: prop2('Field'),
        rowFormatter: function(row: Tabulator.RowComponent) {
            const rowElement = row.getElement();
            const rowData = <TemplateFieldRowModel>row.getData();
            rowElement.setAttribute('data-originData', JSON.stringify(rowData));
            rowElement.setAttribute('data-isediting', 'false');
        },
        rowAdded: function(row: Tabulator.RowComponent) {
            const rowData = <TemplateFieldRowModel>row.getData();
            const rowElement = row.getElement();
            const isMutiple = rowData.FieldCodeCnt == ColFieldSelectType.Mutiple ? false : true;
            const selctedValue = isMutiple ? rowData.FieldDef.split(';') : [rowData.FieldDef];
            if (rowData.FieldType == ColFieldType.????????????) {
                setTimeout(function() {
                    const dropdown = rowElement.querySelector('.dropdown');
                    $(dropdown)
                        .dropdown({
                            useLabels: isMutiple,
                            message: {
                                count: '?????????{count}???',
                            },
                        })
                        .dropdown('set exactly', selctedValue);
                }, 100);
            }
        },
        cellEdited: function(cell: Tabulator.CellComponent) {
            if (cell.getField() == route.GetProperty<TemplateFieldRowModel>('FieldDef')) {
                const cellElement = cell.getElement();
                const row = cell.getRow();
                const rowData = <TemplateFieldRowModel>row.getData();
                switch (rowData.FieldType.toUpperCase()) {
                    case ColFieldType.????????????:
                        const selectEditor = cellElement.querySelector('.dropdown');
                        const selectedValue = cellElement.getAttribute('data-changeData');
                        $(selectEditor).dropdown({
                            useLabels: rowData.FieldCodeCnt == ColFieldSelectType.Mutiple ? false : true,
                            message: {
                                count: '?????????{count}???',
                            },
                        });
                        // .dropdown('set exactly', selectedValue.split(','));
                        break;
                }
            }
        },
        columns: [
            {
                title: '????????????',
                field: prop2('Field'),
                sorter: 'string',
                width: 125,
                visible: false,
                titleFormatter: function() {
                    return lockIcon + '??????<br>??????';
                },
            },
            {
                title: '??????',
                field: prop2('FieldOrder'),
                sorter: 'number',
                editor: 'number',
                width: 75,
                editable: editableInput,
                validator: 'min:0',
                titleFormatter: function() {
                    return editIcon + '??????';
                },
            },
            {
                title: '????????????',
                field: prop2('FieldName'),
                sorter: 'string',
                editor: 'input',
                minWidth: 110,
                editable: editableInput,
                validator: 'required',
                titleFormatter: function() {
                    return editIcon + '??????<br>??????';
                },
            },
            {
                title: '??????',
                field: prop2('IsNullable'),
                sorter: 'boolean',
                editor: 'tickCross',
                width: 75,
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '??????';
                },
                formatter: function(cell, formatterParams) {
                    const enabled: boolean = cell.getValue();
                    switch (enabled) {
                        case true:
                            return Label('???', Color.???);
                        case false:
                            return Label('???', Color.???);
                        default:
                            return Label(cell.getValue(), Color.???);
                    }
                },
            },
            {
                title: '????????????',
                field: prop2('FieldLen'),
                sorter: 'number',
                editor: 'number',
                width: 85,
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '??????<br>??????';
                },
            },
            {
                title: '?????????',
                field: prop2('FieldDef'),
                sorter: 'string',
                width: 210,
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '?????????';
                },
                cellEditing: function(cell: Tabulator.CellComponent) {
                    const cellElement = cell.getElement();
                    const row = cell.getRow();
                    const rowElement = row.getElement();
                    const rowData = <TemplateFieldRowModel>row.getData();
                    const isMutiple = rowData.FieldCodeCnt == ColFieldSelectType.Mutiple ? true : false;
                    const isEditing = rowElement.getAttribute('data-isediting') == 'true' ? true : false;
                    if (rowData.FieldType.toUpperCase() == ColFieldType.????????????) {
                        const editor = cellElement.querySelector('.dropdown');
                        const changeData = IsNULLorEmpty(cellElement.getAttribute('data-changeData'))
                            ? ''
                            : cellElement.getAttribute('data-changeData');
                        const selectedValue = isEditing ? changeData.split(';') : rowData.FieldDef.split(';');
                        setTimeout(function() {
                            $(editor)
                                .dropdown({
                                    useLabels: !isMutiple /*????????????????????????*/,
                                    message: {
                                        count: '?????????{count}???',
                                    },
                                    onChange: function(value, text, $selectedItem) {
                                        const selectedValue = value.toString().split(','); //onchange????????????1,2,3??????
                                        cellElement.setAttribute('data-changeData', selectedValue.join(';'));
                                    },
                                })
                                .dropdown('set selected', selectedValue);
                        }, 100);
                    }
                },
                editor: function(cell, onRendered, success, cancel, editorParams) {
                    const cellElement = cell.getElement();
                    const row = cell.getRow();
                    const rowElement = row.getElement();
                    const originData = <TemplateFieldRowModel>(
                        (JSON.parse(rowElement.getAttribute('data-originData')) || {})
                    );
                    const isEditing = rowElement.getAttribute('data-isediting') == 'true' ? true : false;
                    const initData = cellElement.getAttribute('data-changeData') || originData.FieldDef;
                    switch (originData.FieldType.toUpperCase()) {
                        case ColFieldType.??????:
                            const textEditor: HTMLInputElement = document.createElement('input');
                            textEditor.type = 'text';
                            isEditing ? textEditor.setAttribute('value', initData) : false;

                            textEditor.addEventListener('change', function() {
                                cellElement.setAttribute('data-changeData', textEditor.value);
                                success(textEditor.value);
                            });
                            textEditor.addEventListener('blur', function() {
                                cellElement.setAttribute('data-changeData', textEditor.value);
                                success(textEditor.value);
                            });
                            return textEditor;
                        case ColFieldType.??????:
                            const numberEditor: HTMLInputElement = document.createElement('input');
                            numberEditor.type = 'number';
                            numberEditor.min = '0';
                            numberEditor.step = '1';
                            isEditing ? numberEditor.setAttribute('value', initData) : false;
                            numberEditor.addEventListener('change', function() {
                                cellElement.setAttribute('data-changeData', numberEditor.value);
                                success(numberEditor.value);
                            });
                            numberEditor.addEventListener('blur', function() {
                                cellElement.setAttribute('data-changeData', numberEditor.value);
                                success(numberEditor.value);
                            });
                            return numberEditor;
                        case ColFieldType.??????:
                            const dateEditor: HTMLInputElement = document.createElement('input');
                            dateEditor.type = 'datetime-local';
                            const d = new Date(new Date(initData).toString().split('GMT')[0] + ' UTC')
                                .toISOString()
                                .split('.')[0];
                            isEditing ? dateEditor.setAttribute('value', d) : false;
                            dateEditor.addEventListener('change', function() {
                                cellElement.setAttribute('data-changeData', dateEditor.value);
                                success(dateEditor.value);
                            });
                            dateEditor.addEventListener('blur', function() {
                                cellElement.setAttribute('data-changeData', dateEditor.value);
                                success(dateEditor.value);
                            });
                            return dateEditor;
                        case ColFieldType.????????????:
                            const selectEditor: HTMLElement = cellElement.querySelector('.dropdown');
                            const isMutiple = originData.FieldCodeCnt == ColFieldSelectType.Mutiple ? true : false;
                            $(selectEditor).dropdown({
                                useLabels: !isMutiple /*????????????????????????*/,
                                message: {
                                    count: '?????????{count}???',
                                },
                                // onChange: function(value, text, $selectedItem) {
                                //     const selectedValue = value.toString().split(','); //onchange????????????1,2,3??????
                                //     cellElement.setAttribute('data-changeData', selectedValue.join(';'));
                                //     success(selectedValue.join(';'));
                                // },
                            });

                            return selectEditor;
                    }
                },
                formatter: function(cell, formatterParams) {
                    const cellElement = cell.getElement();
                    const row = cell.getRow();
                    const rowElement = row.getElement();
                    const rowData = <TemplateFieldRowModel>row.getData();
                    cellElement.classList.add('tabulator-operation'); /*???????????????????????????????????????*/
                    //  const originData = <TemplateFieldRowModel>row.getData();
                    const isMutiple = rowData.FieldCodeCnt == ColFieldSelectType.Mutiple ? true : false;
                    const isEditing = rowElement.getAttribute('data-isediting') == 'true' ? true : false;
                    if (!isEditing) {
                        cellElement.setAttribute('data-changeData', rowData.FieldDef);
                    }
                    const initData = isEditing
                        ? cellElement.getAttribute('data-changeData') || ''
                        : IsNULL(cell.getValue())
                        ? StringEnum.Empty
                        : cell.getValue();

                    switch (rowData.FieldType.toUpperCase()) {
                        case ColFieldType.????????????:
                            const newSelect = CreateSelect(
                                IsNullorUndefined(rowData.CustomerCodeList)||rowData.CustomerCodeList.length===0?[]:
                                IsNullorUndefined(rowData.CustomerCodeList[0].SubCodeList)?[]: rowData.CustomerCodeList[0].SubCodeList,
                                isMutiple,
                                'FieldDef'
                            );
                            isEditing ? newSelect.classList.remove('disabled') : newSelect.classList.add('disabled');
                            setTimeout(function() {
                                const dropdown = cellElement.querySelector('.dropdown');
                                $(dropdown)
                                    .dropdown({
                                        useLabels: !isMutiple /*????????????????????????*/,
                                        message: {
                                            count: '?????????{count}???',
                                        },
                                    })
                                    .dropdown('set exactly', initData.toString().split(';'));
                            }, 100);
                            return newSelect;
                        case ColFieldType.??????:
                            return IsNULLorEmpty(initData) ? '' : dayjs(initData).format('YYYY/MM/DD HH:mm:ss');
                        case ColFieldType.??????:
                        case ColFieldType.??????:
                            return initData;
                    }
                },
            },
            {
                title: '????????????',
                field: prop2('IsSearch'),
                sorter: 'boolean',
                editor: 'tickCross',
                width: 80,
                editable: editableInput,
                titleFormatter: function() {
                    return editIcon + '??????<br>??????';
                },
                formatter: function(cell, formatterParams) {
                    const enabled: boolean = cell.getValue();
                    switch (enabled) {
                        case true:
                            return Label('???', Color.???);
                        case false:
                            return Label('???', Color.???);
                        default:
                            return Label(cell.getValue(), Color.???);
                    }
                },
            },
            {
                title: '????????????',
                field: prop2('FieldTypeName'),
                sorter: 'string',
                width: 80,
                titleFormatter: function() {
                    return lockIcon + '??????<br>??????';
                },
            },
            //{
            //    title: '????????????',
            //    field: 'FieldCodeId',
            //    sorter: 'string',
            //    width: 100,
            //    titleFormatter: function() {
            //        return lockIcon + '??????<br>??????';
            //    },
            //},
            {
                title: '????????????',
                field: prop2('FieldType'),
                sorter: 'string',
                hideInHtml: true,
                visible: false,
                titleFormatter: function() {
                    return lockIcon + '??????<br>??????';
                },
            },
            {
                title: '??????',
                field: prop2('Field'),
                hozAlign: 'center',
                width: 125,
                //width: 150, //fsFIELD
                frozen: true,
                titleFormatter: function() {
                    return editIcon + '??????';
                },
                formatter: function(cell, formatterParams) {
                    // cell.getElement().classList.add("tabulator-operation");/*?????????????????????????????????*/
                    const id: string = cell.getValue();
                    const editbtn = EditButton(id, message.Controller);
                    const deletebtn = DeleteButton(id, message.Controller);
                    const savebtn = `<button type="button" name="save" data-inverted="" data-tooltip="??????" data-position="bottom center" class="ui yellow circular icon button" data-Id="${id}">??????</button>`;
                    const cancelbtn = `<button type="button" name="cancel" data-inverted="" data-tooltip="??????" data-position="bottom center" class="ui circular icon button" data-Id="${id}">??????</button>`;
                    const groupbtn = `<div class="btngroup1">${editbtn}${deletebtn}</div><div class="btngroup2" style="display:none;">${savebtn}${cancelbtn}</div>`;
                    return groupbtn;
                },
                cellClick: function(e, cell) {
                    e.preventDefault();
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const cellElement: HTMLElement = cell.getElement();
                    const row = cell.getRow();
                    const rowElement: HTMLElement = row.getElement();
                    const rowData = <TemplateFieldRowModel>row.getData();
                    const CodeId: string = rowData.Field;
                    const cells: Tabulator.CellComponent[] = row.getCells();
                    const originData = <TemplateFieldRowModel>(
                        (JSON.parse(rowElement.getAttribute('data-originData')) || {})
                    );
                    const btngroup1 = <HTMLDivElement>cellElement.querySelector('.btngroup1');
                    const btngroup2 = <HTMLDivElement>cellElement.querySelector('.btngroup2');
                    const editorDropdown = rowElement
                        .querySelector('div[tabulator-field="FieldDef"]')
                        .querySelector('.dropdown');
                    const $editorDropdown = $(editorDropdown);
                    switch (true) {
                        /*Task:?????????????????? */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            rowElement.setAttribute('data-isediting', 'true');
                            btngroup1.style.display = 'none';
                            btngroup2.style.display = 'block';
                            break;
                        /*Task:???????????????????????? */
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'cancel':
                            rowElement.setAttribute('data-isediting', 'false');
                            cellElement.removeAttribute('data-editing'); //TODO ??????
                            btngroup1.style.display = 'block';
                            btngroup2.style.display = 'none';
                            for (const CELL of cells) {
                                const CELLOldValue = CELL.getOldValue();
                                if (!IsNULL(CELLOldValue)) {
                                    CELL.setValue(originData[CELL.getField()], true);
                                }
                            }
                            setTimeout(function() {
                                if (rowData.FieldType == ColFieldType.????????????) {
                                    $editorDropdown
                                        .dropdown({
                                            useLabels:
                                                rowData.FieldCodeCnt == ColFieldSelectType.Mutiple ? false : true,
                                            message: {
                                                count: '?????????{count}???',
                                            },
                                        })
                                        .dropdown('set selected', rowData.FieldDef.split(';'));
                                }
                            }, 100);

                            break;
                        /*Task:?????????????????? */
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'save':
                            if (
                                [ColFieldType.??????, ColFieldType.??????].indexOf(<ColFieldType>rowData.FieldType) > -1 &&
                                Number(rowData.FieldLen) <= 0
                            ) {
                                ErrorMessage('????????????????????????0');
                            } else if (Number(rowData.FieldOrder) < 1) {
                                ErrorMessage('??????????????????0');
                            } else if (!rowData.IsNullable && (IsNULLorEmpty(rowData.FieldDef)||IsNullorUndefined(rowData.FieldDef))) {
                                ErrorMessage('?????????????????????"???????????????",????????????????????????!'); 
                            } else {
                                route
                                    .EditField({
                                        fnTEMP_ID: templateId,
                                        FieldCodeId: CodeId,
                                        FieldCodeCtrl: '',
                                        FieldCodeCnt: 0,
                                        IsMultiline: true,
                                        IsNullable: <boolean>rowData.IsNullable,
                                        IsSearch: <boolean>rowData.IsSearch,
                                        FieldName: rowData.FieldName,
                                        Field: rowData.Field,
                                        FieldType: rowData.FieldType,
                                        FieldWidth: Number(rowData.FieldLen),
                                        FieldLen: Number(rowData.FieldLen),
                                        FieldDesc: rowData.FieldDesc,
                                        FieldOrder: Number(rowData.FieldOrder),
                                        FieldDef:
                                            rowData.FieldType == ColFieldType.????????????
                                                ? $editorDropdown
                                                      .dropdown('get value')
                                                      .toString()
                                                      .replace(/,/g, ';')
                                                : row.getCell('FieldDef').getValue(),
                                    })
                                    .then(res => {
                                        const Records = <EditTempFieldModel>res.Records;
                                        if (Records.FieldType == ColFieldType.????????????) {
                                            $editorDropdown.addClass('disabled');
                                        }
                                        if (res.IsSuccess) {
                                            cogtable.ReactivityUpdate(
                                                Records.Field,
                                                Object.assign(<templateField>{}, res.Data)
                                            );
                                            SuccessMessage(res.Message);
                                            btngroup1.style.display = 'block';
                                            btngroup2.style.display = 'none';
                                            rowElement.setAttribute('data-isediting', 'false');
                                        } else {
                                            ErrorMessage(res.Message);
                                        }
                                    })
                                    .catch(error => {
                                        Logger.error(`????????????????????????,api:${route.api.EditField},??????:${error}`);
                                    });
                            }
                            break;
                        /*Task:?????????????????? */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ModalTask('#ConfirmDeleteCodeModal', true, {
                                closable: false,
                                onApprove: function() {
                                    route
                                        .DeleteField({ id: templateId, field: CodeId })
                                        .then(res => {
                                            if (res.IsSuccess) {
                                                SuccessMessage(res.Message);
                                                cogtable.RemoveRow(CodeId);
                                            } else {
                                                ErrorMessage(res.Message);
                                                cogtable.GetRow(CodeId).style.border = '1px solid red';
                                            }
                                            $(tableLastSelectRow.getElement())
                                                .find("button[name='cog']")
                                                .trigger('click');
                                        })
                                        .catch(error => {
                                            Logger.error(
                                                `??????????????????????????????,api:${route.api.DeleteField},??????:${error}`
                                            );
                                        });
                                },
                                onDeny: function() {
                                    $(tableLastSelectRow.getElement())
                                        .find("button[name='cog']")
                                        .trigger('click');
                                },
                            });
                            break;
                        default:
                            break;
                    }
                },
            },
        ],
    });
};

/**
 * Task2:??????????????????(NVARCHAR,INTERGER,CODE,DATETIME...)???????????????????????????
 * @param dropdownId ????????????????????????Id
 * @param tempId ??????ID
 * @param changeviewId ??????"?????????????????????"???DIV ID
 */
const ChangeViewByType = (dropdownId: string, tempId: number, changeviewId: string) => {
    $(dropdownId).dropdown({
        onChange: function(value, text, $selectedItem) {
            route
                .GetFieldView(tempId, value)
                .then(view => {
                    $(ChooseTypeFormId)
                        .empty()
                        .append(view);
                })
                .catch(error => {
                    Logger.error('????????????????????????????????????,??????:', error);
                    ErrorMessage('????????????????????????????????????');
                })
                .then(() => {
                    if ((value = ColFieldType.????????????)) {
                        $('#fnCODE_CNT').dropdown();
                        const $FieldCodeIdDropdown = GetDropdown(ChooseTypeFormId, 'FieldCodeId');

                        const codeList =
                            $FieldCodeIdDropdown.attr('data-codelist') == undefined
                                ? '{}'
                                : $FieldCodeIdDropdown.attr('data-codelist');
                        const dropdownData = <Array<MainCodeListModel>>JSON.parse(codeList);
                        GetDropdown(ChooseTypeFormId, 'FieldDef').dropdown();
                        //?????????????????????,?????????????????????
                        $FieldCodeIdDropdown.dropdown({
                            onChange: function(value, text, $selectedItem) {
                                const itemlist = dropdownData.filter(item => item.MainCodeId == value)[0];
                                const newSelect = CreateSelect(itemlist.SubCodeList, false, 'FieldDef');
                                GetSelect(ChooseTypeFormId, 'FieldDef').html(newSelect.innerHTML);
                                GetDropdown(ChooseTypeFormId, 'FieldDef').dropdown('refresh');
                            },
                        });
                        //????????????????????????,????????????????????? //TODO 0410 NOtWOKING
                        GetDropdown(ChooseTypeFormId, 'fnCODE_CNT').dropdown({
                            onChange: function(value, text, $selectedItem) {
                                //TODO ?????????????????????
                                const FieldDefDropdown = GetDropdown(ChooseTypeFormId, 'FieldDef');
                                const isMutiple =
                                    value.toString() == ColFieldSelectType.Mutiple.toString() ? true : false;
                                isMutiple
                                    ? $(FieldDefDropdown).attr('mutiple', 'mutiple')
                                    : $(FieldDefDropdown).removeAttr('mutiple');
                                setTimeout(function() {
                                    $(FieldDefDropdown).dropdown();
                                }, 100);
                            },
                        });
                    }
                });
        },
    });
};
/**
 *  Task3:????????????
 * @param changeviewId ??????"?????????????????????"???DIV ID
 * @param templateId ??????Id
 */
const AddNewField = (changeviewId: string, templateId: number) => {
    const $Form = $(ChooseTypeFormId);
    $Form.submit(function(event) {
        event.preventDefault();
        const $Form = $(this);
        const type: 'NVARCHAR' | 'INTEGER' | 'DATETIME' | 'CODE' | string = <string>$(this)
            .find("button[name='CreateField']")
            .attr('data-type');
        //  const $view = $(changeviewId);
        const getparam = (
            type: 'NVARCHAR' | 'INTEGER' | 'DATETIME' | 'CODE' | string,
            templateId: number,
            view: JQuery<HTMLElement>
        ): EditTempFieldModel => {
            switch (type) {
                case ColFieldType.??????:
                    return {
                        fnTEMP_ID: templateId,
                        FieldName: <string>view.find("input[name='FieldName']").val(),
                        Field: '',
                        FieldType: type,
                        FieldDesc: <string>view.find("input[name='FieldDesc']").val(),
                        FieldOrder: Number(view.find("input[name='FieldOrder']").val()),
                        FieldLen: <number>view.find("input[name='FieldLen']").val(),
                        FieldDef: <string>view.find("input[name='FieldDef']").val(),
                        FieldWidth: Number(view.find("input[name='FieldLen']").val()),
                        IsMultiline: view.find(".checkbox[name='ismutiple']").checkbox('is checked') ? true : false,
                        IsNullable: view.find(".checkbox[name='isnullable']").checkbox('is checked') ? true : false,
                        IsSearch: view.find(".checkbox[name='issearch']").checkbox('is checked') ? true : false,
                        FieldCodeId: '', //??????????????????= CODE, ??????????????????????????????[fsCODE_ID]
                        FieldCodeCtrl: '',
                        FieldCodeCnt: 0,
                    };
                case ColFieldType.??????:
                    return {
                        fnTEMP_ID: templateId,
                        FieldName: <string>view.find("input[name='FieldName']").val(),
                        Field: '',
                        FieldType: type,
                        FieldDesc: <string>view.find("input[name='FieldDesc']").val(),
                        FieldOrder: Number(view.find("input[name='FieldOrder']").val()),
                        FieldLen: <number>view.find("input[name='FieldLen']").val(),
                        FieldDef: <string>view.find("input[name='FieldDef']").val(),
                        FieldWidth: Number(view.find("input[name='FieldLen']").val()),
                        IsMultiline: false,
                        IsNullable: view.find(".checkbox[name='isnullable']").checkbox('is checked') ? true : false,
                        IsSearch: false,
                        FieldCodeId: '',
                        FieldCodeCtrl: '',
                        FieldCodeCnt: 0,
                    };
                case ColFieldType.??????:
                    return {
                        fnTEMP_ID: templateId,
                        FieldName: <string>view.find("input[name='FieldName']").val(),
                        Field: '',
                        FieldType: type,
                        FieldDesc: <string>view.find("input[name='FieldDesc']").val(),
                        FieldOrder: Number(view.find("input[name='FieldOrder']").val()),
                        FieldLen: -1,
                        FieldDef: <string>view.find("input[name='FieldDef']").val(),
                        FieldWidth: -1,
                        IsMultiline: false,
                        IsNullable: false,
                        IsSearch: false,
                        FieldCodeId: '',
                        FieldCodeCtrl: '',
                        FieldCodeCnt: 0,
                    };
                case ColFieldType.????????????:
                    return {
                        fnTEMP_ID: templateId,
                        FieldName: <string>view.find("input[name='FieldName']").val(),
                        Field: '',
                        FieldType: type,
                        FieldDesc: <string>view.find("input[name='FieldDesc']").val(),
                        FieldOrder: Number(view.find("input[name='FieldOrder']").val()),
                        FieldLen: -1,
                        FieldDef: <string>view
                            .find("select[name='FieldDef']")
                            .closest('.dropdown')
                            .dropdown('get value'),
                        FieldWidth: -1,
                        IsMultiline: false,
                        IsNullable: view.find(".checkbox[name='isnullable']").checkbox('is checked') ? true : false,
                        IsSearch: false,
                        FieldCodeId: <string>view
                            .find("select[name='FieldCodeId']")
                            .closest('.dropdown')
                            .dropdown('get value'),
                        FieldCodeCtrl: '',
                        FieldCodeCnt: Number(view.find('#fnCODE_CNT').dropdown('get value')), //????????????
                    };
            }
        };
        const validRule =
            type == ColFieldType.??????
                ? valid.AddStringField
                : type == ColFieldType.??????
                ? valid.AddNumberField
                : type == ColFieldType.??????
                ? valid.AddDateField
                : type == ColFieldType.????????????
                ? valid.AddCodeField
                : {};
        const params = getparam(type, templateId, $Form);
        const skipnames: string[] = [];
        const copyValid = Object.assign({}, validRule);
        /*??????????????????,??????????????????????????????,???????????????????????????*/
        const IsRequiredWithDefaultValue =
            type == ColFieldType.?????? ? true : params.IsNullable ? true : IsNULLorEmpty(params.FieldDef) ? false : true;
        if (!IsRequiredWithDefaultValue) {
            ErrorMessage('?????????????????????"???????????????",????????????????????????!');
        }
        //TODO ?????????????????? ????????????????????????
        $Form.find(".ui.checkbox[name='isnullable']").checkbox({
            onChecked: function(this: HTMLInputElement) {
                const $checkbox = $(this).parent('.checkbox');
                const $FieldDefField = $('#FieldDef').parent('.field');
                $FieldDefField.removeClass('error');
                $FieldDefField.find('.prompt.label').remove();
                delete copyValid['FieldDef'];
            },
            onUnchecked: function(this: HTMLInputElement) {
                const $checkbox = $(this).parent('.checkbox');
                const $FieldDefField = $('#FieldDef').parent('.field');
                copyValid['FieldDef'] = {
                    identifier: 'FieldDef',
                    rules: [
                        {
                            type: 'empty',
                            prompt: '?????????????????????"???????????????",???????????????{name}!',
                        },
                    ],
                };
            },
        });

        const IsFieldValid: boolean = CheckForm(ChooseTypeFormId, copyValid);
        if (IsFieldValid && IsRequiredWithDefaultValue) {
            route
                .AddField(params)
                .then(res => {
                    const Record = <EditTempFieldModel>res.Records;
                    const UpdateRowData: TemplateFieldModel = {
                        Field: Record.Field,
                        FieldName: Record.FieldName,
                        FieldType: Record.FieldType,
                        FieldDesc: Record.FieldDesc,
                        FieldLen: Record.FieldLen,
                        FieldOrder: Record.FieldOrder,
                        FieldWidth: Record.FieldWidth,
                        IsMultiline: Record.IsMultiline,
                        IsNullable: Record.IsNullable,
                        FieldDef: Record.FieldDef,
                        FieldCodeId: Record.FieldCodeId,
                        FieldCodeCtrl: Record.FieldCodeCtrl,
                        FieldCodeCnt: Record.FieldCodeCnt,
                        IsSearch: Record.IsSearch,
                    };
                    if (res.IsSuccess) {
                        SuccessMessage(res.Message);
                        cogtable.AddRow(res.Data);
                        $(ChooseTypeFormId).trigger('reset');
                    } else {
                        ErrorMessage(res.Message);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.AddField, '????????????', error, true);
                });
        }
    });
};
