import { CodeBaseModel } from './CodeBaseModel';

/**基本代碼欄位(加備註) */
export interface CodeNoteModel extends CodeBaseModel {
    /**代碼備註 */
    fsNOTE: string;
}
