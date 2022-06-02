import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { ModalTask, ShowModal } from '../../Models/Function/Modal';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { AjaxPost } from '../../Models/Function/Ajax';
import { GroupController, IGroupController } from '../../Models/Controller/GroupController';
import { SuccessMessage, ErrorMessage } from '../../Models/Function/Message';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { FormValidField } from '../../Models/Const/FormValid';
import { GroupMessageSetting } from '../../Models/MessageSetting';
import { CheckForm } from '../../Models/Function/Form';
import { GroupInputModel } from '../../Models/Interface/Group/GroupInputModel';
import { GroupDataModel } from '../../Models/Interface/Group/GroupDataModel';
import { GroupEditModel } from '../../Models/Interface/Group/GroupEditModel';
import { RoleFuncUpdateModel } from '../../Models/Interface/Group/RoleFuncUpdateModel';
import { EditButton, DeleteButton, FunctionButton } from '../../Models/Templete/ButtonTemp';
import { CreateModalId, EditModalId } from '../../Models/Const/Const.';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { UserListViewModel } from '../../Models/Interface/Group/UserListViewModel';
import { UI } from '../../Models/Templete/CompoentTemp';
import { Label } from '../../Models/Templete/LabelTemp';
import { Color } from '../../Models/Enum/ColorEnum';
import { GroupListModel } from '../../Models/Interface/Group/GroupListModel';
/*-----------------------
  宣告變數
------------------------*/
const CreateFormId: string = '#CreateForm';
const EditFormId = '#EditForm';
const message = GroupMessageSetting;
const valid = FormValidField.Group;
var table: ItabulatorService;
var route: IGroupController = new GroupController();
var allchecked: Array<string> = []; //紀錄勾選功能
/**回傳Modal性質*/
const prop = (key: keyof GroupListModel): string => {
    return route.GetProperty<GroupListModel>(key);
};
//==============================================================
/**列表篩選 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('RoleId'), type: Filter.Like, value: word },
        { field: prop('RoleName'), type: Filter.Like, value: word },
        { field: prop('Description'), type: Filter.Like, value: word },
        { field: prop('AccountCounts'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/*-----------------------
  流程
------------------------*/
/**新增角色群組 */
ModalTask(CreateModalId,false,{
    closable:false,
    onApprove:function(){
        const $createform = $(CreateFormId);
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            const param: GroupInputModel = {
                fsNAME: <string>$createform.find("input[name='fsNAME']").val(),
                fsDESCRIPTION: <string>$createform.find("input[name='fsDESCRIPTION']").val(),
            };
            route
                .Create(param)
                .then(res => {
                    const data: GroupDataModel = <GroupDataModel>res.Data;
                    if(res.IsSuccess){
                        SuccessMessage(res.Message);
                        table.AddRow({
                            RoleId: data.fsGROUP_ID,
                            RoleName: data.fsNAME,
                            Description: data.fsDESCRIPTION,
                            AccountCounts: data.tbmUSER_GROUP.length,
                        });
                        $(CreateModalId).modal('hide');
                        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
                    }else{
                        ErrorMessage(res.Message);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.Create, '新增角色群組', error, true);
                });
        } 
        return false;
    },
    onDeny:function(){
        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
    }
}).modal('attach events', "button[name='create']");


