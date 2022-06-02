import * as InspireTreeDOM_ from 'inspire-tree-dom';
import * as InspireTree_ from 'inspire-tree';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { DirController } from '../../Models/Controller/DirController';
import { Logger } from '../../Models/Class/LoggerService';
import { DirTreeData } from '../../Models/Interface/Dir/DirTreeData';
import { YesNo } from '../../Models/Enum/BooleanEnum';
// import {InspireTree}from 'inspire-tree';
//  import InspireTreeDOM from 'inspire-tree-dom';

/*
包裝樹狀組件的處理
用法舉例:
var a=$('#demo').customtree({});
a.nodeClick(()=>{});
*/
(function($){
    /**樹狀圖變數*/ var tree: InspireTree=null;
     /**樹狀圖*/const InspireTree = (<any>InspireTree_).default || InspireTree_;
     /**樹狀圖DOM */ const InspireTreeDOM = (<any>InspireTreeDOM_).default || InspireTreeDOM_;
    /**樹狀背景圖*/const $treeBg: HTMLElement = document.getElementById('treeBg');
    /**樹狀參數預設值 */const defaults:CustomtreeOptions={
        selector:'.tree',
        searchword:'',
        subjcount:true,
        showhide:false,
        dragAndDrop:false
    };
    /**參數*/var config=$.extend({},defaults);
    function CreateNodes(id: number, data: Array<DirTreeData>): Array<NodeConfig> {
        let trees: Array<NodeConfig> = [];
        try {
            for (let i = 0; i < data.length; i++) {
                const IsQueueType: boolean = data[i].DirType == 'Q' ? true : false;
                const IsUsingQueue: boolean = data[i].UsingQueue;   //20201116_added
                trees.push({
                    id: data[i].DirId.toString(),
                    //text: IsQueueType ? `${data[i].DirName}(${data[i].ChildrenLength})` : data[i].DirName,
                    text: data[i].DirName,
                    children: data[i].HasChildren
                        ? true
                        : !IsQueueType
                        ? true
                        : [] /*children=true時才會在擴展時載入子節點*/,
                    
                    itree: {
                        a: {
                            /*自定義屬性*/
                            attributes: {
                                isqueue: IsQueueType ? YesNo.是 : YesNo.否,
                                title: data[i].DirPathStr,
                                usingqueue: IsUsingQueue,  //20201116_added
                            },
                        },
                        //icon: IsQueueType ? 'icon-file-empty' : 'icon-folder',    //20201116_modified
                        icon: IsUsingQueue 
                            ? (IsQueueType ? 'icon-file-empty' : 'icon-folder')
                            : 'icon-folder',
                        state: {
                            collapsed: id == 0 ? false : !IsQueueType && data[i].HasChildren ? true : false,
                            draggable: id == 0 ? false : true,
                            'drop-target': IsQueueType || id == 0 ? false : true,
                            rendered: true,
                            editable: false,
                            hidden: false,
                            loading: data[i].HasChildren ? null : false,
                        },
                    }
                    
                });
                trees[i].itree.state['drop-target'] = IsQueueType || id == 0 ? false : true; //add
            }
            return trees;
        } catch (error) {
            Logger.error(`創建數據為樹節點時發生錯誤,原因:`, error);
        }
    }
    const treeCallback=(options:CustomtreeOptions)=>{
        tree=new InspireTree({
            
            selection: {
                multiple: true,
                autoSelectChildren: false,
                disableDirectDeselection: false,
                mode: 'default' /*default或checkbox*/,
                require: true,
            },
            //editable:true,
            editing:{
                add:false,
                edit:false,
                remove:false
            },
            pagination: {
                limit: 3000 /*一次渲染/加載多少個節點。與延遲一起使用。默認為適合容器的節點。*/,
            },
            dom: {
                deferredRendering: true,
            },
            // deferredLoading: true,/*與 deferredRendering擇一使用,但使用此limit將不再限制,需要使用total*/
            renderer: true,
            //  renderer: function(tree) {
            //     return {
            //         applyChanges: function() {},
            //         attach: function() {},
            //         batch: function() {},
            //         end: function() {}
            //     }
            // },
            visible: true,
            data: function(node: TreeNode, resolve, reject) {
                if(IsNullorUndefined(config.data)){
                    const word =config.searchword; //TODO 要加關鍵字過濾
                    const id = node ? Number(node.id) : 0;
                    if (id == 0) {
                        DirController.GetTree({ id: id, fsKEYWORD: word, showcount: config.subjcount, showhide: config.showhide })
                            .then(res => {
                                let data = <Array<DirTreeData>>res;
                                console.log(data);
                                let trees = CreateNodes(id, data);
                                if (data.length == 1) {
                                    if (data[0].HasChildren) {
                                        DirController.GetTree({
                                            id: data[0].DirId,
                                            fsKEYWORD: word,
                                            showcount: config.subjcount,
                                            showhide:config.showhide,
                                        })
                                            .then(subres => {
                                                const subdata = <Array<DirTreeData>>subres;
                                                let subtrees = CreateNodes(data[0].DirId, subdata);
                                                trees[0].children = subtrees;
                                            })
                                            .then(success => {
                                                $treeBg.style.visibility = 'hidden';
                                                resolve(trees);
                                            })
                                            .catch(error => {
                                                $treeBg.style.visibility = 'visible';
                                                resolve(trees);
                                            });
                                    } else {
                                        $treeBg.style.visibility = 'visible';
                                        trees[0].children = []; //add
                                        resolve(trees);
                                    }
                                }
                            })
                            .catch(error => {
                                $treeBg.style.visibility = 'visible';
                                console.error(`載入樹狀第一層目錄時發生錯誤,api:${DirController.api.GetDir},原因:${error}`);
                            });
                    } else {
                        DirController.GetTree({ id: id, fsKEYWORD: word, showcount: config.subjcount, showhide: config.showhide })
                            .then(res => {
                                $treeBg.style.visibility = 'hidden';
                                if (res != null) {
                                    let data = <Array<DirTreeData>>res;
                                    let trees = CreateNodes(id, data);
                                    node.itree.icon = trees.length > 0 ? 'icon-folder-open' : 'icon-folder';
                                    resolve(trees);
                                } else {
                                    node.itree.icon = 'icon-folder-open';
                                    resolve([]);
                                }
                            })
                            .catch(error => {
                                $treeBg.style.visibility = 'visible';
                                node.itree.icon = 'icon-folder-open';
                                resolve([]);
                            });
                    }
                }else{
                   config.data(node,resolve,reject);
                }
              
            },
        });
        new InspireTreeDOM(tree, {
            target: document.querySelector(config.selector),
            autoLoadMore: true,
            deferredRendering: true,
            dragAndDrop: config.dragAndDrop===false?{enabled:false,validate:():boolean=>{return true;}}:config.dragAndDrop
        });
    };
    var methods:Customtree={
        nodeClick:function(callback?:(event:MouseEvent, node:TreeNode)=>void){
            if(!IsNullorUndefined(callback)){
                tree.on('node.click', function(event:MouseEvent, node: TreeNode) {
                    callback(event,node);
                });
            }
            return tree;
        },
        nodCollapsed:function(callback?:(node:TreeNode)=>void){
            if(!IsNullorUndefined(callback)){
                tree.on('node.collapsed', function(node: TreeNode) {
                    callback(node);
                });
            }
            return tree;
        },
        nodeContextmenu:function(callback?:(event:MouseEvent,node:TreeNode)=>void){
            if(!IsNullorUndefined(callback)){
                tree.on('node.contextmenu', function(event:MouseEvent,node: TreeNode) {
                    callback(event,node);
                });
            }
            return tree;
        },
        nodeSelectd:function(callback?:(node:TreeNode,isLoadEvent:boolean)=>void){
            if(!IsNullorUndefined(callback)){
                tree.on('node.selected',function(node:TreeNode,isLoadEvent:boolean){
                    callback(node,isLoadEvent);
                });
            }
            return tree;
        },
        nodeDeselected:function(callback?:(node:TreeNode)=>void){
            if(!IsNullorUndefined(callback)){
                tree.on('node.deselected',function(node:TreeNode){
                    callback(node);
                })
            }
            return tree;
        },
        rebuild:function(callback?:Function){
            if(!IsNullorUndefined(callback)){ callback();}
            treeCallback(config);
            return tree;
        },
        reload:function(callback?:Function){
            tree.reload();
            if(!IsNullorUndefined(callback)){
                callback();
 　　       };
            return tree;
         },
        searchWord:function(val:string){
            val=IsNullorUndefined(val)?"":val;
            tree.search(val);
            return tree;
        },
        removeAll:function(){
            tree.removeAll();
            return tree;
        },
        addNodes:function(nodes:NodeConfig){
            tree.addNode(nodes);
            return tree;
        },

        createDirNode:function(id: number, data: DirTreeData[]){
            return CreateNodes(id,data);
        },
        get:function(nodeIndex){
            return tree.get(nodeIndex);
        },
        node:function(id:string):TreeNode{
            return tree.node(id);
        },
        deselect:function(){
            tree.deselect();
            
            return tree;
        }
    };
    $.customtree=function(options){
        if(IsNULLorEmpty(options.selector)){
            throw new Error('樹狀選擇器錯誤');
        }
        config=$.extend({},defaults,options);
        treeCallback(options);
        $.extend(this,methods,<CustomtreeConfig>{
            config:config
         });
        return this;
    };

    
})(jQuery);
