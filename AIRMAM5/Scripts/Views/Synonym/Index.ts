import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { SynonymMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { SearchFormId, CreateFormId, EditModalId, DeleteModalId, CreateModalId } from '../../Models/Const/Const.';
import { CheckForm } from '../../Models/Function/Form';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { EditButton, DetailButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { SynonymSearchModel } from '../../Models/Interface/Synonym/SynonymSearchModel';
import { Filter } from '../../Models/Enum/Filter';
import { ISynonymController, SynonymController } from '../../Models/Controller/SynonymController';
import { Logger } from '../../Models/Class/LoggerService';
import { SynonymEditResModel } from '../../Models/Interface/Synonym/SynonymEditResModel';
import { SynonymListModel } from '../../Models/Interface/Synonym/SynonymListModel';
import { GetDropdown } from '../../Models/Function/Element';
import { StringEnum } from '../../Models/Enum/StringEnum';
/*===============================================================*/
/*宣告變數*/
var table: ItabulatorService;
const message = SynonymMessageSetting;
const valid = FormValidField.Synonym;
const deletebtn: string = `<button type="button" name="deleteSynonym" class="ui red button"><i class="delete icon"></i>刪除</button>`;
var CreateSynonymArray = []; /*同義詞新增陣列 */
var EditSynonymArray = []; /*同義詞修改陣列 */
var route: ISynonymController = new SynonymController();
/**回傳Modal性質*/
const prop = (key: keyof SynonymListModel): string => {
    return route.GetProperty<SynonymListModel>(key);
};
//=============================================================
/*頁面載入查詢*/
Search({
    type: '',
    word: '',
});
/*表單查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        Search({
            type: <string>$('#SynonymType').dropdown('get value'),
            word: <string>$('#SearchWord').val(),
        });
    }
});
/**表單清空 */
$("button[name='reset']").click(function() {
    GetDropdown(SearchFormId, 'SynonymType').dropdown('set selected', StringEnum.All);
    $('#SearchWord').val('');
});

/*重建同義詞*/
$('#RebuildBtn').click(function() {
    route
        .Rebuild()
        .then(res => {
            Logger.res(route.api.Rebuild, '重建同義詞', res, false);
            res.Message = IsNULLorEmpty(res.Message) ? '重建成功' : res.Message;
            res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
        })
        .catch(error => {
            Logger.viewres(route.api.Rebuild, '重建同義詞', error, true);
        });
});
/**列表篩選 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fnINDEX_ID'), type: Filter.Like, value: word },
        { field: prop('fsTEXT_LIST'), type: Filter.Like, value: word },
        { field: prop('fsTYPE'), type: Filter.Like, value: word },
        { field: prop('fsNOTE'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: SynonymSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        pagination: 'local',
        addRowPos: 'top',
        index: prop('fnINDEX_ID'),
        columns: [
            { title: 'ID', field: prop('fnINDEX_ID'), width: 90, sorter: 'number', visible: false },
            { title: '同義詞組', field: prop('fsTEXT_LIST'), sorter: 'string', minWidth: 500 },
            { title: '分類', field: prop('fsTYPE'), width: 120, sorter: 'string' },
            { title: '備註', field: prop('fsNOTE'), sorter: 'string', width: 200 },
            {
                title: '操作',
                field: prop('fnINDEX_ID'),
                hozAlign: 'left',
                width: 170,
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
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const indexId: number = parseInt(cell.getValue());
                    switch (true) {
                        /**編輯 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: indexId }).then(IsSuccess => {
                                if (IsSuccess) {
                                    ModalTask(EditModalId, true, {
                                        closable: false,
                                        onShow: function() {
                                            const orginSynonym: Array<string> = (<string>$('#TextList').val())
                                                .split(';')
                                                .filter(x => x != '');
                                            EditSynonymArray = orginSynonym;
                                            EditSynonymArray.forEach(x => {
                                                $('#EditShowSynonymList').append(
                                                    `<tr><td>${x}</td><td>${deletebtn}</td></tr>`
                                                );
                                            });
                                        },
                                        onApprove: function() {
                                            route
                                                .Edit({
                                                    fnINDEX_ID: indexId,
                                                    fsTEXT_LIST:
                                                        EditSynonymArray.join(';') + ';' /*因為最後一個也需要加分號*/,
                                                    fsNOTE: <string>$('#EditModal #fsNOTE').val(),
                                                    fsTYPE: <string>$('#EditModal #fsTYPE').dropdown('get value'),
                                                    OrigSynonyms: <string>$('#OrigSynonyms').val(),
                                                })
                                                .then(res => {
                                                    Logger.res(route.api.Edit, '編輯同義詞', res, true);
                                                    EditSynonymArray = [];
                                                    const Record = <SynonymEditResModel>res.Records;
                                                    table.ReactivityUpdate(Record.fnINDEX_ID, <SynonymListModel>{
                                                        fnINDEX_ID: Record.fnINDEX_ID,
                                                        fsTEXT_LIST: Record.fsTEXT_LIST,
                                                        fsTYPE: Record.fsTYPE,
                                                        fsNOTE: Record.fsNOTE,
                                                    });
                                                })
                                                .catch(error => {
                                                    Logger.viewres(route.api.Edit, '編輯同義詞', error, true);
                                                });
                                        },
                                    });
                                } else {
                                    Logger.viewres(route.api.ShowEdit, '同義詞編輯燈箱', '', true);
                                }
                            });
                            break;
                        /**檢視 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.Synonym, Action.ShowDetails, { id: indexId });
                            break;
                        /**刪除 */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal<IdModel>(DeleteModalId, route.api.ShowDelete, { id: indexId }).then(IsSuccess => {
                                if (IsSuccess) {
                                    ModalTask(DeleteModalId, true, {
                                        closable: false,
                                        onApprove: function() {
                                            route
                                                .Delete(indexId)
                                                .then(res => {
                                                    Logger.res(route.api.Delete, '同義詞刪除', res, true);
                                                    table.RemoveRow(indexId);
                                                })
                                                .catch(error => {
                                                    Logger.viewres(route.api.Delete, '同義詞刪除', error, true);
                                                });
                                        },
                                    });
                                } else {
                                    Logger.viewres(route.api.ShowDelete, '同義詞刪除燈箱', '', true);
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
/**新增同義詞組 */
ModalTask(CreateModalId,false,{
    closable:false,
    onApprove:function(){
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            $('#fsTEXT_LIST').val(CreateSynonymArray.join(';') + ';');
            route
                .Create({
                    fsTEXT_LIST: CreateSynonymArray.join(';') + ';' /*因為最後一個也需要加分號*/,
                    fsNOTE: <string>$('#fsNOTE').val(),
                    fsTYPE: <string>GetDropdown(CreateFormId,"fsTYPE").dropdown('get value'),
                    SynonymTypeList: [],
                })
                .then(res => {
                    table.AddRow(res.Data); 
                    $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
                    $("#ShowSynonymList > tr").remove();
                    $('#fsTEXT_LIST').val("");
                    CreateSynonymArray = [];
                    $(CreateModalId).modal('hide');
                    Logger.res(route.api.Create, '同義詞新增', res, true);
                })
                .catch(error => {
                    Logger.viewres(route.api.Create, '同義詞新增', error, true);
                });
        }
        return false;
    },
    onDeny:function(){
        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
        $("#ShowSynonymList > tr").remove();
        $('#fsTEXT_LIST').val("");
        CreateSynonymArray = [];
    }
})
.modal('attach events', "button[name='createSynonym']");


/**【新增同義詞Modal、編輯同義詞Modal】新增動作*/
$(document).on('click', "button[name='InsertSynonym']", function() {
    const SynonymInput = $(this).siblings("input[name='SynonymWord']");
    const SynonymWord = SynonymInput.val();
    const type: 'Create' | 'Edit' | '' =
        $(this).attr('data-type') == 'Create' ? 'Create' : $(this).attr('data-type') == 'Edit' ? 'Edit' : '';
    if (SynonymWord != '') {
        switch (type) {
            case 'Create':
                if (CreateSynonymArray.indexOf(SynonymWord) <= -1) {
                    CreateSynonymArray.push(SynonymWord);
                    $('#ShowSynonymList').append(`<tr><td>${SynonymWord}</td><td>${deletebtn}</td></tr>`);
                } else {
                    ErrorMessage('詞彙已存在');
                }
                $('#fsTEXT_LIST').val(CreateSynonymArray.join(';'));
                SynonymInput.val('');
                break;
            case 'Edit':
                if (EditSynonymArray.indexOf(SynonymWord) <= -1) {
                    EditSynonymArray.push(SynonymWord);
                    $('#EditShowSynonymList').append(`<tr><td>${SynonymWord}</td><td>${deletebtn}</td></tr>`);
                } else {
                    ErrorMessage('詞彙已存在');
                }
                $('#TextList').val(EditSynonymArray.join(';'));
                SynonymInput.val('');
                break;
        }
    } else {
        ErrorMessage('請輸入要新增的詞彙!');
    }
});

/**【新增同義詞Modal、編輯同義詞Modal】輸入聚焦時,keypress觸發 */
$(document).on('keypress', "input[name='SynonymWord']", function(event) {
    if (
        event.keyCode == 13 &&
        $(this)
            .val()
            .toString().length > 0
    ) {
        $(this)
            .siblings('button[name="InsertSynonym"]')
            .trigger('click');
    }
});

/**【新增同義詞Modal】刪除動作 */
$(CreateModalId).on('click', "button[name='deleteSynonym']", function() {
    const tr = $(this).closest('tr');
    tr.remove();
    CreateSynonymArray = CreateSynonymArray.filter(x => x != tr.find('td:eq(0)').text());
    console.log(CreateSynonymArray);
    $('#fsTEXT_LIST').val(CreateSynonymArray.join(';'));
});

/**【編輯同義詞Modal】刪除動作 */
$(document).on('click', `${EditModalId} button[name='deleteSynonym']`, function() {
    const tr = $(this).closest('tr');
    tr.remove();
    EditSynonymArray = EditSynonymArray.filter(x => x != tr.find('td:eq(0)').text());
    $('#TextList').val(EditSynonymArray.join(';'));
});
