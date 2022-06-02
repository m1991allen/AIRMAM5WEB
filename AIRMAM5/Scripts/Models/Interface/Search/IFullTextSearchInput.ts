import { SortEnum } from '../../Enum/SortEnum';
import { HomoMode } from '../../Enum/HomoMode';
import { SynonymousMode } from '../../Enum/SynonymousMode';

/**提交全文檢索表單參數 */
export interface IFullTextSearchInput {
    /**搜尋關鍵字 */
    fsKEYWORD: string;
    /**要查詢的索引庫名稱，以逗號分隔 [fsINDEX] 例: Video_DEV,Audio_DEV,Photo_DEV,Doc_DEV */
    fsINDEX: string;
    /** 搜尋模式(0:一般模式、2:同義詞搜尋) */
    fnSEARCH_MODE: SynonymousMode;
    /**同音(0:關閉、1:開啟)*/
    fnHOMO: HomoMode;
    /**日期條件 */
    clsDATE: DateSearchModel;
    /**欄位排序 */
    lstCOLUMN_ORDER: Array<ColumnSort>;
    /**樣板Id */
    fnTEMP_ID: number;
    /** */
    lstCOLUMN_SEARCH: Array<AdvancedQryModel>;
    /** 每頁筆數(前端指定)  */
    fnPAGE_SIZE: number;
    /** 開始筆數(前端指定) */
    fnSTART_INDEX: number;
}
/**搜尋日期條件 */
interface DateSearchModel {
    /**日期欄位名稱 */
    fsCOLUMN: string | 'fdCREATED_DATE';
    /**起始日期(需使用斜線) */
    fdSDATE: string;
    /**結束日期(需使用斜線) */
    fdEDATE: string;
}
/**
 *  欄位排序 [clsCOLUMN_ORDER]
 * (應該)日期查詢與DateSearchModel 配合一起使用的
 */
interface ColumnSort {
    /** 欄位名稱 */
    fsCOLUMN: string;
    /** 值 例:1:升冪(ASC)、2:降冪(Desc) */
    fsVALUE: string | SortEnum;
}
/**進階查詢:欄位檢索類別 */
export interface AdvancedQryModel {
    /** 是否全文檢索(true:檢索、false:區間或比對) */
    fbIS_FULLTEXT: boolean;
    /** 欄位名稱 例:fsATTRIBUTE1 */
    fsCOLUMN: string;
    /**值, 例:大時代 */
    fsVALUE: string;
}