table = new tabulatorService(initSetting.TableId, {
    height: TabulatorSetting.height,
    layout: TabulatorSetting.layout,
    ajaxURL: route.api.Search,
    ajaxContentType: 'json',
    ajaxConfig: 'POST',
    index: prop('RoleId'),
    addRowPos: 'top',
    columns: [
        { title: '角色群組代號', field: prop('RoleId'), sorter: 'string', visible: false, download: false },
        { title: '角色群組名稱', field: prop('RoleName'), sorter: 'string', minWidth: 190 },
        { title: '系統群組描述', field: prop('Description'), sorter: 'string', minWidth: 310 },
        {
            title: '帳號數量',
            field: prop('AccountCounts'),
            sorter: 'string',
            width: 160,
            formatter: function(cell, formatterParams) {
                const count = cell.getValue();
                return !IsNULLorEmpty(count) && Number(count) > 0 ? `<a>${count}</a>` : count;
            },
            cellClick: function(e, cell) {
                const hasAccount = Number(cell.getValue()) > 0 ? true : false;
                const roleId = (<GroupEditModel>cell.getRow().getData()).RoleId;
                if (hasAccount) {
                    const UserListModalId = '#UserListModal';
                    route
                        .ShowAccount(roleId)
                        .then(res => {
                            const users = <Array<UserListViewModel>>res.Data;
                            let userContent = '';
                            for (let user of users) {
                                const accountStatus = user.fsIS_ACTIVE
                                    ? Label('<i class="user icon"></i>帳號啟用', Color.綠)
                                    : Label('<i class="user icon"></i>帳號停用', Color.紅);
                                const emailStatus = user.fsEmailConfirmed
                                    ? `<i class="green envelope icon"></i>已驗證`
                                    : `<i class="red envelope icon"></i>未驗證`;
                                const otherStr = IsNULLorEmpty(user.fsDESCRIPTION) ? '' : `(${user.fsDESCRIPTION})`;
                                userContent += ` <div class="ui raised card">
                                                 <div class="content x-user">
                                                   <div class="ui inverted header">${user.fsNAME}</div>
                                                   <div class="meta">${user.C_sDEPTNAME}</div>
                                                   <div class="meta">${user.fsLOGIN_ID}${otherStr}</div>
                                                   <div class="description">
                                                    ${user.fsEMAIL}
                                                   </div>
                                                 </div>
                                                 <div class="extra content"> ${accountStatus} ${emailStatus}</div>
                                               </div>`;
                            }
                            const okContent = `<div class="ui four stackable x-user cards">${userContent}</div>`;
                            const insertContent = res.IsSuccess
                                ? okContent
                                : UI.Error.ErrorSegment('無法取得使用者列表');
                            ModalTask(UserListModalId, true, {
                                closable: false,
                                onShow: function() {
                                    $(UserListModalId)
                                        .children('.content:not(.x-user)')
                                        .html(insertContent);
                                },
                            });
                        })
                        .catch(error => {
                            ModalTask(UserListModalId, true, {
                                closable: false,
                                onShow: function() {
                                    $(UserListModalId)
                                        .children('.content:not(.x-user)')
                                        .html(UI.Error.ErrorSegment('無法取得使用者列表'));
                                },
                            });
                        });
                }
            },
        },
        {
            title: '操作',
            field: prop('RoleId'),
            hozAlign: 'left',
            width: 165,
            formatter: function(cell, formatterParams) {
                cell.getElement().classList.add('tabulator-operation');
                const guid: string = cell.getValue();
                const editbtn = EditButton(guid, message.Controller);
                const funcbtn = FunctionButton(guid, message.Controller);
                const deletebtn = DeleteButton(guid, message.Controller);
                const btngroups: string = editbtn + funcbtn + deletebtn;
                return btngroups;
            },
            cellClick: function(e, cell) {
                const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                const roleId: string = cell.getValue();
                switch (true) {
                    /**Task:編輯 */
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                        ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: roleId }).then(succees => {
                            ModalTask(EditModalId, true, {
                                onApprove: function() {
                                    const editform = $(EditFormId);
                                    route
                                        .Edit({
                                            RoleId: roleId,
                                            RoleName: <string>editform.find("input[name='RoleName']").val(),
                                            Description: <string>editform.find("textarea[name='Description']").val(),
                                        })
                                        .then(res => {
                                            //const data: GroupDataModel = <GroupDataModel>res.Data;
                                            const data: GroupEditModel = <GroupEditModel>res.Data; //Tips_20200110:回傳資料model調整
                                            if (!res.IsSuccess) {
                                                ErrorMessage(res.Message);
                                            } else {
                                                SuccessMessage(res.Message);
                                                table.ReactivityUpdate(roleId, {
                                                    RoleName: data.RoleName, //data.fsNAME,
                                                    Description: data.Description, //data.fsDESCRIPTION,
                                                });
                                            }
                                        })
                                        .catch(error => {
                                            Logger.viewres(route.api.Edit, '編輯角色群組', error, true);
                                        });
                                },
                            });
                        });
                        break;

                    /**Task:刪除*/
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('delete icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                        ShowModal<IdModel>('#DeleteModal', route.api.ShowDelete, { id: roleId }).then(success => {
                            ModalTask('#DeleteModal', true, {
                                onApprove: function() {
                                    route
                                        .Delete(roleId)
                                        .then(res => {
                                            if (!res.IsSuccess) {
                                                ErrorMessage(res.Message);
                                            } else {
                                                SuccessMessage(res.Message);
                                                table.RemoveRow(roleId);
                                            }
                                        })
                                        .catch(error => {
                                            Logger.viewres(route.api.Delete, '刪除群組', error, true);
                                        });
                                },
                            });
                        });
                        break;

                    /**Task:功能列表 */
                    case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('bookmark icon') > -1:
                    case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'function':
                        ShowModal<IdModel>('#RoleFuncsModal', route.api.ShowFunction, { id: roleId }).then(success => {
                            FunctionCheckboxTask(); //功能項目選擇初始化
                            ModalTask('#RoleFuncsModal', true, {
                                onApprove: function() {
                                    let functionids: Array<{ FuncId: string }> = [];
                                    allchecked.forEach(item => {
                                        functionids.push({ FuncId: item });
                                    });
                                    AjaxPost<RoleFuncUpdateModel>(route.api.SaveFunction, {
                                        RoleId: roleId,
                                        FunctionIds: functionids,
                                    })
                                        .then(res => {
                                            !res.IsSuccess ? ErrorMessage(res.Message) : SuccessMessage(res.Message);
                                        })
                                        .catch(error => {
                                            Logger.viewres(route.api.Delete, '刪除角色群組', error, true);
                                        });
                                },
                                onDeny: function() {
                                    allchecked = [];
                                },
                            });
                        });
                        break;
                    default:
                        break;
                }
            },
        },
    ],
});

