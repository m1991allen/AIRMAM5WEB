import { BaseController, IBaseController } from './BaseController';
import { RuleEnglishCategory } from '../Enum/RuleCategory';
import { GetRuleListByProgress } from '../Interface/Rule/GetRuleListByProgress';
import { API_Rule } from '../Const/API';
import { initSetting } from '../initSetting';
import { CreateRuleParamsModel } from '../Interface/Rule/CreateRuleParamsModel';
import { AjaxGet, AjaxPost } from '../Function/Ajax';
import { IsNULLorEmpty } from '../Function/Check';
import { IResponse } from '../Interface/Shared/IResponse';
import { CreateRuleModel } from '../Interface/Rule/CreateRuleModel';
import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { EditRuleFilterModel } from '../Interface/Rule/EditRuleFilterModel';
import { GetRuleModel } from '../Interface/Rule/GetRuleModel';
import { EditRuleModel } from '../../Models/Interface/Rule/EditRuleModel';
import { GetUrl } from '../Function/Url';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { Logger } from '../Class/LoggerService';
import { FilterTableModel } from '../Interface/Rule/FilterTableModel';
import { RuleFilterActiveModel } from '../Interface/Rule/RuleFilterActiveModel';
import { StringEnum } from '../Enum/StringEnum';

/**
 * 規則庫接口
 */
export interface IRuleController extends IBaseController<API_Rule> {
    /**依流程類型取得"新增規則燈箱"下拉選單資料 */
    GetCreateRuleParamsByType(type: RuleEnglishCategory): Promise<CreateRuleParamsModel>;
    /**依流程取得"新增子規則燈箱"的下拉選單 */
    GetProcessTableList(type: RuleEnglishCategory): Promise<IResponse<object, Array<FilterTableModel>>>;
    /**新增規則 */
    CreateMainRule(input: CreateRuleModel): Promise<IResponse>;
    /**
     * 編輯主規則(流程)
     * @param ruleName 流程顯示名稱
     * @param priority 流程優先權
     * @param note 流程備註
     */
    EditCategoryRule(input: EditRuleModel): Promise<IResponse>; //(ruleName: string, priority: number, note: string): Promise<IResponse>;
    /**
     * 啟用或關閉流程
     * @param category 流程代碼,例如:BOOKING、UPLOAD
     * @param isActive 是否啟用
     */
    ActiveRule(category: string, isActive: boolean): Promise<IResponse>;
    /**啟用或關閉子規則*/
    ActiveSubRule(input: RuleFilterActiveModel): Promise<IResponse>;
    /**新增子規則 */
    AddRule(input: EditRuleFilterModel): Promise<IResponse>;
    /**編輯子規則 */
    EditRule(input: EditRuleFilterModel): Promise<IResponse>;
    /**刪除子規則 */
    DeleteRule(input: GetRuleModel): Promise<IResponse>;
}

/**
 *規則庫路由
 */
export class RuleController extends BaseController<API_Rule> implements IRuleController {
    private Progress: object = {};
    private ProgressErrorTime: number = 0;
    constructor() {
        super({
            Search: GetUrl(Controller.Rule, Action.Search).href,
            GetCreateRuleParams: GetUrl(Controller.Rule, Action.CreateRuleParams).href,
            CreateRule: GetUrl(Controller.Rule, Action.Create).href,
            ShowSubCreate: GetUrl(Controller.Rule, Action.ShowSubCreate).href,
            ShowEdit: GetUrl(Controller.Rule, Action.ShowEdit).href,
            ShowDelete: GetUrl(Controller.Rule, Action.ShowDelete).href,
            ShowCategoryEdit: GetUrl(Controller.Rule, Action.ShowCategoryEdit).href,
            Add: GetUrl(Controller.Rule, Action.SubCreate).href,
            Edit: GetUrl(Controller.Rule, Action.Edit).href,
            Delete: GetUrl(Controller.Rule, Action.Delete).href,
            EditCategory: GetUrl(Controller.Rule, Action.EditCategory).href,
            ActiveCategory: GetUrl(Controller.Rule, Action.ActiveCategory).href,
            ActiveSubRule: GetUrl(Controller.Rule, Action.ActiveRuleFilter).href,
            GetProcessTableList: GetUrl(Controller.Rule, Action.GetProcessTableList).href,
        });
    }
    static get api() {
        return new this.api();
    }
    GetCreateRuleParamsByType(type: RuleEnglishCategory): Promise<CreateRuleParamsModel> {
        return new Promise(resolve => {
            if (!(type in this.Progress)) {
                AjaxGet<GetRuleListByProgress>(this.api.GetCreateRuleParams, { ruletype: type }, false)
                    .then(res => {
                        const data = <CreateRuleParamsModel>res.Data;
                        this.Progress[type] = data;
                        resolve(data);
                    })
                    .catch(error => {
                        Logger.viewres(this.api.GetCreateRuleParams, '取得流程類型', error, false);

                        if (this.ProgressErrorTime <= 3) {
                            this.GetCreateRuleParamsByType(type);
                            this.ProgressErrorTime++;
                        } else {
                            resolve(<CreateRuleParamsModel>{});
                        }
                    });
            } else {
                Logger.log(`流程類型${type}已有下拉選單暫存結果`);
                resolve(this.Progress[type]);
            }
        });
    }
    GetProcessTableList(type: RuleEnglishCategory): Promise<IResponse<object, Array<FilterTableModel>>> {
        return new Promise(resolve => {
            return AjaxGet<{ category: string }>(this.api.GetProcessTableList, { category: type }, false);
        });
    }
    CreateMainRule(input: CreateRuleModel): Promise<IResponse> {
        return new Promise(resolve => {
            if (IsNULLorEmpty(input.RuleMaster.RuleName)) {
                resolve(<IResponse>{
                    IsSuccess: false,
                    StatusCode: HttpStatusCode.BadRequest,
                    Message: '必須填寫規則名稱',
                });
            } else if (input.Filters.length == 0) {
                resolve(<IResponse>{
                    IsSuccess: false,
                    StatusCode: HttpStatusCode.BadRequest,
                    Message: '至少需要一個條件設定',
                });
            } else {
                const reuslt = AjaxPost<CreateRuleModel>(this.api.CreateRule, input, false);
                resolve(reuslt);
            }
        });
    }
    EditCategoryRule(input: EditRuleModel): Promise<IResponse> {
        return AjaxPost<EditRuleModel>(this.api.EditCategory, input, false);
    }
    ActiveRule(category: string, isActive: boolean): Promise<IResponse> {
        return AjaxPost<{ category: string; isActive: boolean }>(
            this.api.ActiveCategory,
            { category: category, isActive: isActive },
            false
        );
    }
    ActiveSubRule(input: RuleFilterActiveModel): Promise<IResponse> {
        return AjaxPost<RuleFilterActiveModel>(this.api.ActiveSubRule, input, false);
    }
    AddRule(input: EditRuleFilterModel): Promise<IResponse> {
        return AjaxPost<EditRuleFilterModel>(this.api.Add, input, false);
    }
    EditRule(input: EditRuleFilterModel): Promise<IResponse> {
        return AjaxPost<EditRuleFilterModel>(this.api.Edit, input, false);
    }
    DeleteRule(input: GetRuleModel): Promise<IResponse> {
        return AjaxPost<GetRuleModel>(this.api.Delete, input, false);
    }
}
