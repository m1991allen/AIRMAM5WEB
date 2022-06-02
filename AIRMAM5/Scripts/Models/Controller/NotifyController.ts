import { BaseController } from './BaseController';
import { NotifyCreateModel, NotifyCreateRealModel } from '../Interface/Notify/NotifyCreateModel';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { API_Notify } from '../Const/API';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';

/**訊息通知管理介面 */
export interface INotifyController {
    /**新增訊息 */
    CreateNotify(input: NotifyCreateModel): Promise<IResponse>;
    /**
     * 將訊息標為已讀
     * @param ids 訊息識別碼陣列
     * @param readall 是否將全部設為已讀
     */
    ReadNotify(ids: Array<number>, readall: boolean): Promise<IResponse>;
}
/**訊息通知管理路由 */
export class NotifyController extends BaseController<API_Notify> implements INotifyController {
    constructor() {
        super({
            Create: GetUrl(Controller.Notify, Action.CreateNotify).href,
            Read: GetUrl(Controller.Notify, Action.ReadNotify).href,
        });
    }
    static get api() {
        return new this.api();
    }
    CreateNotify(input: NotifyCreateModel): Promise<IResponse> {
        return AjaxPost<NotifyCreateRealModel>(
            this.api.Create,
            {
                Title: input.Title,
                Content: input.Content,
                Category: input.Category,
                NoticeTo: input.NoticeTo,
                CategoryList: [], //server端有預設
                ExpireDate: '', //server端有預設5天
            },
            true
        );
    }
    ReadNotify(ids: Array<number>, readall: boolean): Promise<IResponse> {
        if (ids.length == 0 && !readall) {
            return new Promise(resolve => {
                resolve(<IResponse>{
                    IsSuccess: false,
                    ResponseTime: new Date().toISOString(),
                    Message: '至少傳送一筆訊息識別碼',
                });
            });
        } else {
            return AjaxPost<{ ids: Array<number>; readall: boolean }>(
                this.api.Read,
                { ids: ids, readall: readall },
                true
            );
        }
    }
}
