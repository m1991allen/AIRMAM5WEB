import { tabulatorService } from '../../Models/Class/tabulatorService';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { ModalTask, ShowModal } from '../../Models/Function/Modal';
import { PasswordVisible } from '../../Models/Function/Password';
import { UserController, IUserController } from '../../Models/Controller/UserController';
import { SuccessMessage, ErrorMessage } from '../../Models/Function/Message';
import {
    EditFormId,
    CreateModalId,
    SearchFormId,
    CreateFormId,
    EditModalId,
    DetailModalId,
} from '../../Models/Const/Const.';
import { FormValidField } from '../../Models/Const/FormValid';
import { UserMessageSetting } from '../../Models/MessageSetting';
import { CheckForm } from '../../Models/Function/Form';
import { Color } from '../../Models/Enum/ColorEnum';
import { Label } from '../../Models/Templete/LabelTemp';
import { EditButton, DetailButton } from '../../Models/Templete/ButtonTemp';
import { UserListModel } from '../../Models/Interface/User/UserListModel';
import { UserResetModel } from '../../Models/Interface/User/UserResetModel';
import { UserSearchModel } from '../../Models/Interface/User/UserSearchModel';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { Filter } from '../../Models/Enum/Filter';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { Logger } from '../../Models/Class/LoggerService';
/*=========================宣告變數===========================*/
const message = UserMessageSetting;
const valid = FormValidField.User;
const $SearchForm = $(SearchFormId);
var route: IUserController = new UserController();
var table: ItabulatorService;
/**回傳Modal性質*/
const prop = (key: keyof UserListModel): string => {
    return route.GetProperty<UserListModel>(key);
};
/**暫存最後一次的查詢條件 */
var lastSearchCondition: UserSearchModel = {
    userid: '',
    loginid: '',
    name: '',
};
//=====================================================
/**重載列表 */
$("button[name='reload']").click(function() {
    Search(lastSearchCondition);
});
/**讓密碼切換可見 */
$('#SeePassBtn').click(function() {
    PasswordVisible('#Password');
});
/**讓確認密碼切換可見 */
$('#SeePassConfirmBtn').click(function() {
    PasswordVisible('#ConfirmPassword');
});
/**預設查詢 */
Search(lastSearchCondition);
/**查詢表單 */
$SearchForm.submit(function(event) {
    event.preventDefault();
    const IsFormValid = CheckForm(SearchFormId, valid.Serch);
    if (IsFormValid) {
        const condition = <string>$SearchForm.find("select[name='condition']").val();
        const keyword = <string>$SearchForm.find("input[name='keyword']").val();
        const input: UserSearchModel = {
            userid: StringEnum.Empty,
            loginid: condition == 'account' ? keyword : StringEnum.Empty,
            name: condition == 'name' ? keyword : StringEnum.Empty,
        };
        lastSearchCondition = input;
        Search(input);
    }
});
/**列表篩選 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fsLOGIN_ID'), type: Filter.Like, value: word },
        { field: prop('fsNAME'), type: Filter.Like, value: word },
        { field: prop('C_sDEPTNAME'), type: Filter.Like, value: word },
        { field: prop('fsEMAIL'), type: Filter.Like, value: word },
        { field: prop('fsDESCRIPTION'), type: Filter.Like, value: word },
    ];
    const ActiveWord: Boolean | '' = word == '啟用' ? true : word == '停用' ? false : '';
    if (!IsNULLorEmpty(ActiveWord)) {
        filter.push({ field: prop('fsIS_ACTIVE'), type: Filter.Like, value: ActiveWord.toString() });
    }
    table.SetFilter(filter);
});
/*查詢結果*/
function Search(SearchParams: UserSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('fsUSER_ID'),
        addRowPos:'top',
        columns: [
            { title: '帳號', field: prop('fsLOGIN_ID'), width: 150, sorter: 'string' },
            { title: '顯示名稱', field: prop('fsNAME'), sorter: 'string', minWidth: 150 },
            { title: '部門', field: prop('C_sDEPTNAME'), sorter: 'string', width: 160 },
            {
                title: '帳號狀態',
                field: prop('fsIS_ACTIVE'),
                sorter: 'boolean',
                width: 105,
                formatter: function(cell, formatterParams) {
                    const status: boolean | null = cell.getValue();
                    const showtext: string = status ? '啟用' : !status ? '停用' : '未知';
                    const color: Color | '' = status ? Color.綠 : !status ? Color.紅 : StringEnum.Empty;
                    return Label(showtext, color);
                },
            },
            {
                title: '電子郵件',
                field: prop('fsEMAIL'),
                sorter: 'string',
                minWidth: 285,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata = <UserListModel>row.getData();
                    const emailconfirmed: boolean = rowdata.fsEmailConfirmed;
                    const showstr: string = rowdata.EmailConfirmedStr;
                    const color: Color | '' = emailconfirmed ? Color.綠 : !emailconfirmed ? Color.紅 : StringEnum.Empty;
                    return Label(showstr, color) + rowdata.fsEMAIL;
                },
            },
            { title: '描述', field: prop('fsDESCRIPTION'), sorter: 'string', visible: false },
            {
                title: '操作',
                field: prop('fsUSER_ID'),
                hozAlign: 'left',
                width: 240,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: string = cell.getValue();
                    const isactive: boolean | null = cell.getRow().getData().fsIS_ACTIVE;
                    const editbtn = EditButton(id, message.Controller);
                    const detailbtn = DetailButton(id, message.Controller);
                    const restorepwdbtn = `<button type="button" name="restore" data-inverted="" data-tooltip="還原密碼" data-position="bottom center" class="ui purple basic circular icon button" data-Id="${id}"><i class="key icon"></i></button>`;
                    const switchbtn = `<div class="Switching"> <div class="slideThree"><input type="checkbox" value="${id}" name="switchAccount" ${
                        isactive ? `checked="checked"` : ''
                    }><label></label></div> </div>`;
                    const btngroups: string = editbtn + detailbtn + restorepwdbtn + switchbtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const row = cell.getRow();
                    const rowdata = <UserListModel>row.getData();
                    const userid: string = rowdata.fsUSER_ID;
                    const username: string = rowdata.fsLOGIN_ID;
                    const target:
                        | HTMLButtonElement
                        | HTMLDivElement
                        | HTMLElement
                        | HTMLInputElement
                        | HTMLLabelElement = <any>e.target;
                    switch (true) {
                        /*點擊:檢視使用者帳號*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            ShowModal<IdModel>(DetailModalId, route.api.ShowDetail, { id: userid }).then(IsSuccess => {
                                if (IsSuccess) {
                                    ModalTask(DetailModalId, true, {
                                        closable: false,
                                        onShow: function() {
                                            $('button[name="emailVerify"]').click(function() {
                                                $(this).addClass('disabled');
                                                route
                                                    .SendVerifyEmail(userid)
                                                    .then(res => {
                                                        Logger.res(route.api.SendEmailVerify, '寄發驗證信', res, true);
                                                        $(this).removeClass('disabled');
                                                    })
                                                    .catch(error => {
                                                        $(this).removeClass('disabled');
                                                        Logger.viewres(
                                                            route.api.SendEmailVerify,
                                                            '寄發驗證信',
                                                            error,
                                                            true
                                                        );
                                                    });
                                            });
                                        },
                                    });
                                } else {
                                    Logger.viewres(route.api.ShowDetail, '檢視使用者帳號燈箱', '', true);
                                }
                            });
                            break;
                        /*點擊:編輯使用者帳號*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: userid }).then(IsSucess => {
                                if (IsSucess) {
                                    ModalTask(EditModalId, true, {
                                        closable: false,
                                        onShow: function() {
                                            $(EditModalId)
                                                .find('.dropdown')
                                                .dropdown();
                                            const $ChangeEmailBtn = $('button[name="changeEmail"]');
                                            const $SaveEmailBtn = $('button[name="saveEmail"]');
                                            const $EmailInput = $('input#fsEMAIL');
                                            //變更電子信箱按鈕
                                            $ChangeEmailBtn.click(function(event) {
                                                event.preventDefault();
                                                $EmailInput.prop('readonly', false);
                                                $(this).hide();
                                                $SaveEmailBtn.show();
                                            });
                                            //儲存電子信箱按鈕
                                            $SaveEmailBtn.click(function(event) {
                                                event.preventDefault();
                                                $EmailInput.prop('readonly', true);
                                                $(this).hide();
                                                $ChangeEmailBtn.show();
                                                route.ChangeEmail(userid, <string>$EmailInput.val()).then(res => {
                                                    Logger.res(route.api.ChangeEmail, '變更電子信箱', res, true);
                                                    if (res.IsSuccess) {
                                                        // route
                                                        //     .SendVerifyEmail(userid)
                                                        //     .then(res => {
                                                        //         Logger.res(
                                                        //             route.api.SendEmailVerify,
                                                        //             '寄發驗證信',
                                                        //             res,
                                                        //             true
                                                        //         );
                                                        //     })
                                                        //     .catch(error => {
                                                        //         Logger.viewres(
                                                        //             route.api.SendEmailVerify,
                                                        //             '寄發驗證信',
                                                        //             error,
                                                        //             true
                                                        //         );
                                                        //     });
                                                    }
                                                });
                                            });
                                        },
                                        onApprove: function() {
                                            const IsFormValid: boolean = CheckForm(EditFormId, valid.Edit);
                                            if (IsFormValid) {
                                                const $EDITFORM = $(EditFormId);
                                                const secretAry = <Array<string>>(
                                                    $EDITFORM.find("select[name='FSecretList']").val()
                                                );
                                                const groupAry = <Array<string>>(
                                                    $EDITFORM.find("select[name='GroupList']").val()
                                                );
                                                route
                                                    .EditUser({
                                                        fsUSER_ID: userid,
                                                        fsPHONE: <string>$EDITFORM.find("input[name='fsPHONE']").val(),
                                                        fsBOOKING_TARGET_PATH: <string>(
                                                            $EDITFORM.find("input[name='fsBOOKING_TARGET_PATH']").val()
                                                        ),
                                                        fsDESCRIPTION: <string>(
                                                            $EDITFORM.find("input[name='fsDESCRIPTION']").val()
                                                        ),
                                                        GroupList: groupAry,
                                                        FSecretList: secretAry,
                                                        fsNAME: <string>$EDITFORM.find("input[name='fsNAME']").val(),
                                                        fsENAME: <string>$EDITFORM.find("input[name='fsENAME']").val(),
                                                        fsTITLE: <string>$EDITFORM.find("input[name='fsTITLE']").val(),
                                                        fsDEPT_ID: <string>(
                                                            $EDITFORM.find("select[name='fsDEPT_ID']").val()
                                                        ),
                                                        fsEMAIL: <string>$EDITFORM.find("input[name='fsEMAIL']").val(),
                                                    })
                                                    .then(res => {
                                                        if (res.IsSuccess) {
                                                            SuccessMessage(res.Message);
                                                            const data: UserListModel = <UserListModel>res.Data;
                                                            table.ReactivityUpdate(userid, {
                                                                fsNAME: data.fsNAME,
                                                                C_sDEPTNAME: data.C_sDEPTNAME,
                                                                fsIS_ACTIVE: data.fsIS_ACTIVE,
                                                                fsEMAIL: data.fsEMAIL,
                                                                fsDESCRIPTION: data.fsDESCRIPTION,
                                                            });
                                                            $(EditModalId).modal('hide');
                                                        } else {
                                                            ErrorMessage(res.Message);
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.EditUser, '編輯帳號', error, true);
                                                    });
                                            }

                                            return false;
                                        },
                                    });
                                } else {
                                    ErrorMessage(`顯示編輯帳號燈箱發生錯誤`);
                                }
                            });
                            break;
                        /*點擊:還原密碼*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('key icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'restore':
                            const RestoreModalId = '#RestorePasswordModal';
                            ModalTask(RestoreModalId, true, {
                                closable: false,
                                onShow: function() {
                                    $(RestoreModalId)
                                        .find('div.content')
                                        .html(`確定要還原此帳號【${username}】的密碼?`);
                                },
                                onApprove: function() {
                                    route
                                        .ResetPassword(userid)
                                        .then(res => {
                                            res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                                        })
                                        .catch(error => {
                                            Logger.viewres(route.api.RestorePassword, '還原密碼', error, true);
                                        });
                                },
                            });
                            break;
                        /*滑動:使用者帳號啟用或停權*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('Switching') > -1:
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('slideThree') > -1:
                        case target instanceof HTMLInputElement && (<HTMLInputElement>target).name == 'switchAccount':
                        case target instanceof HTMLLabelElement:
                            const $checkbox = $(cell.getElement().querySelector("input[name='switchAccount']"));
                            const useraccount: string = <string>$checkbox.val();
                            /*將帳號變更為何種狀態*/
                            const switchstatus: boolean = $checkbox.prop('checked') ? false : true;
                            const SwitchAccountModalId = '#SwitchAccountModal';
                            ModalTask(SwitchAccountModalId, true, {
                                closable: false,
                                onApprove: function() {
                                    route.StopRight(useraccount, switchstatus).then(res => {
                                        const record = <UserResetModel>res.Records;
                                        if (res.IsSuccess) {
                                            $checkbox.prop('checked', !$checkbox.prop('checked'));
                                            SuccessMessage(res.Message);
                                            table.ReactivityUpdate(userid, { fsIS_ACTIVE: record.active });
                                        } else {
                                            ErrorMessage(res.Message);
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
    });
}

/**新增使用者帳號 */
ModalTask(CreateModalId, false, {
    closable: false,
    onApprove: function() {
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            const $CreateForm = $(CreateFormId);
            const secretAry = <Array<string>>$CreateForm.find("select[name='SecretList']").val();
            const roleAry = <Array<string>>$CreateForm.find("select[name='RoleList']").val();
            route
                .CreateUser({
                    UserName: <string>$CreateForm.find("input[name='UserName']").val(),
                    Password: <string>$CreateForm.find("input[name='Password']").val(),
                    ConfirmPassword: <string>$CreateForm.find("input[name='ConfirmPassword']").val(),
                    Name: <string>$CreateForm.find("input[name='Name']").val(),
                    EName: <string>$CreateForm.find("input[name='EName']").val(),
                    Title: <string>$CreateForm.find("input[name='Title']").val(),
                    DeptId: <string>$CreateForm.find("select[name='DeptId']").val(),
                    RoleList: roleAry,
                    SecretList: secretAry,
                    FileSecret: secretAry.join(','),
                    GroupIds: roleAry.join(','),
                    Email: <string>$CreateForm.find("input[name='Email']").val(),
                    Phone: <string>$CreateForm.find("input[name='Phone']").val(),
                    BookingTargetPath: <string>$CreateForm.find("input[name='BookingTargetPath']").val(),
                    Description: <string>$CreateForm.find("textarea[name='Description']").val(),
                })
                .then(res => {
                    if (res.IsSuccess) {
                        SuccessMessage(res.Message);
                        $(CreateModalId).modal('hide');
                        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
                        table.AddRow(<UserListModel>res.Data);
                    } else {
                        ErrorMessage(res.Message);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.Create, '新增使用者', error, true);
                });
        }
        return false;
    },
    onDeny: function() {
        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
    },
}).modal('attach events', "button[name='create']");
