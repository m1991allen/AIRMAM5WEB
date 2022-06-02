/**
 * 基本路由接口
 */
export interface IBaseController<T> {
    /**BaseController_API設定 */
    readonly api: T;
    /**取得傳入介面欄位名稱 */
    GetProperty<TObj>(name: keyof TObj): keyof TObj;
}

class BaseAPIModel<T> {
    private _api: T;
    get api() {
        return this._api;
    }
    set api(value: T) {
        this._api = value;
    }
}

/**
 * 基本路由
 * (抽像類:繼承的class需要進行實現下方定義的抽象方法)
 */
export abstract class BaseController<T> implements IBaseController<T> {
    private model: BaseAPIModel<T> = new BaseAPIModel();
    constructor(apiConfig: T) {
        this.model.api = apiConfig;
    }
    public get api() {
        return this.model.api;
    }
    GetProperty<TObj>(name: keyof TObj): keyof TObj {
        return name;
    }
}
