/*
   PLUGIN:Selectable.js
   https://github.com/Mobius1/Selectable
   Inspired by the jQuery UI Selectable plugin. Functionality and options are identical to the jQuery UI version with some additions and performance enhancements.
*/

// export as namespace MobiusSelectable;

 /**Selectable JS 初始化設定參數 */
 interface SelectanConfig {
    /**定義要將套索附加到的容器,默認:document.body
     * 您可以傳遞CSS3選擇器字符串或節點引用。
     * 最好將容器定義為最接近項目的祖先，並且其中不包含任何其他交互元素（按鈕，輸入等），因為與它們進行交互將取消選擇可選項目。
     */
    appendTo?: string | HTMLElement;
    /**
     * 自動重新計算並緩存項目的尺寸,默認:true
     * 設置為時true，此選項告訴實例重新計算並在mousedown/ 上緩存所有可選項目的尺寸touchstart。
       如果您現在不更改可選項目的尺寸，即文檔中的寬度，高度和位置保持不變，則可以將此選項設置為false。
     */
    autoRefresh?: boolean;
    /**
     * 啟用頁面自動滾動或父滾動,有助於防止在嘗試選擇靠近容器邊界的項目時觸發自動滾動。
     * threshold-開始自動滾動之前，從容器邊緣開始的像素數。默認為0。當鼠標/指針在容器外超出定義的量時，負值將觸發自動滾動-例如，-50當鼠標/指針在容器上方/下方50像素時，值會導致自動滾動開始。
     * increment-滾動容器的像素數量。默認為20。
     */
    autoScroll?: boolean | { threshold: number; increment: number };
    /**樣式類別 */
    classes?: {
        /**lasso -套索使用的類名 */
        lasso?: string;
        /** selected -用於所選項目的類名 */
        selected?: string;
        /**container- appendTo選項定義的容器 */
        container?: string;
        /**selecting -高亮顯示項目以供選擇時使用的類名稱 */
        selecting?: string;
        /**selectable -用於將項目標記為可選的類名稱 */
        selectable?: string;
        /**unselecting -突出顯示要取消選擇的項目時使用的類名稱 */
        unselecting?: string;
    };
    /*
     *定義可選的元素節點,默認:".ui-selectable"
     * 默認情況下，實例將使用.ui-selectable該類搜索元素，但是您可以通過設置filter選項告訴它尋找其他元素。
     */
    filter?: string | Array<HTMLElement> | NodeList | HTMLCollection;
    /**
     * 定義可以單擊的可選元素的後代,默認:false
     * 可以是CSS3選擇器字符串或CSS3選擇器字符串數組。單擊時，定義的任何後代都不會觸發實例開始選擇/切換。
     * 如果可選元素中包含交互元素（例如按鈕，複選框或下拉菜單），這將很方便。
     */
    ignore?: string | Array<string> | boolean;
    /**套索的CSS應用樣式 */
    lasso?: { [key: string]: any }; //???????????????
    /**
     * 設置套索的選擇順序,默認為"normal"
     * 設置為"sequential"允許套索依次選擇項目，而不是僅選擇套索中的項目
     * */
    lassoSelect?: string | 'sequential' | 'normal';
    /**可用於限制可以選擇的項目數。設置false為禁用,默認:false */
    maxSelectable?: boolean | number;
    /**
     * 啟用自動在mouseup/ 上保存當前選擇的選項touchend,默認:false
     * 通過將選項設置為，可以告訴實例存儲最多數量的狀態Number。設置為0或false將禁用自動保存。
     * 可以使用該state()方法手動保存/加載。
     */
    saveState?: boolean | number;
    /**設置在調用另一個resize或scroll回調之前等待的最小間隔（以毫秒為單位）,默認:50
     * 為了使用套索準確地選擇項目，必須基於容器滾動位置和大小來計算每個項目的坐標。每次調整容器大小或滾動時，都會更新和緩存這些坐標。
     * 由於調整大小和滾動回調每隔幾毫秒就會觸發一次，因此在某些情況下會降低性能。Selectable可以限制這些回調，因此它們僅在設置定義的設置時間間隔後觸發throttle。
     * 較高的值可以提高低端設備的性能，但可能會導致自動滾動過程中延遲選擇。
     */
    throttle?: number;
    /**允許切換項目狀態,默認:false
     * 設置為true時，單擊或點擊一個項目將切換其狀態，而不是將所有當前選中的項目標記為取消選擇。
     */
    toggle?: boolean | string;
    /**
     * 定義套索在突出顯示以供選擇之前與可選元素重疊的程度。
     * "touch" -套索只需觸摸項目以突出顯示即可選擇
     * "fit" -該項目必須完全位於套索中以突出顯示以供選擇
     */
    tolerance?: 'touch' | 'fit';
 }


 type SelectEvent =
 | 'init'
 | 'start'
 | 'drag'
 | 'end'
 | 'selecteditem'
 | 'deselecteditem'
 | 'addeditem'
 | 'removeditem'
 | 'update'
 | 'refresh'
 | 'state'
 | 'enabled'
 | 'disabled';
interface _SelectEventBehavior {}

