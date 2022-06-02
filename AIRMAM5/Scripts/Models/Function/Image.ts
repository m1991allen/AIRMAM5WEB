import { HttpStatusCode } from '../Enum/HttpStatusCode';
import { IsNULLorEmpty } from './Check';

/**
 * 檢查image 的Url是否有效
 * @returns Promise<boolean>
 * @param url 圖片網址
 */
export const IsImageValid = (url: string): Promise<boolean> => {
    return new Promise((resolve, reject) => {
        if (IsNULLorEmpty(url)) {
            resolve(false);
        } else {
            const img = new Image();
            img.src = url;
            img.onload = () => {
                resolve(true);
            };
            img.onerror = () => {
                resolve(false);
            };
        }
    });
};

/**
 * 檢查檔案有無存在
 * @returns Promise<boolean>
 * @param url 檔案路徑
 */
export const IsFileValid = (url: string): Promise<boolean> => {
    return new Promise((resolve, reject) => {
        if (IsNULLorEmpty(url)) {
            resolve(false);
        } else {
            const xhr = new XMLHttpRequest();
            xhr.open('HEAD', url, true);
            xhr.send();
            /*
             * UNSENT: 0
             * OPENED: 0
             * LOADING: 200
             * DONE: 200
             */
            xhr.status == HttpStatusCode.NotFound ? resolve(false) : resolve(true);
        }
    });
};

/**
 * 選擇檔案函數,根據可接受類型過濾選擇檔案,並回傳檔案陣列
 * @param contentType 可接受類型 ,例如:image/*,.doc,.txt(與input[type='file']的accept屬性)
 * @param multiple 是否可上傳多個檔案
 */
export function selectFile(acceptType: string, multiple: boolean): Promise<Array<File>> {
    return new Promise((resolve, reject) => {
        const acceptArray: Array<string> = acceptType.split(',');
        let input = document.createElement('input');
        input.type = 'file';
        input.multiple = multiple;
        input.accept = acceptType;
        input.onchange = e => {
            const files = Array.from(input.files).filter(file => {
                const extension = file.name.slice(((file.name.lastIndexOf('.') - 1) >>> 0) + 2); //取副檔名
                if (acceptArray.indexOf('.'.concat(extension)) > -1) {
                    return file;
                }
            });
            multiple ? resolve(files) : resolve([files[0]]);
        };
        input.click(); //開啟選擇檔案視窗
    });
}
