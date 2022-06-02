import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { API_User } from '../Const/API';
import { UserCreateRealModel, UserCreateModel, UserDropdownModel } from '../Interface/User/UserCreateModel';
import { UserSearchModel } from '../Interface/User/UserSearchModel';
import { UserEditModel } from '../Interface/User/UserEditModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';

/**
 * 系統帳號維護介面
 */
export interface IUserController extends IBaseController<API_User> {
    /**搜尋帳號*/
    SearchUser(input: UserSearchModel): Promise<IResponse>;
    /**新增帳號 */
    CreateUser(input: UserCreateModel): Promise<IResponse>;
    /**編輯帳號 */
    EditUser(input: UserEditModel): Promise<IResponse>;
    /**還原密碼 */
    ResetPassword(userid: string): Promise<IResponse>;
    /**使用者帳號啟用或停權 */
    StopRight(id: string, active: boolean): Promise<IResponse>;
    /**寄帳號驗證信 */
    SendVerifyEmail(userid: string): Promise<IResponse>;
    /**變更電子信箱 */
    ChangeEmail(userid: string, email: string): Promise<IResponse>;
}

/**
 * 系統帳號維護路由
 */
export class UserController extends BaseController<API_User> implements IUserController {
    constructor() {
        super({
            /**搜尋帳號 */
            Search: GetUrl(Controller.User, Action.Search).href,
            /**新增帳號 */
            Create: GetUrl(Controller.User, Action.Create).href,
            /**帳號開放功能 */
            SaveFunction: GetUrl(Controller.User, Action.FuncsSave).href,
            /**帳號啟用停權*/
            StopRight: GetUrl(Controller.User, Action.UpdateActive).href,
            /**還原密碼 */
            RestorePassword: GetUrl(Controller.User, Action.RestorePwd).href,
            /**編輯帳號 */
            EditUser: GetUrl(Controller.User, Action.Edit).href,
            ChangeEmail: GetUrl(Controller.User, Action.ChangeEmail).href,
            /**寄帳號驗證信 */
            SendEmailVerify: GetUrl(Controller.User, Action.SendEmailVerify).href,
            /**呼叫編輯頁面*/
            ShowEdit: GetUrl(Controller.User, Action.ShowEdit).href,
            /**呼叫詳細頁面 */
            ShowDetail: GetUrl(Controller.User, Action.ShowDetails).href,
            /**呼叫刪除頁面 */
            ShowDelete: GetUrl(Controller.User, Action.ShowDelete).href,
        });
    }
    static get api() {
        return new this.api();
    }
    SearchUser(input: UserSearchModel): Promise<IResponse> {
        return AjaxPost<UserSearchModel>(this.api.Search, input, false);
    }
    CreateUser(input: UserCreateModel): Promise<IResponse> {
        let INPUT: UserDropdownModel = {
            DeptList: [],
            FileSecretList: [],
            RoleGroupLst: [],
        };
        Object.keys(input).forEach(key => (INPUT[key] = input[key]));
        return AjaxPost<UserCreateRealModel>(this.api.Create, <UserCreateRealModel>input, false);
    }
    EditUser(input: UserEditModel): Promise<IResponse> {
        return AjaxPost(this.api.EditUser, input, false);
    }
    ResetPassword(userid: string): Promise<IResponse> {
        return AjaxPost<{ userid: string }>(this.api.RestorePassword, { userid: userid }, false);
    }
    StopRight(id: string, active: boolean): Promise<IResponse> {
        return AjaxPost<{ id: string; active: boolean }>(this.api.StopRight, { id: id, active: active }, false);
    }
    SendVerifyEmail(userid: string): Promise<IResponse> {
        return AjaxPost<{ userid: string }>(this.api.SendEmailVerify, { userid: userid }, false);
    }
    ChangeEmail(userid: string, email: string): Promise<IResponse> {
        return AjaxPost<{ userid: string; email: string }>(
            this.api.ChangeEmail,
            { userid: userid, email: email },
            false
        );
    }
}
