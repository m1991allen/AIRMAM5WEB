import { initSetting } from '../initSetting';
import { IResponse } from '../Interface/Shared/IResponse';
import { SuccessMessage, ErrorMessage } from '../Function/Message';
import { IsNULL, IsNullorUndefined } from '../Function/Check';

/**
 * 主控台訊息紀錄服務_靜態方法
 */
export class Logger {
    /**基本紀錄 */
    static log(message: string, ...args): void {
        args.length > 0
            ? initSetting.ShowLog && console.log(message, args)
            : initSetting.ShowLog && console.log(message);
    }
    /**警告紀錄 */
    static warn(message: string, ...args): void {
        args.length > 0
            ? initSetting.ShowLog && console.warn(message, args)
            : initSetting.ShowLog && console.warn(message);
    }
    /**錯誤紀錄 */
    static error(message: string, ...args): void {
        args.length > 0
            ? initSetting.ShowLog && console.error(message, args)
            : initSetting.ShowLog && console.error(message);
    }
    /**條件性錯誤消息 */
    static assert(condition: boolean, message: string, ...args): void {
        initSetting.ShowLog && console.assert(condition, message, args);
    }
    /**嵌套日誌記錄組 */
    static group(groupName: any[] | any): void {
        initSetting.ShowLog && console.group(groupName);
    }
    /**折疊日誌記錄組 */
    static groupCollapsed(groupName: any[]): void {
        initSetting.ShowLog && console.groupCollapsed(groupName);
    }
    /**折疊和嵌套日誌記錄組結束 */
    static groupEnd(): void {
        initSetting.ShowLog && console.groupEnd();
    }
    /**物件數據表單記錄 */
    static table(input: object, keyName?: Array<string>) {
        initSetting.ShowLog && console.table(input, keyName);
    }
    /**自定義:IResponse紀錄 */
    static res(api: string, actionStr: string, response: IResponse, showMessage?: boolean): void {
        this.group(`【${response.StatusCode}】${actionStr}執行回應`);
        if (response.IsSuccess) {
            this.log(`訊息:${response.Message}`);
            this.log(`ApiURL:${api}`);
            this.log('Record:', response.Records);
            this.log('Data:', response.Data);
        } else {
            this.error(`訊息:${response.Message}`);
            this.error(`ApiURL:${api}`);
            this.error('Record:', response.Records);
            this.error('Data:', response.Data);
        }
        this.groupEnd();
        showMessage !== false
            ? response.IsSuccess
                ? SuccessMessage(response.Message)
                : ErrorMessage(response.Message)
            : false;
    }
    /**自定義:頁面問題紀錄 */
    static viewres(api: string, actionStr: string, error: any, showMessage?: boolean): void {
        this.group(`【${actionStr}執行錯誤】`);
        if (!IsNullorUndefined(error)) {
            this.error(`訊息:${error.statusText}`);
        }
        this.error(`ApiURL:${api}`);
        this.error('原因:', error);
        this.groupEnd();
        showMessage !== false ? ErrorMessage(`${actionStr}發生問題!請重新再試`) : false;
    }
}
