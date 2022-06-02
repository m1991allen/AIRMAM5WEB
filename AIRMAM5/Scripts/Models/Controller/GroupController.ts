import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost, AjaxGet } from '../Function/Ajax';
import { API_Group } from '../Const/API';
import { IdModel } from '../Interface/Shared/IdModel';
import { GroupEditModel } from '../Interface/Group/GroupEditModel';
import { RoleFuncUpdateModel } from '../Interface/Group/RoleFuncUpdateModel';
import { GroupInputModel } from '../Interface/Group/GroupInputModel';
import { GroupModel } from '../Interface/Group/GroupModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
/**
 * 介面
 */
export interface IGroupController extends IBaseController<API_Group> {
    /** 新增群組 */
    Create(input: GroupInputModel): Promise<IResponse>;
    /**編輯群組 */
    Edit(input: GroupEditModel): Promise<IResponse>;
    /**刪除群組 */
    Delete(id: string): Promise<IResponse>;
    /**功能項目 */
    SaveFunction(input: RoleFuncUpdateModel): Promise<IResponse>;
    /**取得角色帳號列表 */
    ShowAccount(id: string): Promise<IResponse>;
}
/**
 * 群組維護路由
 */
export class GroupController extends BaseController<API_Group> implements IGroupController {
    constructor() {
        super({
            /**查詢群組 */
            Search: GetUrl(Controller.Group, Action.Search).href,
            /**新增群組 */
            Create: GetUrl(Controller.Group, Action.Create).href,
            /**編輯群組 */
            Edit: GetUrl(Controller.Group, Action.Edit).href,
            /**刪除群組 */
            Delete: GetUrl(Controller.Group, Action.Delete).href,
            /**編輯群組開放功能 */
            SaveFunction: GetUrl(Controller.Group, Action.FuncsSave).href,
            /**呼叫編輯PartialView*/
            ShowEdit: GetUrl(Controller.Group, Action.ShowEdit).href,
            /**呼叫刪除PartialView*/
            ShowDelete: GetUrl(Controller.Group, Action.ShowDelete).href,
            /**呼叫功能項目PartialView*/
            ShowFunction: GetUrl(Controller.Group, Action.Function).href,
            /**顯示角色群組的帳號資料 */
            ShowAccount: GetUrl(Controller.Group, Action.ShowAccount).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Create(input: GroupInputModel): Promise<IResponse> {
        return AjaxPost<GroupModel>(
            this.api.Create,
            {
                fsGROUP_ID: '',
                fsNAME: input.fsNAME,
                fsDESCRIPTION: input.fsDESCRIPTION,
                fsTYPE: '',
                fsCREATED_BY: '',
                fsUPDATED_BY: '',
                Discriminator: '',
            },
            false
        );
    }
    Edit(input: GroupEditModel): Promise<IResponse> {
        return AjaxPost<GroupEditModel>(this.api.Edit, input, false);
    }
    Delete(id: string): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.Delete, { id: id }, false);
    }
    SaveFunction(input: RoleFuncUpdateModel): Promise<IResponse> {
        return AjaxPost<RoleFuncUpdateModel>(this.api.SaveFunction, input, false);
    }
    ShowAccount(id: string): Promise<IResponse> {
        return AjaxGet<IdModel>(this.api.ShowAccount, { id: id }, false);
    }
}
