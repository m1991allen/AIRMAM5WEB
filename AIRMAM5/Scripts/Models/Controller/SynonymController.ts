import { IResponse } from '../Interface/Shared/IResponse';
import { SynonymEditModel } from '../Interface/Synonym/SynonymEditModel';
import { API_Synonym } from '../Const/API';
import { BaseController, IBaseController } from './BaseController';
import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { GetUrl } from '../Function/Url';
import { SynonymCreateModel } from '../Interface/Synonym/SynonymCreateModel';
import { AjaxGet, AjaxPost } from '../Function/Ajax';
import { IdModel } from '../Interface/Shared/IdModel';

export interface ISynonymController extends IBaseController<API_Synonym> {
    /**新增同義詞 */
    Create(input: SynonymCreateModel): Promise<IResponse>;
    /**編輯同義詞 */
    Edit(input: SynonymEditModel): Promise<IResponse>;
    /**刪除同義詞 */
    Delete(indexId: number): Promise<IResponse>;
    /**重建同義詞 */
    Rebuild(): Promise<IResponse>;
}

export class SynonymController extends BaseController<API_Synonym> implements ISynonymController {
    constructor() {
        super({
            Search: GetUrl(Controller.Synonym, Action.Search).href,
            Create: GetUrl(Controller.Synonym, Action.Create).href,
            Edit: GetUrl(Controller.Synonym, Action.Edit).href,
            Delete: GetUrl(Controller.Synonym, Action.Delete).href,
            Rebuild: GetUrl(Controller.Synonym, Action.Rebuild).href,
            ShowCreate: GetUrl(Controller.Synonym, Action.ShowCreate).href,
            ShowEdit: GetUrl(Controller.Synonym, Action.ShowEdit).href,
            ShowDetail: GetUrl(Controller.Synonym, Action.ShowDetails).href,
            ShowDelete: GetUrl(Controller.Synonym, Action.ShowDelete).href,
        });
    }
    Create(input: SynonymCreateModel): Promise<IResponse> {
        return AjaxPost<SynonymCreateModel>(this.api.Create, input, false);
    }
    Edit(input: SynonymEditModel): Promise<IResponse> {
        return AjaxPost<SynonymEditModel>(this.api.Edit, input, false);
    }
    Delete(indexId: number): Promise<IResponse> {
        return AjaxPost<IdModel>(this.api.Delete, { id: indexId }, false);
    }
    Rebuild(): Promise<IResponse> {
        return AjaxGet(this.api.Rebuild, {}, false);
    }
}
