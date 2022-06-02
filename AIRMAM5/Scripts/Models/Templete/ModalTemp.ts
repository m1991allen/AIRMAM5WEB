import { IsNULLorEmpty } from "../Function/Check";


/**
 * 確認燈箱
 * @param modalId id
 * @param title 標題
 * @param content 內容
 */
export const ConfirmModal=(modalId:string,title:string, content:string):string=>{
    modalId=modalId.replace("#","");
    return `<div class="ui modal" id="${modalId}">
            <div class="header">
                ${title}
            </div>
            <div class="content">
                ${content}
            </div>
            <div class="actions">
                <button type="button" class="ui ok yellow button">確定</button>
                <button type="button" class="ui cancel button">取消</button>
            </div>
           </div>`;
};

/**
 * Html燈箱
 * @param modalId id 
 * @param title 標題
 * @param content 內容
 * @param actions 按鈕動作
 * @param classList 多個class
 */
export const HtmlModal=(modalId:string,title:string, content:string, actions:string, classList?:string[])=>{
    modalId=modalId.replace("#","");
    const className=IsNULLorEmpty(classList)?"":classList.join(" ");
    return `<div class="ui modal ${className}" id="${modalId}" name="${modalId}">
            <i class="close icon"></i>
            <div class="header">
            ${title}
            </div>
            <div class="scrolling content">
            ${content}
            </div>
            <div class="actions">
            ${actions}
            </div>
         </div>`;
};
/**全螢幕燈箱 */
export const HtmlFullScreenModal=(modalId:string,title:string,content:string,actions:string,name?:string,classList?:string[]):string=>{
    modalId=modalId.replace("#","");
    const modalname=IsNULLorEmpty(name)?modalId:name;
    const className=IsNULLorEmpty(classList)?"":classList.join(" ");
    return `<div class="ui modal ${className}" id="${modalId}" name="${modalname}">
            <div class="header">
            ${title}
            </div>
            <div class="scrolling content">
            ${content}
            </div>
            <div class="actions">
            ${actions}
            </div>
            </div>`;
};

/**
 * 自定義燈箱
 * @param modalId Id 
 * @param title 標題
 * @param content 內容
 * @param actions 按鈕動作
 * @param name name
 * @param classList 多個class 
 */
export const CustomModal=(modalId:string,title:string,content:string,actions:string,name?:string,classList?:string[]):string=>{
    modalId=modalId.replace("#","");
    const modalname=IsNULLorEmpty(name)?modalId:name;
    const className=IsNULLorEmpty(classList)?"":classList.join(" ");
    const header=IsNULLorEmpty(title)?"":`<div class="header">${title}</div>`;
    const innercontent=IsNULLorEmpty(content)?"":`<div class="content">${content}</div>`;
    const footer=IsNULLorEmpty(actions)?"":`<div class="actions">${actions}</div>`;
    return `<div class="${className}" id="${modalId}" name="${modalname}">${header}${innercontent}${footer} </div>`
};