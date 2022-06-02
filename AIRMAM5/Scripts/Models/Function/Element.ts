import { IsNULL } from './Check';

/**
 * 取得 select元素
 * @param containSelector 容器的選擇器
 * @param name 元素name
 */
export function GetSelect(containSelector: string, name: string): JQuery<HTMLElement> {
    return $(containSelector).find(`select[name='${name}']`);
}
/**
 * 取得semantic ui dropdown
 * @param containSelector 容器的選擇器
 * @param name 元素name
 */
export function GetDropdown(containSelector: string, name: string): JQuery<HTMLElement> {
    return GetSelect(containSelector, name).closest('.dropdown');
}
/**使用觀察員找尋元素 */
export function waitForElement(observerContainer: HTMLElement, selector: string, workid: number): Promise<Element> {
    return new Promise(function(resolve, reject) {
        const element = <Element>observerContainer.querySelector(selector);
        if (element) {
            resolve(element);
        } else {
            const observer = new MutationObserver((mutations: MutationRecord[], observer: MutationObserver) => {
                for (let mutation of mutations) {
                    const nodes = Array.from(mutation.addedNodes).concat(
                        Array.from(mutation.removedNodes),
                        [mutation.previousSibling],
                        [mutation.nextSibling]
                    );
                    for (let node of nodes) {
                        observer.disconnect();
                        !IsNULL(node) && (<HTMLElement>node).getAttribute('data-workid') == workid.toString()
                            ? resolve(<HTMLElement>node)
                            : resolve(null);
                    }
                }
            });
            observer.observe(observerContainer, {
                childList: true,
                subtree: true,
                characterData: true,
                attributes: true,
                attributeOldValue: false,
                attributeFilter: ['class'],
            });
        }
    });
}
