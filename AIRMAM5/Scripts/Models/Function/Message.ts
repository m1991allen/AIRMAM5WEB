import { IsNULLorEmpty } from './Check';

/**顯示成功訊息 */
export function SuccessMessage(message: string) {
    toastr.success(message);
}
/**顯示錯誤訊息 */
export function ErrorMessage(message: string) {
    toastr.error(message);
}
/**顯示資訊訊息 */
export function InfoMessage(message: string) {
    toastr.info(message);
}
/**顯示警告訊息 */
export function WarningMessage(message: string) {
    toastr.warning(message);
}

/**
 * 自定義Html訊息
 * @param html html內容
 * @param type 訊息樣式
 */
export function HtmlMessage(html: string, type?: 'success' | 'info' | 'warning' | 'error') {
    type = IsNULLorEmpty(type) ? 'info' : type;
    switch (type) {
        case 'success':
            toastr.success(html);
            break;
        case 'error':
            toastr.error(html);
            break;
        case 'info':
            toastr.info(html);
            break;
        case 'warning':
            toastr.warning(html);
            break;
    }
}

/**子iframe推訊息給父iframe */
export function windowPostMessage<T>(input: T) {
    window.top.postMessage(input, '/');
}
