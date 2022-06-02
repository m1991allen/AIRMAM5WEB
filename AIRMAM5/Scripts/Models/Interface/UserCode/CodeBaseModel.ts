import { CodeIdModel } from './CodeIdModel';

/**基本代碼欄位(無備註) */
export interface CodeBaseModel extends CodeIdModel {
    /*代碼標題*/
    fsTITLE: string;
    /**是否啟用 */
    IsEnabled: boolean;
}
