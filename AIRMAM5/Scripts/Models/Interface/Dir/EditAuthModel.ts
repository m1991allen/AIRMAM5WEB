import { EditAuthInputModel } from './EditAuthInputModel';

/**群組/使用者權限資料擴充*/
export interface EditAuthModel {
    /** 欄位類別 : G群組/U使用者 */
    DataType: 'G' | 'U';
    /**??? */
    fnPARENT_ID: number;
    /**???? */
    C_sDIR_PATH: string;
}
/**編輯群組/使用者權限：丟給後端 */
export interface EditAuthRealModel extends EditAuthModel, EditAuthInputModel {}
