import { PopularKeywordsModel } from './PopularKeywordsModel';

/**
 *  熱索關鍵字統計
 */
export interface HotkeyModel extends PopularKeywordsModel {
    /** 次數 */
    Counts: number;
    /**最近時間 */

    LastTime: string;
}
