import { IsNULL } from '../Function/Check';
import { Logger } from './LoggerService';
/**
 * 共用變數
 * tabsSumWidth 所有tab總長度
 **/
export var menuVisible = false;
var tabsSumWidth = 0;
/*右鍵選單顯示事件 */
export const toggleMenu = (menuId: string, command: 'show' | 'hide') => {
    menuId = menuId.replace('#', '');
    const menu = document.getElementById(menuId);
    if (!IsNULL(menu)) {
        menu.style.display = command === 'show' ? 'block' : 'none';
    }
    menuVisible = !menuVisible;
};
/*設位置 */

export const setPosition = (menuId: string, top: number, left: number) => {
    menuId = menuId.replace('#', '');
    const menu = document.getElementById(menuId);
    menu.style.left = `${left}px`;
    menu.style.top = `${top}px`;
    toggleMenu(menuId, 'show');
};
/**產生右鍵選單偵測事件 */
export function RightMenu(selectorId: string, menuId: string) {
    toggleMenu(menuId, 'hide');
    /*設變數 */
    const menu = document.getElementById(menuId);
    const tab = document.querySelector("#TitleTab a.item[data-tab='" + selectorId + "']");
    const activetab = document.querySelector('#TitleTab .item.active');
    if (window.addEventListener) {
        // IE >= 9; other browsers
        window.addEventListener(
            'contextmenu',
            function(e) {
                e.preventDefault();
                if (<HTMLElement>e.target == tab) {
                    activetab.classList.remove('active');
                    setPosition(menuId, e.pageY, e.pageX);
                    tab.classList.add('active');
                    toggleMenu(menuId, 'show');
                } else {
                    toggleMenu(menuId, 'hide');
                }
                return false;
            },
            false
        );
    } else if ((<any>window).attachEvent) {
        // IE < 9
        (<any>window).attachEvent('oncontextmenu', function(e) {
            if (<HTMLElement>e.target == tab) {
                activetab.classList.remove('active');
                setPosition(menuId, e.pageY, e.pageX);
                tab.classList.add('active');
                toggleMenu(menuId, 'show');
            } else {
                toggleMenu(menuId, 'hide');
            }
            window.event.returnValue = false;
        });
    } else {
        Logger.log('不支援addEventListener與attachEvent事件');
    }
}
