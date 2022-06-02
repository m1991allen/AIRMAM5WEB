import { BaseController, IBaseController } from './BaseController';
import { AjaxPost, Ajax, AjaxFormSerialize, PostJXR } from '../Function/Ajax';
import { API_Subject } from '../Const/api';
import { MediaType } from '../Enum/MediaType';
import { PrimaryButton } from '../Templete/ButtonTemp';
import { IResponse } from '../Interface/Shared/IResponse';
import { ShowModal } from '../Function/Modal';
import { SelectListItem } from '../Interface/Shared/ISelectListItem';
import { TabNameEnum } from '../Enum/TabNameEnum';
import { JsonDateToDate } from '../Function/Date';
import * as dayjs_ from 'dayjs';
import { IsNULLorEmpty, IsNULL } from '../Function/Check';
import { selectids } from '../../Views/Subject/Index';
import { CreateMaterialModel } from '../Interface/Materia/CreateMaterialModel';
import { PermissionDefinition } from '../Enum/PermissionDefinition';
import { SubjectIdModel } from '../Interface/Subject/SubjectIdModel';
import { DeleteMediaModel } from '../Interface/Subject/DeleteMediaModel';
import { KeyFrameCUModel } from '../Interface/Subject/KeyFrameCUModel';
import { SubjectModel } from '../Interface/Subject/SubjectModel';
import { GetKeyFrameModel } from '../Interface/Subject/GetKeyFrameModel';
import { ParagraphCUModel } from '../Interface/Subject/ParagraphCUModel';
import { GetParagraphModel } from '../Interface/Subject/GetParagraphModel';
import { SubjectPreviewModel } from '../Interface/Subject/SubjectPreviewModel';
import { MediaDataInputModel } from '../Interface/Subject/MediaDataInputModel';
import { FileListInputModel } from '../Interface/Subject/FileListInputModel';
import { KeyFrameInputModel } from '../Interface/Subject/KeyFrameInputModel';
import { RetransferModel } from '../Interface/Subject/RetransferModel';
import { MediaDataModel } from '../Interface/Subject/MediaDataModel';
import { KeyFrameDataModel } from '../Interface/Subject/KeyFrameDataModel';
import { ParaDescriptionModel } from '../Interface/Subject/ParaDescriptionModel';
import { UI } from '../Templete/CompoentTemp';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { DecoratorConvert } from '../Function/DecoratorConvert';
import { UserFileSubjectAuthModel } from '../Interface/Subject/UserFileAuthModel';
import { Logger } from '../Class/LoggerService';
import { resolve } from 'dns';
import { DocumentSetting } from '../initSetting';
/*宣告變數*/
const dayjs = (<any>dayjs_).default || dayjs_;

/**暫存（關鍵影格）請求 */
// var GetKeyFrameRequest: Array<JQueryXHR> = [];
/**
 * 介面
 */
export interface ISubjectController extends IBaseController<API_Subject> {
    /**JSON:新增主題 */
    CreateSubject(serializeFormData: string): Promise<IResponse>;
    /**JSON:編輯主題 */
    EditSubject(serializeFormData: string): Promise<IResponse>;
    /**JSON:刪除主題 */
    DeleteSubject(input: SubjectIdModel): Promise<IResponse>;
    /**JSON:取上傳View所需資料 */
    UploadView(id: number, subjid: string): Promise<IResponse>;
    /**
     * JSON:由媒體類型取得預編詮釋資料
     * @param type 媒體類型
     * @param templeteId　樣板Id
     */
    GetArcPreList(type: MediaType, templeteId: number): Promise<Array<SelectListItem>>;
    /**加入借調 */
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse>;
    /**JSON:編輯指定檔案的媒體資料 */
    EditMedia(serializeFormData: string): Promise<IResponse>;
    /**JSON:刪除指定檔案的媒體資料 */
    DeleteMedia(input: DeleteMediaModel): Promise<IResponse>;
    /**重轉 */
    Retransfer(input: SubjectModel): Promise<IResponse>;
    /**JSON:新增關鍵影格 */
    AddKeyFrame(input: KeyFrameCUModel): Promise<IResponse>;
    /**JSON:編輯關鍵影格 */
    EditKeyFrame(input: KeyFrameCUModel): Promise<IResponse>;
    /**JSON:刪除關鍵影格 */
    DeleteKeyFrame(input: GetKeyFrameModel): Promise<IResponse>;
    /**JSON:設置關鍵影格的代表圖 */
    SetHeadFrame(input: GetKeyFrameModel): Promise<IResponse>;

    /**新增段落描述 */
    AddParagraph(input: ParagraphCUModel): Promise<IResponse>;
    /**編輯段落描述 */
    EditParagraph(input: ParagraphCUModel): Promise<IResponse>;
    /**刪除段落描述 */
    DeleteParagraph(input: GetParagraphModel): Promise<IResponse>;

