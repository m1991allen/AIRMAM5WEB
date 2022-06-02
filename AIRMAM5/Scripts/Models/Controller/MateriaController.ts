import { API_Materia } from '../Const/API';
import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost } from '../Function/Ajax';
import { MediaType } from '../Enum/MediaType';
import { FileStatusResult } from '../Interface/Search/ISearchResponseVideoModel';
import { GetTmsStatus } from '../../Views/Search/GetTsmStatus';
import { MaterialListModel } from '../Interface/Materia/MaterialListModel';
import { CreateMaterialModel } from '../Interface/Materia/CreateMaterialModel';
import { CreateBookingModel } from '../Interface/Booking/CreateBookingModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { Logger } from '../Class/LoggerService';

/**
 * 介面
 */
export interface IMateriaController extends IBaseController<API_Materia> {
    //  ShowFilmEdit(id:number):Promise<string>;
    /**查詢 */
    Search(): Promise<Array<MaterialListModel>>;
    /**刪除調用檔案(可複選) */
    DeleteFile(input: Array<number>): Promise<IResponse>;
    /**段落剪輯/粗剪 */
    FilmEdit(input: Array<CreateMaterialModel>): Promise<IResponse>;
    /**調用檔案 送出 */
    Booking(input: CreateBookingModel): Promise<IResponse>;
    /**取得影片類型的檔案的TMS狀態 */
    GetTmsStatus(
        mediaType: MediaType,
        fileNos: Array<string>
    ): Promise<
        IResponse<
            { mediaType: MediaType; fileNos: Array<string> },
            { TsmFileStatus: Array<FileStatusResult>; IsUseTSM: boolean }
        >
    >;
}
/**我的調用清單路由 */
export class MateriaController extends BaseController<API_Materia> implements IMateriaController {
    constructor() {
        super({
            /**JSON:調用清單列表 */
            Search: GetUrl(Controller.Materia, Action.Search).href,
            /**View:調用檔案(選單操作) */
            ShowBooking: GetUrl(Controller.Materia, Action.ShowBooking).href,
            /**JSON: 調用檔案*/
            Booking: GetUrl(Controller.Materia, Action.Booking).href,
            /**View:段落剪輯/粗剪 */
            ShowFilmEdit: GetUrl(Controller.Materia, Action.ShowFilmEdit).href,
            /**View:詳細 */
            ShowDetail: GetUrl(Controller.Materia, Action.ShowDetails).href,
            /**JSON:段落剪輯/粗剪 */
            FilmEdit: GetUrl(Controller.Materia, Action.FilmEdit).href,
            /**刪除調用檔案(可複選) */
            DeleteFile: GetUrl(Controller.Materia, Action.Delete).href,
            /**取得多個影片檔案的Tsm狀態 */
            GetTsmStatus: GetUrl(Controller.Search, Action.GetTsmStatus).href,
        });
    }
    static get api() {
        return new this.api();
    }
    Search(): Promise<Array<MaterialListModel>> {
        return new Promise(resolve => {
            AjaxPost(this.api.Search, {}, false)
                .then(res => {
                    const data = <Array<MaterialListModel>>res.Data;
                    res.IsSuccess ? resolve(data) : resolve([]);
                })
                .catch(error => {
                    Logger.viewres(this.api.Search, '查詢我的清單列表', error, false);
                    resolve([]);
                });
        });
    }
    DeleteFile(input: Array<number>): Promise<IResponse> {
        return AjaxPost<{ ids: Array<number> }>(this.api.DeleteFile, { ids: input }, false);
    }
    FilmEdit(input: Array<CreateMaterialModel>): Promise<IResponse> {
        return AjaxPost<{ model: Array<CreateMaterialModel> }>(this.api.FilmEdit, { model: input }, false);
    }
    Booking(input: CreateBookingModel): Promise<IResponse> {
        return AjaxPost<{ model: CreateBookingModel }>(this.api.Booking, { model: input }, false);
    }
    GetTmsStatus(
        mediaType: MediaType,
        fileNos: Array<string>
    ): Promise<
        IResponse<
            { mediaType: MediaType; fileNos: Array<string> },
            { TsmFileStatus: Array<FileStatusResult>; IsUseTSM: boolean }
        >
    > {
        return GetTmsStatus(this.api.GetTsmStatus, mediaType, fileNos);
    }
}
