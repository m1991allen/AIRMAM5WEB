import { initSetting } from '../initSetting';
import { MediaType } from '../Enum/MediaType';
import { GetImageUrl } from '../Function/Url';
import { IsNULLorEmpty } from '../Function/Check';
import { AirmamImage } from '../Const/Image';

const baseUrl: string = initSetting.APIUrl;
/**媒體類型底圖 */
export const PreviewImage = (type: MediaType | 'noImage', alt?: string): string => {
    switch (type) {
        case MediaType.VIDEO:
            const videoAlt: string = IsNULLorEmpty(alt) ? '影片預覽圖' : alt;
            return `<img src="${
                GetImageUrl(AirmamImage.VideoPreview).href
            }" class="ui image fluid" alt="${videoAlt}" /></div>`;
        case MediaType.AUDIO:
            const audioAlt: string = IsNULLorEmpty(alt) ? '聲音預覽圖' : alt;
            return `<img src="${
                GetImageUrl(AirmamImage.AudioPreview).href
            }" class="ui image fluid" alt="${audioAlt}" /></div>`;
        case MediaType.PHOTO:
            const photoAlt: string = IsNULLorEmpty(alt) ? '圖片預覽圖' : alt;
            return `<img src="${
                GetImageUrl(AirmamImage.DocPreview).href
            }" class="ui image fluid" alt="${photoAlt}}" /></div>`;
        case MediaType.Doc:
            const docAlt: string = IsNULLorEmpty(alt) ? '文件預覽圖' : alt;
            return `<img src="${
                GetImageUrl(AirmamImage.DocPreview).href
            }" class="ui image fluid" alt="${docAlt}" /></div>`;
        case 'noImage':
            return `<img src="${
                GetImageUrl(AirmamImage.NoImage).href
            }" class="ui image fluid" alt="圖片預覽底圖" /></div>`;
    }
};
/**取得圖片 */
export const GetImage = (url: string, alt?: string, classList?: string[],name?:string): string => {
    alt = IsNULLorEmpty(alt) ? '預覽底圖' : alt;
    name =IsNULLorEmpty(name)?'':name;
    const className = IsNULLorEmpty(classList) ? '' : classList.join(' ');
    const noImageURL = GetImageUrl(AirmamImage.NoImage).href;
    return `<img src="${url}" class="${className}" alt="${alt}" onerror="this.src='${noImageURL}'" name="${name}" />`;
};
