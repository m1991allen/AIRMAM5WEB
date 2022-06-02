import { BaseMessageModel } from '../Shared/PostMessage/BaseMessageModel';
import { GetPendingTapeModel } from '../Tsm/GetPendingTapeModel';



/**
 * 傳送刪除訊息
 */
export interface DocumentPostMessageModel extends  BaseMessageModel<'DeleteDocViewer'> {
    /**主題ID */
    subjectId: string;
    /**檔案編號 */
    fileNO: string;
    /**檔案名 */
    fileName: string;
    /**api位置 */
    api: string;
}

/**
 * 傳送開啟文件檢視器訊息
 */
export interface DocumentOpenMessageModel  extends  BaseMessageModel<'OpenDocViewer'>{
    /**文件位置 */
    href: string;
}
/**
 * 傳送關閉文件檢視器訊息
 */
export interface DocumentCloseMessageModel extends  BaseMessageModel<'CloseDocViewer'>{}

/**Tsm更新標籤內容訊息 */
export interface TsmMessageModel extends  BaseMessageModel<'UpdateTsmStatus'>{
    /**檔案編號 */
    fileno: string;
    /**標籤內容 */
    labelHTML: string;
}

/**Tsm上架列表更新*/
export interface TsmCheckInUpdateModel extends  BaseMessageModel< 'UpdateTsmCheckInList'> {
    list: Array<GetPendingTapeModel>;
}
/**告訴父iframe要開新的分頁tab */
export interface OpenTabMessageModel extends BaseMessageModel<'OpenTab'>{
    functionId: string; 
    tabText: string;
    iframeName?: string;
    src?: string;
    description?:string;
}