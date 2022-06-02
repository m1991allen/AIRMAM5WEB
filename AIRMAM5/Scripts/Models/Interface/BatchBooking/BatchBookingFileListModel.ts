import { MediaType } from '../../Enum/MediaType';
/**批次調用檔案列表 */
export interface BatchBookingFileListModel {
    /**代表圖 */
    ImageUrl: string;
    /**檔案編號 */
    fsFILE_NO: string;
    /**標題 */
    Title: string;
    /**描述 */
    Description: string;
    /**媒體類型 */
    FileType: MediaType;
    /**主題編號 */
    fsSUBJECT_ID: string;
    /**版權代號*/
    LicenseCode:string;
    /**版權中文欄位 */
    LicenseStr: string;
    /**版權訊息 */
    LicenseMessage:string;
    /**是否需要顯示警告 */
    IsAlert:boolean;
    /**是否禁止調用  */
    IsForBid:boolean;
    /**版權是否過期 */
    IsExpired:boolean;
    /**版權授權到期日 */
    LicenseEndDate:string;

}
