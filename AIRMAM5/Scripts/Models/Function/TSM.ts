import { TSMFileStatus, ChineseTSMFileStatus } from '../Enum/TSMFileStatus';
import { Color } from '../Enum/ColorEnum';
import { AirmamImage } from '../Const/Image';
import { GetImageUrl } from './Url';
import { getEnumKeyByEnumValue } from './KeyValuePair';

/**
 * 依照tsm狀態取得tsm 顏色
 * @param status
 */
export const GetTsmColor = (status: TSMFileStatus): Color => {
    switch (status) {
        case TSMFileStatus.FileOnDisk:
            return Color.橙;
        case TSMFileStatus.ArchiveOnTape:
            return Color.藍;
        case TSMFileStatus.FileError:
        // return Color.紅;
        case TSMFileStatus.Nodata:
        // return Color.灰;
        case TSMFileStatus.Processing:
        // return Color.灰;
        case TSMFileStatus.FileNotExist:
            // return Color.黃;
            return Color.綠;
        case TSMFileStatus.Online:
            return Color.綠;
        case TSMFileStatus.Offline:
        case TSMFileStatus.OfflineDeep:
            return Color.灰;
    }
};
/**
 * 依照tsm狀態取得tsm icon圖片路徑
 * @param status
 */
export const GetTsmImgUrlByStatus = (status: TSMFileStatus): string => {
    switch (status) {
        case TSMFileStatus.FileOnDisk:
            return GetImageUrl(AirmamImage.TSM_onDisk).href;
        case TSMFileStatus.ArchiveOnTape:
            return GetImageUrl(AirmamImage.TSM_OnTape).href;
        case TSMFileStatus.Online:
            return GetImageUrl(AirmamImage.TSM_Online).href;
        case TSMFileStatus.Offline:
            return GetImageUrl(AirmamImage.TSM_Offline).href;
        case TSMFileStatus.OfflineDeep:
            return GetImageUrl(AirmamImage.TSM_OfflineDeep).href;
        case TSMFileStatus.FileError:
        //    return GetImageUrl(AirmamImage.TSM_OnFileError).href;
        case TSMFileStatus.Nodata:
        //   return GetImageUrl(AirmamImage.TSM_OnNodata).href;
        case TSMFileStatus.Processing:
        //  return GetImageUrl(AirmamImage.TSM_OnProgress).href;
        case TSMFileStatus.FileNotExist:
        //   return GetImageUrl(AirmamImage.TSM_OnNotfoundError).href;

        default:
            return GetImageUrl(AirmamImage.TSM_OnNotfoundError).href;
        // return GetImageUrl(AirmamImage.TSM_OnNodata).href;
    }
};

/**
 * 依照tsm狀態取得tsm 文字
 * @param status
 */
export const GetTsmTextByStatus = (status: TSMFileStatus): string => {
    switch (status) {
        case TSMFileStatus.FileOnDisk:
        case TSMFileStatus.ArchiveOnTape:
        case TSMFileStatus.Online:
        case TSMFileStatus.FileError:
        case TSMFileStatus.Nodata:
        case TSMFileStatus.FileNotExist:
        case TSMFileStatus.Processing:
            return getEnumKeyByEnumValue(ChineseTSMFileStatus, status);
        case TSMFileStatus.Offline:
        case TSMFileStatus.OfflineDeep:
            return getEnumKeyByEnumValue(ChineseTSMFileStatus, TSMFileStatus.Offline); /*目前兩種顯示文字一樣*/
        default:
            return '其他';
    }
};
