import { ThridSystemEnum } from "../../Enum/ThridSystemEnum";

/**第三方系統類型 */
export type ThridSystemType=keyof typeof ThridSystemEnum;

/**第三方系統(如文稿,公文)設置對應參數 */
export interface SaveDraftViewModel{
    /**第三方系統類型 */
   type:keyof typeof ThridSystemEnum;
   /**原始系統Id*/
   OriginalId:string;
   /**第三方Id*/
   ThirdSystemId:string;
}

