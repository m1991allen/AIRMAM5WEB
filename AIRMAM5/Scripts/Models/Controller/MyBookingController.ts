import { API_MyBooking } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { BaseController, IBaseController } from './BaseController';
import { AjaxPost, PostJXR } from '../Function/Ajax';
import { MyBookingSearchModel } from '../Interface/MyBooking/MyBookingSearchModel';
import { Controller } from '../Enum/Controller';
import { GetUrl } from '../Function/Url';
import { Action } from '../Enum/Action';
import { IsNullorUndefined } from '../Function/Check';

/**
 * 我的調用狀態介面
 */
export interface IMyBookingController extends IBaseController<API_MyBooking> {
    /**查詢 */
    Search(input: MyBookingSearchModel): Promise<IResponse>;
    /**檔案進度 */
    Progress(ids:Array<number>):Promise<IResponse>;
    /**取消檔案進度查詢 */
    CancelProgress():void;
}

export class MyBookingController extends BaseController<API_MyBooking> implements IMyBookingController {
    private ProgressRequest: JQueryXHR;
    constructor() {
        super({
            /**JSON:清單列表 */
            Search: GetUrl(Controller.MyBooking, Action.Search).href,
            /**View:檢視主題資訊 */
            ShowDetail: GetUrl(Controller.MyBooking, Action.ShowDetails).href,
            /**取得檔案進度 */
            GetProgress:GetUrl(Controller.L_Upload,Action.GetCurrentProgress).href
        });
    }
    static get api() {
        return new this.api();
    }
    Search(input: MyBookingSearchModel): Promise<IResponse> {
        return AjaxPost<MyBookingSearchModel>(this.api.Search, input, false);
    }
    Progress(ids:Array<number>):Promise<IResponse>{
        if(!IsNullorUndefined(this.ProgressRequest)){
            this.ProgressRequest.abort();
        }
        this.ProgressRequest=PostJXR<{ids:Array<number>}>(this.api.GetProgress, {ids:ids },false);
        return new Promise((resolve,reject)=>{
           this.ProgressRequest
           .then(json=>{
               this.ProgressRequest=null;//要測試0819
               resolve(<IResponse>json);
           })
           .catch(error=>{
             reject(error);
           });
        });
    }
    CancelProgress():void{
        if(!IsNullorUndefined(this.ProgressRequest)){
            this.ProgressRequest.abort();
        }
    }
}
