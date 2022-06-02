import { IDynamicFieldModel } from '../Shared/IDynamicFieldModel';
import { TableUserDateByNameModel } from '../Shared/IDate';

/**媒體資料 Metadata (動態欄位)*/
export interface MediaDataModel {
    /**主題ID */
    fsSUBJECT_ID: string;
    /**檔案編號 */
    fsFILE_NO: string;
    /**分類: A聲音, D文件, P圖片, S主題, V影片 */
    FileCategory: string; //FileType: string;
    /**標題 */
    Title: string;
    /**描述 */
    Description: string; 
    /**檔案機密 中文 */
    FileSecretStr: string; 
    /**擷取文字 (只有類別=D文件 才有此欄位內容) */ 
    Content: string;
    /**動態欄位群組 */
    ArcPreAttributes: Array<IDynamicFieldModel>;
    /**新增異動人員與時間 */ UserDateInfo: TableUserDateByNameModel;
    /**檔案原始檔名 */ OriginFileName: string;
    /**影片/聲音檔案 語音轉文字欄位 */Voice2TextContent: string;
    /**版權中文欄位 */LicenseStr: string;
    /**版權是否調用提醒 */IsAlert: boolean;
    /**版權是否調用禁止 */IsForBid: boolean;
    /**版權是否過期*/IsExpired:boolean;
    /**版權提醒訊息 */LicenseMessage:string;
    /**版權到期日*/LicenseEndDate:string;
    /**自訂標籤HashTag */HashTag:string;
}
