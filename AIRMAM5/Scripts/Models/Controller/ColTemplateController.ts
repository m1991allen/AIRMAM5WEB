import { BaseController, IBaseController } from './BaseController';
import { IResponse } from '../Interface/Shared/IResponse';
import { AjaxPost, Ajax } from '../Function/Ajax';
import { API_ColTemplate } from '../Const/API';
import * as dayjs_ from 'dayjs';
import { IdModel } from '../Interface/Shared/IdModel';
import { EditTempFieldModel } from '../Interface/ColTemplate/EditTempFieldModel';
import { DeleteTempFieldModel } from '../Interface/ColTemplate/DeleteTempFieldModel';
import { EditTempleteModel } from '../Interface/ColTemplate/EditTempleteModel';
import { SelectTempModel } from '../Interface/ColTemplate/SelectTempModel';
import { EditTempleteRealModel } from '../Interface/ColTemplate/EditTempleteRealModel';
import { ColFieldType } from '../Enum/ColTypeEnum';
import { GetUrl } from '../Function/Url';
import { Action } from '../Enum/Action';
import { Controller } from '../Enum/Controller';
const dayjs = (<any>dayjs_).default || dayjs_;

/**自訂樣板欄位介面 */
export interface IColTemplateController extends IBaseController<API_ColTemplate> {
    /**新增或複製樣板*/
    CreateCopyTemp(input: SelectTempModel): Promise<IResponse>;
    /**編輯樣板 */
    EditTemp(input: EditTempleteModel): Promise<IResponse>;
    /**刪除樣板 */
    DeleteTemp(id: number): Promise<IResponse>;
    /**新增欄位 */
    AddField(input: EditTempFieldModel): Promise<IResponse>;
    /**編輯欄位 */
    EditField(input: EditTempFieldModel): Promise<IResponse>;
    /**刪除欄位 */
    DeleteField(input: DeleteTempFieldModel): Promise<IResponse>;
    /**依據選擇類型取得欄位表單View */
    GetFieldView(tempId: number, fieldType: ColFieldType): Promise<string>;
}
/**
 *自訂樣板欄位路由
 * @export
 * @class ColTemplateController
 * @extends {BaseController}
 * @implements {IColTemplateController}
 */
export class ColTemplateController extends BaseController<API_ColTemplate> implements IColTemplateController {
    constructor() {
        super({
            /**樣版列表 */
            Search: GetUrl(Controller.ColTemplate, Action.Search).href,
            /**View:編輯*/
            ShowEdit: GetUrl(Controller.ColTemplate, Action.ShowEdit).href,
            /**View:刪除 */
            ShowDelete:GetUrl(Controller.ColTemplate,Action.ShowDelete).href,
            /**依選擇類型回傳新增表單 */
            ShowChooseView: GetUrl(Controller.ColTemplate, Action.ShowChooseType).href,
            /**顯示代碼子檔 */
            ShowCogView: GetUrl(Controller.ColTemplate, Action.ShowCog).href,
            /**新增樣版 */
            CreateCopy: GetUrl(Controller.ColTemplate, Action.Copy).href,
            /**編輯樣版 */
            Edit: GetUrl(Controller.ColTemplate, Action.Edit).href,
            /**刪除樣版 */
            Delete: GetUrl(Controller.ColTemplate, Action.Delete).href,
            /**新增樣版欄位 */
            AddField: GetUrl(Controller.ColTemplate, Action.AddField).href,
            /**編輯樣版欄位 */
            EditField: GetUrl(Controller.ColTemplate, Action.EditField).href,
            /**刪除樣版欄位 */
            DeleteField: GetUrl(Controller.ColTemplate, Action.DeleteField).href,
            /** 樣板選單 */
            GetTemplateList: GetUrl(Controller.ColTemplate, Action.GetTemplateList).href,
        });
    }
    static get api() {
        return new this.api();
    }
    CreateCopyTemp(input: SelectTempModel): Promise<IResponse> {
        return AjaxPost<SelectTempModel>(this.api.CreateCopy, input, false);
    }
    EditTemp(input: EditTempleteModel): Promise<IResponse> {
        let INPUT = {
            fsCREATED_BY: '',
            fdCREATED_DATE: dayjs().format('YYYY/MM/DD HH:mm:ss'),
            fsUPDATED_BY: '',
            fdUPDATED_DATE: dayjs().format('YYYY/MM/DD HH:mm:ss'),
        };
        Object.keys(input).forEach(key => (INPUT[key] = input[key]));
        return AjaxPost<EditTempleteRealModel>(this.api.Edit, <EditTempleteRealModel>input, false);
    }
    DeleteTemp(id: number): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.Delete, { id: id }, false);
    }
    AddField(input: EditTempFieldModel): Promise<IResponse> {
        return AjaxPost<EditTempFieldModel>(this.api.AddField, input, false);
    }
    EditField(input: EditTempFieldModel): Promise<IResponse> {
        return AjaxPost<EditTempFieldModel>(this.api.EditField, input, false);
    }
    DeleteField(input: DeleteTempFieldModel): Promise<IResponse> {
        return AjaxPost<DeleteTempFieldModel>(this.api.DeleteField, input, false);
    }
    GetFieldView(tempId: number, fieldType: ColFieldType): Promise<string> {
        return Ajax<{ fnTEMP_ID: number; FieldType: string }>(
            'POST',
            this.api.ShowChooseView,
            {
                fnTEMP_ID: tempId,
                FieldType: fieldType,
            },
            false
        );
    }
}
