import { ItabulatorService, ItabulatorExtendDocumentService } from '../Interface/Service/ItabulatorService';
import { ReportEnum } from '../Enum/ReportEnum';
import { initSetting, TabulatorSetting } from '../initSetting';
import { IsNULL, IsNullorUndefined } from '../Function/Check';
import { IResponse } from '../Interface/Shared/IResponse';
import { Logger } from './LoggerService';
/**
 * tabulator js 服務(基本,不含文件下載功能)
 * 使用前需要引用tabulartor css和js
 */
export class tabulatorService implements ItabulatorService {
    protected table: Tabulator;
    constructor(tableId: string, settings?: Tabulator.Options, ShowGoPage?: boolean) {
        settings.height = settings.height || TabulatorSetting.height;
        settings.layout = settings.layout || TabulatorSetting.layout;
        settings.pagination =
            !IsNullorUndefined(settings.pagination) || !IsNullorUndefined(settings.ajaxProgressiveLoad)
                ? settings.pagination
                : TabulatorSetting.pagination;
        settings.paginationSize = settings.paginationSize || TabulatorSetting.paginationSize;
        settings.placeholder = settings.placeholder || TabulatorSetting.placeholder;
        settings.selectable = settings.selectable || TabulatorSetting.selectable;
        settings.locale = settings.locale || TabulatorSetting.locale;
        settings.langs = settings.langs || {
            'zh-tw': {
                ajax: {
                    loading: TabulatorSetting.langs.ajax.loading, //ajax loader text
                    error: TabulatorSetting.langs.ajax.error, //ajax error text
                },
                pagination: {
                    page_size: TabulatorSetting.langs.pagination.page_size,
                    first: TabulatorSetting.langs.pagination.first, //text for the first page button
                    first_title: TabulatorSetting.langs.pagination.first_title, //tooltip text for the first page button
                    last: TabulatorSetting.langs.pagination.last,
                    last_title: TabulatorSetting.langs.pagination.last_title,
                    prev: TabulatorSetting.langs.pagination.prev,
                    prev_title: TabulatorSetting.langs.pagination.prev_title,
                    next: TabulatorSetting.langs.pagination.next,
                    next_title: TabulatorSetting.langs.pagination.next_title,
                },
            },
        };
        settings.renderComplete = function() {
            const table = document.querySelector(tableId);
            const footers = table.querySelectorAll('.tabulator-footer');
            const IsFooterExist = footers.length > 0;
            if ((IsNullorUndefined(ShowGoPage) || ShowGoPage) && IsFooterExist) {
                const footer = footers[0];
                if (footer.querySelectorAll('.gogroup').length == 0) {
                    const filterinput = document.createElement('input');
                    filterinput.autocomplete = 'off';
                    filterinput.id = 'wordFilter';
                    filterinput.type = 'text';
                    filterinput.placeholder = '請輸入篩選詞彙';
                    const gopage = document.createElement('div');
                    gopage.className = `${TabulatorSetting.pagenoInputClassName}`;
                    gopage.insertAdjacentHTML('afterbegin', filterinput.outerHTML);
                    const go = document.createElement('div');
                    go.classList.add('gogroup');
                    go.innerHTML = `快速篩選:${gopage.outerHTML}`;
                    footer.appendChild(go);
                }
            }
        };
        if (IsNullorUndefined(settings.ajaxResponse)) {
            settings.ajaxResponse = function(url, params, response: IResponse<object, object>) {
                const resData = !IsNULL(response.Data) ? JSON.parse(JSON.stringify(response.Data)) : [];
                const tabulatorFooter = document.querySelector(tableId).querySelectorAll('.tabulator-footer');
                if (tabulatorFooter !== undefined && tabulatorFooter.length > 0) {
                    const gogroupElem = tabulatorFooter[0].querySelectorAll('.gogroup');
                    if (gogroupElem.length > 0) {
                        gogroupElem.forEach(elem => {
                            (<HTMLDivElement>elem).style.display =
                                ShowGoPage == false
                                    ? 'none !important'
                                    : resData.length == 0
                                    ? 'none !important'
                                    : 'block !important';
                        });
                    }
                }
                Logger.log('tabulator列表:', response);
                return resData;
            };
        }
        try {
            this.table = new Tabulator(tableId, settings);
        } catch (error) {
            Logger.viewres(settings.ajaxURL, 'tabulator列表', error, false);
        }
    }
    SetFilter(filters: string | any[] | Tabulator.Filter[]) {
        this.table.setFilter([{}, filters]); /*Notice:第一個是And,第二個才是OR*/
    }
    ReactivityUpdate(rowindex: number | string | Tabulator.RowComponent, updateData: object) {
        this.table.updateRow(rowindex, JSON.stringify(updateData));
    }
    AddRow<T = object>(newData: T) {
        this.table.addRow(JSON.stringify(newData));
    }
    RemoveRow(rowindex: number | string | Tabulator.RowComponent) {
        this.table.deleteRow(rowindex);
    }
    GetRow(rowindex: number | string): HTMLElement {
        return this.table.getRow(rowindex).getElement();
    }
    GetRowData(rowindex: number | string): any {
        return this.table.getRow(rowindex).getData();
    }
    GetLastSelectedRows(): Tabulator.RowComponent[] {
        return this.table.getSelectedRows();
    }
    GetData(): any {
        return this.table.getData();
    }
    GetTable(): Tabulator {
        return this.table;
    }
    RedrawTable(): void {
        this.table.redraw();
    }
}

/**
 * tabulator js 服務(進階,含文件下載功能)
 * 使用前需要引用tabulartor css和js
 */
export class tabulatorExtnedDocumentService extends tabulatorService implements ItabulatorExtendDocumentService {
    protected table: Tabulator;
    constructor(tableId: string, settings?: Tabulator.Options) {
        super(tableId, settings);
    }
    /**下載PDF */
    DownloadPDF(docTitle: string, orientationType: ReportEnum) {
        this.table.download('pdf', docTitle + '.pdf');
    }
    DownloadExcel(docTitle: string) {
        this.table.download('xlsx', docTitle + '.xlsx', { sheetName: docTitle });
    }
    DownloadJSON(docTitle: string) {
        this.table.download('json', docTitle + '.json');
    }
    DonwloadCSV(docTitle: string) {
        this.table.download('csv', docTitle + '.csv');
    }
    Print(title: string, html: string) {
        let printwindow = window.open('', '列印', 'resizable=yes');
        printwindow.document.open();
        printwindow.document.write(
            `<!DOCTYPE html><html lang=""><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1.0">`
        );
        printwindow.document.write(`<title>列印</title>`);
        printwindow.document.write(`<style>h1,h2,h3,h4,h5,h6{font-weight:bold;color:#00cccc;}
                                table {margin: 25px auto;border-collapse: collapse;border: 1px solid #eee;width:100%;}
                                table tr:hover {background: #f4f4f4;}
                                table tr:hover td {color: #555;}
                                table th, table td {color: #999;border: 1px solid #eee;padding: 12px 35px;border-collapse: collapse;}
                                table th {background: #00cccc;color: #fff;text-transform: uppercase;font-size: 12px;}
                                table th.last {border-right: none;}
                               @media print{ h1,h2,h3,h4,h5,h6{color:#333;}table th,table td{color:#333;border-color:#888;}}</style>`);
        printwindow.document.write(`</head><body>`);
        printwindow.document.write(title);
        printwindow.document.write(html);
        printwindow.document.write(`</body></html>`);
        printwindow.print();
    }
}
