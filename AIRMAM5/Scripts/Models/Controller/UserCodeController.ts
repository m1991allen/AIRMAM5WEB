import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { API_UserCode } from '../Const/API';
import { YesNo } from '../Enum/BooleanEnum';
import { IsNULLorEmpty } from '../Function/Check';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { CreateUserCodeModel } from '../Interface/UserCode/CreateUserCodeModel';
import { EditUserCodeModel } from '../Interface/UserCode/EditUserCodeModel';
import { CreateUserSubCodeModel } from '../Interface/UserCode/CreateUserSubCodeModel';
import { EditUserSubCodeModel } from '../Interface/UserCode/EditUserSubCodeModel';
import { DeleteUserSubCodeModel } from '../Interface/UserCode/DeleteUserSubCodeModel';
import { CodeIdModel } from '../Interface/UserCode/CodeIdModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';

/**
 * 自定義代碼介面
 */
export interface IUserCodeController extends IBaseController<API_UserCode> {
    /**新增主代碼 */
    Create(input: CreateUserCodeModel): Promise<IResponse>;
    /**編輯主代碼 */
    Edit(input: EditUserCodeModel): Promise<IResponse>;
    /**刪除主代碼 */
    Delete(codeId: string): Promise<IResponse>;
    /**新增子代碼 */
    CreateSubCode(input: CreateUserSubCodeModel): Promise<IResponse>;
    /**編輯子代碼 */
    EditSubCode(input: EditUserSubCodeModel): Promise<IResponse>;
    /**刪除子代碼 */
    DeleteSubCode(input: DeleteUserSubCodeModel): Promise<IResponse>;
}
/**
 * 自定義代碼路由
 */
export class UserCodeController extends BaseController<API_UserCode> implements IUserCodeController {
    constructor() {
        super({
            /**自訂代碼列表 */
            Search: GetUrl(Controller.UserCode, Action.Search).href,
            /**設定資訊 */
            ShowCog: GetUrl(Controller.UserCode, Action.ShowCog).href,
            /**新增自訂主代碼 */
            Create: GetUrl(Controller.UserCode, Action.Create).href,
            /**編輯自訂主代碼 */
            Edit: GetUrl(Controller.UserCode, Action.Edit).href,
            /**刪除自訂主代碼 */
            Delete: GetUrl(Controller.UserCode, Action.Delete).href,
            /**新增自訂子代碼 */
            CreateCode: GetUrl(Controller.UserCode, Action.CreateCode).href,
            /**編輯自訂子代碼 */
            EditCode: GetUrl(Controller.UserCode, Action.EditCode).href,
            /**刪除自訂子代碼 */
            DeleteCode: GetUrl(Controller.UserCode, Action.DeleteCode).href,
            /**View:新增 */
            ShowCreate: GetUrl(Controller.UserCode, Action.ShowCreate).href,
            /**View:編輯*/
            ShowEdit: GetUrl(Controller.UserCode, Action.ShowEdit).href,
            /**View:刪除*/
            ShowDelete: GetUrl(Controller.UserCode, Action.ShowDelete).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Create(input: CreateUserCodeModel): Promise<IResponse> {
        return AjaxPost<CreateUserCodeModel>(this.api.Create, {
            fsCODE_ID: input.fsCODE_ID,
            fsTITLE: input.fsTITLE,
            fsNOTE: input.fsNOTE,
            IsEnabled: input.IsEnabled,
        });
    }
    Edit(input: EditUserCodeModel): Promise<IResponse> {
        return AjaxPost<EditUserCodeModel>(this.api.Edit, {
            fsCODE_ID: input.fsCODE_ID,
            fsTITLE: input.fsTITLE,
            fsNOTE: input.fsNOTE,
            IsEnabled: input.IsEnabled,
            fsIS_ENABLED: input.IsEnabled,
        });
    }
    Delete(codeId: string): Promise<IResponse> {
        return AjaxPost<CodeIdModel>(this.api.Delete, { fsCODE_ID: codeId }, false);
    }
    CreateSubCode(input: CreateUserSubCodeModel): Promise<IResponse> {
        return AjaxPost<CreateUserSubCodeModel>(
            this.api.CreateCode,
            {
                IsEnabled: input.IsEnabled,
                fsCODE_ID: input.fsCODE_ID,
                fsCODE: input.fsCODE,
                fsNAME: input.fsNAME,
                fsENAME: input.fsENAME,
                fsSET: input.fsSET,
                fnORDER: input.fnORDER || 99,
                fsTITLE: input.fsTITLE,
                fsIS_ENABLED: input.IsEnabled ? YesNo.是 : YesNo.否,
                fsNOTE: input.fsNOTE,
            },
            false
        );
    }
    EditSubCode(input: EditUserSubCodeModel): Promise<IResponse> {
        return AjaxPost<EditUserSubCodeModel>(
            this.api.EditCode,
            {
                IsEnabled: input.IsEnabled,
                fsCODE_ID: input.fsCODE_ID,
                fsCODE: input.fsCODE,
                fsNAME: input.fsNAME,
                fsENAME: input.fsENAME,
                fsSET: input.fsSET,
                fnORDER: input.fnORDER || 99,
                fsTITLE: input.fsTITLE,
                fsIS_ENABLED: input.IsEnabled ? YesNo.是 : YesNo.否,
                fsNOTE: input.fsNOTE,
            },
            false
        );
    }
    DeleteSubCode(input: DeleteUserSubCodeModel): Promise<IResponse> {
        if (IsNULLorEmpty(input.CodeId) || IsNULLorEmpty(input.Code)) {
            return new Promise(resolve => {
                resolve(<IResponse>{
                    IsSuccess: false,
                    StatusCode: HttpStatusCode.BadRequest,
                    Message: '無法讀取主檔與子檔代碼',
                    Data: null,
                    ResponseTime: new Date().toISOString(),
                });
            });
        } else {
        }
        return AjaxPost<DeleteUserSubCodeModel>(this.api.DeleteCode, {
            CodeId: input.CodeId,
            Code: input.Code,
        });
    }
}
