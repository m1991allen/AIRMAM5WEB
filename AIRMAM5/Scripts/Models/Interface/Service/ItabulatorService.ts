import { ReportEnum } from '../../Enum/ReportEnum';
/**
 *  tabulator js 服務介面(基本,不含文件下載功能)
 */
export interface ItabulatorService {
    /**
     * 設定過濾器
     * @param filters 過濾器規則
     */
    SetFilter(filters: string | any[] | Tabulator.Filter[]): void;
    /**
     * 更新改行數據
     * @param rowindex 行的索引值或Tabulator行元素
     * @param updateData 欲更新的數據
     */
    ReactivityUpdate(rowindex: number | string | Tabulator.RowComponent, updateData: object);
    //ReactivityUpdate(rowindex:number,updateData:Array<{columnName:string;columnValue:any}>);
    /**
     * 將新數據增加到tabulator,並產生新行
     * @param newData 新數據
     */
    AddRow<T = object>(newData: T);
    /**依據行索引,刪除數據(注意要先設定欄位索引) */
    RemoveRow(rowindex: number | string | Tabulator.RowComponent);
    /**依據行索引,取得該行Element */
    GetRow(rowindex: number | string): HTMLElement;
    /**取得行的數據 */
    GetRowData(rowindex: number | string): any;
    /**取得選擇的行 */
    GetLastSelectedRows(): Tabulator.RowComponent[];
    /**取得table資料 */
    GetData(): any;
    /**取得table */
    GetTable(): Tabulator;
    /**重繪列表 */
    RedrawTable(): void;
}
/**
 * tabulator js 服務介面(進階,含文件下載功能)
 */
export interface ItabulatorExtendDocumentService extends ItabulatorService {
    /**
     * 列印
     * @param title 列印標題
     * @param html 列印內容(html)
     */
    Print(title: string, html: string): void;
    /**
     * 下載PDF
     * @param docTitle 下載檔案名稱
     * @param orientationType 直式或橫式
     */
    DownloadPDF(docTitle: string, orientationType: ReportEnum): void;
    /**
     * 下載excel
     * @param docTitle 下載檔案名稱
     */
    DownloadExcel(docTitle: string): void;
    /**
     * 下載JSON
     * @param docTitle 下載檔案名稱
     */
    DownloadJSON(docTitle: string): void;
    /**
     * 下載CSV
     * @param docTitle 下載檔案名稱
     */
    DonwloadCSV(docTitle: string): void;
}