    /**View:編輯指定檔案的媒體資料燈箱 */
    EditMediaView(MainModalId: string, EditModalId: string, input: SubjectModel): Promise<boolean>;
    /**View:批次編輯指定檔案的媒體資料燈箱 */
    EditBatchMediaView(
        MainModalId: string,
        EditModalId: string,
        input: { type: string; SubjectId: string; fileNo: Array<string> }
    ): Promise<boolean>;
    /**View:刪除指定檔案的媒體資料燈箱 */
    DeleteMediaView(MainModalId: string, DeleteModalId: string, type: string, fileno: string): Promise<boolean>;
    /**View:編輯關鍵影格燈箱*/
    EditKeyframeView(MainModalId: string, EditModalId: string, input: GetKeyFrameModel): Promise<boolean>;
    /**ViewL刪除關鍵影格燈箱 */
    DeleteKeyframeView(MainModalId: string, DeleteModalId: string, input: GetKeyFrameModel): Promise<boolean>;
    /**View:編輯段落描述燈箱  */
    EditParagraphView(MainModalId: string, EditModalId: string, input: GetParagraphModel): Promise<boolean>;
    /**View:刪除段落描述燈箱  */
    DeleteParagraphView(MainModalId: string, DeleteModalId: string, input: GetParagraphModel): Promise<boolean>;
    /**View:置換燈箱 */
    ReplacementView(MainModalId: string, ReplaceModalId: string, input: SubjectModel);

    /**
     * View:媒體預覽圖
     * @param input 查詢參數
     * @param DirAuth 此使用者可使用的權限功能
     */
    Preview(input: SubjectPreviewModel, DirAuth: Array<PermissionDefinition>): Promise<string>;
    /**
     * View:媒體資訊
     * @param input 查詢參數
     * @param DirAuth 此使用者可使用的權限功能
     */
    MediaView(input: MediaDataInputModel, DirAuth: Array<PermissionDefinition>): Promise<string>;
    /**JSON:媒體列表 */
    List(input: FileListInputModel): Promise<IResponse>;
    /**
     * View:關鍵影格Placeholder
     * @param fileNo 檔案編號
     * @param dataLength 關鍵影格數量
     * @param DirAuth 此使用者可使用的權限功能
     */
    KeyFramePlaceholderView(fileNo: string, dataLength: number, DirAuth: Array<PermissionDefinition>): Promise<string>;
    /**
     * Function:取代關鍵影格
     * @param input 查詢參數
     * @param DirAuth 此使用者可使用的權限功能
     */
    KeyFrameView(input: KeyFrameInputModel, DirAuth: Array<PermissionDefinition>): void;
    /**
     * View:段落描述
     * @param input 查詢參數
     * @param DirAuth 此使用者可使用的權限功能
     */
    ParagraphDesView(input: FileListInputModel, DirAuth: Array<PermissionDefinition>): Promise<string>;
}

