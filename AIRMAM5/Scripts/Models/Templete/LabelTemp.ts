import { Color } from '../Enum/ColorEnum';
import { IsNULLorEmpty } from '../Function/Check';
/**樣版:標準標籤 */
export const Label = (text: string, color: Color | '' | string, className?: Array<string>, title?: string) => {
    const classAttr = className == undefined || className.length == 0 ? '' : className.join(' ');
    const titleAttr = IsNULLorEmpty(title) ? '' : `title="${title}"`;
    return `<span class="ui ${color} ${classAttr} label" ${titleAttr}>${text}</span>`;
};
