import { SelectListItem } from '../Interface/Shared/ISelectListItem';
import { StringEnum } from '../Enum/StringEnum';
import { CheckInput, RadioInput } from './InputTemp';
/**依傳入內容創建select下拉選單 */
export const CreateSelect = (data: Array<SelectListItem>, isMuptile: boolean, name: string): HTMLSelectElement => {
    const select = document.createElement('select');
    select.className = 'ui fluid search selection  dropdown';
    select.setAttribute('name', name);
    if (isMuptile) {
        select.classList.add('mutiple');
        select.setAttribute('multiple', '');
    }
    for (let item of data) {
        const option: HTMLOptionElement = document.createElement('option');
        option.value = item.Value;
        option.text = item.Text;
        option.selected = item.Selected;
        select.appendChild(option);
    }
    return select;
};
/**依傳入內容創建option元素下拉選單 */
export const CreateOption = (
    data: SelectListItem | 'none' | 'all',
    ...args: Array<{ attrName: string; attrValue: any }>
): HTMLOptionElement => {
    const option = document.createElement('option');
    switch (data) {
        case 'none':
            option.value = StringEnum.Empty; /**Notice:如果非空值,則可以選 */
            option.text = '無資料';
            option.selected = false;
            // option.disabled = true;
            return option;
        case 'all':
            option.value = StringEnum.All;
            option.text = '--全部--';
            option.selected = true;
            return option;
        default:
            option.value = data.Value;
            option.text = data.Text;
            option.selected = data.Selected;
            for (let attr of args) {
                option.setAttribute(attr.attrName, attr.attrValue);
            }
            return option;
    }
};
/**創建semantic ui checkbox */
export const CreateCheckbox = (input: { label?: string; value?: any; selected?: boolean }): HTMLDivElement => {
    const checkdiv = document.createElement('div');
    checkdiv.className = 'ui checkbox';
    const checkinput = CheckInput({
        label: input.label,
        value: input.value,
        selected: input.selected,
    });
    checkdiv.innerHTML += checkinput;
    return checkdiv;
};
/**創建semantic ui Radiobox */
export const CreateRadiobox = (input: { label?: string; value?: any; selected?: boolean }): HTMLDivElement => {
    const radioCheckdiv = document.createElement('div');
    radioCheckdiv.className = 'ui radio checkbox';
    const radioinput = RadioInput({
        label: input.label,
        value: input.value,
        selected: input.selected,
    });
    radioCheckdiv.append(radioinput);
    return radioCheckdiv;
};