/** 歸檔搬遷_靜態方法Controller */
class SubjectStaticController extends BaseController<API_Subject> {
    private static readonly functionAuthApi: string = GetUrl(Controller.Subject, Action.GetUserDirAuth).href;
    private static readonly subjectAuthApi: string = GetUrl(Controller.Subject, Action.GetUserSubjAuth).href;
    private static readonly fileAuthApi: string = GetUrl(Controller.Subject, Action.GetUserFileAuth).href;
    constructor() {
        super({
            /**JSON:節點的主題列表 */
            Search: GetUrl(Controller.Subject, Action.Search).href,
            /**View:新增主題 */
            ShowCreate: GetUrl(Controller.Subject, Action.ShowCreate).href,
            /**View:編輯主題*/
            ShowEdit: GetUrl(Controller.Subject, Action.ShowEdit).href,
            /**View:刪除主題 */
            ShowDelete:GetUrl(Controller.Subject,Action.ShowDelete).href,
            /**View:檢視主題資訊 */
            ShowDetail: GetUrl(Controller.Subject, Action.ShowDetails).href,
            /**View:編輯MediaData */
            ShowEditMedia: GetUrl(Controller.Subject, Action.ShowEditMedia).href,
            /**View:刪除MediaData */
            ShowDeleteMedia: GetUrl(Controller.Subject, Action.ShowDeleteMedia).href,
            /**View:加關鍵影格 */
            ShowAddKeyFrameView: GetUrl(Controller.Subject, Action.ShowKeyFrameAdd).href,
            /**View:編輯關鍵影格 */
            ShowEditKeyFrameView: GetUrl(Controller.Subject, Action.ShowKeyFrameEdit).href,
            /**View:刪除關鍵影格 */
            ShowDeleteKeyFrameView: GetUrl(Controller.Subject, Action.ShowKeyFrameDelete).href,
            /**媒體預覽 */
            Preview: GetUrl(Controller.Subject, Action.ShowPreview).href,
            /**View:新增段落描述 */
            ShowAddParagraphView: GetUrl(Controller.Subject, Action.ShowAddParagraphView).href,
            /**View:編輯段落描述 */
            ShowEditParagraphView: GetUrl(Controller.Subject, Action.ShowEditParagraphView).href,
            /**View:刪除段落描述 */
            ShowDeleteParagraphView: GetUrl(Controller.Subject, Action.ShowDeleteParagraphView).href,
            /**View:置換 */
            ShowReplacementView: GetUrl(Controller.Subject, Action.ShowChangeMedia).href,
            /**JSON:新增主題 */
            Create: GetUrl(Controller.Subject, Action.Create).href,
            /**JSON:編輯主題 */
            Edit: GetUrl(Controller.Subject, Action.Edit).href,
            /**JSON:刪除主題 */
            Delete: GetUrl(Controller.Subject, Action.Delete).href,
            /**JSON:編輯指定檔案的媒體資料 */
            EditMedia: GetUrl(Controller.Subject, Action.EditMedia).href,
            /**JSON:刪除指定檔案的媒體資料 */
            DeleteMedia: GetUrl(Controller.Subject, Action.DeleteMedia).href,
            /**JSON:重轉 */
            Retransfer: GetUrl(Controller.Subject, Action.ReTransfer).href,
            /**JSON:取得預編詮釋資料 */
            GetArcPreList: GetUrl(Controller.Shared, Action.GetArcPreList).href,
            /**JSON:取得樹狀節點的目錄權限 */
            GetUserDirAuth: GetUrl(Controller.Subject, Action.GetUserDirAuth).href,
            /**JSON:取得主題的目錄權限 */
            GetUserSubjectAuth: GetUrl(Controller.Subject, Action.GetUserSubjAuth).href,
            /**JSON:取得檔案的目錄權限 */
            GetUserFileAuth: GetUrl(Controller.Subject, Action.GetUserFileAuth).href,
            /**JSON:新增關鍵影格 */
            AddKeyFrame: GetUrl(Controller.Subject, Action.KeyFrameAdd).href,
            /**JSON:編輯關鍵影格 */
            EditKeyFrame: GetUrl(Controller.Subject, Action.KeyFrameEdit).href,
            /**JSON:刪除關鍵影格 */
            DeleteKeyFrame: GetUrl(Controller.Subject, Action.KeyFrameDelete).href,
            /**JSON:設置關鍵影格的代表圖 */
            SetHeadFrame: GetUrl(Controller.Subject, Action.SetHeadFrame).href,

            /**JSON:新增段落描述 */
            AddParagraph: GetUrl(Controller.Subject, Action.AddParagraph).href,
            /**JSON:編輯段落描述 */
            EditParagraph: GetUrl(Controller.Subject, Action.EditParagraph).href,
            /**JSON:刪除段落描述 */
            DeleteParagraph: GetUrl(Controller.Subject, Action.DeleteParagraph).href,

            /**JSON:上傳View所需資料 */
            ShowUpload: GetUrl(Controller.Subject, Action.ShowUpload).href,
            /**JSON:媒體預覽 */
            ShowViewer: GetUrl(Controller.Subject, Action.ShowViewer).href,
            /**JSON:媒體列表 */
            ShowList: GetUrl(Controller.Subject, Action.ShowList).href,
            /**JSON:關鍵影格 */
            ShowKeyFrame: GetUrl(Controller.Subject, Action.ShowKeyFrame).href,
            /**JSON:媒體資料 */
            ShowMetaData: GetUrl(Controller.Subject, Action.ShowMetaData).href,
            /**JSON:段落描述 */
            ShowParaDescription: GetUrl(Controller.Subject, Action.ShowParaDescription).href,
            /**JSON:加入借調 */
            AddingBooking: GetUrl(Controller.Booking, Action.AddBooking).href,
        });
    }
    /**
     * 取得使用者在該目錄節點對於主題/影/音/圖/文的權限
     * @param mediaType 媒體類型(S、V、A、D、P)
     * @param NodeId 節點Id
     */
    static GetDirFunctionAuth = (mediaType: MediaType, NodeId: number): Promise<UserFileSubjectAuthModel> => {
        return new Promise(resolve=>{
            Ajax<{ id: number }>('GET', SubjectStaticController.functionAuthApi, { id: NodeId }, false).then(res=>{
                IsNULL(res)?resolve(<UserFileSubjectAuthModel>{}):resolve(res);
            }).catch(error=>{
                Logger.error(error);
                resolve(<UserFileSubjectAuthModel>{});
            });
        });
       // return Ajax<{ id: number }>('GET', SubjectStaticController.functionAuthApi, { id: NodeId }, false);
    };
    /**
     * 取得使用者在該主題編號對於主題/影/音/圖/文的權限
     * @param mediaType 媒體類型(S、V、A、D、P)
     * @param SubjectIds 單個主題編號
     */
    static GetSubjectFunctionAuth = (mediaType: MediaType, SubjectId: string): Promise<UserFileSubjectAuthModel> => {
        return new Promise(resolve=>{
            Ajax<{ id: string }>('GET', SubjectStaticController.subjectAuthApi, { id: SubjectId }).then(res=>{
                IsNULL(res)?resolve(<UserFileSubjectAuthModel>{}):resolve(res);
            }).catch(error=>{
                Logger.error(error);
                resolve(<UserFileSubjectAuthModel>{});
            });
        });
       // return Ajax<{ id: string }>('GET', SubjectStaticController.subjectAuthApi, { id: SubjectId });
    };

