import { MediaUploadType } from '../../Enum/MediaType';
import { SubjectTaskQuery } from './SubjectTaskQuery';
/**上傳用動態參數 */
export interface SubjectUploadQuery extends SubjectTaskQuery {
    MediaType: MediaUploadType;
    TitleDefine: number;
    CustomTitle: string;
}
