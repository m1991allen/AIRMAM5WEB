import { API_Search } from '../Const/API';
import { Ajax, AjaxPost, AjaxFormSerialize, PostJXR } from '../Function/Ajax';
import { BaseController, IBaseController } from './BaseController';
import { MediaType } from '../Enum/MediaType';
import { IFullTextSearchInput } from '../Interface/Search/IFullTextSearchInput';
import { SearchSetting } from '../initSetting';
import { IResponse } from '../Interface/Shared/IResponse';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { SearchResponseBaseModel, FileStatusResult } from '../Interface/Search/ISearchResponseVideoModel';
import { GetTmsStatus } from '../../Views/Search/GetTsmStatus';
import { CreateMaterialModel } from '../Interface/Materia/CreateMaterialModel';
import { SubjectPreviewModel } from '../Interface/Subject/SubjectPreviewModel';
import { IsNULL } from '../Function/Check';
import { UI } from '../Templete/CompoentTemp';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { UserFileSubjectAuthModel } from '../Interface/Subject/UserFileAuthModel';
import { Logger } from '../Class/LoggerService';
import { TabNameEnum } from '../Enum/TabNameEnum';

/**
 * 介面
 */
export interface ISearchController extends IBaseController<API_Search> {
    /**依查詢條件取得查詢筆數，檢索參數*/
    SearchCounts(input: IFullTextSearchInput): Promise<IResponse>;
    /**依媒體種類取得查詢條件*/
    GetSearchView(type: MediaType, input: IFullTextSearchInput): Promise<IResponse>;
    /**查詢條件顯示區域html */
    Condition(input: SearchResponseBaseModel): Promise<string>;
    /**依媒體種類取得左側全文檢索列表 */
    SearchListAsync(type: MediaType, input: IFullTextSearchInput, nowpage: number): Promise<IResponse>;
    /**依媒體類型取得檢索結果的樣版列表 */
    TemplateListAsync(type: MediaType, input: IFullTextSearchInput): Promise<IResponse>;
    /**預覽頁面資訊 */
    Preview(subjectId: string, mediaType: MediaType, fileNo: string): Promise<string>;
    /*View:媒體基本資訊 */
    BasicMedia(input: SubjectPreviewModel): Promise<string>;
    /**View:媒體變動欄位資訊 */
    DynamicMedia(input: SubjectPreviewModel): Promise<string>;
    /**View:關鍵影格 */
    KeyFrame(fileNo: string): Promise<string>;
    /**View:段落描述 */
    Paragraph(mediaType: MediaType, fileNo: string): Promise<string>;
    /**View:聲音專輯資訊 */
    AudioAlbumInfo(subjectId: string, fileNo: string): Promise<string>;
    /**圖片額外資訊 */
    PhotoExifInfo(fileNo: string): Promise<string>;
    /**文件額外資訊 */
    DocInfo(fileNo: string): Promise<string>;
    /**加入借調 */
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse>;
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
    /**JSON:編輯指定檔案的媒體資料 */
    EditMedia(serializeFormData: string): Promise<IResponse>;
    /**
     * 取得使用者在該主題編號對於主題/影/音/圖/文的權限
     * @param mediaType 媒體類型(S、V、A、D、P)
     * @param SubjectIds 單個主題編號
     */
    GetSubjectFunctionAuth(mediaType: MediaType, SubjectId: string): Promise<UserFileSubjectAuthModel>;
    /**檢索結果_單一樣版_資料匯出_20201216 */
    SearchExportAsync(type: MediaType, input: IFullTextSearchInput): Promise<IResponse>;
}

