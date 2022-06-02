import { MediaType, SearchTypeEnum } from '../../Enum/MediaType';
/**
 * 上傳用樣板,用以取得預編詮釋資料
 */
export interface DirTempleteModel {
    /**樣板名稱 */
    fsNAME: string;
    /**是否可查詢 */
    IsSearch: boolean;
    /**檢索專用媒體類型 */
    SearchType: SearchTypeEnum;
    /**資料表S、V、P、D、A */
    fsTABLE: MediaType;
    /**樣板Id */
    fnTEMP_ID: number;
}