/**功能項目選擇初始化 */
const FunctionCheckboxTask = () => {
    allchecked = [];
    $('.list .master.checkbox').checkbox({
        onChecked: function() {
            let $childCheckbox = $(this)
                .closest('.checkbox')
                .siblings('.list')
                .find('.checkbox');
            $childCheckbox.checkbox('check');
        },
        onUnchecked: function() {
            let $childCheckbox = $(this)
                .closest('.checkbox')
                .siblings('.list')
                .find('.checkbox');
            $childCheckbox.checkbox('uncheck');
        },
    });
    $('.list .child.checkbox').checkbox({
        fireOnInit: true,
        onChange: function() {
            let $listGroup = $(this).closest('.list'),
                $parentCheckbox = $listGroup.closest('.item').children('.checkbox'),
                $checkbox = $listGroup.find('.checkbox'),
                allChecked = true,
                allUnchecked = true;
            $checkbox.each(function() {
                $(this).checkbox('is checked') ? (allUnchecked = false) : (allChecked = false);
            });
            if (allChecked) {
                $parentCheckbox.checkbox('set checked');
            } else if (allUnchecked) {
                $parentCheckbox.checkbox('set unchecked');
            } else {
                $parentCheckbox.checkbox('set indeterminate');
            }
        },
        onChecked: function(this) {
            const checkid: string = <string>$(this).val();
            if (allchecked.indexOf(checkid) <= -1) {
                allchecked.push(checkid);
            }
        },
        onUnchecked: function(this) {
            const checkid: string = <string>$(this).val();
            allchecked = allchecked.filter(item => item != checkid);
        },
    });
};