export class SearchController extends BaseController<API_Search> implements ISearchController {
    /**暫存請求 */
    private PostSearchListAsyncRequest: JQueryXHR = null;
    // private PostTemplateListAsyncRequest: JQueryXHR = null;
    private PostPreviewRequest: JQueryXHR = null;
    private PostBasicMediaRequest: JQueryXHR = null;
    private PostDynamicMediaRequest: JQueryXHR = null;
    private PostKeyFrameRequest: JQueryXHR = null;
    private PostParagraphRequest: JQueryXHR = null;
    private PostAudioAlbumInfoRequest: JQueryXHR = null;
    private PostPhotoExifInfoRequest: JQueryXHR = null;
    private PostDocInfoRequest: JQueryXHR = null;
    // private PostSearchExportAsyncRequest: JQueryXHR = null;
    constructor() {
        super({
            /**儲存查詢條件以取得回傳Id */
            SearchParameter: GetUrl(Controller.Search, Action.SearchParameter).href,
            Search2Page: GetUrl(Controller.Search, Action.Index2).href,
            /**全文檢索頁面 */
            Index: GetUrl(Controller.Search, Action.Index).href,
            /**取得影片檢索條件 */
            VideoSearchParameters: GetUrl(Controller.Search, Action.ShowVideo).href,
            /**取得聲音檢索條件 */
            AudioSearchParameters: GetUrl(Controller.Search, Action.ShowAudio).href,
            /**取得圖片檢索條件 */
            PhotoSearchParameters: GetUrl(Controller.Search, Action.ShowPhoto).href,
            /**取得文件檢索條件 */
            DocSearchParameters: GetUrl(Controller.Search, Action.ShowDoc).href,
            /**依查詢條件取得查詢筆數，檢索參數 */
            // GetCountData: GetUrl(Controller.Search, Action.GetCountData).href,
            SearchCounts: GetUrl(Controller.Search, Action.SearchCounts).href,
            /**取得查詢條件頁面 */
            Condition: GetUrl(Controller.Search, Action.ShowCondition).href,
            /**依媒體類型取得檢索結果列表 */
            SearchListAsync: GetUrl(Controller.Search, Action.SearchListAsync).href,
            // ListAsync: GetUrl(Controller.Search, Action.ShowListAsync).href,
            /**依媒體類型取得檢索結果的樣版列表_20201216 */
            TemplateListAsync: GetUrl(Controller.Search, Action.TemplateListAsync).href,
            /**媒體預覽 */
            Preview: GetUrl(Controller.Search, Action.ShowPreview).href,
            /**基本資訊頁面 */
            BasicMedia: GetUrl(Controller.Search, Action.ShowBasicMedia).href,
            /**媒體變動欄位資訊 */
            DynamicMedia: GetUrl(Controller.Search, Action.ShowDynamicMedia).href,
            /**View:關鍵影格 */
            KeyFrame: GetUrl(Controller.Search, Action.ShowKeyFrame).href,
            /**View:段落描述 */
            Paragraph: GetUrl(Controller.Search, Action.ShowParaDescription).href,
            /**View:聲音專輯資訊 */
            AudioAlbumInfo: GetUrl(Controller.Search, Action.ShowAudioAlbumInfo).href,
            /**圖片額外資訊 */
            PhotoExifInfo: GetUrl(Controller.Search, Action.ShowPhotoExifInfo).href,
            /**文件額外資訊 */
            DocInfo: GetUrl(Controller.Search, Action.ShowDocInfo).href,
            /**Top5熱門關鍵字 */
            PopularKeyword: GetUrl(Controller.Search, Action.PopularKeywords).href,
            /**樣板種類 */
            SearchTemplete: GetUrl(Controller.Shared, Action.GetTemplateList).href,
            /**依據樣板得到樣板的動態欄位 */
            GetAttriFieldList: GetUrl(Controller.Shared, Action.GetAttriFieldList).href,
            /**取得多個影片檔案的Tsm狀態 */
            GetTsmStatus: GetUrl(Controller.Search, Action.GetTsmStatus).href,
            /**JSON:編輯指定檔案的媒體資料 */
            EditMedia: GetUrl(Controller.Subject, Action.EditMedia).href,
            /**View:編輯MediaData */
            ShowEditMedia: GetUrl(Controller.Subject, Action.ShowEditMedia).href,
            /**加入調用 */
            AddingBooking: GetUrl(Controller.Booking, Action.AddBooking).href,
            /**取得使用者在該主題編號對於主題/影/音/圖/文的權限*/
            GetUserSubjAuth: GetUrl(Controller.Subject, Action.GetUserSubjAuth).href,
            SearchExportAsync: GetUrl(Controller.Search, Action.SearchExportAsync).href,

        });
    }
    static get api() {
        return new this.api();
    }
    GetSubjectFunctionAuth = (mediaType: MediaType, SubjectId: string): Promise<UserFileSubjectAuthModel> => {
        return Ajax<{ id: string }>('GET', this.api.GetUserSubjAuth, { id: SubjectId });
    };
    SearchCounts(input: IFullTextSearchInput): Promise<IResponse> {
        return AjaxPost<IFullTextSearchInput>(this.api.SearchCounts, input, false);
    }
    GetSearchView(type: MediaType, input: IFullTextSearchInput): Promise<IResponse> {
        switch (type) {
            case MediaType.VIDEO:
                return AjaxPost<IFullTextSearchInput>(this.api.VideoSearchParameters, input, false);
            // return Ajax<IFullTextSearchInput>('POST', api.VideoSearchParameters, input, false);
            case MediaType.AUDIO:
                return AjaxPost<IFullTextSearchInput>(this.api.AudioSearchParameters, input, false);
            // return Ajax<IFullTextSearchInput>('POST', api.AudioSearchParameters, input, false);
            case MediaType.PHOTO:
                return AjaxPost<IFullTextSearchInput>(this.api.PhotoSearchParameters, input, false);
            //  return Ajax<IFullTextSearchInput>('POST', api.PhotoSearchParameters, input, false);
            case MediaType.Doc:
                return AjaxPost<IFullTextSearchInput>(this.api.DocSearchParameters, input, false);
            //  return Ajax<IFullTextSearchInput>('POST', api.DocSearchParameters, input, false);
            default:
                Logger.error(`不存在的類型,無法取得檢索條件`);
                return new Promise(resolve => {
                    resolve(<IResponse>{
                        IsSuccess: false,
                        Message: '不存在的類型,無法取得檢索條件',
                        StatusCode: HttpStatusCode.BadRequest,
                    });
                });
        }
    }
    Condition(input: SearchResponseBaseModel): Promise<string> {
        return Ajax<SearchResponseBaseModel>('POST', this.api.Condition, input, false);
    }
    SearchListAsync(type: MediaType, input: IFullTextSearchInput, nowpage: number): Promise<IResponse> {
        /*索引必須從1開始*/
        //   input.fnSTART_INDEX = input.fnSTART_INDEX <= 0 ? 1 : input.fnSTART_INDEX;
        if (!IsNULL(this.PostSearchListAsyncRequest)) {
            this.PostSearchListAsyncRequest.abort();
            Logger.log('取消最近一次列表請求');
        }
        this.PostSearchListAsyncRequest = PostJXR<{ mediaType: MediaType; model: IFullTextSearchInput }>(
            this.api.SearchListAsync,
            {
                mediaType: type,
                model: {
                    fsKEYWORD: input.fsKEYWORD,
                    fsINDEX: input.fsINDEX,
                    fnSEARCH_MODE: input.fnSEARCH_MODE,
                    fnHOMO: input.fnHOMO,
                    clsDATE: input.clsDATE,
                    lstCOLUMN_ORDER: input.lstCOLUMN_ORDER,
                    fnTEMP_ID: input.fnTEMP_ID,
                    lstCOLUMN_SEARCH: input.lstCOLUMN_SEARCH,
                    fnPAGE_SIZE: input.fnPAGE_SIZE,
                    fnSTART_INDEX: SearchSetting.pageSize * (nowpage - 1) + 1,
                },
            },
            false
        );
        return new Promise(resolve => {
            this.PostSearchListAsyncRequest.then(data => {
                Logger.log('列表請求完成');
                resolve(data);
            }).catch(error => {
                if (this.PostSearchListAsyncRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('列表請求錯誤,原因:', error);
                }
                resolve(error);
            });
        });
        // return AjaxPost<{ mediaType: MediaType; model: IFullTextSearchInput }>(
        //     api.SearchListAsync,
        //     { mediaType: type, model: input },
        //     false
        // );
    }
    TemplateListAsync(type: MediaType, input: IFullTextSearchInput): Promise<IResponse> {
        return AjaxPost(this.api.TemplateListAsync,  { param: input, mediaType: type },  false);
   
    }
    SearchExportAsync(type: MediaType, input: IFullTextSearchInput): Promise<IResponse>{
        return AjaxPost<{ param: IFullTextSearchInput; mediaType: string }>(
            this.api.SearchExportAsync,
            { param: input, mediaType: type },
            false
        );
    }
    Preview(subjectId: string, mediaType: MediaType, fileNo: string): Promise<string> {
        if (!IsNULL(this.PostPreviewRequest)) {
            this.PostPreviewRequest.abort();
            Logger.log('取消最近一次檢索預覽頁請求');
        }

        this.PostPreviewRequest = PostJXR<{ fsSUBJECT_ID: string; mediaType: string; fileNo: string }>(
            this.api.Preview,
            { fsSUBJECT_ID: subjectId, mediaType: mediaType, fileNo: fileNo },
            false
        );
        return new Promise(resolve => {
            this.PostPreviewRequest.then(view => {
                Logger.log('檢索預覽頁請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostPreviewRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('檢索預覽頁請求錯誤,原因:', error);
                    resolve(UI.Error.ErrorSegment().outerHTML);
                } else {
                    resolve(UI.Error.CorrectSegment('加載新一筆預覽資料', '請求中', 'spinner loading').outerHTML);
                }
            });
        });

        // return Ajax<{ fsSUBJECT_ID: string; mediaType: string; fileNo: string }>(
        //     'POST',
        //     api.Preview,
        //     { fsSUBJECT_ID: subjectId, mediaType: mediaType, fileNo: fileNo },
        //     false
        // );
    }
    BasicMedia(input: SubjectPreviewModel): Promise<string> {
        if (!IsNULL(this.PostBasicMediaRequest)) {
            this.PostBasicMediaRequest.abort();
            Logger.log('取消最近一次基本資料請求');
        }
        this.PostBasicMediaRequest = PostJXR<{ fsSUBJECT_ID: string; mediaType: string; fileNo: string }>(
            this.api.BasicMedia,
            {
                fsSUBJECT_ID: input.fsSUBJECT_ID,
                mediaType: input.type,
                fileNo: input.fileNo,
            },
            false
        );
        return new Promise(resolve => {
            this.PostBasicMediaRequest.then(view => {
                Logger.log('基本資料請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostBasicMediaRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('基本資料請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached active tab" data-tab="${TabNameEnum.BaseMeta}">${UI.Error.ErrorSegment().outerHTML
                        }</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached active tab" data-tab="${TabNameEnum.BaseMeta}">${UI.Error.CorrectSegment('加載新一筆基本資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ fsSUBJECT_ID: string; mediaType: string; fileNo: string }>(
        //     'POST',
        //     api.BasicMedia,
        //     {
        //         fsSUBJECT_ID: input.fsSUBJECT_ID,
        //         mediaType: input.type,
        //         fileNo: input.fileNo,
        //     },
        //     false
        // );
    }
    DynamicMedia(input: SubjectPreviewModel): Promise<string> {
        if (!IsNULL(this.PostDynamicMediaRequest)) {
            this.PostDynamicMediaRequest.abort();
            Logger.log('取消最近一次詳細資料請求');
        }
        this.PostDynamicMediaRequest = PostJXR<{ fsSUBJECT_ID: string; mediaType: string; fileNo: string }>(
            this.api.DynamicMedia,
            {
                fsSUBJECT_ID: input.fsSUBJECT_ID,
                mediaType: input.type,
                fileNo: input.fileNo,
            },
            false
        );
        return new Promise(resolve => {
            this.PostDynamicMediaRequest.then(view => {
                Logger.log('詳細資料請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostDynamicMediaRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('詳細資料請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.DetailMeta}">${UI.Error.ErrorSegment().outerHTML
                        }</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.DetailMeta}">${UI.Error.CorrectSegment('加載新一筆詳細資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ fsSUBJECT_ID: string; mediaType: string; fileNo: string }>(
        //     'POST',
        //     api.DynamicMedia,
        //     {
        //         fsSUBJECT_ID: input.fsSUBJECT_ID,
        //         mediaType: input.type,
        //         fileNo: input.fileNo,
        //     },
        //     false
        // );
    }
    KeyFrame(fileNo: string): Promise<string> {
        if (!IsNULL(this.PostKeyFrameRequest)) {
            this.PostKeyFrameRequest.abort();
            Logger.log('取消最近一次關鍵影格請求');
        }
        this.PostKeyFrameRequest = PostJXR<{ fileNo: string }>(this.api.KeyFrame, { fileNo: fileNo }, false);
        return new Promise(resolve => {
            this.PostKeyFrameRequest.then(view => {
                Logger.log('關鍵影格請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostKeyFrameRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('關鍵影格請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.KeyFrame
                        }" style="height: 100%;width:100%;min-width:100%;">${UI.Error.ErrorSegment().outerHTML}</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.KeyFrame
                        }" style="height: 100%;width:100%;min-width:100%;">${UI.Error.CorrectSegment('加載新一筆關鍵影格資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ fileNo: string }>('POST', api.KeyFrame, { fileNo: fileNo }, false);
    }
    Paragraph(mediaType: MediaType, fileNo: string): Promise<string> {
        if (!IsNULL(this.PostParagraphRequest)) {
            this.PostParagraphRequest.abort();
            Logger.log('取消最近一次段落描述請求');
        }
        this.PostParagraphRequest = PostJXR<{ mediaType: string; fileNo: string }>(
            this.api.Paragraph,
            { mediaType: mediaType, fileNo: fileNo },
            false
        );
        return new Promise(resolve => {
            this.PostParagraphRequest.then(view => {
                Logger.log('段落描述請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostParagraphRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('段落描述請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.Paragraph
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.ErrorSegment().outerHTML}</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.Paragraph
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.CorrectSegment('加載新一筆段落描述資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ mediaType: string; fileNo: string }>(
        //     'POST',
        //     api.Paragraph,
        //     { mediaType: mediaType, fileNo: fileNo },
        //     false
        // );
    }
    AudioAlbumInfo(subjectId: string, fileNo: string): Promise<string> {
        if (!IsNULL(this.PostAudioAlbumInfoRequest)) {
            this.PostAudioAlbumInfoRequest.abort();
            Logger.log('取消最近一次聲音專輯請求');
        }
        this.PostAudioAlbumInfoRequest = PostJXR<{ fsSUBJECT_ID: string; fileNo: string }>(
            this.api.AudioAlbumInfo,
            { fsSUBJECT_ID: subjectId, fileNo: fileNo },
            false
        );
        return new Promise(resolve => {
            this.PostAudioAlbumInfoRequest.then(view => {
                Logger.log('聲音專輯請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostAudioAlbumInfoRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('聲音專輯請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.albumInfo
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.ErrorSegment().outerHTML}</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.albumInfo
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.CorrectSegment('加載新一筆聲音專輯資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ fsSUBJECT_ID: string; fileNo: string }>(
        //     'POST',
        //     api.AudioAlbumInfo,
        //     { fsSUBJECT_ID: subjectId, fileNo: fileNo },
        //     false
        // );
    }
    PhotoExifInfo(fileNo: string): Promise<string> {
        if (!IsNULL(this.PostPhotoExifInfoRequest)) {
            this.PostPhotoExifInfoRequest.abort();
            Logger.log('取消最近一次圖片資訊請求');
        }
        this.PostPhotoExifInfoRequest = PostJXR<{ fileNo: string }>(this.api.PhotoExifInfo, { fileNo: fileNo }, false);
        return new Promise(resolve => {
            this.PostPhotoExifInfoRequest.then(view => {
                Logger.log('圖片資訊請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostPhotoExifInfoRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('圖片資訊請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.exifInfo
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.ErrorSegment().outerHTML}</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.exifInfo
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.CorrectSegment('加載新一筆圖片資訊資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ fileNo: string }>('POST', api.PhotoExifInfo, { fileNo: fileNo }, false);
    }
    DocInfo(fileNo: string): Promise<string> {
        if (!IsNULL(this.PostDocInfoRequest)) {
            this.PostDocInfoRequest.abort();
            Logger.log('取消最近一次文件資訊請求');
        }
        this.PostDocInfoRequest = PostJXR<{ fileNo: string }>(this.api.DocInfo, { fileNo: fileNo }, false);
        return new Promise(resolve => {
            this.PostDocInfoRequest.then(view => {
                Logger.log('文件資訊請求完成');
                resolve(view);
            }).catch(error => {
                if (this.PostDocInfoRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('文件資訊請求錯誤,原因:', error);
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.documentInfo
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.ErrorSegment().outerHTML}</div>`
                    );
                } else {
                    resolve(
                        `<div class="ui bottom attached tab" data-tab="${TabNameEnum.documentInfo
                        }" style="flex-direction: column;padding: 0 .3em;">${UI.Error.CorrectSegment('加載新一筆文件資訊資料', '請求中', 'spinner loading').outerHTML
                        }</div>`
                    );
                }
            });
        });
        // return Ajax<{ fileNo: string }>('POST', api.DocInfo, { fileNo: fileNo }, false);
    }
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse> {
        return AjaxPost<{ models: Array<CreateMaterialModel> }>(this.api.AddingBooking, { models: input }, false);
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
    EditMedia(serializeFormData: string): Promise<IResponse> {
        return AjaxFormSerialize(this.api.EditMedia, serializeFormData, false);
    }
}
