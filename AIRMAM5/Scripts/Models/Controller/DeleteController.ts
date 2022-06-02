import { BaseController, IBaseController } from './BaseController';
import { MediaType } from '../Enum/MediaType';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { API_Delete } from '../Const/API';
import { IdModel } from '../Interface/Shared/IdModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
/**
 * 刪除紀錄介面
 */
export interface IDeleteController extends IBaseController<API_Delete> {
    /**刪除 */
    Delete(id: number, type: MediaType, fileno: string): Promise<IResponse>;
    /**還原 */
    Recycle(id: number): Promise<IResponse>;
}
/**刪除紀錄路由 */
export class DeleteController extends BaseController<API_Delete> implements IDeleteController {
    constructor() {
        super({
            Search: GetUrl(Controller.Delete, Action.Search).href,
            /**View:刪除頁面 */
            ShowDelete: GetUrl(Controller.Delete, Action.ShowDelete).href,
            /**刪除實體檔案 */
            Delete: GetUrl(Controller.Delete, Action.Delete).href,
            /**View:回復媒體檔案頁面 */
            ShowRedo: GetUrl(Controller.Delete, Action.ShowRedo).href,
            /**View:檢視刪除資訊 */
            ShowDetail: GetUrl(Controller.Delete, Action.ShowDetails).href,
            /**回復媒體檔案 */
            Redo: GetUrl(Controller.Delete, Action.Redo).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Delete(id: number, type: MediaType, fileno: string): Promise<IResponse> {
        return AjaxPost<{ id: number; type: MediaType; fileno: string }>(
            this.api.Delete,
            { id: id, type: type, fileno: fileno },
            false
        );
    }
    Recycle(id: number): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.Redo, { id: id }, false);
    }
}
