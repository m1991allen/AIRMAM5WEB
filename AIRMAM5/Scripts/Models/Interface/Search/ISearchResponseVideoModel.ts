import { ISearchResult } from './ISearchResult';
import { IFullTextSearchInput } from './IFullTextSearchInput';
import { TSMFileStatus } from '../../Enum/TSMFileStatus';

/**檢索結果-【影片】 */
export interface SearchResponseVideoModel {
    /**檢索結果-Basic */
    SearchBase: SearchResponseBaseModel;
    /** 影片-關鍵影格清單 */
    KeyFrameList: Array<VideoKeyFrameModel>;
    /** 影片-段落描述清單 */
    ParaDescription: Array<SubjectFileSeqmentModel>;
}
/**檢索結果-【聲音】 */
export interface SearchResponseAudioModel {
    /**檢索結果-Basic */
    SearchBase: SearchResponseBaseModel;
    /**段落描述清單 */
    ParaDescription: Array<SubjectFileSeqmentModel>;
    /** 聲音檔-專輯資訊*/
    AudioAlbumInfo: AudioInfoModel;
}
/**檢索結果-【圖片】 */
export interface SearchResponsePhotoModel {
    /**檢索結果-Basic */
    SearchBase: SearchResponseBaseModel;
    /** 圖片-EXIF資訊 */
    PhotoExifInfo: ExifInfo;
}
/** 檢索結果-【文件】 */
export interface SearchResponseDocModel {
    /**檢索結果-Basic */
    SearchBase: SearchResponseBaseModel;
    /** 文件-文件資訊 */
    DocInfo: ExtendIfno;
}

/**檢索結果-Basic*/
export interface SearchResponseBaseModel {
    /** 1檢索參數 */
    SearchParam: IFullTextSearchInput;
    /** 2查詢條件文字化顯示。例, 檢索類型：影片、聲音、圖片、文字。 查詢方式：同音、同義。 建立日期區間：2019/01/01~2019/12/31 新到舊。  */
    ConditionStr: ConditionModel;
    /** 3檢索符合筆數 */
    CountData: SearchCountResponseModel;
    /** 4檢索符合檔案編號清單*/
    MetaDataList: Array<ISearchResult>;
}
/** 查詢條件文字化顯示*/
export interface ConditionModel {
    /**檢索類型*/
    SearchType: string;
    /**查詢方式*/
    SearchMode: string;
    /**日期區間*/
    DateInterval: string;
    /**進階查詢*/
    AdvancedQry: string;
}
/**檢索符合筆數API Response Model*/
export interface SearchCountResponseModel {
    /**主題分類筆數 */
    fnSUBJECT_COUNT: number;
    /** 影片分類筆數 */
    fnVIDEO_COUNT: number;
    /** 聲音分類筆數 */
    fnAUDIO_COUNT: number;
    /**圖片分類筆數 */
    fnPHOTO_COUNT: number;
    /**圖片分類筆數 */
    fnDOC_COUNT: number;
}
/**段落描述清單 */
interface SubjectFileSeqmentModel {
    /**檔案編號 */
    fsFILE_NO: string;
    /**流水號 */
    SeqNo: number | 0;
    /** 描述  */
    Description: string;
    /**開始時間 (ex: 4.53) */
    BegTime: number | 0;
    /**結束時間 (ex: 22.369) */
    EndTime: number | 0;
}
/**(影)點選顯示頁: 點選左側檔案列表,右側分頁'關鍵影格'(只有影) */
interface VideoKeyFrameModel {
    /** 檔案編號 */
    fsFILE_NO: string;
    /** 標題 */
    Title: string;
    /** 描述 */
    Description: string;
    /**是否為檔案的代表圖 */
    IsHeadFrame: boolean;
    /** 檔案路徑 */
    FilePath: string;
    /** 時間  */
    Time: string;
    /**截圖URL */
    ImageUrl: string;
    /** 檔案資訊(EX: 關鍵影格編號: 20190311_0000012) */
    FileInfo: string;
}
/**專輯資訊 */
interface AudioInfoModel {
    /** 專輯名稱*/
    Album: string;
    /** 專輯標題*/
    AlbumTitle: string;
    /** 專輯演出者*/
    AlbumArtists: string;
    /** 歌曲演出者*/
    AlbumPerformers: string;
    /** 歌曲作曲者*/
    AlbumComposers: string;
    /** 專輯年份*/
    AlbumYear: number | 0;
    /** 著作權*/
    AlbumCopyright: string;
    /** 內容類型*/
    AlbumGenres: string;
    /** 備註*/
    AlbumComment: string;
}
/**EXIF資訊 */
interface ExifInfo {
    /**圖片寬 */
    PhotoWidth: number | 0;
    /**圖片高 */
    PhotoHeight: number | 0;
    /**XDPI */
    PhotoXdpi: number | 0;
    /**YDPI */
    PhotoYdpi: number | 0;
    /**相機廠牌 */
    CameraMake: string;
    /** 相機型號 */
    CameraModel: string;
    /**焦距 */
    FocalLength: string;
    /** 曝光時間 */
    ExposureTime: string;
    /**光圈 */
    Aperture: string;
    /**ISO */
    ISO: number | 0;
}
/**文件資訊 */
interface ExtendIfno {
    /** 文件內容 */
    Content: string;
    /**文件建立日期 */
    FileCreatedDate: string | null;
    /**文件修改日期 */
    DateTime: string | null;
}

/**查詢檔案TSM狀態結果 */
export interface FileStatusResult {
    /**檔案編號 */
    FILE_NO: string;
    /**檔案狀態 */
    FILE_STATUS: TSMFileStatus;
}
