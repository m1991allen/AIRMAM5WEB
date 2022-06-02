import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { initSetting } from '../initSetting';
import { GetUrl } from './Url';
import { IResponse } from '../Interface/Shared/IResponse';
import { IsNULLorEmpty, IsNullorUndefined } from './Check';
import { Logger } from '../Class/LoggerService';
/*
 宣告共用 domain
 統一宣告所有API位置
 */
const baseUrl: string = initSetting.APIUrl;
/**
 * Ajax GET
 * @param controller 控制器
 * @param action 動作
 * @param parameters 參數
 * @param cache 是否要快取
 * @param beforeSendEvent 提交前事件
 * @param successEvent 成功事件
 * @param errorEvent 錯誤事件
 * @param completeEvent 完成後事件
 */
export function Get<T = object>(
    controller: Controller,
    action: Action,
    parameters?: T /*{[key: string]: any}*/,
    cache?: boolean,
    beforeSendEvent?: Function,
    successEvent?: Function,
    errorEvent?: Function,
    completeEvent?: Function
) {
    //Notice:如果沒有encode的話，可能會SQL Injection
    const searchParams =(IsNullorUndefined(parameters)||typeof(parameters)!=='object')? "":  Object.keys(parameters).map(key => { return encodeURIComponent(key) + '=' + encodeURIComponent(parameters[key]);}).join('&');
    $.ajax({
        url:IsNULLorEmpty(searchParams)? GetUrl(controller, action).href : GetUrl(controller, action).href + '?' + searchParams.toString(), /*IE Edge一定要toString才能解析*/
        type: 'GET',
        cache: cache,
        contentType: 'application/json',
        beforeSend: function(data) {
            if (beforeSendEvent != null) {
                beforeSendEvent(data);
            }
        },
        success: function(data) {
            if (successEvent != null) {
                successEvent(data);
            }
        },
        error: function(data) {
            if (errorEvent != null) {
                errorEvent(data);
            }
        },
        complete: function(data) {
            if (completeEvent != null) {
                completeEvent(data);
            }
        },
    });
}

/**
 * Ajax POST
 * T=規定的參數model
 * @param controller 控制器
 * @param action 動作
 * @param parameters 參數
 * @param cache 是否要快取
 * @param beforeSendEvent 提交前事件
 * @param successEvent 成功事件
 * @param errorEvent 錯誤事件
 * @param completeEvent 完成後事件
 */
export function Post<T = object>(
    controller: Controller,
    action: Action,
    parameters?: T,
    cache?: boolean,
    beforeSendEvent?: Function,
    successEvent?: Function,
    errorEvent?: Function,
    completeEvent?: Function
) {
    Logger.log(JSON.stringify(parameters));
    $.ajax({
        url: GetUrl(controller, action).href,
        type: 'POST',
        data: JSON.stringify(parameters),
        cache: cache,
        contentType: 'application/json',
        beforeSend: function(data) {
            if (beforeSendEvent != null) {
                beforeSendEvent(data);
            }
        },
        success: function(data) {
            if (successEvent != null) {
                successEvent(data);
            }
        },
        error: function(data) {
            if (errorEvent != null) {
                errorEvent(data);
            }
        },
        complete: function(data) {
            if (completeEvent != null) {
                completeEvent(data);
            }
        },
    });
}

/*-------------JXR輸出-----------------*/

export const GetJXR = <T>(url: string, parameters?: T, cache?: boolean): JQuery.jqXHR<any> => {
    //Notice:如果沒有encode的話，可能會SQL Injection
    const searchParams =(IsNullorUndefined(parameters)||typeof(parameters)!=='object')? "":  Object.keys(parameters).map(key => { return encodeURIComponent(key) + '=' + encodeURIComponent(parameters[key]);}).join('&');
    return $.ajax({
        url: IsNULLorEmpty(searchParams)?url:url + '?' + searchParams.toString(),
        type: 'GET',
        cache: cache,
        contentType: 'application/json',
    });
};

export const PostJXR = <T>(url: string, parameters: T, cache?: boolean): JQuery.jqXHR<any> => {
    return $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(parameters),
        cache: cache,
        contentType: 'application/json;charset=UTF-8',
    });
};

export const PostSerializeJXR = (url: string, serializeData: string, cache?: boolean): JQuery.jqXHR<any> => {
    return $.ajax({
        url: url,
        type: 'POST',
        data: serializeData,
        cache: cache,
        statusCode: {
            404: function() {
                Logger.log('ajax 404導向NotFound頁面');
                window.location.href = [initSetting.WebUrl, 'Views/Shared/NotFound.cshtml'].join('/');
                // window.location.href = baseUrl + '/Views/Shared/NotFound.cshtml';
            },
            401: function() {
                Logger.log('ajax 401導向:未經授權的錯誤頁面');
                window.location.href = [initSetting.WebUrl, 'Views/Shared/NoAuth.cshtml'].join('/');
            },
            403: function() {
                Logger.log('ajax 403導向:無權限頁面');
                window.location.href = [initSetting.WebUrl, 'Views/Shared/NoAuth.cshtml'].join('/');
            },
            405: function() {
                Logger.log('ajax 401導向:方法不允許的錯誤頁面');
                window.location.href = [initSetting.WebUrl, 'Views/Shared/Error.cshtml'].join('/');
            },
        },
    });
};
/**
 * Ajax Get <T=參數泛型>
 * @param url 路由位置 (string型態)
 * @param parameters 參數
 * @param cache 是否快取
 * @returns IResponse
 */
export function AjaxGet<T = object>(url: string, parameters?: T, cache?: boolean): Promise<IResponse> {
    return new Promise((resolve, reject) => {
        const task = GetJXR(url, parameters, cache);
        task.then(data => {
            resolve(data);
        }).catch(data => {
            reject(data);
        });
    });
}
/**
 * Ajax Post<T=參數泛型>
 * @param url 路由位置 (string型態)
 * @param parameters 參數
 * @param cache 是否快取
 * @returns IResponse
 */
export function AjaxPost<T = object>(url: string, parameters: T, cache?: boolean): Promise<IResponse> {
    Logger.log(JSON.stringify(parameters));
    return new Promise((resolve, reject) => {
        const task = PostJXR(url, parameters, cache);
        task.then(data => {
            resolve(data);
        }).catch(data => {
            reject(data);
        });
    });
}
/**
 * 自定義Ajax()
 * @param method 方法
 * @param url api位置
 * @param parameters 參數
 * @param cache
 * @returns 任何型態參數
 */
export function Ajax<T = object>(method: 'GET' | 'POST', url: string, parameters?: T, cache?: boolean): Promise<any> {
    return new Promise((resolve, reject) => {
        const task = method == 'GET' ? GetJXR(url, parameters, cache) : PostJXR(url, parameters, cache);
        task.then(data => {
            resolve(data);
        }).catch(data => {
            reject(data);
        });
    });
}

/**
 * 表單序列化Ajax
 * @param url api位置
 * @param formSerializeData 表單Data
 * @param cache 是否快取
 */
export function AjaxFormSerialize(url: string, serializeData: string, cache?: boolean): Promise<IResponse> {
    Logger.log(JSON.stringify(serializeData));
    return new Promise((resolve, reject) => {
        const task = PostSerializeJXR(url, serializeData, cache);
        task.then(data => {
            resolve(data);
        }).catch(data => {
            reject(data);
        });
    });
}
