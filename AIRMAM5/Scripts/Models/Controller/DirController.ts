import { API_Dir } from '../Const/API';
import { BaseController, IBaseController } from './BaseController';
import { AjaxPost, Ajax, AjaxGet, GetJXR } from '../Function/Ajax';
import { IResponse } from '../Interface/Shared/IResponse';
import { IdModel } from '../Interface/Shared/IdModel';
import * as dayjs_ from 'dayjs';
import { CreateNodeModel, CreateNodeRealModel } from '../Interface/Dir/CreateNodeModel';
import { EditNodeModel, EditNodeRealModel } from '../Interface/Dir/EditNodeModel ';
import { EditAuthInputModel } from '../Interface/Dir/EditAuthInputModel';
import { EditAuthModel, EditAuthRealModel } from '../Interface/Dir/EditAuthModel';
import { CreateGroupAuthModel, CreateGroupRealAuthModel } from '../Interface/Dir/CreateGroupAuthModel';
import { CreateInfoModel, UpdateInfoModel } from '../Interface/Shared/IDate';
import { CreateUserAuthModel, CreateUserRealAuthModel } from '../Interface/Dir/CreateUserAuthModel';
import { DirTreeData } from '../Interface/Dir/DirTreeData';
import { DecoratorConvert } from '../Function/DecoratorConvert';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { IsNullorUndefined } from '../Function/Check';
import { Logger } from '../Class/LoggerService';
const dayjs = (<any>dayjs_).default || dayjs_;

/**
 * 系統目錄維護介面
 */
export interface IDirController extends IBaseController<API_Dir> {
    /**取得樹狀目錄 */
    GetDir(input: {
        id: number;
        fsKEYWORD: string;
        showcount: boolean;
        showhide: boolean;
    }): Promise<Array<DirTreeData> | []>;
    /**新增節點 */
    CreateNode(input: CreateNodeModel): Promise<IResponse>;
    /**編輯節點 */
    EditNode(input: EditNodeModel): Promise<IResponse>;
    /**刪除節點 */
    DeleteNode(nodeId: number): Promise<IResponse>;
    /**新增群組權限 */
    CreateGroupAuth(input: CreateGroupAuthModel): Promise<IResponse>;
    /**編輯群組權限 */
    EditGroupAuth(input: EditAuthInputModel): Promise<IResponse>;
    /**新增使用者權限 */
    CreateUserAuth(input: CreateUserAuthModel): Promise<IResponse>;
    /**編輯使用者權限 */
    EditUserAuth(input: EditAuthInputModel): Promise<IResponse>;
    /**刪除 目錄-群組/使用者-權限 */
    DeleteOperationAuth(id: number, type: string, idvalue: string): Promise<IResponse>;
    /**取得節點內容 : 目錄資訊與欄位 */
    GetDirInfoView(id: number): Promise<string>;
    //    /**JSON:系統目錄樹狀節點 (id=0，Root directory)*/
    //    GetDir(input:GetDirModel):Promise<DirTreeData[]>;
    //    /**系統目錄資訊資訊 */
    //    ShowInfor();
    //    /**系統目錄權限資訊 */
    //    ShowAuthr();
    //    /**系統目錄編輯資訊 */
    //    ShowEditedAuthr();
    //    /**系統目錄刪除 */
    //    DeleteOperationAuthr();
    //    /**新增節點頁面 */
    //    ShowCreateNoder();
    //    /**編輯節點頁面 */
    //    ShowEditNoder();
    //    /**刪除節點頁面 */
    //    ShowDeleteNoder();
    //    /**新增節點 */
    //    CreateNoder();
    //    /**編輯節點 */
    //    EditNoder();
    //    /**刪除節點 */
    //    DeleteNoder();
}
/**系統目錄維護_靜態方法Controller */
class DirtaticController extends BaseController<API_Dir> {
    /*Notice:05/28因快上線前,才決議DirController要改只讓Admin和MediaManager使用,所以目錄會防角色,才會改換Subject的GetDir*/
    private static staticapi: string = GetUrl(Controller.Subject, Action.GetDir).href;
    private static searchTreeDataRequest: JQueryXHR = null;
    constructor() {
        super({
            /**JSON:系統目錄樹狀節點 (id=0，Root directory)*/
            GetDir: DirtaticController.staticapi,
            /**系統目錄資訊資訊 */
            ShowInfo: GetUrl(Controller.Dir, Action.ShowDirInfo).href,
            /**系統目錄權限資訊 */
            ShowAuth: GetUrl(Controller.Dir, Action.ShowDirAuth).href,
            /**系統目錄編輯資訊 */
            ShowEditedAuth: GetUrl(Controller.Dir, Action.ShowDirAuthEdit).href,
            /**系統目錄刪除 */
            DeleteOperationAuth: GetUrl(Controller.Dir, Action.DeleteOperationAuth).href,
            /**新增節點頁面 */
            ShowCreateNode: GetUrl(Controller.Dir, Action.ShowCreateDir).href,
            /**編輯節點頁面 */
            ShowEditNode: GetUrl(Controller.Dir, Action.ShowEditDir).href,
            /**刪除節點頁面 */
            ShowDeleteNode: GetUrl(Controller.Dir, Action.ShowDeleteDir).href,
            /**新增節點 */
            CreateNode: GetUrl(Controller.Dir, Action.CreateDir).href,
            /**編輯節點 */
            EditNode: GetUrl(Controller.Dir, Action.EditDir).href,
            /**刪除節點 */
            DeleteNode: GetUrl(Controller.Dir, Action.DeleteDir).href,
            /**新增使用者權限 */
            CreateUserAuth: GetUrl(Controller.Dir, Action.CreateUserAuth).href,
            /**新增群組權限 */
            CreateGroupAuth: GetUrl(Controller.Dir, Action.CreateGroupAuth).href,
            /**編輯使用者權限 */
            EditUserAuth: GetUrl(Controller.Dir, Action.EditUserAuth).href,
            /**編輯群組權限 */
            EditGroupAuth: GetUrl(Controller.Dir, Action.EditGroupAuth).href,
        });
    }
    static GetTree(input: {
        id: number;
        fsKEYWORD: string;
        showcount: boolean;
        showhide: boolean;
    }): Promise<Array<DirTreeData> | []> {
       if(!IsNullorUndefined(this.searchTreeDataRequest)){
           this.searchTreeDataRequest.abort();
       }
       this.searchTreeDataRequest= GetJXR<{ id: number; fsKEYWORD: string; showcount: boolean; showhide: boolean }>(this.staticapi,input,false);
        return new Promise(resolve=>{
            this.searchTreeDataRequest.then(data=>{
                resolve(<Array<DirTreeData>>data);
            }).catch(error=>{
                Logger.error(error);
                resolve(<Array<DirTreeData>>[]);
            });
        });
    }
    static get api() {
        return new this.api();
    }
}

