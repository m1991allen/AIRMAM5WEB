import { ThridSystemEnum } from "../../Enum/ThridSystemEnum";

/**文稿對應設定參數 */
export interface DraftSetModel{
    /**第三方系統類型 */
   ExecType:keyof typeof ThridSystemEnum;
   /**片庫媒體檔案號碼 */
   FileNo:string;
   /**第三方系統對應主key */
   Fields:Array<string>;
   /**第三方系統對應參數 */
   Values:Array<string>;
}