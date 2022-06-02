import { IsNULLorEmpty, IsNULL } from '../Function/Check';

/**input[type='text']樣板 */
export const TextInput = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    placeholder?: string;
    value?: any;
    readonly?: boolean;
}): string => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Placeholder = IsNULLorEmpty(input.placeholder) ? '' : `placeholder="${input.placeholder}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    return `${Label}<input type="text" ${Id} ${Name} ${ClassName} ${Placeholder} ${Value} ${Readonly}>`;
};

/**input[type='number']樣板 */
export const NumberInput = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    placeholder?: string;
    value?: any;
    readonly?: boolean;
}): string => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Placeholder = IsNULLorEmpty(input.placeholder) ? '' : `placeholder="${input.placeholder}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    return `${Label}<input type="number" min="0" step="1" ${Id} ${Name} ${ClassName} ${Placeholder}${Value}${Readonly}>`;
};
/**input[type='date']樣板 */
export const DateInput = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    placeholder?: string;
    value?: any;
    readonly?: boolean;
}): string => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Placeholder = IsNULLorEmpty(input.placeholder) ? '' : `placeholder="${input.placeholder}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    return `${Label}<input type="date" ${Id} ${Name} ${ClassName} ${Placeholder}${Value} ${Readonly}>`;
};

/**input[type='datetime-local']樣板 */
export const DateTimeInput = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    placeholder?: string;
    value?: any;
    readonly?: boolean;
}): string => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Placeholder = IsNULLorEmpty(input.placeholder) ? '' : `placeholder="${input.placeholder}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    return `${Label}<input type="datetime-local" ${Id} ${Name} ${ClassName} ${Placeholder}${Value}${Readonly}>`;
};

/**TextArea樣板 */
export const TextArea = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    placeholder?: string;
    row?: number;
    value?: any;
    readonly?: boolean;
}): string => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Placeholder = IsNULLorEmpty(input.placeholder) ? '' : `placeholder="${input.placeholder}"`;
    const Row = IsNULLorEmpty(input.row) ? '' : `row="${input.row}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    return `${Label}<textarea ${Id} ${Name} ${ClassName} ${Placeholder} ${Row} ${Value} ${Readonly}></textarea>`;
};
/**input[type='checkbox'] */
export const CheckInput = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    value?: any;
    readonly?: boolean;
    selected?: boolean;
}) => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    const Selected = IsNULLorEmpty(input.selected) ? '' : input.selected ? 'selected' : '';
    return `${Label}<input type="checkbox" ${Id} ${Name} ${ClassName} ${Value} ${Readonly} ${Selected}>`;
};

/**input[type='radiobox'] */
export const RadioInput = (input?: {
    label?: string;
    id?: string;
    name?: string;
    classList?: string[];
    value?: any;
    readonly?: boolean;
    selected?: boolean;
}) => {
    const Label = IsNULLorEmpty(input.label) ? '' : `<label for="${input.id}">${input.label}</label>`;
    const Id = IsNULLorEmpty(input.id) ? '' : `id="${input.id}"`;
    const Name = IsNULLorEmpty(input.name) ? '' : `name="${input.name}"`;
    const ClassName = IsNULLorEmpty(input.classList) ? '' : `class="${input.classList.join(' ')}"`;
    const Value = IsNULL(input.value) ? '' : `value="${input.value}"`;
    const Readonly = IsNULLorEmpty(input.readonly) ? '' : 'readonly';
    const Selected = IsNULLorEmpty(input.selected) ? '' : input.selected ? 'selected' : '';
    return `${Label}<input type="radio" ${Id} ${Name} ${ClassName} ${Value} ${Readonly} ${Selected}>`;
};
