import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost, PostJXR } from '../Function/Ajax';
import { API_LUpload } from '../Const/API';
import { LUploadSearchModel } from '../Interface/ILUploadIndex';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { Action } from '../Enum/Action';
import { Controller } from '../Enum/Controller';
import { GetUrl } from '../Function/Url';
import { IsNullorUndefined } from '../Function/Check';
/**
 * 入庫轉檔介面
 */
export interface ILUploadController extends IBaseController<API_LUpload> {
    Search(input: LUploadSearchModel): Promise<IResponse>;
    /**編輯 */
    Edit(workid: number, priority: number, note: string): Promise<IResponse>;
    /**重轉 */
    Retran(ids: Array<string>): Promise<IResponse>;
    /**檔案進度 */
    Progress(ids:Array<number>):Promise<IResponse>;
    /**取消檔案進度查詢 */
    CancelProgress():void;
}
/**
 * 入庫轉檔維護路由
 */
export class LUploadController extends BaseController<API_LUpload> implements ILUploadController {
    private ProgressRequest: JQueryXHR;
    constructor() {
        super({
            Search: GetUrl(Controller.L_Upload, Action.Search).href,
            /**View:詳細燈箱 */
            ShowDetail: GetUrl(Controller.L_Upload, Action.Details).href,
            /**View:編輯燈箱 */
            ShowEdit: GetUrl(Controller.L_Upload, Action.ShowEdit).href,
            /**取得檔案上傳進度 */
            GetCurrentProgress: GetUrl(Controller.L_Upload, Action.GetCurrentProgress).href,
            /**編輯 */
            Edit: GetUrl(Controller.L_Upload, Action.Edit).href,
            /**重轉 */
            ReTran: GetUrl(Controller.L_Upload, Action.ReTran).href,
            /**取得檔案進度 */
            GetProgress:GetUrl(Controller.L_Upload,Action.GetCurrentProgress).href
        });
    }
    Search(input: LUploadSearchModel): Promise<IResponse> {
        return AjaxPost<LUploadSearchModel>(this.api.Search, input, false);
    }
    Edit(workid: number, priority: number, note: string): Promise<IResponse> {
        priority = priority < 0 ? 1 : priority;
        return AjaxPost<{ workid: number; priority: number; note: string }>(
            this.api.Edit,
            { workid: workid, priority, note: note },
            false
        );
    }
    static get api() {
        return new this.api();
    }
    Retran(ids: Array<string>): Promise<IResponse> {
        if (ids.length == 0) {
            return new Promise(resolve => {
                resolve(<IResponse>{
                    IsSuccess: false,
                    StatusCode: HttpStatusCode.BadRequest,
                    Message: '至少選擇一筆資料!',
                });
            });
        } else {
            return AjaxPost<{ id: Array<string> }>(this.api.ReTran, { id: ids }, false);
        }
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
