import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { VerifyBookingSearchModel } from '../Interface/VerifyBooking/VerifyBookingSearchModel';
import { AjaxPost } from '../Function/Ajax';
import { API_VerifyBooking } from '../Const/API';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
/**審核調用介面 */
export interface IVerifyBookingController extends IBaseController<API_VerifyBooking> {
    /**查詢審核列表 */
    Search(input: VerifyBookingSearchModel): Promise<IResponse>;
    /**批次標註過審或不過審 */
    Verify(VerifyIds: Array<number>, IsPass: boolean, Reason: string): Promise<IResponse>;
}
/**審核調用路由 */
export class VerifyBookingController extends BaseController<API_VerifyBooking> implements IVerifyBookingController {
    constructor() {
        super({
            Search: GetUrl(Controller.VerifyBooking, Action.Search).href,
            /**JSON:審核 */
            Verify: GetUrl(Controller.VerifyBooking, Action.Verify).href,
            /**View:檢視*/
            ShowDetail: GetUrl(Controller.VerifyBooking, Action.ShowDetails).href,
            /**View:刪除*/
            ShowDelete: GetUrl(Controller.VerifyBooking, Action.ShowDelete).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Search(input: VerifyBookingSearchModel): Promise<IResponse> {
        return AjaxPost<VerifyBookingSearchModel>(this.api.Search, input, false);
    }
    Verify(VerifyIds: Array<number>, IsPass: boolean, Reason: string): Promise<IResponse> {
        return AjaxPost<{ VerifyIds: Array<number>; IsPass: boolean; Reason: string }>(this.api.Verify, {
            VerifyIds: VerifyIds,
            IsPass: IsPass,
            Reason: Reason,
        });
    }
}
