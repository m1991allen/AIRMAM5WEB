//  import * as InspireTreeDOM_ from 'inspire-tree-dom';
// import * as InspireTree_ from 'inspire-tree';
///<reference path="../../Models/@types/selectablejs.d.ts"/>
import { ArchiveMoveMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { DirTreeData } from '../../Models/Interface/Dir/DirTreeData';
import { TrueFalse, YesNo } from '../../Models/Enum/BooleanEnum';
import { IArchiveMoveController, ArchiveMoveController } from '../../Models/Controller/ArchiveMoveController';
import { ErrorMessage, WarningMessage } from '../../Models/Function/Message';
import { SubjectListModel } from '../../Models/Interface/Subject/SubjectListModel';
import { UI } from '../../Models/Templete/CompoentTemp';
import { Color } from '../../Models/Enum/ColorEnum';
import { SubjectFilesViewModel } from '../../Models/Interface/ArchiveMove/SubjectFilesViewModel';
import { SubjFileModel } from '../../Models/Interface/ArchiveMove/SubjFileModel';
import { MediaType } from '../../Models/Enum/MediaType';
import { GetMediaType } from '../../Models/Function/ConvertMedia';
import { IsNULL, IsNULLorEmpty } from '../../Models/Function/Check';
import { ModalTask } from '../../Models/Function/Modal';
import { GetImage } from '../../Models/Templete/ImageTemp';
import { Label } from '../../Models/Templete/LabelTemp';
import { Logger } from '../../Models/Class/LoggerService';
import { DirController } from '../../Models/Controller/DirController';
import { GetDirAndSubjectsByDirFilter } from '../../Models/Interface/ArchiveMove/GetDirAndSubjectsByDirFilter';

/*
TODO 上方檔案名過濾純css
TODO 檔案拖拉動作不夠敏銳
*/

/*---------------------------------------
        變數
-------------------------------------------*/

//  /**樹狀圖DOM */ const InspireTreeDOM = (<any>InspireTreeDOM_).default || InspireTreeDOM_;
// /**樹狀圖 */ var tree:InspireTree;
/**拖拉目標的樹狀圖 */ var droptree: Customtree;
/**歸檔搬遷訊息*/ const message = ArchiveMoveMessageSetting;
/**驗證規則*/ const valid = FormValidField.ArchiveMove;
/**來源樹狀節點搜尋Input*/ const $SearchNode: JQuery<HTMLElement> = $('#SearchNode');
/**目標樹狀節點搜尋Input*/ const $SearchDropNode: JQuery<HTMLElement> = $('#SearchDropNode');
/**歸檔搬遷路由*/ const route: IArchiveMoveController = new ArchiveMoveController();
/**來源樹狀*/ const $SourceTree = $('#treeList');
/**目標樹狀*/ const $TargetTree = $('#treeDropList');
/**來源主題區域 */ const $SourceSubjectList = $('#SourceSubjectList');
/**目標主題區域 */ const $TargetSubjectList = $('#TargetSubjectList');
/**影音圖文顯示區域 */ const $FileArea = $('#FileArea');
/**影音圖文顯示區域遮罩 */ const $FileAreaCover = $('#FileAreaCover');
/** 影音圖文顯示區域*/ const $FileAreas = $('.x-area');
/**影顯示區域 */ const $VideoFiles = $('#VideoFiles');
/**音顯示區域 */ const $AudioFiles = $('#AudioFiles');
/**圖顯示區域 */ const $PhotoFiles = $('#PhotoFiles');
/**文顯示區域 */ const $DocFiles = $('#DocFiles');
/**檔案搜尋 */ const $SearchFileNameInput = $('#SearchFile');
/**資料夾路徑 */ const $DirectonaryPath = $('#DirectonaryPath');
/**影選擇器*/ var VSelect: Selectable= null;
/**音選擇器*/ var ASelect: Selectable = null;
/**圖選擇器*/ var PSelect: Selectable = null;
/**文選擇器*/ var DSelect: Selectable = null;
/**目前是否開啟目標樹狀的拖放區域*/ var IsInDropArea: boolean = false;
/*jquery contain擴展:因為原有的contain功能必須區分大小寫,即使使用toLowerCase和toUpperCase也會有bug */
$.extend($.expr[':'], {
    containsi: function(elem, i, match, array) {
        return (elem.textContent || elem.innerText || '').toLowerCase().indexOf((match[3] || '').toLowerCase()) >= 0;
    },
});
/*--------------------------------------------
    任務
---------------------------------------------*/
/**創建檔案卡片 */
const createFileCard = (title: string, files: Array<SubjFileModel>): DocumentFragment => {
    const cardfragment = document.createDocumentFragment();
    if (files.length > 0) {
        for (let file of files) {
            const card = document.createElement('div');
            card.id = file.FileNo + Math.round(new Date().getTime() / 1000);
            card.className = 'ui raised link card';
            card.setAttribute('data-tempid', file.TempId.toString());
            card.setAttribute('data-fileno', file.FileNo);

            card.innerHTML = `<div class="ui image">
                                ${GetImage(file.ImageUrl, file.FileTitle)}
                               </div>
                               <div class="center aligned content">
                                   <span class="ui inverted sub header">${file.FileTitle}</span>
                               </div>`;
            cardfragment.appendChild(card);
        }
    }
    return cardfragment;
};
/**創建主題列表 */
const createSubjectItems = (dirId: number, $Area: JQuery<HTMLElement>) => {
    route
        .GetSubjectList(dirId)
        .then(res => {
            Logger.res(route.api.GetSubjectList, '目錄節點創建主題列表', res, false);
            if (res.IsSuccess) {
                const datas = <Array<SubjectListModel>>res.Data;
                if (datas.length == 0) {
                    $Area
                        .empty()
                        .append(UI.Error.CorrectSegment('查無主題', '試試其他目錄節點', 'clipboard list', Color.藍));
                } else {
                    const itemsfragnment = document.createDocumentFragment();
                    for (let data of datas) {
                        const item = document.createElement('div');
                        item.className = 'item link';
                        item.setAttribute('data-subjectId', data.fsSUBJECT_ID);
                        item.id = data.fsSUBJECT_ID;
                        item.innerHTML = `<div class="middle aligned content"> ${data.fsSUBJECT_TITLE}</div>`;
                        itemsfragnment.appendChild(item);
                    }
                    $Area.empty().append(itemsfragnment);
                }
            } else {
                ErrorMessage(res.Message);
                $Area
                    .empty()
                    .append(
                        UI.Error.CorrectSegment('查詢主題列表發生錯誤', '請重新查詢或重整頁面進行', 'search', Color.紅)
                    );
            }
        })
        .catch(error => {
            Logger.viewres(route.api.GetSubjectList, '目錄節點創建主題列表', error);
            $Area
                .empty()
                .append(
                    UI.Error.CorrectSegment('查詢主題列表發生錯誤', '請重新查詢或重整頁面進行', 'search', Color.紅)
                );
        });
};
/**創建Target目錄的主題列表_Added_20201120 */
const createTargetSubjects = (input: GetDirAndSubjectsByDirFilter, $Area: JQuery<HTMLElement>) => {
    route.GetSubjectList2(input)
    .then(res => {
        Logger.res(route.api.GetSubjectList, 'Target目錄節點創建主題列表', res, false);
        if (res.IsSuccess) {
            const datas = <Array<SubjectListModel>>res.Data;
            if (datas.length == 0) {
                $Area.empty()
                .append(UI.Error.CorrectSegment('無符合的主題', '試試其他目錄節點', 'clipboard list', Color.藍));
            } else {
                const itemsfragnment = document.createDocumentFragment();
                for(let data of datas) {
                    const item = document.createElement('div');
                    item.className = 'item link';
                    item.setAttribute('data-subjectId', data.fsSUBJECT_ID);
                    item.id = data.fsSUBJECT_ID;
                    item.innerHTML = `<div class="middle aligned content"> ${data.fsSUBJECT_TITLE}</div>`;
                    itemsfragnment.appendChild(item);
                }
                $Area.empty().append(itemsfragnment);
            }
        } else {
            ErrorMessage(res.Message);
            $Area.empty().append(
                UI.Error.CorrectSegment('查詢主題列表發生錯誤', '請重新查詢或重整頁面進行', 'search', Color.紅)
            );
        }
    })
    .catch(error => {
        Logger.viewres(route.api.GetSubjectList, 'Target目錄節點創建主題列表', error);
        $Area.empty().append(
            UI.Error.CorrectSegment('查詢主題列表發生錯誤', '請重新查詢或重整頁面進行', 'search', Color.紅)
        );
    });
};

/**依照傳入的媒體類別取得選擇器 */
const getSelectableByType = (type: MediaType): Selectable => {
    switch (type) {
        case MediaType.VIDEO:
            return VSelect;
        case MediaType.AUDIO:
            return ASelect;
        case MediaType.PHOTO:
            return PSelect;
        case MediaType.Doc:
            return DSelect;
    }
};

/*--------------------------------------------
    樹狀圖
---------------------------------------------*/
/**樹狀節點初始化 */
const tree=$.customtree({
    selector:'#treeList',
    searchword:'',
    subjcount:false,
    showhide:true,
    dragAndDrop:{
        enabled:false,
        validateOn:'dragstart',
        validate:(sourceNode,targetNode)=>{
            return true;
        }
    }
});
    /**選擇的節點可以拖動 */
    tree.nodeSelectd(function(node:TreeNode, isLoadEvent: boolean) {
        node.itree.a.attributes.draggable = 'true';
    });
    tree.nodeDeselected(function(node:TreeNode) {
        node.itree.a.attributes.draggable = 'false';
    });
    /**節點點擊時載入來源主題列表 */
    tree.nodeClick(function(event,node) {
        const nodePath = node.itree.a.attributes.title;
        $DirectonaryPath.html(nodePath).attr('data-path', nodePath);
        $FileAreas.hide();
        const isUsingQueue = node.itree.a.attributes.usingqueue;   //20201117_added
        //if (node.itree.a.attributes.isqueue == YesNo.是) {    //20201117_modified
        if ((isUsingQueue && node.itree.a.attributes.isqueue == YesNo.是) || !isUsingQueue) {
            /*表示Queue點*/
            $('#TipResult').hide();
            $('#ShowResult').show();
            $FileAreaCover.show();
            const $SourceItems = $SourceSubjectList.children('.items');
            const dirId: number = Number(node.id);
            createSubjectItems(dirId, $SourceItems);
        } else {
            $('#TipResult').show();
            $('#ShowResult').hide();
        }
    });



/**重新加載樹狀節點 */
$('#ReloadTreeBtn').click(function() {
    tree.reload();
});
/**TODO 搜尋節點 */
$SearchNode.on('keyup', function(event) {
    const word: string = <string>$(this).val();
    if (word.length > 0) {
        DirController.GetTree({ id: 0, fsKEYWORD: word, showcount: true, showhide: true })
            .then(res => {
                const data = <Array<DirTreeData>>res;
                const newnodes = tree.createDirNode(0, data);
                tree.removeAll().addNodes(newnodes);
                return true;
            })
            .catch(error => {
                Logger.viewres(route.api.GetDir, '搜尋來源節點', error, false);
                tree.removeAll(); //TODO 錯誤顯示
                return false;
            })
            .then(success => {
                tree.searchWord(word);
            });
    } else {
        tree.reload();
    }
});

/*--------------------------------------------
   選擇器  TODO 可能不要分開比較好，常發生selecting undefined 要確認
---------------------------------------------*/
const SelectTask = (type: MediaType) => {
    const commonSetting = {
        filter: document.querySelectorAll('.card'),
        autoRefresh: true,
        toggle: true,
        // ignore: ['button', '.card[draggable="true"]'],
        ignore: ['button', '.card-draggable','.ui-disable'],
        tolerance: 'touch',
        lasso: {
            border: '2px dashed rgba(22, 120, 194, 1)',
            borderRadius: '10px',
            backgroundColor: 'rgba(22, 120, 194, 0.4)',
        },
    };
    switch (type) {
        case MediaType.VIDEO:
            /**影片選擇器 */
            if (!IsNULL(VSelect)) {
                VSelect.destroy();
            }
            VSelect = new Selectable(
                Object.assign(
                    <SelectanConfig>{},
                    {
                        // appendTo: document.getElementById('VideoFiles'),
                        appendTo: document.getElementById('VideoCards'),
                    },
                    commonSetting
                )
            );
            VSelect.on('selecteditem', function(item) {
                (<HTMLElement>item.node).setAttribute('draggable', 'true');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.add('card-draggable');
                }
            });
            VSelect.on('deselecteditem', function(item) {
                (<HTMLElement>item.node).removeAttribute('draggable');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.remove('card-draggable');
                }
            });
            break;
        case MediaType.AUDIO:
            /**聲音選擇器 */
            if (!IsNULL(ASelect)) {
                ASelect.destroy();
            }
            ASelect = new Selectable(
                Object.assign(
                    <SelectanConfig>{},
                    {
                        appendTo: document.getElementById('AudioCards'),
                    },
                    commonSetting
                )
            );
            ASelect.on('selecteditem', function(item) {
                (<HTMLElement>item.node).setAttribute('draggable', 'true');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.add('card-draggable');
                }
            });
            ASelect.on('deselecteditem', function(item) {
                (<HTMLElement>item.node).removeAttribute('draggable');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.remove('card-draggable');
                }
            });
            break;
        case MediaType.PHOTO:
            /**圖片選擇器 */
            if (!IsNULL(PSelect)) {
                PSelect.destroy();
            }
            PSelect = new Selectable(
                Object.assign(
                    <SelectanConfig>{},
                    {
                        appendTo: document.getElementById('PhotoCards'),
                    },
                    commonSetting
                )
            );
            PSelect.on('selecteditem', function(item) {
                (<HTMLElement>item.node).setAttribute('draggable', 'true');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.add('card-draggable');
                }
            });
            PSelect.on('deselecteditem', function(item) {
                (<HTMLElement>item.node).removeAttribute('draggable');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.remove('card-draggable');
                }
            });
            break;
        case MediaType.Doc:
            /**文件選擇器 */
            if (!IsNULL(DSelect)) {
                DSelect.destroy();
            }
            DSelect = new Selectable(
                Object.assign(
                    <SelectanConfig>{},
                    {
                        appendTo: document.getElementById('DocCards'),
                    },
                    commonSetting
                )
            );
            DSelect.on('selecteditem', function(item) {
                (<HTMLElement>item.node).setAttribute('draggable', 'true');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.add('card-draggable');
                }
            });
            DSelect.on('deselecteditem', function(item) {
                (<HTMLElement>item.node).removeAttribute('draggable');
                if (IsInDropArea) {
                    (<HTMLElement>item.node).classList.remove('card-draggable');
                }
            });
            break;
    }
}; //TODO 搬遷是否判斷主題不能與原來相同才能搬遷?
/*--------------------------------------------
       主題區域
---------------------------------------------*/
/**
 * 來源樹狀區域被放置來源主題
 * Notice:為了使drop事件發生在div元素上，必須取消ondragenter和ondragover事件=>此取消對於'drop'事件觸發非常重要
 * 原因可參考 https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API/Drag_operations#droptargets
 */

