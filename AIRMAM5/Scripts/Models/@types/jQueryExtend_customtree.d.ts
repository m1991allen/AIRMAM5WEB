
type  TreeNode = import('inspire-tree'). TreeNode;
type InspireTree =import('inspire-tree').InspireTree;
type NodeConfig=import('inspire-tree').NodeConfig;
type DirTreeData=import('../Interface/Dir/DirTreeData').DirTreeData;

interface CustomtreeOptions{
    selector:string;
    /** 關鍵字搜索 */
    searchword: string;
    /**是否顯示節點主題數量 */
    subjcount: boolean;
    /** 是否顯示隱藏目錄 */
    showhide: boolean;
    /**拖曳設定 */
    dragAndDrop:false| {
        enabled: boolean,
        validateOn: 'dragstart'|'dragover';
        validate:(sourceNode: TreeNode, targetNode:TreeNode)=>boolean;
     };
     /**節點資料產生方式 */
    data?:(node:TreeNode, resolve, reject)=>void;
}
interface CustomtreeConfig{
    /**設定參數 */
    config:CustomtreeOptions;
}
interface Customtree{
    /**點擊節點 */
    nodeClick:(callback?:(event:MouseEvent, node:TreeNode)=>void)=>InspireTree;
    /**節點收合 */
    nodCollapsed:(callback?:(node:TreeNode)=>void)=>InspireTree;
    /**節點右鍵 */
    nodeContextmenu:(callback?:(event:MouseEvent,enode:TreeNode)=>void)=>InspireTree;
    /**節點選擇 */
    nodeSelectd:(callback?:(node:TreeNode,isLoadEvent:boolean)=>void)=>InspireTree;
    nodeDeselected:(callback?:(node:TreeNode)=>void)=>InspireTree;
    /** */
    node:(id:string)=>TreeNode;
    /**樹狀重建 */
    rebuild:(callback?:Function)=>InspireTree;
    /**樹狀重整 */
    reload:(callback?:Function)=>InspireTree;
    /**搜尋關鍵字 */
    searchWord:(val:string)=>InspireTree;
    /**移除所有節點 */
    removeAll:()=>InspireTree;
    /**加入樹狀節點 */
    addNodes:(nodes:NodeConfig)=>InspireTree;
    /**創建片庫目錄結構的樹狀結構 */
    createDirNode:(id: number, data:Array<DirTreeData>)=>Array<NodeConfig>;
    /**由節點的索引取得節點 */
    get(nodeIndex:number):TreeNode;
    /**取消所有節點選擇 */
    deselect:()=>InspireTree;
}

// interface JQuery{
//     customtree(options: CustomtreeOptions):Customtree;
// }
interface JQueryStatic{
    customtree(options: CustomtreeOptions):CustomtreeConfig & Customtree;
}
