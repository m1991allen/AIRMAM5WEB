import { BaseController, IBaseController } from './BaseController';
import { API_ArchiveMove } from '../Const/API';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost, AjaxGet } from '../Function/Ajax';
import { SubjectSearchModel } from '../Interface/Subject/SubjectSearchModel';
import { GetDirByFileNoLoadOnDemand } from '../Interface/ArchiveMove/GetDirByFileNoLoadOnDemand';
import { MoveFilesSaveModel } from '../Interface/ArchiveMove/MoveFilesSaveModel';
import { MoveSubjectsModel } from '../Interface/ArchiveMove/MoveSubjectsModel';
import { MoveTreeNodeModel } from '../Interface/ArchiveMove/MoveTreeNodeModel';
import { DecoratorConvert } from '../Function/DecoratorConvert';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { GetDirAndSubjectsByDirFilter } from '../Interface/ArchiveMove/GetDirAndSubjectsByDirFilter';

/**
 * 歸檔搬遷介面
 */
export interface IArchiveMoveController extends IBaseController<API_ArchiveMove> {
    /**可使用API */
    // readonly api: API_ArchiveMove;
    /**藉由節點Id取得主題列表 */
    GetSubjectList(dirId: number): Promise<IResponse>;
    /**因應"目錄節點Queue"是否啟用需求, 目標目錄節點的主題列表要呼叫另一支預存 */
    GetSubjectList2(input: GetDirAndSubjectsByDirFilter): Promise<IResponse>;

    /**藉由主題Id取得檔案列表 */
    GetFileList(subjectId: string): Promise<IResponse>;
    /**
     * 依檔案編號 取得{影/音/圖/文} 相同樣版的目錄節點樹狀資料
     * Tips: 前端若選擇多筆檔案, 傳入時僅以第一筆檔案編號為查詢值。
     */
    GetTargetDir(input: GetDirByFileNoLoadOnDemand): Promise<IResponse>;
    /**
     * 檔案搬移存檔
     * @param input
     */
    MoveFiles(input: MoveFilesSaveModel): Promise<IResponse>;
    /**
     * 主題搬移存檔
     * @param input
     */
    MoveSubject(input: MoveSubjectsModel): Promise<IResponse>;
    /**樹狀節點搬移存檔 */
    MoveTreeNode(input: MoveTreeNodeModel): Promise<IResponse>;
}
/** 歸檔搬遷_靜態方法Controller */
class ArchiveMoveStaticController extends BaseController<API_ArchiveMove> {
    private static readonly staticapi: string = GetUrl(Controller.ArchiveMove, Action.GetTargetDir).href;
    constructor() {
        super({
            GetSubjectList: GetUrl(Controller.Subject, Action.Search).href,
            GetSubjectList2: GetUrl(Controller.ArchiveMove, Action.GetSubjectsByDirFilter).href,
            GetFileList: GetUrl(Controller.ArchiveMove, Action.GetSubjFiles).href,
            GetDir: GetUrl(Controller.Subject, Action.GetDir).href,
            GetTargetDir: ArchiveMoveStaticController.staticapi,
            MoveSave: GetUrl(Controller.ArchiveMove, Action.MoveSave).href,
            SubjMoveSave: GetUrl(Controller.ArchiveMove, Action.SubjMoveSave).href,
            MoveTreeNode: GetUrl(Controller.ArchiveMove, Action.MoveTreeNode).href,
        });
    }
    static GetTagetTree(input: GetDirByFileNoLoadOnDemand): Promise<IResponse> {
        return AjaxPost<GetDirByFileNoLoadOnDemand>(this.staticapi, input, false);
    }
}

/**
 *歸檔搬遷路由
 */
@DecoratorConvert
export class ArchiveMoveController extends ArchiveMoveStaticController implements IArchiveMoveController {
    constructor() {
        super();
    }
    static get api() {
        return new this.api();
    }
    GetTagetTree(input: GetDirByFileNoLoadOnDemand): Promise<IResponse> {
        return ArchiveMoveStaticController.GetTagetTree(input);
    }
    GetSubjectList(dirId: number): Promise<IResponse> {
        return AjaxPost<SubjectSearchModel>(this.api.GetSubjectList, { id: dirId }, false);
    }
    GetSubjectList2(input: GetDirAndSubjectsByDirFilter): Promise<IResponse> {
        return AjaxPost<GetDirAndSubjectsByDirFilter>(this.api.GetSubjectList2, input, false);
    }
    GetFileList(subjectId: string): Promise<IResponse> {
        return AjaxGet<{ subjid: string }>(this.api.GetFileList, { subjid: subjectId }, false);
    }
    GetTargetDir(input: GetDirByFileNoLoadOnDemand): Promise<IResponse> {
        return AjaxPost<GetDirByFileNoLoadOnDemand>(this.api.GetTargetDir, input, false);
    }
    MoveFiles(input: MoveFilesSaveModel): Promise<IResponse> {
        return AjaxPost<MoveFilesSaveModel>(this.api.MoveSave, input, false);
    }
    MoveSubject(input: MoveSubjectsModel): Promise<IResponse> {
        return AjaxPost<MoveSubjectsModel>(this.api.SubjMoveSave, input, false);
    }
    MoveTreeNode(input: MoveTreeNodeModel): Promise<IResponse> {
        return AjaxPost<MoveTreeNodeModel>(this.api.MoveTreeNode, input, false);
    }
}
