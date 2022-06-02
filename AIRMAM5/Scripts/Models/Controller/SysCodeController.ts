import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { API_SysCode } from '../Const/API';
import { YesNo } from '../Enum/BooleanEnum';
import { IsNULLorEmpty } from '../Function/Check';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { CreateUserCodeModel } from '../Interface/UserCode/CreateUserCodeModel';
import { CodeNoteModel } from '../Interface/UserCode/CodeNoteModel';
import { CreateUserSubCodeModel } from '../Interface/UserCode/CreateUserSubCodeModel';
import { EditUserSubCodeModel } from '../Interface/UserCode/EditUserSubCodeModel';
import { DeleteUserSubCodeModel } from '../Interface/UserCode/DeleteUserSubCodeModel';
import { CodeIdModel } from '../Interface/UserCode/CodeIdModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';

/**
 * 系統代碼介面
 */
export interface ISysCodeController extends IBaseController<API_SysCode> {
    /**新增主代碼 */
    Create(input: CreateUserCodeModel): Promise<IResponse>;
    /**編輯主代碼 */
    Edit(input: CodeNoteModel): Promise<IResponse>;
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
 * 系統代碼路由
 */
export class SysCodeController extends BaseController<API_SysCode> implements ISysCodeController {
    constructor() {
        super({
            /**系統代碼列表 */
            Search: GetUrl(Controller.SysCode, Action.Search).href,
            /**設定資訊 */
            ShowCog: GetUrl(Controller.SysCode, Action.ShowCog).href,
            /**新增系統主代碼 */
            Create: GetUrl(Controller.SysCode, Action.Create).href,
            /**編輯系統主代碼 */
            Edit: GetUrl(Controller.SysCode, Action.Edit).href,
            /**刪除系統主代碼 */
            Delete: GetUrl(Controller.SysCode, Action.Delete).href,
            /**新增系統子代碼 */
            CreateCode: GetUrl(Controller.SysCode, Action.CreateCode).href,
            /**編輯系統子代碼 */
            EditCode: GetUrl(Controller.SysCode, Action.EditCode).href,
            /**刪除系統子代碼 */
            DeleteCode: GetUrl(Controller.SysCode, Action.DeleteCode).href,

            /**View:新增 */
            ShowCreate: GetUrl(Controller.SysCode, Action.ShowCreate).href,
            /**View:編輯*/
            ShowEdit: GetUrl(Controller.SysCode, Action.ShowEdit).href,
            /**View:刪除*/
            ShowDelete: GetUrl(Controller.SysCode, Action.ShowDelete).href,
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
    Edit(input: CodeNoteModel): Promise<IResponse> {
        return AjaxPost<CodeNoteModel>(this.api.Edit, {
            fsCODE_ID: input.fsCODE_ID,
            fsTITLE: input.fsTITLE,
            fsNOTE: input.fsNOTE,
            IsEnabled: input.IsEnabled,
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
                fnORDER: input.fnORDER | 99,
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
                fnORDER: input.fnORDER | 99,
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
