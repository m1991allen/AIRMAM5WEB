import { MediaType } from '../Enum/MediaType';

/**由媒體類型取得Icon */
export const getIconByMediaType = (type: MediaType): string => {
    switch (type) {
        case MediaType.VIDEO:
            return `<i class="icon video grey"></i>`;
        case MediaType.AUDIO:
            return `<i class="icon music yellow"></i>`;
        case MediaType.PHOTO:
            return `<i class="icon file image blue"></i>`;
        case MediaType.Doc:
            return `<i class="icon file alternate green"></i>`;
        case MediaType.SUBJECT:
            return `<i class="icon bookmark purple"></i>`;
        default:
            return `<i class="icon file"></i>`;
    }
};
