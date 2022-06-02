import { BaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { IPopularKeywords } from '../Interface/Search/IPopularKeywords';
import { AjaxGet, Ajax, AjaxPost } from '../Function/Ajax';
import { MediaType } from '../Enum/MediaType';
import { API_Shared } from '../Const/API';
import { SelectListItem } from '../Interface/Shared/ISelectListItem';
import { IFullTextSearchInput } from '../Interface/Search/IFullTextSearchInput';
import { IsNULLorEmpty } from '../Function/Check';
import { IdModel } from '../Interface/Shared/IdModel';
import { SearchParameterLogModel } from '../Interface/Search/SearchParameterLogModel';
import { BookingOptionModel } from '../Interface/Booking/BookingOptionModel';
import { GetUrl } from '../Function/Url';
import { Action } from '../Enum/Action';
import { Controller } from '../Enum/Controller';
import { DecoratorConvert } from '../Function/DecoratorConvert';
import { Logger } from '../Class/LoggerService';
/**
 * 共用Layout介面
 */
export interface ISharedController {
    /**熱門關鍵字 */
    PopularKeywords(word: string): Promise<IResponse>;
    /**樣板種類 */
    SearchTemplete(types: Array<MediaType>): Promise<Array<SelectListItem>>;
    /**依照樣板回傳動態欄位(如果search=null 會回傳全部欄位,如果要進階搜尋就回傳true) */
    GetAttriFieldList(templeteId: number, search?: boolean): Promise<Array<SelectListItem>>;
    /**取得全文檢索頁面路徑 */
    SearchIndex(input: IFullTextSearchInput): Promise<string>;
    /**取得調用樣板 */
    // GetBookingOption(id: number): Promise<BookingOptionModel>;
    /**取得我的最愛列表 */
    GetFavorite(): Promise<IResponse>;
    /**加入我的最愛列表 */
    AddFavorite(input: IdModel): Promise<IResponse>;
    /**移除我的最愛 */
    RemoveFavorite(input: IdModel): Promise<IResponse>;
    /**告訴Server要刪除DocumentViewer的暫存檔 */
    DeleteViewerFile(input: { fsSUBJECT_ID: string; fsFILE_NO: string; ViewFileName: string }): Promise<IResponse>;
}
class SharedStaticController extends BaseController<API_Shared> {
    /**STATIC:調用選項API */
    private static readonly staticapi = GetUrl(Controller.Shared, Action.GetBookingOption).href;
    constructor() {
        super({
            /**調用選項 */
            GetBookingOption: SharedController.staticapi,
            /**取得我的最愛 */
            GetFavorite: GetUrl(Controller.Home, Action.GetFavorite).href,
            /**加入我的最愛 */
            AddFavorite: GetUrl(Controller.Home, Action.AddFavorite).href,
            /**移除我的最愛 */
            DeleteFavorite: GetUrl(Controller.Home, Action.DelFavorite).href,
            /**儲存查詢條件以取得回傳Id */
            SearchParameter: GetUrl(Controller.Search, Action.SearchParameter).href,
            Search2Page: GetUrl(Controller.Search, Action.Index2).href,
            /**Top5熱門關鍵字 */
            PopularKeyword: GetUrl(Controller.Search, Action.PopularKeywords).href,
            /**樣板種類 */
            SearchTemplete: GetUrl(Controller.Shared, Action.GetTemplateList).href,
            /**依據樣板得到樣板的動態欄位 */
            GetAttriFieldList: GetUrl(Controller.Shared, Action.GetAttriFieldList).href,
        });
    }
    /**取得調用樣板 */
    static GetBookingOption(id: number): Promise<BookingOptionModel> {
        return Ajax<IdModel>('GET', this.staticapi, { id: id }, true);
    }
    static get api() {
        return new this.api();
    } 
}
/**
 * 共用Layout路由
 */
@DecoratorConvert
export class SharedController extends SharedStaticController {
    constructor() {
        super();
    }
    PopularKeywords(word: string): Promise<IResponse> {
        const wordArray: Array<string> = word.split(/[\s,|]+/gm);
        word =
            word.indexOf('|') == -1 && word.indexOf(',') == -1 && /\s/.test(word)
                ? word
                : wordArray[wordArray.length - 1];
        return AjaxGet<IPopularKeywords>(this.api.PopularKeyword, { word: word }, false);
    }
    SearchTemplete(types: Array<MediaType>): Promise<Array<SelectListItem>> {
        return Ajax<{ type: string }>('GET', this.api.SearchTemplete, { type: types.join(';') }, false);
    }
    GetAttriFieldList(templeteId: number, search?: boolean): Promise<Array<SelectListItem>> {
        return Ajax<{ tempid: number; search?: boolean }>(
            'GET',
            this.api.GetAttriFieldList,
            { tempid: templeteId, search: search },
            false
        );
    }
    SearchIndex(input: IFullTextSearchInput): Promise<string> {
        input.clsDATE.fdEDATE = input.clsDATE.fdEDATE
            .replace(/\\/g, '/')
            .replace(/\-/g, '/')
            .replace(/\=/g, '/');
        input.clsDATE.fdSDATE = input.clsDATE.fdSDATE
            .replace(/\\/g, '/')
            .replace(/\-/g, '/')
            .replace(/\=/g, '/');
        input.fnTEMP_ID = IsNULLorEmpty(input.fnTEMP_ID) ? 0 : input.fnTEMP_ID;

        const parameter: IFullTextSearchInput = input;
        return new Promise(resolve => {
            AjaxPost<IFullTextSearchInput>(this.api.SearchParameter, parameter, false)
                .then(res => {
                    Logger.res(this.api.SearchParameter, '紀錄全文檢索條件', res, false);
                    const logdata = <SearchParameterLogModel>res.Data;
                    if (res.IsSuccess) {
                        //SuccessMessage(res.Message);
                        const iframeSrc = this.api.Search2Page + '?id=' + logdata.tblSRH.fnSRH_ID;
                        resolve(iframeSrc);
                    } else {
                        // ErrorMessage(res.Message);
                        const iframeSrc = this.api.Search2Page + '?id=0';
                        resolve(iframeSrc);
                    }
                })
                .catch(error => {
                    Logger.viewres(this.api.SearchParameter, '紀錄全文檢索條件', error, false);
                    const iframeSrc = this.api.Search2Page + '?id=0';
                    resolve(iframeSrc);
                });
        });
    }
    GetFavorite(): Promise<IResponse> {
        return AjaxGet(this.api.GetFavorite, {}, false);
    }
    AddFavorite(input: IdModel): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.AddFavorite, { id: input.id }, false);
    }
    RemoveFavorite(input: IdModel): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.DeleteFavorite, { id: input.id }, false);
    }
    DeleteViewerFile(input: { api: string; subjectId: string; fileNO: string; fileName: string }): Promise<IResponse> {
        return AjaxPost<{ fsSUBJECT_ID: string; fsFILE_NO: string; ViewFileName: string }>(
            input.api,
            {
                fsSUBJECT_ID: input.subjectId,
                fsFILE_NO: input.fileNO,
                ViewFileName: input.fileName,
            },
            false
        );
    }
}