/**Selectable JS 服務宣告 */
declare class Selectable {
    constructor(options: SelectanConfig);
    /*-------屬性-------------------*/
    /**返回boolean表示當前容器是否為的document.body */
    bodyContainer: boolean;
    /**返回boolean表示當前是否按下ctrlKey/ 的指示metaKey。 */
    cmdDown: boolean;
    /**返回當前實例的配置選項。 */
    config: SelectanConfig;
    /**返回當前容器元素 */
    container: HTMLElement;
    /**返回boolean表示當前實例是否啟用的指示 */
    enabled: boolean;
    /**返回實例附加到DOM的當前事件偵聽器的列表 */
    events: object;
    /**返回Array的Objects所有當前可選項目 */
    items: Array<{
        /**元素節點 */
        node: HTMLElement;
        /**元素節點的邊界矩形 */

        rect: DOMRect;
        /**該元素是否已在mousedown / touchstart上選擇 */
        startselected: boolean;
        /**該元素目前是否已選擇 */

        selected: boolean;
        /**該元素目前是否選擇中 */

        selecting: boolean;
        /**該元素目前是否取消選擇 */

        unselecting: boolean;
    }>;
    /**返回當前套索元素，如果已禁用，則返回false */
    lasso: HTMLElement | boolean;
    /**返回當前附加的自定義事件偵聽器 */
    listeners: object;
    /**返回當前可選節點的列表 */
    nodes: Array<any>;
    /**返回當前容器元素的DOMRect */
    rect: object;
    /**返回當前容器的滾動尺寸*/
    scroll: {
        /**當前水平滾動位置 */
        x: number;
        /**當前垂直滾動位置 */
        y: number;
        max: {
            /**最大水平滾動（以像素為單位） */
            x: number;
            /**the maximum vertical scroll in px */
            y: number;
        };
    };
    /**返回Boolean表示shiftKey當前是否按下的指示 */
    shiftDown: boolean;
    /**返回當前實例的版本號 */
    version: string;
    /**返回一個Boolean表示當前實例是否具有自動滾動功能的指示 */
    autoscroll: boolean;
    /*-------方法-------------------*/
    /**銷毀後初始化實例。 */
    init(): void;
    /**反轉當前選擇 */
    invert(): void;
    /**設置選擇容器。無法在此容器外部進行選擇 */
    setContainer(container: string | HTMLElement): void;
    /**
     * 保存，加載或清除保存的狀態
     * "save" -保存當前選擇。
     * "undo" -返回上一個選擇
     * "redo" -恢復上一次撤消的選擇
     * "clear" -刪除所有保存的選擇。
     */
    state(action: 'save' | 'undo' | 'redo' | 'clear'): void;
    /**更新當前實例。這將重新計算並緩存所有尺寸。在調整大小和滾動事件期間，將自動調用此方法，但如果需要，可以手動調用。 */
    update(): void;
    /**將節點添加到實例中使其成為可選節點*/
    add(items: HTMLElement | Array<HTMLElement> | NodeList | HTMLCollection): void;
    /**取消選擇所有項目 */
    clear(): void;
    select(items: HTMLElement | Object | number | Array<HTMLElement> | NodeList | HTMLCollection): void;
    /**
     * 取消選擇一個項目或項目集合
     * HTMLElement -您要選擇的可選元素。
     * Number-可選元素的索引（不是的索引parentNode）。
     * Object-對item存儲在items數組中的引用。
     * Array-節點，索引或對象的數組。您也可以傳遞HTMLCollection或的實例NodeList。
     */
    deselect(items: HTMLElement | Object | number | Array<HTMLElement> | NodeList | HTMLCollection): void;
    /**選擇所有有效項 */
    selectAll(): void;
    /**銷毀實例。這將在初始化之前將DOM返回到其初始狀態。 */
    destroy(): void;
    /**啟用實例 */
    enable(): void;
    /**這將刪除所有事件偵聽器，以防止進一步的交互。您可以使用enable()方法重新啟用實例。 */
    disable(): void;
    /**重新計算所有可選項目的坐標
     * 為了消除在mousemove/ touchmove（妨礙性能的任務）期間套索何時與物料接觸的需要，需要緩存物料的尺寸。
     * 如果任何一個或多個項目的尺寸發生變化，則需要重新裝填。只需隨時調用此方法即可重新緩存維度。
     * 請注意，如果您將autoRefresh選項設置為true，則在每個mousedown/ touchstart事件上都會調用此方法。
     */
    refresh();
    /**刪除使它們不可選擇的項目 */
    remove(items: HTMLElement | number | Array<HTMLElement> | Array<number> | HTMLCollection | NodeList): void;
    /**返回項目參考。 */
    get(
        items: HTMLElement | number | Array<HTMLElement> | Array<number> | HTMLCollection | NodeList
    ): Array<{
        node: HTMLElement; // the element node
        rect: DOMRect; // the element node's bounding rects
        startselected: boolean; // item was already selected on mousedown / touchstart
        selected: boolean; // item is currently selected
        selecting: boolean; // item is currently being selected
        unselecting: boolean; // item is currently being deselected
    }>;
    /**返回Array所有項目的 */
    getItems(): Array<{
        node: string;
        rect: { x1: number; x2: number; y1: number; y2: number; height: number; width: number };
        startselected: boolean;
        selected: boolean;
        selecting: boolean;
        deselecting: false;
    }>;
    /**返回Array所有可選元素節點的 */
    getNodes(): Array<string>;
    /**返回Array所有選定項目的 */
    getSelectedItems(): Array<{
        node: string;
        rect: { x1: number; x2: number; y1: number; y2: number; height: number; width: number };
        startselected: boolean;
        selected: boolean;
        selecting: boolean;
        deselecting: false;
    }>;
    /**返回Array所有選定元素節點的。 */
    getSelectedNodes(): Array<string>;

    /**刪除實例的自定義事件偵聽器 */
    off(event: SelectEvent, callback: (...args) => void): void;
    on(event: SelectEvent, callback: (...args) => void): void;
}
declare  module 'mobiusSelectable'{
     export default Selectable;
}
