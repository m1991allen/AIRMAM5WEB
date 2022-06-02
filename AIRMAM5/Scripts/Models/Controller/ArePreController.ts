import { API_ArcPre } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { BaseController, IBaseController } from './BaseController';
import { Ajax, AjaxPost } from '../Function/Ajax';
import { MediaType } from '../Enum/MediaType';
import { SelectListItem } from '../Interface/Shared/ISelectListItem';
import { ArePreEditInputModel } from '../Interface/ArePre/ArePreEditInputModel';
import { GetUrl } from '../Function/Url';
import { Action } from '../Enum/Action';
import { Controller } from '../Enum/Controller';

/**
 * 預編詮釋介面
 */
export interface IArcPreController extends IBaseController<API_ArcPre> {
    /**由媒體類型取得預編詮釋資料樣板 */
    GetArcTempleteByType(type: MediaType): Promise<Array<SelectListItem>>;
    /**新增預編詮釋 */
    CreateArcPre(input: ArePreEditInputModel, formSerilize): Promise<IResponse>;
    /**編輯預編詮釋 */
    EditArcPre(input: ArePreEditInputModel): Promise<IResponse>;
    /**刪除預編詮釋 */
    DeleteArcPre(fnpreid: number): Promise<IResponse>;
}

/**
 *預編詮釋路由
 */
export class ArcPreController extends BaseController<API_ArcPre> implements IArcPreController {
    constructor() {
        super({
            /**Json:取預編樣板 */
            GetArcTemplete: GetUrl(Controller.ColTemplate, Action.GetTemplateList).href,
            /**JSON:查詢預編詮釋資料 */
            Search: GetUrl(Controller.ArcPre, Action.Search).href,
            /**JSON:新增預編詮釋資料 */
            Create: GetUrl(Controller.ArcPre, Action.Create).href,
            /**JSON:編輯預編詮釋資料*/
            Edit: GetUrl(Controller.ArcPre, Action.Edit).href,
            /**JSNO:刪除預編詮釋資料*/
            Delete: GetUrl(Controller.ArcPre, Action.Delete).href,
            /**View:編輯預編詮釋資料頁面*/
            ShowEdit: GetUrl(Controller.ArcPre, Action.ShowEdit).href,
            /**View:刪除預編詮釋資料頁面 */
            ShowDelete:GetUrl(Controller.ArcPre,Action.ShowDelete).href,
            /**View:新增預編詮釋資料頁面*/
            ShowSubCreate: GetUrl(Controller.ArcPre, Action.ShowSubCreate).href,
        });
    }
    static get api() {
        return new this.api();
    }
    GetArcTempleteByType(type: MediaType): Promise<Array<SelectListItem>> {
        return Ajax<{ table: MediaType }>('GET', this.api.GetArcTemplete, { table: type }, true);
    }
    CreateArcPre(input: ArePreEditInputModel, FormData: FormData): Promise<IResponse> {
        return AjaxPost<{ model: ArePreEditInputModel; form: FormData }>(
            this.api.Create,
            { model: input, form: FormData },
            false
        );
    }
    EditArcPre(input: ArePreEditInputModel): Promise<IResponse> {
        return AjaxPost<ArePreEditInputModel>(this.api.Edit, input, false);
    }
    DeleteArcPre(preId: number): Promise<IResponse> {
        return AjaxPost<{ fnpreid: number }>(this.api.Delete, { fnpreid: preId }, false);
    }
}
