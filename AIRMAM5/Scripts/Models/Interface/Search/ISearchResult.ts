import { MediaType } from '../../Enum/MediaType';
/**檢索列表結果 */
export interface ISearchResult {
    /** 符合關鍵字附近的字 */
    fsMATCH: string;
    /**主題編號 */
    fsSUBJECT_ID: string;
    /**檔案編號: 影音圖文 回傳內容皆相同 */
    fsFILE_NO: string;
    /**檔案標題 */
    Title: string;
    /**主題標題 */
    SubjectTitle: string;
    /**建立日期  */
    CreateDate: string;
    /**檔案類型 (ex: doc,JPG,ppt,....) */
    FileType: string;
    /**{影/音}時間表示 (ex: 00:13:05.32, 00:03:29.63 ) */
    Duration: string;
    /** 代表圖 */
    HeadFrame: string;
    /**媒體檢索類別 */
    SearchType: MediaType;
    /**檔案狀態: 0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中) => 通常為 0 或 1  */
    TSMFileStatus: number;
    /**檔案狀態顯示文字 */
    TSMFileStatusStr: string;
    /**檔案 橫向H / 直向V */
    fsDIRECTION: string;
    /**轉檔工作狀態 */
    fsSTATUS: string;
    /**檔案寬 */
    fnWIDTH: number;
    /**檔案高 */
    fnHEIGHT: number;
    /**版權代碼*/
    LicenseCode: string;
    /**版權中文 */
    LicenseStr: string;
    /**版權是否調用提醒*/
    IsAlert: boolean;
    /**版權是否調用禁止*/
    IsForBid: boolean;
    /**版權是否過期*/
    IsExpired:boolean;
    /**版權提醒訊息 */
    LicenseMessage: string;
    /**版權到期日*/
    LicenseEndDate:string;
    /**自訂標籤HashTag */
    HashTag: Array<string>|string;//NOTCIE:後端傳回來一樣結構但此欄位卻有可能是陣列或文字
}