/**
 * 系統目錄維護路由
 */
@DecoratorConvert
export class DirController extends DirtaticController implements IDirController {
    constructor() {
        super();
    }
    GetDir(input: {
        id: number;
        fsKEYWORD: string;
        showcount: boolean;
        showhide: boolean;
    }): Promise<Array<DirTreeData> | []> {
        return Ajax<{ id: number; fsKEYWORD: string; showcount: boolean; showhide: boolean }>(
            'GET',
            this.api.GetDir,
            input,
            false
        );
    }
    CreateNode(input: CreateNodeModel): Promise<IResponse> {
        let INPUT = {
            fsCREATED_BY: '',
            fdCREATED_DATE: dayjs().format('YYYY/MM/DD HH:mm:ss'),
            fsUPDATED_BY: '',
            fdUPDATED_DATE: dayjs().format('YYYY/MM/DD HH:mm:ss'),
        };
        Object.keys(input).forEach(key => (INPUT[key] = input[key]));
        return AjaxPost<CreateNodeRealModel>(this.api.CreateNode, <CreateNodeRealModel>INPUT, false);
    }
    EditNode(input: EditNodeModel): Promise<IResponse> {
        let INPUT = {
            fsCREATED_BY: '',
            fdCREATED_DATE: dayjs().format('YYYY/MM/DD HH:mm:ss'),
            fsUPDATED_BY: '',
            fdUPDATED_DATE: dayjs().format('YYYY/MM/DD HH:mm:ss'),
        };
        Object.keys(input).forEach(key => (INPUT[key] = input[key]));
        return AjaxPost<EditNodeRealModel>(this.api.EditNode, <EditNodeRealModel>input, false);
    }
    DeleteNode(nodeId: number): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.DeleteNode, { id: nodeId }, false);
    }
    CreateGroupAuth(input: CreateGroupAuthModel): Promise<IResponse> {
        let CreateInfo: CreateInfoModel = {
            fsCREATED_BY: '',
            fdCREATED_DATE: new Date().toISOString(),
        };
        let UpdateInfo: UpdateInfoModel = {
            fsUPDATED_BY: '',
            fdUPDATED_DATE: new Date().toISOString(),
        };
        Object.keys(input).forEach(key => (CreateInfo[key] = input[key]));
        Object.keys(input).forEach(key => (UpdateInfo[key] = input[key]));
        return AjaxPost<CreateGroupRealAuthModel>(this.api.CreateGroupAuth, <CreateGroupRealAuthModel>input, false);
    }
    CreateUserAuth(input: CreateUserAuthModel): Promise<IResponse> {
        let CreateInfo: CreateInfoModel = {
            fsCREATED_BY: '',
            fdCREATED_DATE: new Date().toISOString(),
        };
        let UpdateInfo: UpdateInfoModel = {
            fsUPDATED_BY: '',
            fdUPDATED_DATE: new Date().toISOString(),
        };
        Object.keys(input).forEach(key => (CreateInfo[key] = input[key]));
        Object.keys(input).forEach(key => (UpdateInfo[key] = input[key]));
        return AjaxPost<CreateUserRealAuthModel>(this.api.CreateUserAuth, <CreateUserRealAuthModel>input, false);
    }
    EditGroupAuth(input: EditAuthInputModel): Promise<IResponse> {
        let INPUT: EditAuthModel = {
            DataType: 'G',
            fnPARENT_ID: -1,
            C_sDIR_PATH: '',
        };
        Object.keys(input).forEach(key => (INPUT[key] = input[key]));
        return AjaxPost<EditAuthRealModel>(this.api.EditGroupAuth, <EditAuthRealModel>input, false);
    }
    EditUserAuth(input: EditAuthInputModel): Promise<IResponse> {
        let INPUT: EditAuthModel = {
            DataType: 'U',
            fnPARENT_ID: -1,
            C_sDIR_PATH: '',
        };
        Object.keys(input).forEach(key => (INPUT[key] = input[key]));
        return AjaxPost<EditAuthRealModel>(this.api.EditUserAuth, <EditAuthRealModel>input, false);
    }
    DeleteOperationAuth(id: number, type: string, idvalue: string): Promise<IResponse> {
        return AjaxPost<{ id: number; type: string; idvalue: string }>(
            this.api.DeleteOperationAuth,
            { id: id, type: type, idvalue: idvalue },
            false
        );
    }

    GetDirInfoView(id: number): Promise<string> {
        return Ajax<IdModel>('GET', this.api.ShowInfo, { id: id }, false);
    }
}
