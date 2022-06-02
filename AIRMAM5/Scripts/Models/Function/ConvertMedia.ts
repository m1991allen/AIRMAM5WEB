import { MediaType, MediaUploadType, SearchTypeEnum } from '../Enum/MediaType';
import { getEnumKeyByEnumValue } from './KeyValuePair';

/**
 * 傳入的媒體類型轉換為MediaType類型
 * @param media 'V' | 'A' | 'D' | 'P' | 'S'
 */
export function GetMediaType(media: 'V' | 'A' | 'D' | 'P' | 'S'): MediaType {
    const key = getEnumKeyByEnumValue(MediaType, media);
    console.log('key=' + key + ',mediatype=' + Object(MediaType)[key]);
    return Object(MediaType)[key];
}
/**
 * 傳入的媒體類型轉換為MediaUploadType類型
 * @param media 'V' | 'A' | 'D' | 'P' | 'S'
 */
export function GetMediaUploadType(media: 'V' | 'A' | 'D' | 'P' | 'S'): MediaUploadType {
    const key = getEnumKeyByEnumValue(MediaUploadType, media);
    return Object(MediaUploadType)[key];
}
/**
 * 傳入的媒體類型轉換為SearchTypeEnum類型
 * @param media 'V' | 'A' | 'D' | 'P' | 'S'
 */
export function GetMediaSearchType(media: 'V' | 'A' | 'D' | 'P' | 'S'): SearchTypeEnum {
    const value =
        media == 'V'
            ? SearchTypeEnum.VIDEO
            : media == 'A'
            ? SearchTypeEnum.AUDIO
            : media == 'D'
            ? SearchTypeEnum.DOC
            : media == 'P'
            ? SearchTypeEnum.PHOTO
            : SearchTypeEnum.Subject;
    const key = getEnumKeyByEnumValue(SearchTypeEnum, value);
    return Object(SearchTypeEnum)[key];
}

/**
 *  傳入的MediaUploadType類型轉換為MediaType類型
 * @param media MediaType
 */
export function GetMediaTypeByUploadType(media: MediaUploadType): MediaType {
    switch (media) {
        case MediaUploadType.VIDEO:
            return MediaType.VIDEO;
        case MediaUploadType.AUDIO:
            return MediaType.AUDIO;
        case MediaUploadType.DOC:
            return MediaType.Doc;
        case MediaUploadType.PHOTO:
            return MediaType.PHOTO;
        default:
            return MediaType.SUBJECT;
    }
}
