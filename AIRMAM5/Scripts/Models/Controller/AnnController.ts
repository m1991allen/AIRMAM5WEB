import { API_Ann } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { BaseController, IBaseController } from './BaseController';
import { AjaxPost } from '../Function/Ajax';
import { IdModel } from '../Interface/Shared/IdModel';
import { IsNULLorEmpty } from '../Function/Check';
import { AnnCreateModel } from '../Interface/Ann/AnnCreateModel';
import { AnnEditModel } from '../Interface/Ann/AnnEditModel';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { GetUrl } from '../Function/Url';

/**公告介面 */
export interface IAnnController extends IBaseController<API_Ann> {
    /**新增公告 */
    Create(input: AnnCreateModel): Promise<IResponse>;
    /**編輯公告 */
    Edit(input: AnnEditModel): Promise<IResponse>;
    /**刪除公告 */
    Delete(id: number): Promise<IResponse>;
}

/**公告路由 */
export class AnnController extends BaseController<API_Ann> implements IAnnController {
    constructor() {
        super({
            /**查詢公告 */
            Search: GetUrl(Controller.Ann, Action.Matain).href,
            /**新增公告 */
            Create: GetUrl(Controller.Ann, Action.Create).href,
            /**編輯公告 */
            Edit: GetUrl(Controller.Ann, Action.Edit).href,
            /**刪除公告 */
            Delete: GetUrl(Controller.Ann, Action.Delete).href,
            /**View:新增 */
            ShowCreate: GetUrl(Controller.Ann, Action.ShowCreate).href,
            /**View:編輯燈箱 */
            ShowEdit: GetUrl(Controller.Ann, Action.ShowEdit).href,
            /**View:刪除燈箱 */
            ShowDelete: GetUrl(Controller.Ann, Action.ShowDelete).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Create(input: AnnCreateModel): Promise<IResponse> {
        return AjaxPost<AnnCreateModel>(
            this.api.Create,
            {
                AnnounceId: 0,
                fsTITLE: input.fsTITLE,
                fsCONTENT: IsNULLorEmpty(input.fsCONTENT) ? '' : input.fsCONTENT,
                fdSDATE: input.fdSDATE,
                fdEDATE: input.fdEDATE,
                fsTYPE: input.fsTYPE,
                fnORDER: input.fnORDER,
                GroupList: input.GroupList,
                fsIS_HIDDEN: input.fsIS_HIDDEN,
                fsDEPT: input.fsDEPT,
                fsNOTE: input.fsNOTE,
            },
            false
        );
    }
    Edit(input: AnnEditModel): Promise<IResponse> {
        return AjaxPost<AnnEditModel>(
            this.api.Edit,
            {
                AnnounceId: input.AnnounceId,
                fsTITLE: input.fsTITLE,
                fsCONTENT: input.fsCONTENT,
                fdSDATE: input.fdSDATE,
                fdEDATE: input.fdEDATE,
                fsTYPE: input.fsTYPE,
                fnORDER: input.fnORDER,
                GroupList: input.GroupList,
                fsIS_HIDDEN: input.fsIS_HIDDEN,
                fsDEPT: input.fsDEPT,
                fsNOTE: input.fsNOTE,
            },
            false
        );
    }
    Delete(id: number): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.Delete, { id: id }, false);
    }
}