$SourceTree
    // .on('dragover', false)
    // .on('dragleave', false)
    .on('dragover', function(event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    })
    .on('dragleave', function(event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    })
    .on('drop', function(event) {
        if (event.originalEvent.type == 'drop') {
            const targetDirId = event.target.getAttribute('data-uid') || '';
            const targetIsQueue = event.target.getAttribute('isqueue') || '';
            const usingIsQueue = event.target.getAttribute('usingqueue') || '';  //20201118_added_是否啟用目錄節點queue判斷
            const sourceDirId = event.originalEvent.dataTransfer.getData('DIRID') || '';
            const dirText = event.target.textContent;
            const subjectId = event.originalEvent.dataTransfer.getData('SUBJECTID');
            //節點拖動
            if (!IsNULLorEmpty(targetDirId) && !IsNULLorEmpty(sourceDirId)) {
                //if (targetIsQueue == YesNo.否) {  //20201118_modified
                if (usingIsQueue==TrueFalse.False || (usingIsQueue==TrueFalse.True && targetIsQueue == YesNo.否)) { 
                    //是目錄,可以放置
                    const sourceDirText = event.originalEvent.dataTransfer.getData('DIRTEXT') || '';
                    const dirText = event.target.textContent;
                    event.target.style.border = '1px dashed #fff';
                    ModalTask('#DirDropConfirm', true, {
                        closable: false,
                        onShow: function() {
                            $('#DirDropConfirm').find('.content')
                                .html(`<div class="ui sub red inverted header">※注意:</div>
                                       是否要將目錄節點【${sourceDirText}】搬遷至目錄【${dirText}】`);
                        },
                        onApprove: function() {
                            const targetNode = tree.node(targetDirId.toString());
                            const sourceNode = tree.node(sourceDirId.toString());
                            route
                                .MoveTreeNode({
                                    MoveDirId: Number(sourceDirId),
                                    TargetParentId: Number(targetDirId),
                                })
                                .then(res => {
                                    Logger.res(route.api.MoveTreeNode, '搬移目錄節點', res);
                                    if (res.IsSuccess) {
                                        event.target.style.border = 'none';
                                        sourceNode.deselect().remove();
                                        targetNode.addChild(sourceNode);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.MoveTreeNode, '搬移目錄節點', error);
                                });
                        },
                        onDeny: function() {
                            event.target.style.border = 'none';
                        },
                    });
                } else {
                    //target是節點或未知,不可以放置
                    WarningMessage('Queue不可為目的節點!');
                }
            }
            //主題拖動
            if (!IsNULLorEmpty(targetDirId) && !IsNULLorEmpty(subjectId)) {
                //if (targetIsQueue == YesNo.是) {  //20201118_modified
                if (usingIsQueue==TrueFalse.False || (usingIsQueue==TrueFalse.True && targetIsQueue == YesNo.是)) {
                    const subjectText = event.originalEvent.dataTransfer.getData('SUBJECTTEXT');
                    event.target.style.border = '1px dashed #fff';
                    ModalTask('#SubjectDropConfirm', true, {
                        closable: false,
                        onShow: function() {
                            $('#SubjectDropConfirm')
                                .find('.content')
                                .text(`是否要將主題【${subjectText}】搬遷至目錄【${dirText}】`);
                        },
                        onApprove: function() {
                            route
                                .MoveSubject({
                                    TargetDirId: Number(targetDirId),
                                    MoveSubjIds: [subjectId],
                                })
                                .then(res => {
                                    Logger.res(route.api.SubjMoveSave, '搬移主題', res);
                                    if (res.IsSuccess) {
                                        $('#' + subjectId).fadeOut();
                                        event.target.style.border = 'none';
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.SubjMoveSave, '搬移主題', error);
                                });
                        },
                        onDeny: function() {
                            event.target.style.border = 'none';
                        },
                    });
                } else {
                    WarningMessage('搬遷目錄須為Queue!');
                }
            }
        }
    })
    .on('dragstart', 'a[draggable="true"]', function(event) {
        const dragDirId = $(this).attr('data-uid') || '';
        const dragIsQueue = $(this).attr('isqueue') == YesNo.是 ? YesNo.是 : YesNo.否;
        event.originalEvent.dataTransfer.setData('DIRID', dragDirId);
        event.originalEvent.dataTransfer.setData('DIRTEXT', $(this).text());
    })
    .on('drag', 'a[draggable="true"]', function(event) {
        event.preventDefault();
    })
    .on('dragend', 'a[draggable="true"]', function(event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    })
    .on('dragexit', 'a[draggable="true"]', function(event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    });

