import { IsNULLorEmpty } from '../Function/Check';

/**樣版:檢視按鈕 */
export const DetailButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="detail" data-inverted="" data-tooltip="查看${TipName}" data-position="bottom center" class="ui blue basic circular icon button" data-Id="${id}"><i class="list icon"></i></button>`;
};

/**樣版:設定按鈕 */
export const CogButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="cog" data-inverted="" data-tooltip="設定${TipName}" data-position="bottom center" class="ui orange basic circular icon button" data-Id="${id}"><i class="cog icon"></i></button>`;
};

/**樣版:編輯按鈕 */
export const EditButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="edit" data-inverted="" data-tooltip="編輯${TipName}" data-position="bottom center" class="ui green basic circular icon button" data-Id="${id}"><i class="edit icon"></i></button>`;
};

/**樣版:刪除按鈕 */
export const DeleteButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="delete" data-inverted="" data-tooltip="刪除${TipName}" data-position="bottom center" class="ui red basic  circular icon button" data-Id="${id}"><i class="delete icon"></i></button>`;
};

/**樣版:還原按鈕 */
export const RecycleButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="recycle" data-inverted="" data-tooltip="還原${TipName}" data-position="bottom center" class="ui olive basic circular icon button" data-Id="${id}"><i class="recycle icon"></i></button>`;
};
/**樣板:剪裁按鈕 */
export const FilmButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="edit" data-inverted="" data-tooltip="段落剪輯${TipName}" data-position="bottom center" class="ui green basic circular icon button" data-Id="${id}"><i class="film icon"></i></button>`;
};
/**樣板:上傳按鈕 */
export const UploadButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="upload" data-inverted="" data-tooltip="上傳檔案" data-position="bottom center" class="ui pink basic circular icon button" data-Id="${id}"><i class="upload icon"></i></button>`;
};
/**樣板:功能設定按鈕 */
export const FunctionButton = (id: string | number, TipName: string = ''): string => {
    return `<button type="button" name="function" data-inverted="" data-tooltip="設定功能項目" data-position="bottom center" class="ui orange basic circular icon button" data-Id="${id}"><i class="bookmark icon"></i></button>`;
};

/**AIRMAM5自定義主色按鈕(細框白,滑過細框黃) */
export const PrimaryButton = (
    id: string,
    icon?: string,
    text?: string,
    name?: string,
    type?: 'button' | 'submit' | 'reset',
    className?: Array<string>
) => {
    type = IsNULLorEmpty(type) ? 'button' : type;
    id = IsNULLorEmpty(id) ? '' : id;
    name = IsNULLorEmpty(name) ? id : name;
    const ID = IsNULLorEmpty(id) ? '' : `id=${id}`;
    const IconClass = !IsNULLorEmpty(icon) && IsNULLorEmpty(text) ? 'icon' : '';
    className = IsNULLorEmpty(className) ? [] : className;
    className.push(IconClass);
    const ClassName = className.join(' ');
    return `<button type="${type}" class="ui _darkGrey ${ClassName} button" ${ID} name="${name}"><i class="${icon} icon"></i>${text}</button>`;
};

/**取消視窗按鈕 */
export const CancelButton = (): string => {
    return `<button class="ui cancel button" type="button">取消</button>`;
};
/**關閉視窗按鈕 */
export const CloseButton = (): string => {
    return `<button class="ui cancel button" type="button">關閉</button>`;
};
/**確認視窗 */
export const OKButton = (): string => {
    return `<button class="ui yellow ok button" type="submit">確定</button>`;
};
