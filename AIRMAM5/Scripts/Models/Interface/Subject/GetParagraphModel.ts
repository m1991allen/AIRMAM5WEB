import { SubjectBaseModel } from './SubjectBaseModel';

/**取得段落描述參數 */
export interface GetParagraphModel extends SubjectBaseModel {
    /**段落序號 */
    seqno: number;
}
