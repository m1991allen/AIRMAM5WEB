import { IBaseController, BaseController } from './BaseController';
import { API_BatchBooking } from '../Const/API';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { GetUrl } from '../Function/Url';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost, AjaxGet } from '../Function/Ajax';
import { SubjectSearchModel } from '../Interface/Subject/SubjectSearchModel';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { MediaType } from '../Enum/MediaType';
import { BatchBookingCreateModel } from '../Interface/BatchBooking/BatchBookingCreateModel';
import { BatchBookingFileListModel } from '../Interface/BatchBooking/BatchBookingFileListModel';
/**
 * 批次調用介面
 */
export interface IBatchBookingController extends IBaseController<API_BatchBooking> {
    /**取得主題列表 */
    GetSubjectList(DirId: number): Promise<IResponse>;
    /**取得檔案列表 */
    GetFileList(input: { SubjectId: string; MediaType: MediaType }): Promise<IResponse>;
    /**批次調用儲存 */
    SaveBooking(input: BatchBookingCreateModel): Promise<IResponse>;
}
/**
 * 批次調用路由
 */
export class BatchBookingController extends BaseController<API_BatchBooking> implements IBatchBookingController {
    constructor() {
        super({
            GetDir: GetUrl(Controller.Dir, Action.GetDir).href,
            GetSubjectList: GetUrl(Controller.Subject, Action.Search).href,
            GetFileList: GetUrl(Controller.BatchBooking, Action.MediaFileList).href,
            SaveBooking: GetUrl(Controller.BatchBooking, Action.Booking).href,
        });
    }
    GetSubjectList(DirId: number): Promise<IResponse> {
        return AjaxPost<SubjectSearchModel>(this.api.GetSubjectList, { id: DirId }, false);
    }
    GetFileList(input: { SubjectId: string; MediaType: MediaType }): Promise<IResponse> {
        return AjaxGet<{ subjectid: string; type: MediaType }>(
            this.api.GetFileList,
            {
                subjectid: input.SubjectId,
                type: input.MediaType,
            },
            false
        );
    }
    SaveBooking(input: BatchBookingCreateModel): Promise<IResponse> {
        return AjaxPost<BatchBookingCreateModel>(this.api.SaveBooking, input, false);
    }
}
