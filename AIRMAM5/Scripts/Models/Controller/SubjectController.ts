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
/*????????????*/
const dayjs = (<any>dayjs_).default || dayjs_;

/**?????????????????????????????? */
// var GetKeyFrameRequest: Array<JQueryXHR> = [];
/**
 * ??????
 */
export interface ISubjectController extends IBaseController<API_Subject> {
    /**JSON:???????????? */
    CreateSubject(serializeFormData: string): Promise<IResponse>;
    /**JSON:???????????? */
    EditSubject(serializeFormData: string): Promise<IResponse>;
    /**JSON:???????????? */
    DeleteSubject(input: SubjectIdModel): Promise<IResponse>;
    /**JSON:?????????View???????????? */
    UploadView(id: number, subjid: string): Promise<IResponse>;
    /**
     * JSON:???????????????????????????????????????
     * @param type ????????????
     * @param templeteId?????????Id
     */
    GetArcPreList(type: MediaType, templeteId: number): Promise<Array<SelectListItem>>;
    /**???????????? */
    AddBooking(input: Array<CreateMaterialModel>): Promise<IResponse>;
    /**JSON:????????????????????????????????? */
    EditMedia(serializeFormData: string): Promise<IResponse>;
    /**JSON:????????????????????????????????? */
    DeleteMedia(input: DeleteMediaModel): Promise<IResponse>;
    /**?????? */
    Retransfer(input: SubjectModel): Promise<IResponse>;
    /**JSON:?????????????????? */
    AddKeyFrame(input: KeyFrameCUModel): Promise<IResponse>;
    /**JSON:?????????????????? */
    EditKeyFrame(input: KeyFrameCUModel): Promise<IResponse>;
    /**JSON:?????????????????? */
    DeleteKeyFrame(input: GetKeyFrameModel): Promise<IResponse>;
    /**JSON:?????????????????????????????? */
    SetHeadFrame(input: GetKeyFrameModel): Promise<IResponse>;

    /**?????????????????? */
    AddParagraph(input: ParagraphCUModel): Promise<IResponse>;
    /**?????????????????? */
    EditParagraph(input: ParagraphCUModel): Promise<IResponse>;
    /**?????????????????? */
    DeleteParagraph(input: GetParagraphModel): Promise<IResponse>;

    /**View:??????????????????????????????????????? */
    EditMediaView(MainModalId: string, EditModalId: string, input: SubjectModel): Promise<boolean>;
    /**View:????????????????????????????????????????????? */
    EditBatchMediaView(
        MainModalId: string,
        EditModalId: string,
        input: { type: string; SubjectId: string; fileNo: Array<string> }
    ): Promise<boolean>;
    /**View:??????????????????????????????????????? */
    DeleteMediaView(MainModalId: string, DeleteModalId: string, type: string, fileno: string): Promise<boolean>;
    /**View:????????????????????????*/
    EditKeyframeView(MainModalId: string, EditModalId: string, input: GetKeyFrameModel): Promise<boolean>;
    /**ViewL???????????????????????? */
    DeleteKeyframeView(MainModalId: string, DeleteModalId: string, input: GetKeyFrameModel): Promise<boolean>;
    /**View:????????????????????????  */
    EditParagraphView(MainModalId: string, EditModalId: string, input: GetParagraphModel): Promise<boolean>;
    /**View:????????????????????????  */
    DeleteParagraphView(MainModalId: string, DeleteModalId: string, input: GetParagraphModel): Promise<boolean>;
    /**View:???????????? */
    ReplacementView(MainModalId: string, ReplaceModalId: string, input: SubjectModel);

