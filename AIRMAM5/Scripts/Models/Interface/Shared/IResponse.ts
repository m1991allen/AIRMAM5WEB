import { HttpStatusCode } from '../../Enum/HttpStatusCode';
/**
 * Json回應格式,
 * R=Record回傳格式(不能確定格式請回傳object),
 * T=Data回傳格式(不能確定格式請回傳object)
 */
export interface IResponse<R = object, T = object> extends IShortResponse<T> {
    StatusCode: HttpStatusCode;
    ResponseTime: string;
    Records?: R;
    /** 配合後端修改, 此參數移到 IShortResponse */ // Data?:object;
}
/**
 * Json回應格式(簡短),
 * T=Data回傳格式(不能確定格式請回傳object)
 */
export interface IShortResponse<T = object> {
    IsSuccess: boolean;
    Message: string;
    ErrorException?: any;
    Data?: T;
}
