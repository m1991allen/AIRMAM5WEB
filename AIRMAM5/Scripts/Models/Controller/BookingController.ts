import { BaseController, IBaseController } from './BaseController';
/**
 * API變數
 */
import { API, API_Booking } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost, PostJXR } from '../Function/Ajax';
import { CreateMaterialModel } from '../Interface/Materia/CreateMaterialModel';
import { MyBookingSearchModel } from '../Interface/MyBooking/MyBookingSearchModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { IsNullorUndefined } from '../Function/Check';
/**
 * 介面
 */
export interface IBookingController extends IBaseController<API_Booking> {
    /**查詢 */
    Search(input: MyBookingSearchModel): Promise<IResponse>;
    /**加入借調 */
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse>;
    /**選擇資料列重設借調 */
    ReBooking(ids: Array<string>): Promise<IResponse>;
    /**取消已選擇的借調 */
    CancelBooking(ids:Array<string>):Promise<IResponse>;
    /**設定優先權 */
    Edit(input: { workid: number; priority: number }): Promise<IResponse>;
    /**檔案進度 */
    Progress(ids:Array<number>):Promise<IResponse>;
    /**取消檔案進度查詢 */
    CancelProgress():void;
}
/**
 * 借調路由
 */
export class BookingController extends BaseController<API_Booking> implements IBookingController {
    private ProgressRequest: JQueryXHR;
    constructor() {
        super({
            /**JSON:清單列表 */
            Search: GetUrl(Controller.Booking, Action.Search).href,
            /**加入調用 */
            AddingBooking: GetUrl(Controller.Booking, Action.AddBooking).href,
            /**呼叫設定優先權PartialView*/
            ShowEdit: GetUrl(Controller.Booking, Action.ShowEdit).href,
            /**設定優先權 */
            Edit: GetUrl(Controller.Booking, Action.Edit).href,
            /**重設借調 */
            ReBooking: GetUrl(Controller.Booking, Action.ReBooking).href,
            /**取消借調 */
            BookingCancel: GetUrl(Controller.Booking, Action.BookingCancel).href,
            /**取得檔案進度 */
            GetProgress:GetUrl(Controller.L_Upload,Action.GetCurrentProgress).href
        });
        this.ProgressRequest=null;
    }
    static get api() {
        return new this.api();
    }
    Search(input: MyBookingSearchModel): Promise<IResponse> {
        return AjaxPost<MyBookingSearchModel>(this.api.Search, input, false);
    }
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse> {
        return AjaxPost<Array<CreateMaterialModel>>(this.api.AddingBooking, input, false);
    }
    ReBooking(ids: Array<string>): Promise<IResponse> {
        return AjaxPost<{ id: Array<string> }>(this.api.ReBooking, { id: ids }, false);
    }
    CancelBooking(ids:Array<string>):Promise<IResponse>{
        return AjaxPost<{ id: Array<string> }>(this.api.BookingCancel, { id: ids }, false);
    }
    Edit(input: { workid: number; priority: number }): Promise<IResponse> {
        return AjaxPost<{ workid: number; priority: number }>(
            this.api.Edit,
            {
                workid: input.workid,
                priority: input.priority,
            },
            false
        );
    }
    Progress(ids:Array<number>):Promise<IResponse>{
        if(!IsNullorUndefined(this.ProgressRequest)){
            this.ProgressRequest.abort();
        }
        this.ProgressRequest=PostJXR<{ids:Array<number>}>(this.api.GetProgress, {ids:ids },false);
        return new Promise((resolve,reject)=>{
           this.ProgressRequest
           .then(json=>{
               this.ProgressRequest=null;
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