    /**
     * View:???????????????
     * @param input ????????????
     * @param DirAuth ????????????????????????????????????
     */
    Preview(input: SubjectPreviewModel, DirAuth: Array<PermissionDefinition>): Promise<string>;
    /**
     * View:????????????
     * @param input ????????????
     * @param DirAuth ????????????????????????????????????
     */
    MediaView(input: MediaDataInputModel, DirAuth: Array<PermissionDefinition>): Promise<string>;
    /**JSON:???????????? */
    List(input: FileListInputModel): Promise<IResponse>;
    /**
     * View:????????????Placeholder
     * @param fileNo ????????????
     * @param dataLength ??????????????????
     * @param DirAuth ????????????????????????????????????
     */
    KeyFramePlaceholderView(fileNo: string, dataLength: number, DirAuth: Array<PermissionDefinition>): Promise<string>;
    /**
     * Function:??????????????????
     * @param input ????????????
     * @param DirAuth ????????????????????????????????????
     */
    KeyFrameView(input: KeyFrameInputModel, DirAuth: Array<PermissionDefinition>): void;
    /**
     * View:????????????
     * @param input ????????????
     * @param DirAuth ????????????????????????????????????
     */
    ParagraphDesView(input: FileListInputModel, DirAuth: Array<PermissionDefinition>): Promise<string>;
}

