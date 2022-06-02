
/**
 * 建立Cookie
 * @param name Cookie名稱
 * @param value Cookie值
 * @param days 有效時間
 */
export function createCookie(name: string, value: string, days: number): void {
    let date = new Date();
    date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
    const expires = days <= 0 ? '' : '; expires=' + date.toUTCString();
    document.cookie = name + '=' + value + expires + '; path=/';
}

/**
 * 讀取Cookie
 * @param name Cookie名稱
 */
export function readCookie(name): string {
    var nameEQ = name + '=';
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') {
            c = c.substring(1, c.length);
        }
        if (c.indexOf(nameEQ) === 0) {
            return c.substring(nameEQ.length, c.length);
        }
    }
    return null;
}
