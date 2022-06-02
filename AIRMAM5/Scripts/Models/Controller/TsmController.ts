import { IBaseController, BaseController } from './BaseController';
import { API_Tsm } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { AjaxPost, Ajax } from '../Function/Ajax';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
/**
 * 磁帶管理介面
 */
export interface ITsmController extends IBaseController<API_Tsm> {
    /**單一筆下架 */
    CheckOut(id: string): Promise<IResponse>;
    /**批示下架 */
    BatchCheckOut(ids: Array<string>): Promise<IResponse>;
    /**上架(沒分單筆或多筆) */
    CheckIn(): Promise<IResponse>;
    /**
     * 查詢是否有待上架作業
     * @return {boolean} true=有上架中作業 / false=沒有上架中作業
     */
    CheckInWorks(): Promise<IResponse<object, boolean>>;
}
/**
 * 磁帶管理路由
 */
export class TsmController extends BaseController<API_Tsm> implements ITsmController {
    constructor() {
        super({
            GetAllTapeInfo: GetUrl(Controller.Tsm, Action.GetAllTapeInfo).href,
            GetTapeInfoInLib: GetUrl(Controller.Tsm, Action.GetTapeInfoInLib).href,
            CheckOut: GetUrl(Controller.Tsm, Action.CheckOut).href,
            GetPendingTape: GetUrl(Controller.Tsm, Action.GetPendingTape).href,
            CheckIn: GetUrl(Controller.Tsm, Action.CheckIn).href,
            CheckInWorks: GetUrl(Controller.Tsm, Action.CheckInWorks).href,
        });
    }
    static get api() {
        return new this.api();
    }
    CheckIn(): Promise<IResponse> {
        return AjaxPost(this.api.CheckIn, {}, false);
    }
    CheckOut(id: string): Promise<IResponse> {
        return AjaxPost<{ tapes: Array<string> }>(this.api.CheckOut, { tapes: [id] }, false);
    }
    BatchCheckOut(ids: Array<string>): Promise<IResponse> {
        if (ids.length > 0) {
            return AjaxPost<{ tapes: Array<string> }>(this.api.CheckOut, { tapes: ids }, false);
        }
        return Promise.resolve(<IResponse>{
            ResponseTime: new Date().toISOString(),
            Message: '至少選擇一個',
            StatusCode: HttpStatusCode.BadRequest,
        });
    }
    CheckInWorks(): Promise<IResponse<object, boolean>> {
        return Ajax('POST', this.api.CheckInWorks, {}, false);
    }
}