/** ????????????_????????????Controller */
class SubjectStaticController extends BaseController<API_Subject> {
    private static readonly functionAuthApi: string = GetUrl(Controller.Subject, Action.GetUserDirAuth).href;
    private static readonly subjectAuthApi: string = GetUrl(Controller.Subject, Action.GetUserSubjAuth).href;
    private static readonly fileAuthApi: string = GetUrl(Controller.Subject, Action.GetUserFileAuth).href;
    constructor() {
        super({
            /**JSON:????????????????????? */
            Search: GetUrl(Controller.Subject, Action.Search).href,
            /**View:???????????? */
            ShowCreate: GetUrl(Controller.Subject, Action.ShowCreate).href,
            /**View:????????????*/
            ShowEdit: GetUrl(Controller.Subject, Action.ShowEdit).href,
            /**View:???????????? */
            ShowDelete:GetUrl(Controller.Subject,Action.ShowDelete).href,
            /**View:?????????????????? */
            ShowDetail: GetUrl(Controller.Subject, Action.ShowDetails).href,
            /**View:??????MediaData */
            ShowEditMedia: GetUrl(Controller.Subject, Action.ShowEditMedia).href,
            /**View:??????MediaData */
            ShowDeleteMedia: GetUrl(Controller.Subject, Action.ShowDeleteMedia).href,
            /**View:??????????????? */
            ShowAddKeyFrameView: GetUrl(Controller.Subject, Action.ShowKeyFrameAdd).href,
            /**View:?????????????????? */
            ShowEditKeyFrameView: GetUrl(Controller.Subject, Action.ShowKeyFrameEdit).href,
            /**View:?????????????????? */
            ShowDeleteKeyFrameView: GetUrl(Controller.Subject, Action.ShowKeyFrameDelete).href,
            /**???????????? */
            Preview: GetUrl(Controller.Subject, Action.ShowPreview).href,
            /**View:?????????????????? */
            ShowAddParagraphView: GetUrl(Controller.Subject, Action.ShowAddParagraphView).href,
            /**View:?????????????????? */
            ShowEditParagraphView: GetUrl(Controller.Subject, Action.ShowEditParagraphView).href,
            /**View:?????????????????? */
            ShowDeleteParagraphView: GetUrl(Controller.Subject, Action.ShowDeleteParagraphView).href,
            /**View:?????? */
            ShowReplacementView: GetUrl(Controller.Subject, Action.ShowChangeMedia).href,
            /**JSON:???????????? */
            Create: GetUrl(Controller.Subject, Action.Create).href,
            /**JSON:???????????? */
            Edit: GetUrl(Controller.Subject, Action.Edit).href,
            /**JSON:???????????? */
            Delete: GetUrl(Controller.Subject, Action.Delete).href,
            /**JSON:????????????????????????????????? */
            EditMedia: GetUrl(Controller.Subject, Action.EditMedia).href,
            /**JSON:????????????????????????????????? */
            DeleteMedia: GetUrl(Controller.Subject, Action.DeleteMedia).href,
            /**JSON:?????? */
            Retransfer: GetUrl(Controller.Subject, Action.ReTransfer).href,
            /**JSON:???????????????????????? */
            GetArcPreList: GetUrl(Controller.Shared, Action.GetArcPreList).href,
            /**JSON:????????????????????????????????? */
            GetUserDirAuth: GetUrl(Controller.Subject, Action.GetUserDirAuth).href,
            /**JSON:??????????????????????????? */
            GetUserSubjectAuth: GetUrl(Controller.Subject, Action.GetUserSubjAuth).href,
            /**JSON:??????????????????????????? */
            GetUserFileAuth: GetUrl(Controller.Subject, Action.GetUserFileAuth).href,
            /**JSON:?????????????????? */
            AddKeyFrame: GetUrl(Controller.Subject, Action.KeyFrameAdd).href,
            /**JSON:?????????????????? */
            EditKeyFrame: GetUrl(Controller.Subject, Action.KeyFrameEdit).href,
            /**JSON:?????????????????? */
            DeleteKeyFrame: GetUrl(Controller.Subject, Action.KeyFrameDelete).href,
            /**JSON:?????????????????????????????? */
            SetHeadFrame: GetUrl(Controller.Subject, Action.SetHeadFrame).href,

            /**JSON:?????????????????? */
            AddParagraph: GetUrl(Controller.Subject, Action.AddParagraph).href,
            /**JSON:?????????????????? */
            EditParagraph: GetUrl(Controller.Subject, Action.EditParagraph).href,
            /**JSON:?????????????????? */
            DeleteParagraph: GetUrl(Controller.Subject, Action.DeleteParagraph).href,

            /**JSON:??????View???????????? */
            ShowUpload: GetUrl(Controller.Subject, Action.ShowUpload).href,
            /**JSON:???????????? */
            ShowViewer: GetUrl(Controller.Subject, Action.ShowViewer).href,
            /**JSON:???????????? */
            ShowList: GetUrl(Controller.Subject, Action.ShowList).href,
            /**JSON:???????????? */
            ShowKeyFrame: GetUrl(Controller.Subject, Action.ShowKeyFrame).href,
            /**JSON:???????????? */
            ShowMetaData: GetUrl(Controller.Subject, Action.ShowMetaData).href,
            /**JSON:???????????? */
            ShowParaDescription: GetUrl(Controller.Subject, Action.ShowParaDescription).href,
            /**JSON:???????????? */
            AddingBooking: GetUrl(Controller.Booking, Action.AddBooking).href,
        });
    }
    /**
     * ?????????????????????????????????????????????/???/???/???/????????????
     * @param mediaType ????????????(S???V???A???D???P)
     * @param NodeId ??????Id
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
     * ?????????????????????????????????????????????/???/???/???/????????????
     * @param mediaType ????????????(S???V???A???D???P)
     * @param SubjectIds ??????????????????
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
     * ???????????????????????????????????????/???/???/???/????????????
     * @param mediaType ????????????(S???V???A???D???P)
     * @param FileNos  ??????????????????
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
 * ????????????
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
                Logger.error(`???????????????????????????,??????????????????????????????`);
                resolve(UI.Error.ErrorSegment('Oops,???????????????????????????', '??????????????????????????????...').outerHTML);
            } else {
                if (!IsNULL(this.PreviewRequest)) {
                    this.PreviewRequest.abort();
                    Logger.log('????????????????????????????????????');
                }
                this.PreviewRequest = PostJXR<{ fsSUBJECT_ID: string; type: string; fileNo: string }>(
                    this.api.Preview,
                    { fsSUBJECT_ID: input.fsSUBJECT_ID, type: input.type, fileNo: input.fileNo },
                    false
                );

                this.PreviewRequest.then(view => {
                    Logger.log('????????????????????????');
                    resolve(view);
                }).catch(error => {
                    if (this.PreviewRequest.statusText.toLowerCase() != 'abort') {
                        Logger.error('????????????????????????,??????:', error);
                        resolve(UI.Error.ErrorSegment().outerHTML);
                    } else {
                        resolve(UI.Error.CorrectSegment('???????????????????????????', '?????????', 'spinner loading').outerHTML);
                    }
                });
            }
        });
    }
    MediaView(input: MediaDataInputModel, DirAuth: Array<PermissionDefinition>): Promise<string> {
        if (!IsNULL(this.MediaViewRequest)) {
            this.MediaViewRequest.abort();
            Logger.log('??????????????????????????????????????????');
        }
        this.MediaViewRequest = PostJXR<MediaDataInputModel>(this.api.ShowMetaData, input, false);
        return new Promise(resolve => {
            this.MediaViewRequest.then(json => {
                const res = <IResponse>json;
                const data: MediaDataModel = <MediaDataModel>res.Data;
                if (res.IsSuccess) {
                    if (!IsNULLorEmpty(data)) {
                        const isDisabled = selectids.length > 1 ? 'disabled' : ''; /*???????????????,?????????????????????*/
                        const buttonArray:Array<{sort:number;btnTemplete:string}>=[];
                        /**???????????????????????? */const disabledBOOKING =data.IsExpired|| data.IsForBid ? "disabled" : "";
                        /**??????????????????????????????????????? */const licenseMSG =data.IsExpired|| data.IsAlert || data.IsForBid ? `(${data.LicenseMessage})${IsNULLorEmpty(data.LicenseEndDate)?"":"[ "+data.LicenseEndDate+" ]"}` : "";
                        switch(true){
                            case  DirAuth.indexOf(PermissionDefinition.B) > -1:
                                buttonArray.push({sort:1,btnTemplete:`<button type="button" class="ui _darkGrey ${isDisabled} mini button ${disabledBOOKING?'red':''}"  name="addMateria" data-Id="${data.fsFILE_NO}" ${disabledBOOKING} alertMessage="${data.IsAlert?data.LicenseMessage:""}">${data.IsExpired?'<i class="stop icon"></i>????????????': disabledBOOKING?'<i class="stop icon"></i>????????????':'<i class="add icon"></i>????????????'}</button>`});
                            case  DirAuth.indexOf(PermissionDefinition.D) > -1:
                                buttonArray.push({sort:2,btnTemplete: `<button type="button" class="ui _darkGrey ${isDisabled} mini button"  name="delete" data-Id="${data.fsFILE_NO}"><i class="delete icon"></i>??????</button>`});
                            case  DirAuth.indexOf(PermissionDefinition.U) > -1:
                                buttonArray.push({sort:3,btnTemplete:`<button type="button" class="ui _darkGrey ${isDisabled} mini button"  name="edit" data-Id="${data.fsFILE_NO}"><i class="edit icon"></i>??????</button>`});
                                buttonArray.push({sort:4,btnTemplete:`<button type="button" class="ui _darkGrey  mini  button"  name="batchedit" data-Id="${data.fsFILE_NO}"><i class="edit icon"></i>????????????</button>`});
                                if(DocumentSetting.ShowDocSystemButton && input.type===MediaType.VIDEO){
                                    buttonArray.push({sort:7,btnTemplete:`<button type="button" class="ui _darkGrey mini button ${isDisabled}"  name="docsystem" data-Id="${data.fsFILE_NO}"><i class="compass icon"></i>????????????</button>`});
                                }
                                if(DocumentSetting.ShowOfficeDocSystemButton && input.type===MediaType.Doc){
                                    buttonArray.push({sort:7,btnTemplete:`<button type="button" class="ui _darkGrey mini button ${isDisabled}"  name="docsystem" data-Id="${data.fsFILE_NO}"><i class="compass icon"></i>????????????</button>`});
                                }
                            case DirAuth.indexOf(PermissionDefinition.I) > -1:
                                buttonArray.push({sort:5,btnTemplete:`<button type="button" class="ui _darkGrey mini ${isDisabled} button"  name="reTransfer" data-Id="${data.fsFILE_NO}"><i class="redo icon"></i>??????</button>`});
                                buttonArray.push({sort:6,btnTemplete:`<button type="button" class="ui _darkGrey mini ${isDisabled} button"  name="replaceMent" data-Id="${data.fsFILE_NO}"><i class="upload icon"></i>??????</button>`});
                                break;
                        }
                        const buttons=buttonArray.sort((a, b) => a.sort - b.sort).map(item=>item.btnTemplete).join("");
                        const hashTags=IsNULLorEmpty(data.HashTag)?"": data.HashTag.split('^').filter(x=>x!="").map(x=>`<label class="ui blue x-hashtag label">#`+x+`</label>`).join("");//????????????????????????
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
                        /** added_20210831_??????????????? */
                        if (data.FileCategory == 'V' || data.FileCategory == 'A')
                        {
                            attrField += `<div class="ui right aligned grid _styleMarginZero">
                            <label class="center aligned three wide column" for="fsFILE_NO">???????????????</label>
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
                                                 <label class="center aligned three wide column" for="fsFILE_NO">????????????</label>
                                                 <span class="left aligned thirteen wide column">${data.fsFILE_NO}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="Title">??????</label>
                                                 <span class="left aligned thirteen wide column">${data.Title}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">??????</label>
                                                 <span class="left aligned thirteen wide column">${data.Description}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${hashTags}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${data.FileSecretStr} </span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">??????</label>
                                                 <span class="left aligned thirteen wide column">${data.LicenseStr} ${licenseMSG} </span>
                                              </div>
                                              ${attrField}
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${data.OriginFileName}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${insDt}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${userdateInfo.CreatedBy}` + (IsNULLorEmpty(userdateInfo.CreatedByName) ? '' : '(' + userdateInfo.CreatedByName + ')') + `</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${insDt2}</span>
                                              </div>
                                              <div class="ui right aligned grid _styleMarginZero">
                                                 <label class="center aligned three wide column" for="fsNAME">????????????</label>
                                                 <span class="left aligned thirteen wide column">${userdateInfo.UpdatedBy}` + (IsNULLorEmpty(userdateInfo.UpdatedByName) ? '' : '(' + userdateInfo.UpdatedByName + ')') + `</span>
                                            </div>
                                           </div>
                                       </div>
                                </div>`;
                        resolve(temp);
                    } else {
                        resolve(UI.Error.ErrorSegment('??????????????????', '~???????????????????????????~').outerHTML);
                    }
                } else {
                    resolve(UI.Error.ErrorSegment().outerHTML);
                }
            }).catch(error => {
                if (this.MediaViewRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error('??????????????????????????????,??????:', error);
                    resolve(UI.Error.ErrorSegment().outerHTML);
                } else {
                    resolve(UI.Error.CorrectSegment('?????????????????????????????????', '?????????', 'spinner loading').outerHTML);
                }
            });
        });
    }
    List(input: FileListInputModel): Promise<IResponse> {
        return AjaxPost<FileListInputModel>(this.api.ShowList, input, false);
    }
    KeyFramePlaceholderView(fileNo: string, dataLength: number, DirAuth: Array<PermissionDefinition>): Promise<string> {
        const addkfBtn = DirAuth.indexOf(PermissionDefinition.I) > -1 ? PrimaryButton(fileNo, 'add', '??????', 'create', 'button',['mini']) : '';
        const templete = (cards: string): string => {
            return ` <div class="x-keybuttons">${addkfBtn}
                     <button name="cardInfo" type="button" class="ui blue icon button right floated" title="??????????????????/??????"><i class="eye icon slash"></i></button></div>
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
            Logger.log('????????????????????????????????????');
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
                        ? `  <button type="button" class="ui _darkGrey mini button"  name="create" data-Id="${input.fileNo}"><i class="add icon"></i>??????</button>`
                        : '';
                const templete: string = `<div class="x-keybuttons"> ${addparagraphBtn}</div>
                                         <div class="ui inverted relaxed middle aligned list cuslist"> ${paragraphs.innerHTML} </div>`;
                resolve(templete);
            }).catch(error => {
                if (this.ParagraphDesViewRequest.statusText.toLowerCase() != 'abort') {
                    Logger.error(`?????? ParagraphDesView????????????,??????:`, error);
                    resolve(UI.Error.ErrorSegment('????????????????????????').outerHTML);
                } else {
                    resolve(UI.Error.CorrectSegment('??????????????????', '?????????', 'spinner loading').outerHTML);
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
    //             modaldiv.innerHTML = `<div class="ui icon header"><i class="archive icon"></i>???????????????</div>
    //                                   <div class="content"> <object type="text/html" data="${href}" ></object> </div>
    //                                   <div class="actions"></div>`;
    //             if (document.querySelectorAll('#' + id).length > 0) {
    //                 $('#' + id).remove();
    //             }
    //             $('#OtherArea').append(modaldiv.outerHTML);
    //             resolve(true);
    //         } catch (error) {
    //             initSetting.ShowLog && console.error('???????????????????????????????????????');
    //             initSetting.ShowLog && console.error(error);
    //             resolve(false);
    //         }
    //     });
    // }
}
