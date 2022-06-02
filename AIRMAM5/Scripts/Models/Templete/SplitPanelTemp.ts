import { IsNULLorEmpty } from "../Function/Check";

/*------------------------------------------
 https://github.com/shagstrom/split-pane
 使用前要引用(1)split-pane.css(2)split-pane.js
--------------------------------------------*/
/**分割為兩拖拉區塊 */
export const SplitTwolPanel=(leftComponent:{id:string,html:string,className?:string},rightComponent:{id:string,html:string,className?:string},divider:{id:string,className?:string}):string=>{
return `${SplitPanelComponent(leftComponent.id,leftComponent.html,leftComponent.className)}
        ${SplitDivider(divider.id,divider.className)}
        ${SplitPanelComponent(rightComponent.id,rightComponent.html,rightComponent.className)}`;
};


/** 拖拉主容器*/
export const SplitMainPanel=(id:string,html:string,fixed?:"left"|"right"):string=>{
    const fixedDirection:"fixed-left"|"fixed-right"=(IsNULLorEmpty(fixed)||fixed=="left" )?"fixed-left":"fixed-right";
    return `<div id="${id}" class="split-pane ${fixedDirection}">${html}</div>`;
}

/**拖拉區塊 */
export const SplitPanelComponent=(id:string,html:string,className?:string):string=>{
    className=IsNULLorEmpty(className)?"":className;
    return `<div class="split-pane-component ${className}" id="${id}">${html}</div>`;
}
/**拖拉分割線 */
export const SplitDivider=(id:string,className?:string):string=>{
    className=IsNULLorEmpty(className)?"":className;
    return `<div class="split-pane-divider ${className}" id="${id}"></div>`;
}