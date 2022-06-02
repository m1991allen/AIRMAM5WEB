import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { initSetting } from '../initSetting';
import { IsNULLorEmpty } from './Check';

/**
 * 取得指定網址的指定參數值
 * @param url 指定網址
 * @param paramName 指定參數名稱
 */
export function GetUrlParameters(url: URL | 'currentPage', paramName: string): string {
    let getURL: URL = url == 'currentPage' ? new URL(window.location.href) : url;
    let paramValue = new URLSearchParams(getURL.search).get(paramName);
    return paramValue;
}

/**
 * 由控制器與方法取API位置
 * @param controller 控制器
 * @param action 方法
 */
export function GetUrl(controller: Controller, action: Action): URL {
    const path: string = [initSetting.APIUrl, controller, action].join('/');
    return new URL(path);
}
/**
 * 由控制器與方法取WEB URL位置
 * @param controller 控制器
 * @param action 方法
 */
export function GetWebUrl(controller: Controller, action: Action): URL {
    const path: string = [initSetting.WebUrl, controller, action].join('/');
    return new URL(path);
}

/**
 * 取得圖片位置,例如A.png=>http://localhost/Images/A.png
 * (此功能假定圖片都放在http://XXXX/Images目錄中)
 * @param pictureName 圖片名稱
 */
export function GetImageUrl(pictureName: string): URL {
    const path: string = IsNULLorEmpty(pictureName) ? '' : [initSetting.WebUrl, 'Images', pictureName].join('/');
    return new URL(path);
}
