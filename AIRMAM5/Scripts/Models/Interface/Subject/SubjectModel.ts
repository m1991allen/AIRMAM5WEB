import { SubjectBaseModel } from './SubjectBaseModel';

/**主題基本參數+主題Id */
export interface SubjectModel extends SubjectBaseModel {
    /**主題Id  */
    SubjectId: string;
}
