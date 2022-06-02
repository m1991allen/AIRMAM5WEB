import { SubjectModel } from './SubjectModel';

/**主題參數主檔參數 */
interface SubjectMainModel extends SubjectModel {
    /**主題標題 */
    Title: string;
    /**主題描述 */
    Description: string;
}
