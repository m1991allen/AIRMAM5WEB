import { Get, Ajax } from '../../Models/Function/Ajax';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import {ModalTask, ShowModal } from '../../Models/Function/Modal';
import { CheckForm } from '../../Models/Function/Form';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { DirthFunctionCode, DirDataType, DirRelationship } from '../../Models/Enum/DirAuthEnum';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { toggleMenu, setPosition } from '../../Models/Class/dynamicTabClass';
import { EditFormId, EditModalId } from '../../Models/Const/Const.';
import { DirController, IDirController } from '../../Models/Controller/DirController';
import { ErrorMessage, WarningMessage, SuccessMessage } from '../../Models/Function/Message';
import { FormValidField } from '../../Models/Const/FormValid';
import { DirMessageSetting } from '../../Models/MessageSetting';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { EditButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { DirTreeData } from '../../Models/Interface/Dir/DirTreeData';
import { CreateNodeModel } from '../../Models/Interface/Dir/CreateNodeModel';
import { ShowAuthListModel } from '../../Models/Interface/Dir/ShowAuthListModel';
import { EditAuthInputModel } from '../../Models/Interface/Dir/EditAuthInputModel';
import { EditNodeResponseModel } from '../../Models/Interface/Dir/EditNodeResponseModel';
import { Filter } from '../../Models/Enum/Filter';
import { UI } from '../../Models/Templete/CompoentTemp';
import { Logger } from '../../Models/Class/LoggerService';
import { GetDropdown, GetSelect } from '../../Models/Function/Element';
import { CreateAuthResponseModel } from '../../Models/Interface/Dir/CreateAuthResponseModel';



/*=====================宣告變數=====================*/
const valid = FormValidField.Dir;
const message = DirMessageSetting;
const route: IDirController = new DirController();
const codetype = ['V', 'I', 'U', 'D', 'B'];
var table: ItabulatorService;
var dirId: number = 0;
var dirName: string = '';
const CreateFormId = '#CreateDirForm';
const EditDirFormId = '#EditDirForm';
const DeleteFormId = '#DeleteForm';
const MENUID = '#DirMenu';
const CreateDirModalId = '#CreateDirModal';
const EditDirModalId = '#EditDirModal';
const DeleteDirModalId = '#DeleteDirModal';
const CreateGroupModalId = '#CreateGroupModal';
const CreateGroupFormId = '#CreateGroupForm';
const CreateUserModalId = '#CreateUserModal';
const CreateUserFormId = '#CreateUserForm';
const $SearchNode: JQuery<HTMLElement> = $('#SearchNode');
/**暫存的上一次點擊節點*/var LAST_CONTEXTMENU_NODE:TreeNode=null;
/**回傳Modal性質*/
const prop = (key: keyof ShowAuthListModel): string => {
    return route.GetProperty<ShowAuthListModel>(key);
};
/*-------------------------------
  右鍵選單
--------------------------------*/
/**關閉樹狀圖區域的瀏覽器contextmenu*/
document.querySelector('.tree').addEventListener('contextmenu', function(e: Event) {
    e.preventDefault();
});
/**document點擊時就關閉樹狀圖右側目錄*/
document.onclick = function() {
    toggleMenu(MENUID, 'hide');
};

/*-------------------------------
      樹狀圖
--------------------------------*/
/**樹狀節點初始化 */
const tree=$.customtree({
    selector:'.tree',
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
/**重新加載樹狀節點 */
$('#ReloadTreeBtn').click(function() {
    tree.reload();
});
/**搜尋節點 */
$SearchNode.on('keyup', function(event) {
    const word: string = <string>$(this).val();
    if (word.length > 0) {
        DirController.GetTree({ id: 0, fsKEYWORD: word, showcount: false, showhide: true })
        .then(res => {
            const data = <Array<DirTreeData>>res;
            const newnodes = tree.createDirNode(0, data);
            tree.removeAll().addNodes(newnodes);
        })
        .catch(error => {
            tree.removeAll(); //TODO 錯誤顯示
        })
        .then(() => {
            tree.searchWord(word);
        });
    } else {
        tree.reload();
    }
});

/**新增節點 */
$(".item[name='addnode']").click(function() {
    const node = LAST_CONTEXTMENU_NODE;
    const nodeId: number = Number(node.id);
    const isQueue = node.itree.a.attributes.isqueue;
    const isUsingQueue = node.itree.a.attributes.usingqueue;   //20201116_added
    if (isUsingQueue === true && isQueue === YesNo.是) {
        WarningMessage('Queue無法新增節點!');
        return false;
    } else {
        ShowModal<IdModel>(CreateDirModalId, route.api.ShowCreateNode, { id: nodeId }).then(IsSuccess => {
            if (IsSuccess) {
                ModalTask(CreateDirModalId, true, {
                    onShow: function() {
                        $(CreateDirModalId)
                            .find('.dropdown')
                            .dropdown();
                    },
                    onApprove: function() {
                        const $CreateForm = $(CreateFormId);
                        const fsADMIN_GROUP: Array<string> = $CreateForm
                            .find("select[name='DirGroupsAry']")
                            .closest('.dropdown')
                            .dropdown('get value');
                        const fsADMIN_USER: Array<string> = $CreateForm
                            .find("select[name='DirUsersAry']")
                            .closest('.dropdown')
                            .dropdown('get value');
                        const IsFormValid: boolean = CheckForm(CreateFormId, valid.CreateNode);
                        const parentId = Number($CreateForm.find("input[name='fnPARENT_ID']").val());
                        if (IsFormValid) {
                            route
                                .CreateNode({
                                    fnDIR_ID: Number(nodeId),
                                    fsNAME: <string>$CreateForm.find("input[name='fsNAME']").val(),
                                    fnPARENT_ID: parentId,
                                    fsDESCRIPTION: <string>$CreateForm.find("textarea[name='fsDESCRIPTION']").val(),
                                    fsDIRTYPE: $CreateForm.find(".checkbox[name='fsDIRTYPE']").checkbox('is checked')
                                        ? 'Q'
                                        : '',
                                    IsQueue: $CreateForm.find(".checkbox[name='fsDIRTYPE']").checkbox('is checked')
                                        ? true
                                        : false,
                                    fnORDER: Number($CreateForm.find("input[name='fnORDER']").val()),
                                    fnTEMP_ID_SUBJECT: Number(
                                        $CreateForm
                                            .find("select[name='fnTEMP_ID_SUBJECT']")
                                            .closest('.dropdown')
                                            .dropdown('get value')
                                    ),
                                    fnTEMP_ID_VIDEO: Number(
                                        $CreateForm
                                            .find("select[name='fnTEMP_ID_VIDEO']")
                                            .closest('.dropdown')
                                            .dropdown('get value')
                                    ),
                                    fnTEMP_ID_AUDIO: Number(
                                        $CreateForm
                                            .find("select[name='fnTEMP_ID_AUDIO']")
                                            .closest('.dropdown')
                                            .dropdown('get value')
                                    ),
                                    fnTEMP_ID_PHOTO: Number(
                                        $CreateForm
                                            .find("select[name='fnTEMP_ID_PHOTO']")
                                            .closest('.dropdown')
                                            .dropdown('get value')
                                    ),
                                    fnTEMP_ID_DOC: Number(
                                        $CreateForm
                                            .find("select[name='fnTEMP_ID_DOC']")
                                            .closest('.dropdown')
                                            .dropdown('get value')
                                    ),
                                    fsADMIN_GROUP: IsNULLorEmpty(fsADMIN_GROUP) ? '' : fsADMIN_GROUP.join(';'),
                                    fsADMIN_USER: IsNULLorEmpty(fsADMIN_USER) ? '' : fsADMIN_USER.join(';'),
                                    fsSHOWTYPE: $CreateForm
                                        .find("select[name='fsSHOWTYPE']")
                                        .closest('.dropdown')
                                        .dropdown('get value'),
                                })
                                .then(res => {
                                    Logger.res(route.api.CreateNode, '新增節點', res);
                                    if (res.IsSuccess) {
                                        const data = <CreateNodeModel>res.Data;
                                        const newChild = tree.createDirNode(data.fnDIR_ID, [
                                            {
                                                DirId: data.fnDIR_ID,
                                                DirName: data.fsNAME,
                                                HasChildren: !data.IsQueue ? true : false,
                                                ChildrenLength: 0,
                                                DirType: data.IsQueue ? 'Q' : 'U',
                                                DirPathStr: node.itree.a['title'],
                                                UsingQueue: isUsingQueue, //20201116_added
                                            },
                                        ]);
                                        node.addChild(newChild[0]);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.CreateNode, '新增節點', error, false);
                                });
                        } else {
                            //Tips_20200518_目錄管理群組與目錄管理使用者非必選
                            //WarningMessage('必須選擇目錄管理群組和使用者');
                            return false;
                        }
                    },
                });
            }
        });
    }
});
/**編輯節點 */
$(".item[name='editnode']").click(function() {
    const node = LAST_CONTEXTMENU_NODE;
    const nodeId: number = Number(node.id);
    const parentId = node.getParent() === undefined ? Number(tree.get(0).id) : Number(node.getParent().id);
    const isQueue = node.itree.a.attributes.isqueue;
    ShowModal<IdModel>(EditDirModalId, route.api.ShowEditNode, { id: nodeId }).then(IsSuccess => {
        if (IsSuccess) {
            ModalTask(EditDirModalId, true, {
                closable: false,
                onShow: function() {
                    $(EditDirModalId)
                        .find('.dropdown')
                        .dropdown();
                },
                onApprove: function() {
                    const EDITDIRFORM = $(EditDirFormId);
                    const IsFormValid: boolean = CheckForm(EditDirFormId, valid.EditNode);
                    const fsADMIN_GROUP: Array<string> = EDITDIRFORM.find("select[name='DirGroupsAry']")
                        .closest('.dropdown')
                        .dropdown('get value');
                    const fsADMIN_USER: Array<string> = EDITDIRFORM.find("select[name='DirUsersAry']")
                        .closest('.dropdown')
                        .dropdown('get value');
                    if (IsFormValid) {
                        route
                            .EditNode({
                                fnDIR_ID: Number(nodeId),
                                fsNAME: <string>EDITDIRFORM.find("input[name='fsNAME']").val(),
                                fnPARENT_ID: parentId,
                                fsDESCRIPTION: <string>EDITDIRFORM.find("textarea[name='fsDESCRIPTION']").val(),
                                fsDIRTYPE: EDITDIRFORM.find(".checkbox[name='fsDIRTYPE']").checkbox('is checked')
                                    ? 'Q'
                                    : '',
                                IsQueue: EDITDIRFORM.find(".checkbox[name='fsDIRTYPE']").checkbox('is checked')
                                    ? true
                                    : false,
                                fnORDER: Number(EDITDIRFORM.find("input[name='fnORDER']").val()),
                                fnTEMP_ID_SUBJECT: Number(
                                    EDITDIRFORM.find("select[name='fnTEMP_ID_SUBJECT']")
                                        .closest('.dropdown')
                                        .dropdown('get value')
                                ),
                                fnTEMP_ID_VIDEO: Number(
                                    EDITDIRFORM.find("select[name='fnTEMP_ID_VIDEO']")
                                        .closest('.dropdown')
                                        .dropdown('get value')
                                ),
                                fnTEMP_ID_AUDIO: Number(
                                    EDITDIRFORM.find("select[name='fnTEMP_ID_AUDIO']")
                                        .closest('.dropdown')
                                        .dropdown('get value')
                                ),
                                fnTEMP_ID_PHOTO: Number(
                                    EDITDIRFORM.find("select[name='fnTEMP_ID_PHOTO']")
                                        .closest('.dropdown')
                                        .dropdown('get value')
                                ),
                                fnTEMP_ID_DOC: Number(
                                    EDITDIRFORM.find("select[name='fnTEMP_ID_DOC']")
                                        .closest('.dropdown')
                                        .dropdown('get value')
                                ),
                                fsADMIN_GROUP: IsNULLorEmpty(fsADMIN_GROUP) ? '' : fsADMIN_GROUP.join(';'),
                                fsADMIN_USER: IsNULLorEmpty(fsADMIN_USER) ? '' : fsADMIN_USER.join(';'),
                                fsSHOWTYPE: EDITDIRFORM.find("select[name='fsSHOWTYPE']")
                                    .closest('.dropdown')
                                    .dropdown('get value'),
                            })
                            .then(res => {
                                Logger.res(route.api.EditNode, '編輯節點', res);
                                const data = <EditNodeResponseModel>res.Data;
                                if (res.IsSuccess) {
                                    node.set('text', data.fsNAME);
                                    $(EditDirModalId).modal('hide');
                                    route
                                        .GetDirInfoView(Number(nodeId))
                                        .then(view => {
                                            $('._directoryTab')
                                                .parent()
                                                .html(view);
                                        })
                                        .catch(error => {
                                            $('._directoryTab')
                                                .parent()
                                                .html(UI.Error.ErrorSegment('無法取得目錄資訊頁面內容'));
                                        });
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.EditNode, '編輯節點', error, false);
                            });
                    }
                    return false;
                },
            });
        }
    });
});
/**刪除節點 */
$(".item[name='removenode']").click(function() {
    const node = LAST_CONTEXTMENU_NODE;
    const nodeId: number = Number(node.id);
    if(nodeId == 1) { WarningMessage('根目錄無法刪除!'); return false; }  //20201119_added
    ShowModal<IdModel>(DeleteDirModalId, route.api.ShowDeleteNode, { id: nodeId }).then(IsSuccess => {
        if (IsSuccess) {
            ModalTask(DeleteDirModalId, true, {
                closable: false,
                onShow: function() {
                    $(DeleteDirModalId)
                        .find('.dropdown')
                        .dropdown();
                },
                onApprove: function() {
                    const IsFormValid: boolean = CheckForm(DeleteFormId, valid.DeleteNode);
                    if (IsFormValid) {
                        route
                            .DeleteNode(nodeId)
                            .then(res => {
                                //Logger.viewres(route.api.DeleteNode, '刪除節點', res);
                                Logger.res(route.api.CreateNode, '新增節點', res);
                                if (res.IsSuccess) {
                                    node.remove();
                                    $(DeleteDirModalId).modal('hide');
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.DeleteNode, '刪除節點', error, false);
                            });
                    }
                    return false;
                },
            });
        }
    });
});

/*-------------------------------
      節點
--------------------------------*/

tree.nodeContextmenu(function(event,node){
    tree.deselect();
    node.select();
    LAST_CONTEXTMENU_NODE=node;
    event.preventDefault();
    const clientX = event.pageX;
    const clientY = event.pageY;
    setPosition(MENUID, clientY, clientX);
    toggleMenu(MENUID, 'show');
    const parentNodeId = node.getParent() === undefined ? tree.get(0).id : node.getParent().id;
    const isQueue: YesNo = node.itree.a.attributes.isqueue;
    $(".item[name='addnode']")
        .attr('data-nodeId', node.id)
        .attr('data-parentnodeId', parentNodeId)
        .attr('isqueue', isQueue);
    $(".item[name='editnode']")
        .attr('data-nodeId', node.id)
        .attr('data-parentnodeId', parentNodeId)
        .attr('isqueue', isQueue);
    $(".item[name='removenode']")
        .attr('data-nodeId', node.id)
        .attr('data-parentnodeId', parentNodeId)
        .attr('isqueue', isQueue);
});
// tree.on('node.collapsed', function(node: InspireTree_.TreeNode) {
//     Logger.log(`${node.text}節點收合觸發`);
// });
/**節點按下右鍵 */
// tree.on('node.contextmenu', function(event: MouseEvent, node: InspireTree_.TreeNode) {
//     tree.deselect();
//     node.select();
//     event.preventDefault();
//     const clientX = event.pageX;
//     const clientY = event.pageY;
//     setPosition(MENUID, clientY, clientX);
//     toggleMenu(MENUID, 'show');
//     const parentNodeId = node.getParent() === undefined ? tree.get(0).id : node.getParent().id;
//     const isQueue: YesNo = node.itree.a.attributes.isqueue;
//     LAST_CONTEXTMENU_NODE = node;
//     $(".item[name='addnode']")
//         .attr('data-nodeId', node.id)
//         .attr('data-parentnodeId', parentNodeId)
//         .attr('isqueue', isQueue);
//     $(".item[name='editnode']")
//         .attr('data-nodeId', node.id)
//         .attr('data-parentnodeId', parentNodeId)
//         .attr('isqueue', isQueue);
//     $(".item[name='removenode']")
//         .attr('data-nodeId', node.id)
//         .attr('data-parentnodeId', parentNodeId)
//         .attr('isqueue', isQueue);
// });

/**點擊節點時，判斷是不是末端節點 */
tree.nodeClick(function(event,node){
    event.preventDefault();
    LAST_CONTEXTMENU_NODE=node;
    //event.preventTreeDefault(); // 取消默認監聽器
    //handler(); //調用原始樹邏輯
    const nodeId: number = Number(node.id);
    const nodeText: string = node.text;
    //再插入新頁面前,先銷毀所有仍存在的使用者與群組權限燈箱(非共用)
    $('.modals').find(CreateUserModalId).remove();
    $('.modals').find(CreateGroupModalId).remove();

    
    //Step1:更新目錄資訊頁面
    Ajax<IdModel>('GET', route.api.ShowInfo, { id: nodeId })
        .then(view => {
            $('#ShowResult .tab[data-tab="first"]')
                .empty()
                .append(view);
        })
        .catch(error => {
            Logger.viewres(route.api.ShowInfo, `載入【${nodeText}】目錄資訊頁面`, error);
            $('#ShowResult .tab[data-tab="first"]')
                .empty()
                .append(UI.Error.ErrorSegment(`無法載入【${nodeText}】目錄權限頁面`, '頁面發生錯誤'));
        });
    //Step2:更新目錄權限頁面
    Ajax<{ id: Number; type: 'U' | 'G' }>('GET', route.api.ShowAuth, { id: nodeId, type: 'G' }, false)
        .then(view => {
            $('#ShowResult .tab[data-tab="second"]')
                .empty()
                .append(view);
        })
        .catch(error => {
            $('#ShowResult .tab[data-tab="second"]')
                .empty()
                .append(UI.Error.ErrorSegment(`無法載入【${nodeText}】目錄權限頁面`, '頁面發生錯誤'));
        })
        .then(() => {
            _DirAuthViewInit();
        });

    $('#TipResult').hide();
    $('#ShowResult').show(); 
});
// tree.on('node.click', function(event, node: InspireTree_.TreeNode, handler) {
//     event.preventDefault();
//     //event.preventTreeDefault(); // 取消默認監聽器
//     handler(); //調用原始樹邏輯
//     const nodeId: number = Number(node.id);
//     const nodeText: string = node.text;
//     //再插入新頁面前,先銷毀所有仍存在的使用者與群組權限燈箱(非共用)
//     $('.modals').find(CreateUserModalId).remove();
//     $('.modals').find(CreateGroupModalId).remove();

    
//     //Step1:更新目錄資訊頁面
//     Ajax<IdModel>('GET', route.api.ShowInfo, { id: nodeId })
//         .then(view => {
//             $('#ShowResult .tab[data-tab="first"]')
//                 .empty()
//                 .append(view);
//         })
//         .catch(error => {
//             Logger.viewres(route.api.ShowInfo, `載入【${nodeText}】目錄資訊頁面`, error);
//             $('#ShowResult .tab[data-tab="first"]')
//                 .empty()
//                 .append(UI.Error.ErrorSegment(`無法載入【${nodeText}】目錄權限頁面`, '頁面發生錯誤'));
//         });
//     //Step2:更新目錄權限頁面
//     Ajax<{ id: Number; type: 'U' | 'G' }>('GET', route.api.ShowAuth, { id: nodeId, type: 'G' }, false)
//         .then(view => {
//             $('#ShowResult .tab[data-tab="second"]')
//                 .empty()
//                 .append(view);
//         })
//         .catch(error => {
//             $('#ShowResult .tab[data-tab="second"]')
//                 .empty()
//                 .append(UI.Error.ErrorSegment(`無法載入【${nodeText}】目錄權限頁面`, '頁面發生錯誤'));
//         })
//         .then(() => {
//             _DirAuthViewInit();
//         });

//     $('#TipResult').hide();
//     $('#ShowResult').show();
// });

/*=======================宣告Task============================ */
/**Task:table中創建label顯示 */
const labelFragmentTask = (cellvalue: string): HTMLDivElement => {
    const authFuncs: Array<string> = IsNULLorEmpty(cellvalue) ? [] : cellvalue.split(','); /*具有的權限種類*/
    const unauthFuncs: Array<string> = codetype.filter(function(i) {
        return authFuncs.indexOf(i) < 0;
    }); /*不具有的權限種類*/
    let labels = document.createElement('div');
    labels.classList.add('x-labels');
    let labelfragment = document.createDocumentFragment();
    for (let i = 0; i < authFuncs.length; i++) {
        const code = authFuncs[i];
        const label = document.createElement('span');
        label.textContent = getEnumKeyByEnumValue(DirthFunctionCode, code);
        switch (code) {
            case DirthFunctionCode.刪除:
                label.className = 'ui red label';
                labelfragment.appendChild(label);
                break;
            case DirthFunctionCode.新增:
                label.className = 'ui yellow label';
                labelfragment.appendChild(label);
                break;
            case DirthFunctionCode.檢視:
                label.className = 'ui blue label';
                labelfragment.appendChild(label);
                break;
            case DirthFunctionCode.編輯:
                label.className = 'ui green label';
                labelfragment.appendChild(label);
                break;
            case DirthFunctionCode.調用:
                label.className = 'ui violet label';
                labelfragment.appendChild(label);
                break;
            default:
                label.className = 'ui black label';
                label.textContent = IsNULLorEmpty(code) ? '空值' : code;
                labelfragment.appendChild(label);
                break;
        }
    }
    for (let j = 0; j < unauthFuncs.length; j++) {
        const unauthcode = unauthFuncs[j];
        const unauthlabel = document.createElement('span');
        unauthlabel.className = 'ui label';
        unauthlabel.textContent = getEnumKeyByEnumValue(DirthFunctionCode, unauthcode);
        labelfragment.appendChild(unauthlabel);
    }
    labels.appendChild(labelfragment);
    return labels;
};

/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('USER_NAME'), type: Filter.Like, value: word },
        { field: prop('GROUP_NAME'), type: Filter.Like, value: word },
    ];

    const CWord =
        word.indexOf('繼承') > -1 ? DirRelationship.繼承 : word.indexOf('直接') > -1 ? DirRelationship.直接 : '';
    if (!IsNULLorEmpty(CWord)) {
        filter.push({ field: prop('C_ADMIN'), type: Filter.Like, value: CWord });
        filter.push({ field: prop('C_USER'), type: Filter.Like, value: CWord });
    }
    table.SetFilter(filter);
});
/**Task:創建table */
const tableTask = async (selecttype: 'U' | 'G' | string) => {
    const json = JSON.parse(JSON.stringify($('#SaveData').data('json')));
    table = new tabulatorService(initSetting.TableId, {
        addRowPos: 'top',
        data: json,
        layout: TabulatorSetting.layout,
        columns: [
            {
                title: '使用者/群組',
                field: prop('DATATYPE'),
                sorter: 'string',
                minWidth: 110,
                titleFormatter: function(cell, formatterParams, onRendered) {
                    const text: string = getEnumKeyByEnumValue(DirDataType, selecttype);
                    return text;
                },
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const data: ShowAuthListModel = cell.getRow().getData();
                    switch (value) {
                        case DirDataType.使用者:
                            const username: string = data.USER_NAME;
                            return username;
                        case DirDataType.群組:
                            const groupname: string = data.GROUP_NAME;
                            return groupname;
                        default:
                            return value;
                    }
                },
            },
            {
                title: '目錄管理權限',
                field: prop('C_ADMIN'),
                sorter: 'string',
                width: 110,
                titleFormatter: function(cell, formatterParams, onRendered) {
                    return '目錄管理<br>權限';
                },
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const text: string = getEnumKeyByEnumValue(DirRelationship, value);
                    return text;
                },
            },
            {
                title: '主題/檔案權限',
                field: prop('C_USER'),
                sorter: 'string',
                width: 110,
                titleFormatter: function(cell, formatterParams, onRendered) {
                    return '主題/檔案<br>權限';
                },
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const text: string = getEnumKeyByEnumValue(DirRelationship, value);
                    return text;
                },
            },
            {
                title: '主題',
                field: prop('LIMIT_SUBJECT'),
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const labels = labelFragmentTask(value);
                    return labels.outerHTML.toString();
                },
            },
            {
                title: '影片',
                field: prop('LIMIT_VIDEO'),
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const labels = labelFragmentTask(value);
                    return labels.outerHTML.toString();
                },
            },
            {
                title: '聲音',
                field: prop('LIMIT_AUDIO'),
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const labels = labelFragmentTask(value);
                    return labels.outerHTML.toString();
                },
            },
            {
                title: '圖片',
                field: prop('LIMIT_PHOTO'),
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const labels = labelFragmentTask(value);
                    return labels.outerHTML.toString();
                },
            },
            {
                title: '文件',
                field: prop('LIMIT_DOC'),
                minWidth: 120,
                formatter: function(cell, formatterParams) {
                    const value = cell.getValue();
                    const labels = labelFragmentTask(value);
                    return labels.outerHTML.toString();
                },
            },
            { title: '目錄Id', field: prop('fnDIR_ID'), visible: false, download: false },
            { title: '群組Id', field: prop('GROUP_ID'), visible: false, download: false },
            { title: '使用者Id', field: prop('USER_ID'), visible: false, download: false },
            {
                title: '操作',
                field: prop('fnDIR_ID'),
                hozAlign: 'left',
                width: 130,
                formatter: function(cell, formatterParams) {
                    const row = cell.getRow();
                    const rowdata: ShowAuthListModel = row.getData();
                    const id = rowdata.fnDIR_ID;
                    const adminAuth: 'Y' | 'y' | string = rowdata.C_ADMIN;
                    const userAuth: 'Y' | 'y' | string = rowdata.C_USER;
                    const editbtn =
                        adminAuth === DirRelationship.繼承 || adminAuth === DirRelationship.直接
                            ? StringEnum.Empty
                            : EditButton(id, message.Controller);
                    const deletebtn =
                        adminAuth == '' && (userAuth === DirRelationship.直接 || userAuth === '')
                            ? DeleteButton(id, message.Controller)
                            : StringEnum.Empty;
                    cell.getElement().classList.add('tabulator-operation');
                    const btngroups: string = editbtn + deletebtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const row = cell.getRow();
                    const rowid = row.getIndex();
                    const rowdata: ShowAuthListModel = row.getData();
                    const userid = rowdata.USER_ID;
                    const groupid = rowdata.GROUP_ID;
                    const directoryId: string = cell.getValue();
                    const adminAuth: 'Y' | 'y' | string = rowdata.C_ADMIN;
                    const userAuth: 'Y' | 'y' | string = rowdata.C_USER;
                    const selecttype: 'U' | 'G' = $('#DataType').dropdown('get value');
                    const idvalue = selecttype == 'G' ? groupid : userid;
                    switch (true) {
                        /**Task:編輯 */
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            const ConfirmAdminModalId = '#ConfirmAdminModal';
                            if (IsNULLorEmpty(adminAuth)) {
                                switch (userAuth) {
                                    case DirRelationship.直接:
                                        EidtAuthModal({
                                            id: Number(directoryId),
                                            type: selecttype,
                                            idvalue: idvalue,
                                            groupid: groupid,
                                            rowdata: rowdata,
                                            row: row,
                                        });
                                        break;
                                    case DirRelationship.繼承:
                                    default:
                                        ModalTask(ConfirmAdminModalId, true, {
                                            onApprove: function() {
                                                EidtAuthModal({
                                                    id: Number(directoryId),
                                                    type: selecttype,
                                                    idvalue: idvalue,
                                                    groupid: groupid,
                                                    rowdata: rowdata,
                                                    row: row,
                                                });
                                            },
                                        });
                                        break;
                                }
                            } else {
                                ModalTask('#AlertAdminModal',true,{
                                    closable:false
                                });
                                // OtherModal('#AlertAdminModal', function(){},function(){});
                            }
                            break;
                        /**Task:刪除*/
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            if (adminAuth == '') {
                                ModalTask('#ConfirmDeleteModal',true,{
                                    closable:false,
                                    onApprove:function(){
                                        route
                                        .DeleteOperationAuth(Number(directoryId), selecttype, idvalue)
                                        .then(res => {
                                            if (res.IsSuccess) {
                                                /*Tip_20201228: 回覆內容調整: 資料列內容✚新的下拉清單內容(角色or使用者) */
                                                const data = <CreateAuthResponseModel>res.Data;
                                                /*Tip_20201228 *///更新角色or使用者的dropdownList = data.RoleOrUserList;
                                                SuccessMessage(res.Message);
                                                table.RemoveRow(row);
                                                switch(selecttype){
                                                    case 'U':
                                                      const $UDropdown =  <JQuery<HTMLElement>|false>GetDropdown(CreateUserModalId,'fsLOGIN_ID').dropdown('clear').dropdown('get item',rowdata[prop('LOGIN_ID')]);
                                                      console.log('20201224---$UDropdown',$UDropdown);
                                                      $UDropdown === false
                                                      ? GetSelect(CreateUserModalId,'fsLOGIN_ID').append(`<option value="${rowdata[prop('LOGIN_ID')]}">${rowdata[prop('LOGIN_ID')]} ${rowdata[prop('USER_NAME')]}</option>`).dropdown('refresh')
                                                      : $UDropdown.removeClass('disabled');        
                                                        break;
                                                    case 'G':
                                                       const $GDropdown=<JQuery<HTMLElement>|false>GetDropdown(CreateGroupModalId,'fsGROUP_ID').dropdown('clear').dropdown('get item',idvalue);
                                                       $GDropdown===false
                                                       ? GetSelect(CreateGroupModalId,'fsGROUP_ID').append(`<option value="${idvalue}">${rowdata[prop('GROUP_NAME')]}</option>`).dropdown('refresh')
                                                     : $GDropdown.removeClass('disabled');                                                   
                                                        break;
                                                }
                                            } else {
                                                ErrorMessage(res.Message);
                                            }
                                        });
                                    }
                                });
        
                            } else {
                                ModalTask('#AlertDeleteModal',true,{
                                    closable:false
                                });
                            }
                            break;
                        default:
                            break;
                    }
                },
            },
        ],
    });
};
/**Task:變更下拉選單 */
const changetypeTask = () => {
    $('#DataType').dropdown({
        onChange: function(this, value) {
            const text: string = $(this)
                .find('option:selected')
                .text();
            dirId = Number($('#DirId').val());
            Get(
                Controller.Dir,
                Action.ShowDirAuth,
                { id: dirId, type: value },
                false,
                null,
                function(view) {
                    $('#ShowResult .tab[data-tab="second"]')
                        .empty()
                        .append(view);
                },
                function(view) {
                    $('#ShowResult .tab[data-tab="second"]')
                        .empty()
                        .append(view);
                    ErrorMessage('系統發生錯誤，無法載入【' + dirName + '】目錄權限頁面');
                },
                function() {
                    _DirAuthViewInit();
                }
            );
        },
    });
};
/**Task:新增目錄使用者/群組權限 */
const createTask = (selecttype: 'U' | 'G' | string) => {
    switch (selecttype) {
        case DirDataType.群組:
            ModalTask(CreateGroupModalId, true, {
                closable: false,
                onApprove: function() {
                    const $CreateGroupForm: JQuery<HTMLElement> = $(CreateGroupFormId);
                    const IsFormValid: boolean = CheckForm(CreateGroupFormId, valid.CreateGroupAuth);
                    if (IsFormValid) {
                        const $fsGROUP_ID: string =GetDropdown(CreateGroupFormId,'fsGROUP_ID').dropdown('get value');
                        const $fsLIMIT_SUBJECT: Array<string> =GetDropdown(CreateGroupFormId,'fsLIMIT_SUBJECT').dropdown('get value');
                        const $fsLIMIT_VIDEO: Array<string> = GetDropdown(CreateGroupFormId,'fsLIMIT_VIDEO').dropdown('get value'); 
                        const $fsLIMIT_AUDIO: Array<string> =  GetDropdown(CreateGroupFormId,'fsLIMIT_AUDIO').dropdown('get value');
                        const $fsLIMIT_PHOTO: Array<string> =  GetDropdown(CreateGroupFormId,'fsLIMIT_PHOTO').dropdown('get value');
                        const $fsLIMIT_DOC: Array<string> = GetDropdown(CreateGroupFormId,'fsLIMIT_DOC').dropdown('get value');
                        route
                            .CreateGroupAuth({
                                AuthType: 'G',
                                fnDIR_ID: Number($CreateGroupForm.find("input[name='fnDIR_ID']").val()),
                                fsGROUP_ID: IsNULLorEmpty($fsGROUP_ID) ? StringEnum.Empty : $fsGROUP_ID,
                                fsLIMIT_SUBJECT: IsNULLorEmpty($fsLIMIT_SUBJECT)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_SUBJECT.join(','),
                                fsLIMIT_VIDEO: IsNULLorEmpty($fsLIMIT_VIDEO)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_VIDEO.join(','),
                                fsLIMIT_AUDIO: IsNULLorEmpty($fsLIMIT_AUDIO)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_AUDIO.join(','),
                                fsLIMIT_PHOTO: IsNULLorEmpty($fsLIMIT_PHOTO)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_PHOTO.join(','),
                                fsLIMIT_DOC: IsNULLorEmpty($fsLIMIT_DOC) ? StringEnum.Empty : $fsLIMIT_DOC.join(','),
                            })
                            .then(res => {
                                if (res.IsSuccess) {
                                    SuccessMessage(res.Message);
                                    $(CreateGroupFormId).trigger('reset');
                                    $(CreateGroupModalId).modal('hide');
                                    // const data = <ShowAuthListModel>res.Data;
                                    // table.AddRow(data);
                                    /*Tip_20201228: 回覆內容調整: 資料列內容✚新的下拉清單內容(角色or使用者) */
                                    const data = <CreateAuthResponseModel>res.Data;
                                    table.AddRow(data.DirAuthority);
                                    /*Tip_20201228 *///角色or使用者的dropdownList = data.RoleOrUserList;
                                    /*disabled處理_20201231_marked *///GetDropdown(CreateGroupFormId,'fsGROUP_ID').dropdown('clear').dropdown('get item',$fsGROUP_ID).addClass('disabled');
                                    //↓↓_20201231_處理_↓↓
                                    var eleSelect = document.getElementById("fsGROUP_ID");
                                    var eleSelectOpts = document.querySelectorAll('#fsGROUP_ID option');
                                    eleSelectOpts.forEach(f => { f.remove(); }); //移除Option
                                    data.RoleOrUserList.forEach(f => {
                                        var opt = document.createElement("option");
                                        opt.text = f.Text;
                                        opt.value = f.Value;
                                        eleSelect.append(opt);
                                    });//---20201231_end_
                                } else {
                                    ErrorMessage(res.Message);
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.CreateGroupAuth, '新增目錄群組權限', error, true);
                            });
                    }
                    return false;
                },
            });
            break;
        case DirDataType.使用者:
            ModalTask(CreateUserModalId, true, {
                closable: false,
                onApprove: function() {
                    const $CreateUserForm: JQuery<HTMLElement> = $(CreateUserFormId);
                    const IsFormValid: boolean = CheckForm(CreateUserFormId, valid.CreateUserAuth);
                    if (IsFormValid) {
                        const $fsLOGIN_ID: string = GetDropdown(CreateUserFormId,'fsLOGIN_ID').dropdown('get value'); 
                        const $fsLIMIT_SUBJECT: Array<string> = GetDropdown(CreateUserFormId,'fsLIMIT_SUBJECT').dropdown('get value');
                        const $fsLIMIT_VIDEO: Array<string> = GetDropdown(CreateUserFormId,'fsLIMIT_VIDEO').dropdown('get value');
                        const $fsLIMIT_AUDIO: Array<string> =GetDropdown(CreateUserFormId,'fsLIMIT_AUDIO').dropdown('get value') ;
                        const $fsLIMIT_PHOTO: Array<string> = GetDropdown(CreateUserFormId,'fsLIMIT_PHOTO').dropdown('get value');
                        const $fsLIMIT_DOC: Array<string> = GetDropdown(CreateUserFormId,'fsLIMIT_DOC').dropdown('get value');
                        route
                            .CreateUserAuth({
                                AuthType: 'U',
                                fnDIR_ID: Number($CreateUserForm.find("input[name='fnDIR_ID']").val()),
                                fsLOGIN_ID: IsNULLorEmpty($fsLOGIN_ID) ? StringEnum.Empty : $fsLOGIN_ID,
                                fsLIMIT_SUBJECT: IsNULLorEmpty($fsLIMIT_SUBJECT)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_SUBJECT.join(','),
                                fsLIMIT_VIDEO: IsNULLorEmpty($fsLIMIT_VIDEO)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_VIDEO.join(','),
                                fsLIMIT_AUDIO: IsNULLorEmpty($fsLIMIT_AUDIO)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_AUDIO.join(','),
                                fsLIMIT_PHOTO: IsNULLorEmpty($fsLIMIT_PHOTO)
                                    ? StringEnum.Empty
                                    : $fsLIMIT_PHOTO.join(','),
                                fsLIMIT_DOC: IsNULLorEmpty($fsLIMIT_DOC) ? StringEnum.Empty : $fsLIMIT_DOC.join(','),
                            })
                            .then(res => {
                                if (res.IsSuccess) {
                                    console.log(res);
                                    SuccessMessage(res.Message);
                                    $(CreateUserFormId).trigger('reset');
                                    $(CreateUserModalId).modal('hide');
                                    //const data = <ShowAuthListModel>res.Data;
                                    //table.AddRow(data);
                                    /*Tip_20201228: 回覆內容調整: 資料列內容✚新的下拉清單內容(角色or使用者) */
                                    const data = <CreateAuthResponseModel>res.Data;
                                    table.AddRow(data.DirAuthority);
                                    //*Tip_20201228 角色or使用者的dropdownList *///var newList = data.RoleOrUserList;
                                    /*disabled處理_20201231_marked *///GetDropdown(CreateUserFormId,'fsLOGIN_ID').dropdown('clear').dropdown('get item',$fsLOGIN_ID).addClass('disabled');
                                    //↓↓_20201231_↓↓
                                    var eleSelect = document.getElementById("fsLOGIN_ID");
                                    var eleSelectOpts = document.querySelectorAll('#fsLOGIN_ID option');
                                    eleSelectOpts.forEach(f => { f.remove(); });
                                    data.RoleOrUserList.forEach(f=>{
                                        var opt = document.createElement("option");
                                        opt.text = f.Text;
                                        opt.value = f.Value;
                                        eleSelect.append(opt);
                                    });//---20201231_end_
                                } else {
                                    ErrorMessage(res.Message);
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.CreateUserAuth, '新增使用者權限', error, true);
                            });
                    }
                    return false;
                },
            });
            break;
    }
};
/**Init:載入目錄權限頁面後的初始化執行事件 */
const _DirAuthViewInit = () => {
    /*Step1 初始化下拉選單*/
    $('.ui.dropdown:not(#DataType)').dropdown();
    /*Step2 點擊新增按鈕,依選擇類型顯示不同modal */
    $("button[name='create']").click(function() {
        const selecttype: 'U' | 'G' = $('#DataType').dropdown('get value');
        createTask(selecttype);
    });
    /*Step3 執行變更下拉選單 */
    changetypeTask();
    /*Step4 如果有列表就創建為tabulator列表 */
    if ($(initSetting.TableId).length > 0) {
        const selecttype: 'U' | 'G' = $('#DataType').dropdown('get value');
        tableTask(selecttype);
        // bindingBtnFunctionTask();
    }
};

