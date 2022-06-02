import { SelectListItem } from '../Shared/ISelectListItem';

/**調用參數(調用樣板選項) */
export interface BookingOptionModel {
    /**調用原因 */
    ResonStr: string;
    /**調用原因列 */
    ResonList: Array<SelectListItem>;
    /** 調用路徑  */
    PathStr: string;
    /**調用路徑列 */
    PathList: Array<SelectListItem>;
    /** 轉出格式(for 影片) */
    ProfileV: string;
    VideoProfileList: Array<SelectListItem>;
    /**轉出格式(for 聲音) */
    ProfileA: string;
    /**浮水印列 */
    AudioProfileList: Array<SelectListItem>;
    /**浮水印 */
    WatermarkStr: string;
    /**浮水印列 */
    WatermarkList: Array<SelectListItem>;
    /**調用說明是否可為空值 : true-空值、false-不可空值。 */
    DescIsNullable: boolean | true;
    /**調用編號s [fsMATERIAL_ID] (多筆以"^"為分隔符號 */
    MaterialIds: string;
}