    /**
     * 取得使用者在該檔案對於主題/影/音/圖/文的權限
     * @param mediaType 媒體類型(S、V、A、D、P)
     * @param FileNos  單個檔案編號
     */
    static GetFileFunctionAuth = (mediaType: MediaType, FileNo: string): Promise<UserFileSubjectAuthModel> => {
        return new Promise(resolve=>{
            Ajax<{ id: string }>('GET', SubjectStaticController.fileAuthApi, { id: FileNo }).then(res=>{
                IsNULL(res)?resolve(<UserFileSubjectAuthModel>{}):resolve(res);
            }).catch(error=>{
                Logger.error(error);
                resolve(<UserFileSubjectAuthModel>{});
            });
        });
      //  return Ajax<{ id: string }>('GET', SubjectStaticController.fileAuthApi, { id: FileNo });
    };
    static get api() {
        return new this.api();
    }
}

/**
 * 主題路由
 */
@DecoratorConvert
export class SubjectController extends SubjectStaticController implements ISubjectController {
    // private EditMediaViewRequest: JQueryXHR = null;
    // private EditBatchMediaViewRequest: JQueryXHR = null;
    // private DeleteMediaViewRequest: JQueryXHR = null;
    // private EditKeyframeViewRequest: JQueryXHR = null;
    // private DeleteKeyframeViewRequest: JQueryXHR = null;
    // private EditParagraphViewRequest: JQueryXHR = null;
    // private DeleteParagraphViewRequest: JQueryXHR = null;
    // private ReplacementViewRequest: JQueryXHR = null;
    private PreviewRequest: JQueryXHR = null;
    private MediaViewRequest: JQueryXHR = null;
    // private KeyFrameViewRequest: JQueryXHR = null;
    private ParagraphDesViewRequest: JQueryXHR = null;
    private GetKeyFrameRequest: Array<JQueryXHR> = [];
    // private KeyFrameViewRequestTime: number = 0;
    constructor() {
        super();
    }
    static get api() {
        return new this.api();
    }
    CreateSubject(serializeFormData: string): Promise<IResponse> {
        return AjaxFormSerialize(this.api.Create, serializeFormData, false);
    }
    EditSubject(serializeFormData: string): Promise<IResponse> {
        return AjaxFormSerialize(this.api.Edit, serializeFormData, false);
    }
    DeleteSubject(input: SubjectIdModel): Promise<IResponse> {
        return AjaxPost(this.api.Delete, { subjid: input.fsSUBJECT_ID }, false);
    }
    UploadView(id: number, subjid: string) {
        return AjaxPost<{ id: number; subjid: string }>(this.api.ShowUpload, { id: id, subjid: subjid }, false);
    }
    GetArcPreList(type: MediaType, templeteId: number): Promise<Array<SelectListItem>> {
        return Ajax<{ type: MediaType; template: number; noopts: boolean }>(
            'POST',
            this.api.GetArcPreList,
            { type: type, template: templeteId, noopts: true },
            false
        );
    }
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse> {
        return AjaxPost<{ models: Array<CreateMaterialModel> }>(this.api.AddingBooking, { models: input }, false);
    }
    EditMedia(serializeFormData: string): Promise<IResponse> {
        return AjaxFormSerialize(this.api.EditMedia, serializeFormData, false);
    }
    DeleteMedia(input: DeleteMediaModel): Promise<IResponse> {
        return AjaxPost<DeleteMediaModel>(this.api.DeleteMedia, input, false);
    }
    Retransfer(input: SubjectModel): Promise<IResponse> {
        return AjaxPost<RetransferModel>(
            this.api.Retransfer,
            {
                fsSUBJECT_ID: input.SubjectId,
                fsFILE_NO: input.fileNo,
                FileCategory: <'V' | 'A'>input.type,
            },
            false
        );
    }
    AddKeyFrame(input: KeyFrameCUModel): Promise<IResponse> {
        return AjaxPost<KeyFrameCUModel>(this.api.AddKeyFrame, input, false);
    }
    EditKeyFrame(input: KeyFrameCUModel): Promise<IResponse> {
        return AjaxPost<KeyFrameCUModel>(this.api.EditKeyFrame, input, false);
    }
    DeleteKeyFrame(input: GetKeyFrameModel): Promise<IResponse> {
        return AjaxPost<GetKeyFrameModel>(this.api.DeleteKeyFrame, input, false);
    }
    SetHeadFrame(input: GetKeyFrameModel): Promise<IResponse> {
        return AjaxPost<GetKeyFrameModel>(this.api.SetHeadFrame, input, false);
    }
    AddParagraph(input: ParagraphCUModel): Promise<IResponse> {
        return AjaxPost<ParagraphCUModel>(this.api.AddParagraph, input, false);
    }
    EditParagraph(input: ParagraphCUModel): Promise<IResponse> {
        return AjaxPost<ParagraphCUModel>(this.api.EditParagraph, input, false);
    }
    DeleteParagraph(input: GetParagraphModel): Promise<IResponse> {
        return AjaxPost<GetParagraphModel>(this.api.DeleteParagraph, input, false);
    }
    EditMediaView(MainModalId: string, EditModalId: string, input: SubjectModel): Promise<boolean> {
        return ShowModal<{ subjid: string; type: string; fileno: string }>(
            EditModalId,
            this.api.ShowEditMedia,
            { subjid: input.SubjectId, type: input.type, fileno: input.fileNo },
            [MainModalId, "div[data-tab='" + TabNameEnum.MetaData + "']"].join(' ')
        );
    }
    EditBatchMediaView(
        MainModalId: string,
        EditModalId: string,
        input: { type: string; SubjectId: string; fileNo: Array<string> }
    ): Promise<boolean> {
        return ShowModal<{ subjid: string; type: string; fileno: string }>(
            EditModalId,
            this.api.ShowEditMedia,
            { subjid: input.SubjectId, type: input.type, fileno: input.fileNo.join(',') },
            [MainModalId, "div[data-tab='" + TabNameEnum.MetaData + "']"].join(' ')
        );
    }
    DeleteMediaView(MainModalId: string, DeleteModalId: string, type: string, fileno: string): Promise<boolean> {
        return ShowModal<{ type: string; fileno: string }>(
            DeleteModalId,
            this.api.ShowDeleteMedia,
            { type: type, fileno: fileno },
            [MainModalId, "div[data-tab='" + TabNameEnum.MetaData + "']"].join(' ')
        );
    }
    EditKeyframeView(MainModalId: string, EditModalId: string, input: GetKeyFrameModel): Promise<boolean> {
        return ShowModal<GetKeyFrameModel>(
            EditModalId,
            this.api.ShowEditKeyFrameView,
            input,
            [MainModalId, "div[data-tab='" + TabNameEnum.KeyFrame + "']"].join(' ')
        );
    }
    DeleteKeyframeView(MainModalId: string, DeleteModalId: string, input: GetKeyFrameModel): Promise<boolean> {
        return ShowModal<GetKeyFrameModel>(
            DeleteModalId,
            this.api.ShowDeleteKeyFrameView,
            input,
            [MainModalId, "div[data-tab='" + TabNameEnum.KeyFrame + "']"].join(' ')
        );
    }
    EditParagraphView(MainModalId: string, EditModalId: string, input: GetParagraphModel): Promise<boolean> {
        return ShowModal<GetParagraphModel>(
            EditModalId,
            this.api.ShowEditParagraphView,
            { type: input.type, fileNo: input.fileNo, seqno: input.seqno },
            [MainModalId, "div[data-tab='" + TabNameEnum.Paragraph + "']"].join(' ')
        );
    }
    DeleteParagraphView(MainModalId: string, DeleteModalId: string, input: GetParagraphModel): Promise<boolean> {
        return ShowModal<GetParagraphModel>(
            DeleteModalId,
            this.api.ShowDeleteParagraphView,
            { type: input.type, fileNo: input.fileNo, seqno: input.seqno },
            [MainModalId, "div[data-tab='" + TabNameEnum.Paragraph + "']"].join(' ')
        );
    }
    ReplacementView(MainModalId: string, ReplaceModalId: string, input: SubjectModel) {
        return ShowModal<{ subjid: string; type: string; fileno: string }>(
            ReplaceModalId,
            this.api.ShowReplacementView,
            {
                subjid: input.SubjectId,
                type: input.type,
                fileno: input.fileNo,
            },
            [MainModalId, "div[data-tab='" + TabNameEnum.MetaData + "']"].join(' ')
        );
    }
    Preview(input: SubjectPreviewModel, DirAuth: Array<PermissionDefinition>): Promise<string> {
        return new Promise(resolve => {
            if (IsNULLorEmpty(input.type) || IsNULLorEmpty(input.fsSUBJECT_ID)) {
                Logger.error(`載入預覽頁發生錯誤,因為傳入的參數有問題`);
                resolve(UI.Error.ErrorSegment('Oops,載入預覽頁發生錯誤', '看起來發生了一些狀況...').outerHTML);
            } else {
                if (!IsNULL(this.PreviewRequest)) {
                    this.PreviewRequest.abort();
                    Logger.log('取消最近一次預覽畫面請求');
                }
                this.PreviewRequest = PostJXR<{ fsSUBJECT_ID: string; type: string; fileNo: string }>(
                    this.api.Preview,
                    { fsSUBJECT_ID: input.fsSUBJECT_ID, type: input.type, fileNo: input.fileNo },
                    false
                );

                this.PreviewRequest.then(view => {
                    Logger.log('預覽畫面請求完成');
                    resolve(view);
                }).catch(error => {
                    if (this.PreviewRequest.statusText.toLowerCase() != 'abort') {
                        Logger.error('預覽畫面請求錯誤,原因:', error);
                        resolve(UI.Error.ErrorSegment().outerHTML);
                    } else {
                        resolve(UI.Error.CorrectSegment('加載新一筆預覽畫面', '請求中', 'spinner loading').outerHTML);
                    }
                });
            }
        });
    }
    MediaView(input: MediaDataInputModel, DirAuth: Array<PermissionDefinition>): Promise<string> {
        if (!IsNULL(this.MediaViewRequest)) {
            this.MediaViewRequest.abort();
            Logger.log('取消最近一次媒體資訊畫面請求');
        }
        this.MediaViewRequest = PostJXR<MediaDataInputModel>(this.api.ShowMetaData, input, false);
        return new Promise(resolve => {
            this.MediaViewRequest.then(json => {
                const res = <IResponse>json;
                const data: MediaDataModel = <MediaDataModel>res.Data;
                if (res.IsSuccess) {
                    if (!IsNULLorEmpty(data)) {
                        const isDisabled = selectids.length > 1 ? 'disabled' : ''; /*當選擇多列,只允許批次功能*/
                        const buttonArray:Array<{sort:number;btnTemplete:string}>=[];
                        /**版權是否調用禁止 */const disabledBOOKING =data.IsExpired|| data.IsForBid ? "disabled" : "";
                        /**版權是否調用提醒、訊息內容 */const licenseMSG =data.IsExpired|| data.IsAlert || data.IsForBid ? `(${data.LicenseMessage})${IsNULLorEmpty(data.LicenseEndDate)?"":"[ "+data.LicenseEndDate+" ]"}` : "";
                        switch(true){
                            case  DirAuth.indexOf(PermissionDefinition.B) > -1:
                                buttonArray.push({sort:1,btnTemplete:`<button type="button" class="ui _darkGrey ${isDisabled} mini button ${disabledBOOKING?'red':''}"  name="addMateria" data-Id="${data.fsFILE_NO}" ${disabledBOOKING} alertMessage="${data.IsAlert?data.LicenseMessage:""}">${data.IsExpired?'<i class="stop icon"></i>授權到期': disabledBOOKING?'<i class="stop icon"></i>禁止借調':'<i class="add icon"></i>加入借調'}</button>`});
                            case  DirAuth.indexOf(PermissionDefinition.D) > -1:
                                buttonArray.push({sort:2,btnTemplete: `<button type="button" class="ui _darkGrey ${isDisabled} mini button"  name="delete" data-Id="${data.fsFILE_NO}"><i class="delete icon"></i>刪除</button>`});
                            case  DirAuth.indexOf(PermissionDefinition.U) > -1:
                                buttonArray.push({sort:3,btnTemplete:`<button type="button" class="ui _darkGrey ${isDisabled} mini button"  name="edit" data-Id="${data.fsFILE_NO}"><i class="edit icon"></i>修改</button>`});
                                buttonArray.push({sort:4,btnTemplete:`<button type="button" class="ui _darkGrey  mini  button"  name="batchedit" data-Id="${data.fsFILE_NO}"><i class="edit icon"></i>批次修改</button>`});
                                if(DocumentSetting.ShowDocSystemButton && input.type===MediaType.VIDEO){
                                    buttonArray.push({sort:7,btnTemplete:`<button type="button" class="ui _darkGrey mini button ${isDisabled}"  name="docsystem" data-Id="${data.fsFILE_NO}"><i class="compass icon"></i>文稿設定</button>`});
                                }
                                if(DocumentSetting.ShowOfficeDocSystemButton && input.type===MediaType.Doc){
                                    buttonArray.push({sort:7,btnTemplete:`<button type="button" class="ui _darkGrey mini button ${isDisabled}"  name="docsystem" data-Id="${data.fsFILE_NO}"><i class="compass icon"></i>公文設定</button>`});
                                }
                            case DirAuth.indexOf(PermissionDefinition.I) > -1:
                                buttonArray.push({sort:5,btnTemplete:`<button type="button" class="ui _darkGrey mini ${isDisabled} button"  name="reTransfer" data-Id="${data.fsFILE_NO}"><i class="redo icon"></i>重轉</button>`});
                                buttonArray.push({sort:6,btnTemplete:`<button type="button" class="ui _darkGrey mini ${isDisabled} button"  name="replaceMent" data-Id="${data.fsFILE_NO}"><i class="upload icon"></i>置換</button>`});
                                break;
                        }
                        const buttons=buttonArray.sort((a, b) => a.sort - b.sort).map(item=>item.btnTemplete).join("");
                        const hashTags=IsNULLorEmpty(data.HashTag)?"": data.HashTag.split('^').filter(x=>x!="").map(x=>`<label class="ui blue x-hashtag label">#`+x+`</label>`).join("");//處理自訂標籤樣式
                        const fieldsList = data.ArcPreAttributes;
                        const userdateInfo = data.UserDateInfo;
                        let insDt = dayjs(JsonDateToDate(userdateInfo.CreatedDate)).format('YYYY/MM/DD HH:mm:ss');
                        let insDt2 = dayjs(JsonDateToDate(userdateInfo.UpdatedDate)).format('YYYY/MM/DD HH:mm:ss');
                        let attrField = ``;
                        if (fieldsList.length > 0) {
                            for (let i = 0; i < fieldsList.length; i++) {
                                const element = fieldsList[i];
                                attrField += `<div class="ui right aligned grid _styleMarginZero">
                                     <label class="center aligned three wide column" for="${element.Field}">${element.FieldName}</label>
                                     <span class="left aligned thirteen wide column">${element.FieldValue}</span>
                                     </div>`;
                            }
                        }
                        /** added_20210831_語音轉文字 */
                        if (data.FileCategory == 'V' || data.FileCategory == 'A')
                        {
                            attrField += `<div class="ui right aligned grid _styleMarginZero">
                            <label class="center aligned three wide column" for="fsFILE_NO">語音轉文字</label>
                            <span class="left aligned thirteen wide column">${data.Voice2TextContent}</span>
                            </div>`;
                        }//
                        const temp = `<div class="ui stackable grid x-grid">
                                        <div class="sixteen wide column x-fixed">
                                          <input type='hidden' value="${data.fsFILE_NO}">
                                          ${buttons}
                                        </div>
                                       <div class="x-dataview">
                                            <div class="_directoryTab"> 
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsFILE_NO">檔案編號</label>
                                                 <span class="left aligned thirteen wide column">${data.fsFILE_NO}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="Title">標題</label>
                                                 <span class="left aligned thirteen wide column">${data.Title}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">描述</label>
                                                 <span class="left aligned thirteen wide column">${data.Description}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">自訂標籤</label>
                                                 <span class="left aligned thirteen wide column">${hashTags}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">機密等級</label>
                                                 <span class="left aligned thirteen wide column">${data.FileSecretStr} </span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">版權</label>
                                                 <span class="left aligned thirteen wide column">${data.LicenseStr} ${licenseMSG} </span>
                                              </div>
                                              ${attrField}
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">原始檔名</label>
                                                 <span class="left aligned thirteen wide column">${data.OriginFileName}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">新增時間</label>
                                                 <span class="left aligned thirteen wide column">${insDt}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">新增人員</label>
                                                 <span class="left aligned thirteen wide column">${userdateInfo.CreatedBy}` + (IsNULLorEmpty(userdateInfo.CreatedByName) ? '' : '(' + userdateInfo.CreatedByName + ')') + `</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">修改時間</label>
                                                 <span class="left aligned thirteen wide column">${insDt2}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">修改人員</label>
                                                 <span class="left aligned thirteen wide column">${userdateInfo.UpdatedBy}` + (IsNULLorEmpty(userdateInfo.UpdatedByName) ? '' : '(' + userdateInfo.UpdatedByName + ')') + `</span>
                                            </div>
                                           </div>
                                       </div>
                                </div>`;
                        resolve(temp);
                    } else {
                        resolve(UI.Error.ErrorSegment('目前沒有資料', '~嘗試查詢其他檔案吧~').outerHTML);
                    }
                } else {
                    resolve(UI.Error.ErrorSegment().outerHTML);
                }
            }).catch(error => {
                if (this.MediaViewRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('媒體資訊畫面請求錯誤,原因:', error);
                    resolve(UI.Error.ErrorSegment().outerHTML);
                } else {
                    resolve(UI.Error.CorrectSegment('加載新一筆媒體資訊畫面', '請求中', 'spinner loading').outerHTML);
                }
            });
        });
    }
    List(input: FileListInputModel): Promise<IResponse> {
        return AjaxPost<FileListInputModel>(this.api.ShowList, input, false);
    }
    KeyFramePlaceholderView(fileNo: string, dataLength: number, DirAuth: Array<PermissionDefinition>): Promise<string> {
        const addkfBtn = DirAuth.indexOf(PermissionDefinition.I) > -1 ? PrimaryButton(fileNo, 'add', '新增', 'create', 'button',['mini']) : '';
        const templete = (cards: string): string => {
            return ` <div class="x-keybuttons">${addkfBtn}
                     <button name="cardInfo" type="button" class="ui blue icon button right floated" title="影格資訊顯示/隱藏"><i class="eye icon slash"></i></button></div>
                     <div class="x-keycards"><div class="ui four doubling stackable cards">${cards}</div></div> `;
        };
        return new Promise((resolve, reject) => {
            let cards: string = '';
            for (let i = 0; i < dataLength; i++) {
                cards += `<div class="ui card placecard">
                     <div class="ui inverted placeholder">
                       <div class="square image">
                         <label class="ui top right attached black link label"> <div class="line"></div></label>
                       </div>
                     </div>
                     <div class="content">
                        <div class="ui active inverted placeholder">
                           <div class="paragraph">
                             <div class="line"></div>
                           </div>
                        </div>
                     </div>
                     <div class="ui bottom attached buttons">
                     <div class="ui mini icon black button"><div class="line"></div></div>
                     <div class="ui mini icon black button"><div class="line"></div></div>
                     </div>
                   </div>`;
            }
            resolve(templete(cards));
        });
    }
    KeyFrameView(input: KeyFrameInputModel, DirAuth: Array<PermissionDefinition>): void {
        if (this.GetKeyFrameRequest.length > 1) {
            for (let request of this.GetKeyFrameRequest.slice(0, -1)) {
                request.abort();
            }
            this.GetKeyFrameRequest = [];
        }
        const newKeyFrameRequest = PostJXR<KeyFrameInputModel>(this.api.ShowKeyFrame, input, false);
        this.GetKeyFrameRequest.push(newKeyFrameRequest);
        newKeyFrameRequest.then(async json => {
            const datas = <Array<KeyFrameDataModel>>json.Data;
            const $KTab: JQuery<HTMLElement> = $(`.cusmodal.active div.tab[data-tab='${TabNameEnum.KeyFrame}']`);
            const $cardsArea: JQuery<HTMLElement> = $KTab.find('.cards');
            const cards: HTMLDivElement = document.createElement('div');
            const fragment: DocumentFragment = document.createDocumentFragment();
            for (let Data of datas) {
                const showEditBtn = DirAuth.indexOf(PermissionDefinition.U) > -1 ? true : false;
                const showDeleteBtn = DirAuth.indexOf(PermissionDefinition.D) > -1 ? true : false;
                const card = UI.KeyFrame.KeyFrameCard(Data, true, showEditBtn, showDeleteBtn);
                fragment.appendChild(card);
            }
            cards.appendChild(fragment);
            $cardsArea.empty().append(cards.innerHTML);
        });
    }
    ParagraphDesView(input: FileListInputModel, DirAuth: Array<PermissionDefinition>): Promise<string> {
        if (!IsNULL(this.ParagraphDesViewRequest)) {
            this.ParagraphDesViewRequest.abort();
            Logger.log('取消最近一次段落描述畫面');
        }
        this.ParagraphDesViewRequest = PostJXR<FileListInputModel>(this.api.ShowParaDescription, input, false);
        return new Promise(resolve => {
            this.ParagraphDesViewRequest.then(json => {
                const datas = <Array<ParaDescriptionModel>>json.Data;
                const paragraphs: HTMLDivElement = document.createElement('div');
                const fragment = document.createDocumentFragment();
                for (let Data of datas) {
                    const showEditBtn = DirAuth.indexOf(PermissionDefinition.U) > -1 ? true : false;
                    const showDeleteBtn = DirAuth.indexOf(PermissionDefinition.D) > -1 ? true : false;
                    const paragraph = UI.Paragraph.ParagraphItem(Data, showEditBtn, showDeleteBtn);
                    fragment.appendChild(paragraph);
                }
                paragraphs.appendChild(fragment);
                const addparagraphBtn =
                    DirAuth.indexOf(PermissionDefinition.I) > -1
                        ? `  <button type="button" class="ui _darkGrey mini button"  name="create" data-Id="${input.fileNo}"><i class="add icon"></i>新增</button>`
                        : '';
                const templete: string = `<div class="x-keybuttons"> ${addparagraphBtn}</div>
                                         <div class="ui inverted relaxed middle aligned list cuslist"> ${paragraphs.innerHTML} </div>`;
                resolve(templete);
            }).catch(error => {
                if (this.ParagraphDesViewRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error(`創建 ParagraphDesView發生錯誤,原因:`, error);
                    resolve(UI.Error.ErrorSegment('無法取得段落描述').outerHTML);
                } else {
                    resolve(UI.Error.CorrectSegment('加載段落描述', '請求中', 'spinner loading').outerHTML);
                }
            });
        });
    }
    // DocumentView(documenthref: URL): Promise<boolean> {
    //     return new Promise(resolve => {
    //         try {
    //             const href = documenthref.href;
    //             const id = 'DocumentViewerModal';
    //             const modaldiv = document.createElement('div');
    //             modaldiv.id = id;
    //             modaldiv.className = 'ui basic modal';
    //             modaldiv.innerHTML = `<div class="ui icon header"><i class="archive icon"></i>存檔舊郵件</div>
    //                                   <div class="content"> <object type="text/html" data="${href}" ></object> </div>
    //                                   <div class="actions"></div>`;
    //             if (document.querySelectorAll('#' + id).length > 0) {
    //                 $('#' + id).remove();
    //             }
    //             $('#OtherArea').append(modaldiv.outerHTML);
    //             resolve(true);
    //         } catch (error) {
    //             initSetting.ShowLog && console.error('創建文件檢視器燈箱發生錯誤');
    //             initSetting.ShowLog && console.error(error);
    //             resolve(false);
    //         }
    //     });
    // }
}
