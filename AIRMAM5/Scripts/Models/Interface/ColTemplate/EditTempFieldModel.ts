import { CreateTempFieldModel } from './CreateTempFieldModel';

/**編輯欄位 */
export interface EditTempFieldModel extends CreateTempFieldModel {
    //,templateFieldTime{
    /**樣板Id */
    fnTEMP_ID: number;
    /**代碼編號 */
    FieldCodeId: string; //fsCODE_ID:string;
    /**控制項類型 */
    FieldCodeCtrl: string; //fsCODE_CTRL:string;
}
