import { CodeNoteModel } from './CodeNoteModel';

/**編輯自定義代碼  */
export interface EditUserCodeModel extends CodeNoteModel {
    /**是否啟用 */
    fsIS_ENABLED: boolean;
}
