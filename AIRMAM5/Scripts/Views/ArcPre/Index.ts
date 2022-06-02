import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { CheckForm, AddDynamicNullable } from '../../Models/Function/Form';
import { Controller } from '../../Models/Enum/Controller';
import {  DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Action } from '../../Models/Enum/Action';
import { IResponse } from '../../Models/Interface/Shared/IResponse';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { EditModalId, CreateModalId, CreateFormId, SearchFormId, EditFormId, DeleteModalId } from '../../Models/Const/Const.';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { IArcPreController, ArcPreController } from '../../Models/Controller/ArePreController';
import { ChineseMediaType, MediaType } from '../../Models/Enum/MediaType';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { getIconByMediaType } from '../../Models/Function/Icon';
import { FormValidField } from '../../Models/Const/FormValid';
import { ArcPreMessageSetting } from '../../Models/MessageSetting';
import { EditButton, DetailButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { setCalendar } from '../../Models/Function/Date';
import { ArePreSearchModel } from '../../Models/Interface/ArePre/ArePreSearchModel';
import { ArePreMainModel } from '../../Models/Interface/ArePre/ArePreMainModel';
import { ArePreDynamicField } from '../../Models/Interface/ArePre/ArePreDynamicField';
import { ArePreEditInputModel } from '../../Models/Interface/ArePre/ArePreEditInputModel';
import { CreateArePreDynamicField } from '../../Models/Interface/ArePre/CreateArePreDynamicField';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
import { ColFieldType } from '../../Models/Enum/ColTypeEnum';
import { GetDropdown } from '../../Models/Function/Element';
/*=====================宣告變數====================================*/
// const api = API.ArcPre;
var table: ItabulatorService;
var route: IArcPreController = new ArcPreController();
const valid = FormValidField.ArcPre;
const message = ArcPreMessageSetting;
const SubCreateFormId = '#SubCreateForm';
const prop = (key: keyof ArePreMainModel): string => {
    return route.GetProperty<ArePreMainModel>(key);
};
/*======================================================*/
/*頁面載入查詢*/
Search({ fsNAME: '', fsTYPE: '' });
/*表單查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const name: string = <string>$('#TempleteName').val();
        const type: string = <string>$('#TempleteType')
            .parent('.dropdown')
            .dropdown('get value');
        Search({ fsNAME: name, fsTYPE: type == '*' ? '' : type });
    }
});
/**表單清空 */
$("button[name='reset']").click(function() {
    $('#TempleteName').val('');
    GetDropdown(SearchFormId, 'Type').dropdown('set selected', StringEnum.All);
});

/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fnPRE_ID'), type: Filter.Like, value: word },
        { field: prop('fsNAME'), type: Filter.Like, value: word },
        { field: prop('fsTITLE'), type: Filter.Like, value: word },
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
    if (!IsNULLorEmpty(TypeWord)) {
        filter.push({
            field: 'fsTYPE',
            type: Filter.Equal,
            value: TypeWord,
        });
    }
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: ArePreSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        addRowPos: 'top',
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('fnPRE_ID'),
        columns: [
            { title: '編號', field: prop('fnPRE_ID'), width: 90, sorter: 'number' },
            { title: '預編名稱', field: prop('fsNAME'), sorter: 'string', width: 330 },
            {
                title: '類型',
                field: prop('fsTYPE'),
                width: 95,
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    const type = cell.getValue();
                    const icon = getIconByMediaType(<MediaType>type);
                    return icon + getEnumKeyByEnumValue(ChineseMediaType, type);
                },
            },
            { title: '標題', field: prop('fsTITLE'), sorter: 'string', minWidth: 440 },
            {
                title: '操作',
                field: prop('fnPRE_ID'),
                hozAlign: 'center',
                width: 165,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: number = parseInt(cell.getValue());
                    const editbtn = EditButton(id, message.Controller);
                    const detailbtn = DetailButton(id, message.Controller);
                    const deletebtn = DeleteButton(id, message.Controller);
                    const btngroups: string = editbtn + detailbtn + deletebtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const preId: number = Number(cell.getValue());
                    const rowData = <ArePreMainModel>cell.getRow().getData();
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    switch (true) {
                        /**編輯 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<{ fnPREID: number }>(EditModalId, route.api.ShowEdit, { fnPREID: preId }).then(
                                success => {
                                    if (success) {
                                        ModalTask(EditModalId, true, {
                                            onShow: function() {
                                                $(EditModalId)
                                                .find('.dropdown:not(.x-hashtag)')
                                                .dropdown();
                                                $(EditModalId).find('.ui.dropdown.x-hashtag')
                                                .dropdown({
                                                    allowAdditions: true,
                                                    keys: {
                                                        delimiter: 54
                                                    },
                                                    onChange:function(value, text, $selectedItem){
                                                        const newValue=value.replace(/,/g,"^");
                                                        $("input[name='HashTag']").val(newValue);
                                                    }
                                                });
                                                setCalendar(`${EditModalId} .calendar`, 'date');
                                            },
                                            onApprove: function() {
                                                /**
                                                 * Notice:動態驗證條件,注意深淺拷貝問題
                                                 *深拷貝初始的驗證條件,因為編輯的驗證會隨選單進行初始化,所以要由此去進行擴增
                                                 */
                                                const copyValid = Object.assign({}, valid.Edit);
                                                const validObject = AddDynamicNullable(EditFormId, copyValid);
                                                const IsFormValid = CheckForm(EditFormId, Object.assign(validObject,valid.Edit));
                                                if (IsFormValid) {
                                                    const _aryVal = (() => {
                                                        const filedArray: Array<ArePreDynamicField> = [];
                                                        const fieldListValue = <string>$(EditModalId)
                                                            .find('#fieldList')
                                                            .val();
                                                        if (!IsNULLorEmpty(fieldListValue)) {
                                                            const aryFed: Array<string> = fieldListValue.split('^');
                                                            Logger.log(`動態欄位參數=${aryFed}`);
                                                            for (let aryItem of aryFed) {
                                                                const el = $(EditModalId).find('#' + aryItem);
                                                                const _fieldvalue =
                                                                    el.attr('data-fieldtype').toUpperCase() ==
                                                                    ColFieldType.自訂代碼
                                                                        ? Array.isArray(el.dropdown('get value'))?el.dropdown('get value').join(';'):el.dropdown('get value')
                                                                        : <string>el.val() || '';/*NOTICE:多選只能用字串join分號回傳給後端*/
                                                                filedArray.push({
                                                                    fnTEMP_ID: rowData.fnTEMP_ID,
                                                                    Field: el.attr('data-field'),
                                                                    FieldName: el.attr('data-fieldname'),
                                                                    FieldValue: _fieldvalue,
                                                                });
                                                            }
                                                        }
                                                        return filedArray;
                                                    })();
                                                    route
                                                        .EditArcPre({
                                                            fnPRE_ID: preId,
                                                            fsNAME: <string>$(EditModalId)
                                                                .find('input[name="fsNAME"]')
                                                                .val(),
                                                            fsTYPE: <string>$(EditModalId)
                                                                .find('input[name="fsTYPE"]')
                                                                .val(),
                                                            fsTYPE_NAME: <string>$(EditModalId)
                                                                .find('input[name="fsTYPE_NAME"]')
                                                                .val(),
                                                            fnTEMP_ID: rowData.fnTEMP_ID,
                                                            fsTEMP_NAME: <string>$(EditModalId)
                                                                .find('input[name="fsTEMP_NAME"]')
                                                                .val(),
                                                            fsTITLE: <string>$(EditModalId)
                                                                .find('input[name="fsTITLE"]')
                                                                .val(),
                                                            fsDESCRIPTION: <string>$(EditModalId)
                                                                .find('textarea[name="fsDESCRIPTION"]')
                                                                .val(),
                                                            fsHashTag: <string>$(EditModalId)
                                                                .find("input[name='HashTag']")
                                                                .val()||'',
                                                            HashTag: (<string>$(EditModalId)
                                                                .find("input[name='HashTag']")
                                                                .val()||'').trim().split('^'), //自訂標籤陣列
                                                            ArcPreAttributes: JSON.parse(JSON.stringify(_aryVal)),
                                                        })
                                                        .then(res => {
                                                            Logger.res(route.api.Edit, '編輯預編詮釋', res);
                                                            if (res.IsSuccess) {
                                                                const data = <ArePreEditInputModel>res.Data;
                                                                const updatedata: ArePreMainModel = {
                                                                    fnPRE_ID: data.fnPRE_ID,
                                                                    fsNAME: data.fsNAME,
                                                                    fsTYPE: data.fsTYPE,
                                                                    fsTYPE_NAME: data.fsTEMP_NAME,
                                                                    fnTEMP_ID: data.fnTEMP_ID,
                                                                    fsTEMP_NAME: data.fsTEMP_NAME,
                                                                    fsTITLE: data.fsTITLE,
                                                                    fsDESCRIPTION: data.fsDESCRIPTION,
                                                                    fsHashTag: data.fsHashTag,
                                                                    HashTag: [], //自訂標籤陣列
                                                                };
                                                                $(EditFormId).trigger('reset');
                                                                table.ReactivityUpdate(preId, updatedata);
                                                                $(EditModalId).modal('hide');
                                                            }
                                                        })
                                                        .catch(error => {
                                                            Logger.viewres(route.api.Edit, '編輯預編', error, true);
                                                        });
                                                }
                                                return false;
                                            },
                                        });
                                    } else {
                                        ErrorMessage(`系統發生錯誤，無法開啟預編詮釋編輯視窗`);
                                    }
                                }
                            );
                            break;
                        /**詳細 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.ArcPre, Action.ShowDetails, { fnPREID: preId }, { dropdown: true });
                            break;
                        /**刪除 */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal(DeleteModalId,route.api.ShowDelete,{ fnpreid: preId }).then(success=>{
                                if(success){
                                    ModalTask(DeleteModalId,true,{
                                        closable:false,
                                        onShow:function(){
                                            $(DeleteModalId).find('.dropdown').dropdown('refresh');
                                        },
                                        onApprove:function(){
                                            route
                                            .DeleteArcPre(preId)
                                            .then(res => {
                                                if (res.IsSuccess) {
                                                    SuccessMessage(res.Message);
                                                    table.RemoveRow(preId);
                                                } else {
                                                    ErrorMessage(res.Message);
                                                }
                                            })
                                            .catch(error => {
                                                Logger.viewres(route.api.Delete, '刪除預編', error, true);
                                            });
                                        }
                                    });
                                }else{
                                    Logger.viewres(route.api.ShowDelete, '刪除預編頁面','', true);                                    
                                }

                            }).catch((error)=>{
                                Logger.viewres(route.api.ShowDelete, '刪除預編頁面',error, true);            
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

/**切換樣板類型:影音圖文主題 */
$("input:radio[name='templetetype']").change(function() {
    const mediaType = <MediaType>$(this).val();
    createTemplete(mediaType);
});

/**點擊:新增預編詮釋樣板標籤 */
$('#templeteLayout').on('click', "label[for='templetelayout']", function() {
    $(this)
        .siblings("input[type='radio']")
        .prop('checked', true);
});
/*產生預編詮釋樣板 */
const createTemplete = (type: MediaType) => {
    route.GetArcTempleteByType(type).then(list => {
        const fragment: DocumentFragment = document.createDocumentFragment();
        for (let i = 0; i < list.length; i++) {
            const item: SelectListItem = list[i];
            const fakediv: HTMLDivElement = document.createElement('div');
            const defaultChecked: string = i == 0 ? 'checked' : StringEnum.Empty;
            fakediv.className = 'four wide column';
            fakediv.innerHTML = `<div class="ui inverted segment">
                               <div class="ui radio checkbox">
                                   <input type="radio" value="${item.Value}" name="templetelayout" required ${defaultChecked}>
                                   <label for="templetelayout">${item.Text}</label>
                               </div>
                           </div>`;
            fragment.appendChild(fakediv);
        }
        $('#templeteLayout')
            .empty()
            .append(fragment);
    });
};
/**開啟新增預編詮釋資料燈箱 */
ModalTask(CreateModalId, false, {
    closable: false,
    onShow: function() {
        $("input:radio[name='templetetype']:first").attr('checked', 'true');
        createTemplete(MediaType.SUBJECT);
    },
    onApprove: function() {
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            const SubCreateModalId = '#SubCreateModal';
            const typeId: string = <string>$("input:radio[name='templetetype']:checked").val();
            const templeteId: number = Number($("input:radio[name='templetelayout']:checked").val());
            ShowModal<{ type: string; tempid: number }>(SubCreateModalId, route.api.ShowSubCreate, {
                type: typeId,
                tempid: templeteId,
            }).then(success => {
                if (success) {
                    const $SUBFORM = $(SubCreateFormId);
                    ModalTask(SubCreateModalId, true, {
                        closable: false,
                        onShow: function() {
                                $(SubCreateModalId)
                                .find('.dropdown:not(.x-hashtag)')
                                .dropdown();
                                $(SubCreateModalId).find('.ui.dropdown.x-hashtag')
                                .dropdown({
                                    allowAdditions: true,
                                    keys: {
                                        delimiter: 54
                                    },
                                    onChange:function(value, text, $selectedItem){
                                        const newValue=value.replace(/,/g,"^");
                                        $("input[name='HashTag']").val(newValue);
                                    }
                                });
                            setCalendar(`${SubCreateModalId} .calendar`, 'date');
                            $(CreateFormId).trigger('reset'); /*將前一個Model設為預設,否則樣板會與類型衝突*/
                        },
                        onApprove: function() {
                            /**
                             * Notice:動態驗證條件,注意深淺拷貝問題
                             *深拷貝初始的驗證條件,因為新增的驗證會隨選單進行初始化,所以要由此去進行擴增
                             */
                            const copyValid = Object.assign({}, valid.SubCreate);
                            const validObject = AddDynamicNullable(SubCreateFormId, copyValid);
                            const isFormValid = CheckForm(SubCreateFormId, Object.assign(validObject,valid.SubCreate));
                            if (isFormValid) {
                                const getArcPreAttribute = (() => {
                                    let ArcPreAttributes: Array<CreateArePreDynamicField> = [];
                                    const div = $SUBFORM.find("div[name='ArcPreAttributes']");
                                    div.find('input,textarea,select').each(function(index) {
                                        const input = $(this);
                                        const orginAttr = input.attr('data-ArcPreAttribute');
                                        if (typeof orginAttr !== 'undefined' && orginAttr !== undefined) {
                                            const inputArcPreAttribute = orginAttr.length > 0 ? orginAttr : '{}';
                                            let initArcPreAttribute: CreateArePreDynamicField = JSON.parse(
                                                inputArcPreAttribute
                                            );
                                            initArcPreAttribute.FieldValue = IsNULLorEmpty(input.val())
                                                ? ''
                                                : input.val().toString().replace(/,/g,';');
                                            ArcPreAttributes.push(initArcPreAttribute);
                                        }
                                    });
                                    return ArcPreAttributes;
                                })();
                                route
                                    .CreateArcPre(
                                        {
                                            fnPRE_ID: 0,
                                            fsNAME: <string>$SUBFORM.find("input[name='fsNAME']").val(),
                                            fsTYPE: typeId,
                                            fsTYPE_NAME: <string>$SUBFORM.find("input[name='fsTYPE_NAME']").val(),
                                            fnTEMP_ID: templeteId,
                                            fsTEMP_NAME: <string>$SUBFORM.find("input[name='fsTEMP_NAME']").val(),
                                            fsTITLE: <string>$SUBFORM.find("input[name='fsTITLE']").val(),
                                            fsDESCRIPTION: <string>(
                                                $SUBFORM.find("textarea[name='fsDESCRIPTION']").val()
                                            ),
                                            fsHashTag: <string>(
                                                $SUBFORM.find("input[name='HashTag']").val()||''
                                            ),
                                            HashTag: (<string>(
                                                $SUBFORM.find("input[name='HashTag']").val()||''
                                            )).trim().split('#'), //自訂標籤陣列
                                            ArcPreAttributes: getArcPreAttribute,
                                        },
                                        $SUBFORM.serialize()
                                    )
                                    .then(res => {
                                        if (res.IsSuccess) {
                                            SuccessMessage(res.Message);
                                            table.AddRow(res.Data);
                                            $(SubCreateFormId).trigger('reset');
                                            $(SubCreateModalId).modal('hide');
                                        } else {
                                            ErrorMessage(res.Message);
                                        }
                                    });
                            }
                            return false;
                        },
                    });
                } else {
                    ErrorMessage('系統發生錯誤，無法顯示新增預編詮釋資料燈箱');
                }
            });
        }
        return false;
    },
    onDeny: function() {
        $(CreateFormId).trigger('reset');
    },
}).modal('attach events', "button[name='create']");