/*點擊、拖曳來源主題列元素*/
$SourceSubjectList
    .on('click', '.item', function(event) {
        event.preventDefault();
        $SearchFileNameInput.val(''); //清空檔案關鍵字
        const _this = $(this);
        _this.attr('draggable') !== undefined ? _this.removeAttr('draggable') : _this.attr('draggable', 'true');
        $FileArea.show();
        $FileAreaCover.hide();
        const subjectId = _this.attr('data-subjectid') || '';
        const title = _this.text();
        if (!$(this).hasClass('selected')) {
            _this.toggleClass('selected');
            _this.siblings('.selected').removeClass('selected');
            $DirectonaryPath.html($DirectonaryPath.attr('data-path') + '>>' + title);
            route
                .GetFileList(subjectId)
                .then(res => {
                    if (res.IsSuccess) {
                        const data = <SubjectFilesViewModel>res.Data;
                        const dataObj: {
                            [key: string]: {
                                files: Array<SubjFileModel>;
                                container: JQuery<HTMLElement>;
                                card: string;
                            };
                        } = {
                            Video: {
                                files: data.VideoFiles,
                                container: $VideoFiles,
                                card: 'VideoCards',
                            },
                            Audio: {
                                files: data.AudioFiles,
                                container: $AudioFiles,
                                card: 'AudioCards',
                            },
                            Photo: {
                                files: data.PhotoFiles,
                                container: $PhotoFiles,
                                card: 'PhotoCards',
                            },
                            Doc: {
                                files: data.DocFiles,
                                container: $DocFiles,
                                card: 'DocCards',
                            },
                        };
                        //------各類別檔案區域處理------------
                        for (let key in dataObj) {
                            const item = dataObj[key];
                            if (item.files.length > 0) {
                                const files = createFileCard(title, item.files);
                                item.container
                                    .removeClass('x-hide')
                                    .find('.cards')
                                    .empty()
                                    .append(files)
                                    .siblings('.x-fileheader')
                                    .children("span[name='count']")
                                    .text(item.files.length);
                                item.container.show();
                            } else {
                                item.container.addClass('x-hide').hide();
                                item.container
                                    .find('.x-fileheader')
                                    .children("span[name='count']")
                                    .text('0');
                            }
                        }
                        //------完全沒有檔案--------------
                        if (
                            data.VideoFiles.length === 0 &&
                            data.AudioFiles.length === 0 &&
                            data.PhotoFiles.length === 0 &&
                            data.DocFiles.length === 0
                        ) {
                            $VideoFiles
                                .find('.cards')
                                .empty()
                                .append(
                                    UI.Error.CorrectSegment(
                                        `主題【${title}】沒有權屬檔案`,
                                        '小秘訣:可以到【主題與檔案】新增檔案',
                                        'lightbulb',
                                        Color.藍
                                    )
                                );
                            $VideoFiles.show();
                        }
                    } else {
                        ErrorMessage(res.Message);
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.GetFileList, '主題載入檔案', error);
                    $FileAreas
                        .empty()
                        .append(
                            UI.Error.CorrectSegment(
                                '載入檔案時發生錯誤',
                                '請重新查詢或重整頁面進行',
                                'search',
                                Color.紅
                            )
                        );
                })
                .then(() => {
                    $FileAreas.each(function(this: HTMLElement, index: number, element: HTMLElement) {
                        if ($(this).is(':visible')) {
                            const id = this.getAttribute('id') || '';
                            const type = <MediaType>this.getAttribute('data-type');
                            if (!IsNULLorEmpty(id)) {
                                SelectTask(type);
                            }
                        }
                    });
                    /**檔案關鍵字 */
                    const path = $DirectonaryPath.attr('data-path');
                    $SearchFileNameInput.keyup(function(event) {
                        event.preventDefault();
                        const word = <string>$(this).val();
                        const SubjectStr = $SourceSubjectList.find('.item.selected').text();
                        if (IsNULLorEmpty(word)) {
                            $FileAreas.find('.card').show();
                            $DirectonaryPath.html(path);
                        } else {
                            $DirectonaryPath.html(`在「${SubjectStr}」中的搜尋結果`);
                            $FileAreas
                                .find('.card')
                                .show()
                                .find('.content')
                                .find(
                                    `span:not(:containsi('${word.toLowerCase()}')),span:not(:containsi('${word.toUpperCase()}'))`
                                )
                                .closest('.card')
                                .hide();
                        }
                    });
                });
        }
    })
    .on('dragstart', '.item.selected', function(event) {
        event.originalEvent.dataTransfer.setData('SUBJECTID', event.target.id);
        event.originalEvent.dataTransfer.setData('SUBJECTTEXT', $(this).text());
    })
    .on('drag', '.item.selected', function(event) {
        event.preventDefault();
    })
    .on('dragend', '.item.selected', function(event) {
        event.preventDefault();
    });
