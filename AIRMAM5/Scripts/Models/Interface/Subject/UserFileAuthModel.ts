import { UserDirAuthModel } from '../Subject/UserDirAuthModel';

/**使用者藉由檔案取得權限回應資訊 */
export interface UserFileSubjectAuthModel extends UserDirAuthModel {
    /**主題編號 */
    SubjectId: string;
    /**檔案編號 */
    FileNo: string;
}
