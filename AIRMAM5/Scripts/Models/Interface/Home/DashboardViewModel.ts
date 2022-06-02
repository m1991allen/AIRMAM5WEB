import { StatisticsModel } from './StatisticsModel';
import { HotkeyModel } from './HotkeyModel';
import { ChartModel } from './ChartModel';
import { AnnListModel } from '../Ann/AnnListModel';

/** DashBoard 資料model */
export interface DashboardViewModel {
    StatisticsData: Array<StatisticsModel>;
    /** 統計圖Chart資料model */
    Charts: ChartModel;
    /** 熱索關鍵字統計資料model */
    HotkeyData: Array<HotkeyModel>;
    /**系統公告資料model */
    AnnounceData: Array<AnnListModel>;
}