/**編輯權限燈箱 */
const EidtAuthModal = (input: {
    id: number;
    type: 'G' | 'U';
    idvalue: string;
    groupid: string;
    rowdata: ShowAuthListModel;
    row: Tabulator.RowComponent;
}) => {
    const id = input.id;
    const groupid = input.groupid;
    const rowdata = input.rowdata;
    const type = input.type;
    const row = input.row;
    ShowModal<{ id: number; type: 'G' | 'U'; idvalue: string }>(EditModalId, route.api.ShowEditedAuth, {
        id: input.id,
        type: input.type,
        idvalue: input.idvalue,
    }).then(IsSuccess => {
        if (IsSuccess) {
            ModalTask(EditModalId, true, {
                onShow: function() {
                    $(EditModalId)
                        .find('.dropdown')
                        .dropdown();
                },
                onApprove: function() {
                    const IsFormValid = CheckForm(EditFormId, valid.EditAuth); //TODO todo 確認Dir EditForm Valid
                    if (IsFormValid) {
                        const limitSUBJECT = <Array<string>>$(EditFormId)
                            .find("select[name='LimitSubject']")
                            .parent('.dropdown')
                            .dropdown('get value');
                        const limitVIDEO = <Array<string>>$(EditFormId)
                            .find("select[name='LimitVideo']")
                            .parent('.dropdown')
                            .dropdown('get value');
                        const limitAUDIO = <Array<string>>$(EditFormId)
                            .find("select[name='LimitAudio']")
                            .parent('.dropdown')
                            .dropdown('get value');
                        const limitPHOTO = <Array<string>>$(EditFormId)
                            .find("select[name='LimitPhoto']")
                            .parent('.dropdown')
                            .dropdown('get value');
                        const limitDOC = <Array<string>>$(EditFormId)
                            .find("select[name='LimitDoc']")
                            .parent('.dropdown')
                            .dropdown('get value');
                        const input: EditAuthInputModel = {
                            DirId: Number(id),
                            GroupId: groupid,
                            GroupName: rowdata.GROUP_NAME,
                            UserId: rowdata.USER_ID,
                            LoginId: rowdata.LOGIN_ID,
                            ShowName: rowdata.USER_NAME,
                            C_ADMIN: rowdata.C_ADMIN,
                            C_USER: rowdata.C_USER,
                            LimitSubject: limitSUBJECT,
                            LimitVideo: limitVIDEO,
                            LimitAudio: limitAUDIO,
                            LimitPhoto: limitPHOTO,
                            LimitDoc: limitDOC,
                        };
                        if (type == 'G') {
                            route.EditGroupAuth(input).then(res => {
                                res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                                if (res.IsSuccess) {
                                    const resjson = <ShowAuthListModel>res.Data;
                                    console.log(res);
                                    console.log(resjson);
                                    table.ReactivityUpdate(row, {
                                        fnDIR_ID: resjson.fnDIR_ID,
                                        LOGIN_ID: resjson.LOGIN_ID,
                                        LIMIT_SUBJECT: resjson.LIMIT_SUBJECT,
                                        LIMIT_VIDEO: resjson.LIMIT_VIDEO,
                                        LIMIT_AUDIO: resjson.LIMIT_AUDIO,
                                        LIMIT_PHOTO: resjson.LIMIT_PHOTO,
                                        LIMIT_DOC: resjson.LIMIT_DOC,
                                    });
                                    $(EditModalId).modal('hide');
                                }
                            });
                        } else {
                            route
                                .EditUserAuth(input)
                                .then(res => {
                                    res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                                    if (res.IsSuccess) {
                                        const resjson = <ShowAuthListModel>res.Data;
                                        console.log(res);
                                        console.log(resjson);
                                        table.ReactivityUpdate(row, {
                                            fnDIR_ID: resjson.fnDIR_ID,
                                            LOGIN_ID: resjson.LOGIN_ID,
                                            LIMIT_SUBJECT: resjson.LIMIT_SUBJECT,
                                            LIMIT_VIDEO: resjson.LIMIT_VIDEO,
                                            LIMIT_AUDIO: resjson.LIMIT_AUDIO,
                                            LIMIT_PHOTO: resjson.LIMIT_PHOTO,
                                            LIMIT_DOC: resjson.LIMIT_DOC,
                                        });
                                        $(EditModalId).modal('hide');
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.EditUserAuth, '編輯使用者群組', error, true);
                                });
                        }
                    }
                    return false;
                },
            });
        } else {
            ErrorMessage(`權限編輯視窗發生錯誤`);
        }
    });
};