/**
 * click:點擊目標主題列元素
 * drop:將檔案拖曳到目標主題列
 * Notice:為了使drop事件發生在div元素上，必須取消ondragenter和ondragover事件=>此取消對於'drop'事件觸發非常重要
 * 原因可參考 https://developer.mozilla.org/en-US/docs/Web/API/HTML_Drag_and_Drop_API/Drag_operations#droptargets
 */
$TargetSubjectList
    .on('click', '.item', function() {
        const title = $(this).text();
        const subjectId = $(this).attr('data-subjectid');
        const $XArea = $('.x-area:visible').first();
        const movetype = <MediaType>$XArea.attr('data-type');
        const moveselectble: Selectable = getSelectableByType(movetype);
        const $count = $XArea.find('span[name="count"]');
        const selectItems = moveselectble.getSelectedNodes();
        if (selectItems.length == 0) {
            WarningMessage('請先選擇要搬移的檔案');
        } else {
            ModalTask('#DropConfirm', true, {
                closable: false,
                onShow: function() {
                    $(this)
                        .find('.content')
                        .html(
                            `確定要搬遷 ${Label(selectItems.length.toString(), Color.紅)} 個檔案至主題【${title}】？`
                        );
                },
                onApprove: function() {
                    const selectedCards = $XArea.find('.card.ui-selected');
                    const filenos = selectedCards
                        .map(function(_, el) {
                            return <string>$(el).data('fileno');
                        })
                        .get();
                    route
                        .MoveFiles({
                            TargetSubjId: subjectId,
                            FileType: movetype,
                            MoveFileNos: filenos,
                        })
                        .then(res => {
                            Logger.res(route.api.MoveSave, '檔案搬移', res);
                            if (res.IsSuccess) {
                                $XArea.find('.card.ui-selected').fadeOut();
                                $count.text(Number($count.text()) - filenos.length);
                                $('#FileDropArea')
                                    .find(`.item[data-subjectid="${subjectId}"]`)
                                    .css({ border: 'none' });
                                moveselectble.deselect(selectItems);
                            }
                        })
                        .catch(error => {
                            Logger.viewres(route.api.MoveSave, '檔案搬移', error);
                        });
                },
                onDeny: function() {},
            });
        }
    })
    .on('dragover', function(event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    })
    .on('dragleave', function(event) {
        event.preventDefault();
        event.stopPropagation();
        return false;
    })
    .on('drop', '.item', function(event) {
        event.preventDefault();
        /*Notice:必須關閉下方事件,否則在Firefox拖曳時圖片會開新連結(例如:wwww.檔案編號.com)*/
        event.originalEvent.dataTransfer.effectAllowed = 'none';
        event.originalEvent.dataTransfer.dropEffect = 'none';

        if (event.originalEvent.type == 'drop') {
            const $XArea = $('.x-area:visible').first();
            const movetype = <MediaType>$XArea.attr('data-type');
            const title = event.target.textContent;
            const subjectId =
                event.target.id ||
                event.target.getAttribute('data-subjectid') ||
                event.target.closest('.item').getAttribute('data-subjectid') ||
                '';
            const selectedCards = $XArea.find('.card.ui-selected');
            const moveselectble: Selectable = getSelectableByType(movetype);
            const $count = $XArea.find('span[name="count"]');
            const selectItems = moveselectble.getSelectedNodes();
            const filenos = selectedCards
                .map(function(_, el) {
                    return <string>$(el).data('fileno');
                })
                .get();
            if (!IsNULLorEmpty(subjectId) && filenos.length > 0) {
                event.target.closest('.item').style.border = '1px dashed #fff';
                ModalTask('#DropConfirm', true, {
                    closable: false,
                    onShow: function() {
                        $(this)
                            .find('.content')
                            .html(
                                `確定要搬遷 ${Label(
                                    selectedCards.length.toString(),
                                    Color.紅
                                )} 個檔案至主題【${title}】`
                            );
                    },
                    onApprove: function() {
                        const selectedCards = $XArea.find('.card.ui-selected');
                        const filenos = selectedCards
                            .map(function(_, el) {
                                return <string>$(el).data('fileno');
                            })
                            .get();
                        route
                            .MoveFiles({
                                TargetSubjId: subjectId,
                                FileType: movetype,
                                MoveFileNos: filenos,
                            })
                            .then(res => {
                                Logger.res(route.api.MoveSave, '檔案搬移', res);
                                if (res.IsSuccess) {
                                    $XArea.find('.card.ui-selected').fadeOut();
                                    $count.text(Number($count.text()) - filenos.length);
                                    event.target.closest('.item').style.border = 'none';
                                    moveselectble.deselect(selectItems);
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.MoveSave, '檔案搬移', error);
                            });
                    },
                    onDeny: function() {
                        event.target.style.border = 'none';
                    },
                });
            }
        }
    });

