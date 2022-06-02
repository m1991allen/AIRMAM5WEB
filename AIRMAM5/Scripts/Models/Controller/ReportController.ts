import { BaseController, IBaseController } from './BaseController';
import { ReportParameterModel } from '../Interface/Report/ReportParameterModel';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { API_Report } from '../Const/API';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';

/**
 * 報表介面
 */
export interface IReportController extends IBaseController<API_Report> {
    Search(input: ReportParameterModel): Promise<IResponse>;
}
/**
 * 報表路由
 */
export class ReportController extends BaseController<API_Report> implements IReportController {
    constructor() {
        super({
            /**查詢報表 */
            Search: GetUrl(Controller.Report, Action.Search).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Search(input: ReportParameterModel): Promise<IResponse> {
        return AjaxPost<ReportParameterModel>(this.api.Search, {
            StartDate: input.StartDate,
            EndDate: input.EndDate,
            RptItem: input.RptItem,
        });
    }
}
