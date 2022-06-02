/**
 *檢查是不是null
 *
 * @param {*} param 變數名稱
 * @returns boolean(是否)
 */
export function IsNULL(param: any): boolean {
    return param == null ? true : false;
}
/**
 * 檢查是不是空字串
 * @param param
 */
export function IsEmpty(param: any): boolean {
    return param == '' ? true : false;
}
/**
 * 檢查是不是null或空字串
 * @param param
 */
export function IsNULLorEmpty(param: any): boolean {
    return param == '' || param == null ? true : false;
}
/**
 * 檢查是不是null或undefined
 * @param param
 */
export function IsNullorUndefined(param: any): boolean {
    return param == null || param === undefined || typeof param === 'undefined' ? true : false;
}
/**檢查是不是空物件 */
export function IsNULLObject(item: object): boolean {
    for (var key in item) {
        if (this.hasOwnProperty(key)) return false;
    }
    return true;
}