/*--------------------------------------------
       檔案區域
---------------------------------------------*/

/*點擊檔案區域標題以展開或收合該媒體的檔案區域*/
$FileArea.find('.x-fileheader').click(function() {
    $(this)
        .siblings('.x-filelist')
        .slideToggle();
    $(this)
        .children('i')
        .toggleClass('right')
        .toggleClass('down');
});
/**選擇的檔案拖曳至目標樹狀 */
$FileArea
    .on('dragstart', '.card[draggable="true"]', function(event) {
        event.originalEvent.dataTransfer.setData('Text', event.target.id);
    })
    .on('drag', '.item.selected', function(event) {
        event.preventDefault();
    })
    .on('dragend', '.item.selected', function(event) {
        event.preventDefault();
    });

/*--------------------------------------------
       移動
---------------------------------------------*/
/**開側邊拖拉 */
$("button[name='move']").click(function(event) {
    event.preventDefault();
    IsInDropArea = !IsInDropArea;
    const $XArea = $(this).parent('.x-area');
    const $ArrowIcon = $(this).children('i');
    const $CardsParent = $XArea.find('.cards');
    const $Cards = $XArea.find('.card');
    const dataType = $XArea.attr('data-type');
    const $treeBg: HTMLElement = document.getElementById('treeBg');
    const movetype: MediaType = GetMediaType(<'V' | 'A' | 'D' | 'P'>dataType);
    /*------處理容器隱藏或顯示效果---*/
    $('#FileDropArea,#treeSidebar').toggle(100, 'swing');
    $('#FileContainer')
        .toggleClass('thirteen wide column')
        .toggleClass('sixteen wide column');
    $(`.x-area:not(.x-hide)[data-type !="${movetype}"]`).slideToggle();
    $(this).toggleClass('active');
    $TargetTree.show();
    $SourceSubjectList.toggle();
    $TargetSubjectList.hide();
    $ArrowIcon.toggleClass('right').toggleClass('left');
    $FileArea.toggleClass('thirteen wide column').toggleClass('nine wide column');
    $CardsParent.toggleClass('draggable');

    if(IsInDropArea){
        $XArea.find('.card.ui-selected').addClass('card-draggable');
        $XArea.find('.card').addClass("ui-disable")
    }else{
        $XArea.find('.card.ui-selected').removeClass('card-draggable');
        $XArea.find('.card').removeClass("ui-disable")
    }
    /*----處理選擇檔案的樹狀加載---------*/
    if ($(this).hasClass('active')) {
        /*處理目標樹狀圖*/
        const firstFileNo: string = $Cards.first().attr('data-fileno') || '';
        if (!IsNULLorEmpty(firstFileNo)) {
           const input= {
                DirId: 0,
                FileType: movetype,
                FileNo: firstFileNo,
                UserName: '',
                KeyWord: '',
            };
            droptree= $.customtree({
                  selector:'#treeDropList',
                  searchword:'',
                  subjcount:false,
                  showhide:false,
                  dragAndDrop: {
                    enabled:false,
                    validateOn:'dragstart',
                    validate:()=>{return true;}
                 },
                 data:function(node: TreeNode, resolve, reject){
                    const id = node ? Number(node.id) : 0;
                    if (id == 0) {
                        ArchiveMoveController.GetTagetTree(Object.assign({ DirId: id }, input))
                            .then(res => {
                                let data = <Array<DirTreeData>>res.Data;
                                let trees =droptree.createDirNode(id, data);
                                if (data.length == 1) {
                                    if (data[0].HasChildren) {
                                        ArchiveMoveController.GetTagetTree({
                                            DirId: data[0].DirId,
                                            FileType: input.FileType,
                                            FileNo: input.FileNo,
                                            UserName: input.UserName,
                                            KeyWord: input.KeyWord,
                                        })
                                            .then(subres => {
                                                const subdata = <Array<DirTreeData>>subres.Data;
                                                let subtrees =droptree.createDirNode(data[0].DirId, subdata);
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
                                Logger.viewres(ArchiveMoveController.api.GetTargetDir, '載入樹狀第一層目錄', error, false);
                            });
                    } else {
                        ArchiveMoveController.GetTagetTree({
                            DirId: id,
                            FileType: input.FileType,
                            FileNo: input.FileNo,
                            UserName: input.UserName,
                            KeyWord: input.KeyWord,
                        }).then(res => {
                                $treeBg.style.visibility = 'hidden';
                                if (res != null) {
                                    let data = <Array<DirTreeData>>res.Data;
                                    let trees =droptree.createDirNode(id, data);
                                    node.itree.icon = trees.length > 0 ? 'icon-folder-open' : 'icon-folder';
                                    resolve(trees);
                                } else {
                                    node.itree.icon = 'icon-folder-open';
                                    resolve([]);
                                }
                            })
                            .catch(error => {
                                Logger.viewres(ArchiveMoveController.api.GetTargetDir, '載入樹狀其他層目錄', error, false);
                                $treeBg.style.visibility = 'visible';
                                node.itree.icon = 'icon-folder-open';
                                resolve([]);
                            });
                    }
                 }
            });

           
           
            droptree.nodeClick(function(event, node:TreeNode) {
                const nodePath = node.itree.a.attributes.title;
                const dirId: number = Number(node.id);
                const isUsingQueue: boolean = node.itree.a.attributes.usingqueue;
                if (!isUsingQueue || (isUsingQueue && node.itree.a.attributes.isqueue == YesNo.是)) {
                    $TargetSubjectList.show();
                    const $TargetItems = $TargetSubjectList.children('.items');
                    const md : GetDirAndSubjectsByDirFilter = {
                         DirId: dirId,
                         FileType: movetype,
                         FileNo: firstFileNo
                    };
                    createTargetSubjects(md, $TargetItems);
                } else {
                    $TargetSubjectList.hide();
                }
            });
        }
    }
});
/**目標樹狀_搜尋節點 */
$SearchDropNode.on('keyup', function(event) {
    const word: string = <string>$(this).val();
    const $XArea = $('.x-area:visible').first();
    const movetype = <MediaType>$XArea.attr('data-type');
    const selectedCards = $XArea.find('.card.ui-selected');
    const fileno = selectedCards.first().data('fileno');

    if (word.length > 0) {
        route
            .GetTargetDir({
                DirId: 0,
                FileType: movetype,
                FileNo: fileno,
                UserName: '',
                KeyWord: word,
            })
            .then(res => {
                Logger.res(route.api.GetTargetDir, '搜尋目標樹狀圖的節點', res, false);
                if (res.IsSuccess) {
                    const data = <Array<DirTreeData>>res.Data;
                    const newnodes =droptree.createDirNode(0, data);
                    droptree.removeAll().addNodes(newnodes);
                    return true;
                } else {
                    return false;
                }
            })
            .catch(error => {
                droptree.removeAll(); //TODO 錯誤顯示
                Logger.viewres(route.api.GetTargetDir, '搜尋目標樹狀圖的節點', error, false);
                return false;
            })
            .then(success => {
                droptree.searchWord(word);
            });
    } else {
        droptree.reload();
    }
});
/**重load目標樹狀 */
$("#reloadTargetBtn").click(function(){
    droptree.reload();
});
