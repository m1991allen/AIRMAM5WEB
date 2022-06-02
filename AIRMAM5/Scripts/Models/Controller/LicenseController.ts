import { API_License } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { BaseController, IBaseController } from './BaseController';
import { AjaxPost } from '../Function/Ajax';
import { IsNULLorEmpty } from '../Function/Check';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { GetUrl } from '../Function/Url';
import { LicenseCreateModel } from '../Interface/License/LicenseCreateModel';
import { LicenseEditModel } from '../Interface/License/LicenseEditModel';

/** 介面 */
export interface ILicenseController extends IBaseController<API_License> {
    /**新增 版權 */
    Create(input: LicenseCreateModel): Promise<IResponse>;
    /**編輯 版權 */
    Edit(input: LicenseCreateModel): Promise<IResponse>;
    
}

/** 路由 */
export class LicenseController extends BaseController<API_License> implements ILicenseController {
    constructor() {
        super({
            /**查詢  */
            Search: GetUrl(Controller.License, Action.Search).href,
            /**新增  */
            Create: GetUrl(Controller.License, Action.Create).href,
            /**編輯  */
            Edit: GetUrl(Controller.License, Action.Edit).href,
            /**View:新增 */
            ShowCreate: GetUrl(Controller.License, Action.ShowCreate).href,
            /**View:編輯燈箱 */
            ShowEdit: GetUrl(Controller.License, Action.ShowEdit).href,
        });
    }
    static get api() {
        return new this.api();
    }

    /**新增版權 */
    Create(input: LicenseCreateModel): Promise<IResponse>{
        return AjaxPost<LicenseCreateModel>(
            this.api.Create,
            {
                LicenseCode: input.LicenseCode,
                LicenseName: input.LicenseName,
                LicenseDesc: IsNULLorEmpty(input.LicenseDesc) ? '' : input.LicenseDesc,
                EndDate: input.EndDate,
                AlertMessage: IsNULLorEmpty(input.AlertMessage) ? '': input.AlertMessage,
                IsBookingAlert: input.IsBookingAlert,
                IsNotBooking: input.IsNotBooking,
                IsActive: input.IsActive, //true,
                Order: input.Order,
            },
            false
        );
    }
    /**編輯版權 */
    Edit(input: LicenseEditModel): Promise<IResponse> {
        return AjaxPost<LicenseEditModel>(
            this.api.Edit,
            {
                LicenseCode: input.LicenseCode,
                LicenseName: input.LicenseName,
                LicenseDesc: IsNULLorEmpty(input.LicenseDesc) ? '' : input.LicenseDesc,
                EndDate: input.EndDate,
                AlertMessage: IsNULLorEmpty(input.AlertMessage) ? '': input.AlertMessage,
                IsBookingAlert: input.IsBookingAlert,
                IsNotBooking: input.IsNotBooking,
                IsActive: input.IsActive, //true,
                Order: input.Order,
            },
            false
        );
    }
}
