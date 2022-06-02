import { ErrorMessage} from './Message';
import { IsNULL, IsNullorUndefined } from './Check';;
import { Logger } from '../Class/LoggerService';
/*===============================================================*/
/**
 *取得表單的所有值
 * @export
 * @param {string} formId 表單Id
 * @returns {object} 回傳json object
 */
export function FromSerialize(formId: string): object {
    return (function() {
        let output: object = [];
        const IsNumber: Function = (value: string): boolean => {
            return /^\d+$/.test(value);
        };
        try {
            const array: Array<string> = JSON.parse(JSON.stringify($('#EditForm').serializeArray()));
            let orginKeys: Array<string> = [];
            array.forEach(x => {
                const item: { name: string; value: string } = JSON.parse(JSON.stringify(x));
                orginKeys.push(item.name);
            });
            let repeatKeys: Array<string> = orginKeys.filter(x => x.length > 1); //取得重複的鍵值
            array.forEach(x => {
                const item: { name: string; value: string } = JSON.parse(JSON.stringify(x));
                if (repeatKeys.indexOf(item.name) > -1) {
                    //有重複的鍵值
                    if (item.name in output) {
                        output[item.name] = output[item.name].concat(',', item.value);
                    } else {
                        output[item.name] = item.value;
                    }
                } else {
                    //沒有重複的鍵值
                    output[item.name] = IsNumber(item.value) ? Number(item.value) : item.value;
                }
            });
            return output;
        } catch (ex) {
            Logger.error('表單序列化發生錯誤:' + ex);
        }
    })();
}

/**
 * 表單驗證是否通過
 * @param formSelector 表單選擇器,例如:"#SearchForm"或".form"
 * @param checkObject 驗證欄位物件
 */
export function CheckForm(formSelector: string, checkObject: any): boolean {
    const form = $(formSelector);
    form.removeClass('error');
    form.form({
        inline: true,
        on: 'blur',
        fields: checkObject,
    }).form('validate form');
    if (form.form('is valid')) {
        form.removeClass('error');
        return true;
    } else {
        form.addClass('error');
        ErrorMessage('表單驗證錯誤,請修正輸入條件');
        return false;
    }
}
/**
 *
 * @param formSelector 表單選擇器
 * @param checkObject 原有的檢查條件
 * @param skipObject 自定動態檢查條件要忽略的元素名
 */
export function AddDynamicNullable(formSelector: string, checkObject: any, skipObject?: string[]): object {
    const $Form: HTMLElement = document.getElementById(formSelector.replace('#', ''));
    const $Inputs: NodeListOf<HTMLInputElement> = $Form.querySelectorAll("input[data-nullable='False']");
    const $Selects: NodeListOf<HTMLSelectElement> = $Form.querySelectorAll("select[data-nullable='False']");
    const $TextAreas: NodeListOf<HTMLTextAreaElement> = $Form.querySelectorAll("textarea[data-nullable='False']");
    const $NullableDateInputss: NodeListOf<HTMLInputElement>= $Form.querySelectorAll("input[data-nullable='True'][data-fieldtype='DATETIME']");
    const ruleAssign = (element: HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement) => {
        const keyname = element.name;
        const rule = element instanceof HTMLSelectElement
                   ? {type: 'minCount[1]',prompt: '{name}至少選擇一個'}
                   : {type: 'empty',prompt: '請輸入{name}'};
        const daterule={ type: "YYYYMMDD[param]", optional: true,prompt: "日期格式必須為YYYY/MM/DD(例如:2020/01/01),且必須為有效日期"};
        const rules=[rule];
        if(!IsNullorUndefined(element.getAttribute('data-fieldtype')) && element.getAttribute('data-fieldtype')==='DATETIME'){
            rules.push(daterule);
        }
        checkObject[keyname] = {
            identifier: keyname,
            rules: rules,
        };
        for (let skipname in skipObject) {
            try {
                delete checkObject[skipname];
            } catch (error) {
                Logger.error(`刪除checkobject[${skipname}]時發生錯誤:`, error);
            }
        }
        return checkObject;
    };
    if ($Inputs.length > 0) {
        Array.from($Inputs, (input, key) => {
            if (!IsNULL(skipObject) && skipObject.length > 0) {
                skipObject.indexOf(input.name) == -1 ? (checkObject = ruleAssign(input)) : false;
            } else {
                checkObject = ruleAssign(input);
            }
        });
    }
    if ($TextAreas.length > 0) {
        Array.from($TextAreas, (textarea, key) => {
            if (!IsNULL(skipObject) && skipObject.length > 0) {
                skipObject.indexOf(textarea.name) == -1 ? (checkObject = ruleAssign(textarea)) : false;
            } else {
                checkObject = ruleAssign(textarea);
            }
        });
    }
    if ($Selects.length > 0) {
        Array.from($Selects, (select, key) => {
            if (!IsNULL(skipObject) && skipObject.length > 0) {
                skipObject.indexOf(select.name) == -1 ? (checkObject = ruleAssign(select)) : false;
            } else {
                checkObject = ruleAssign(select);
            }
        });
    }
    if($NullableDateInputss.length>0){
        Array.from($NullableDateInputss,(input,key)=>{
            checkObject[input.name] = {
                identifier: input.name,
                optional: true,
                rules:[{ type: "YYYYMMDD[param]",prompt: "日期格式必須為YYYY/MM/DD(例如:2020/01/01),且必須為有效日期"}]
            };
        });
    }
    return checkObject;
}
