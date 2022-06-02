
import { DirTreeData } from '../../Models/Interface/Dir/DirTreeData';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { DirController } from '../../Models/Controller/DirController';
import { Logger } from '../../Models/Class/LoggerService';
import { BatchBookingController, IBatchBookingController } from '../../Models/Controller/BatchBookingController';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { MediaType, ChineseMediaType } from '../../Models/Enum/MediaType';
import { GetImage } from '../../Models/Templete/ImageTemp';
import { ModalTask } from '../../Models/Function/Modal';
import { CreateModalId, CreateFormId } from '../../Models/Const/Const.';
import { DeleteButton } from '../../Models/Templete/ButtonTemp';
import { UI } from '../../Models/Templete/CompoentTemp';
import { getIconByMediaType } from '../../Models/Function/Icon';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { ErrorMessage, WarningMessage } from '../../Models/Function/Message';
import { SubjectListModel } from '../../Models/Interface/Subject/SubjectListModel';
import { Filter } from '../../Models/Enum/Filter';
import { BatchBookingFileListModel } from '../../Models/Interface/BatchBooking/BatchBookingFileListModel';
import { SharedController } from '../../Models/Controller/SharedController';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { RefreshBookingMessage } from '../../Models/Interface/Shared/PostMessage/RefreshBookingMessage';
import { IsNULLorEmpty } from '../../Models/Function/Check';
/*=====================宣告變數=====================*/

/**來源樹狀節點搜尋Input*/ const $SearchNode: JQuery<HTMLElement> = $('#SearchNode');
/**批次調用路由*/ const route: IBatchBookingController = new BatchBookingController();
/**目標列表Id*/ const TargetTableId = '#TargetTable';
/**主題選單按鈕*/ const $XSubjectBtn = $('#x-subject-button');
/**影音圖文按鈕*/ const $XMenuItem = $('.x-menuitem');
/**放置主題列區域*/ const $SubjectItems = $('#SubjectItems');
/**檔案列表loader*/ const $XTableLoader = $('#x-table-loader');
/**檔案列表loader內的訊息*/ const $Msg = $XTableLoader.find('.tabulator-loader-msg');
/**回傳Modal性質*/
const prop = (key: keyof BatchBookingFileListModel): string => {
    return route.GetProperty<BatchBookingFileListModel>(key);
};
/**更新主題選單的文字與SubjectId屬性 */
const updateXSubjectBtn = (ItemStr: string, SubjectId: string) => {
    $XSubjectBtn.html(`${ItemStr}<i class="dropdown icon"></i>`);
    $XSubjectBtn.attr('data-subjectid', SubjectId);
    SubjectId === '' ? $XMenuItem.hide() : $XMenuItem.show();
};
/*--------------------------------------------
 初始
---------------------------------------------*/
/**選擇主題_菜單按鈕 */
$XSubjectBtn.popup({
    on: 'click',
});
$XMenuItem.hide();
$XTableLoader.hide();
/*--------------------------------------------
    樹狀圖
---------------------------------------------*/
/**
 * 樹狀節點初始化
 * Notice:此處不能顯示隱藏的目錄
 */
 const tree=$.customtree({
    selector:'#treeList',
    searchword:'',
    subjcount:false,
    showhide:false,
    dragAndDrop:{
        enabled:false,
        validateOn:'dragstart',
        validate:(sourceNode,targetNode)=>{
            return true;
        }
    }
});
/**節點點擊時載入來源主題列表 */
tree.nodeClick(function(event, node: TreeNode){
    const nodePath = node.itree.a.attributes.title;
    const isqueue: boolean = node.itree.a.attributes.usingqueue;    //20201119_added
    if (!isqueue || (isqueue && node.itree.a.attributes.isqueue == YesNo.是)) {
        /*表示Queue點*/
        CreateSubItems(Number(node.id));
        /*Notice:第一次都是載入影片,所以要重設影片選單為active*/
        $XMenuItem
            .first()
            .addClass('active')
            .siblings()
            .removeClass('active');
        $XMenuItem.show();
    } else {
        updateXSubjectBtn('無選擇主題', '');
        SetData(table, <Array<BatchBookingFileListModel>>[]);
        $SubjectItems.empty().html(`<label class="ui label">請選擇Queue點</label>`);
    }
});


/**重新加載樹狀節點 */
$('#ReloadTreeBtn').click(function() {
    // tree.reload();
    $SearchNode.val(StringEnum.Empty);
    tree.rebuild();
});
/**
 * 搜尋節點
 * Notice:此處不能顯示隱藏的目錄
 */
$SearchNode.on('keyup', function(event) {
    const word: string = <string>$(this).val();
    if (word.length > 0) {
        DirController.GetTree({ id: 0, fsKEYWORD: word, showcount: true, showhide: false })
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
    檔案列表
---------------------------------------------*/
const table = new tabulatorService(
    initSetting.TableId,
    {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        selectable: true,
        movableRows: true,
        movableRowsConnectedTables: TargetTableId,
        movableRowsReceiver: 'add',
        movableRowsSender: 'delete',
        placeholder: `<div class='ui basic center aligned segment'>
                   <h2 class="ui inverted grey header"> <div class="content">目前沒有檔案可以選擇</div></h2>
                   </div>`,
        headerVisible: true,
        paginationButtonCount: 3,
        index: prop('fsFILE_NO'),
        initialSort: [
            {
                column: prop('fsFILE_NO'),
                dir: 'desc',
            },
            {
                column: prop('Title'),
                dir: 'asc',
            },
        ],
        data: <Array<BatchBookingFileListModel>>[],
        rowContextMenu: [
            // {
            //     label: "<i class='hand pointer icon'></i>選擇此列",
            //     action: function(e, row) {
            //         row.select();
            //     },
            // },
            // {
            //     label: "<i class='close icon'></i>取消選擇此列",
            //     action: function(e, row) {
            //         row.deselect();
            //     },
            // },
            {
                label: "<i class='mouse pointer icon'></i> 選取全部",
                action: function(e, row) {
                    row.getTable().selectRow('all');
                },
            },
            {
                label: "<i class='window close icon'></i> 取消選取",
                action: function(e, row) {
                    row.getTable().deselectRow();
                },
            },
            {
                label: "<i class='counterclockwise rotated mouse pointer icon'></i> 反向選取",
                action: function(e, row) {
                    const allrows = row.getTable().getRows();
                    allrows.forEach(function(row) {
                        row.isSelected() ? row.deselect() : row.select();
                    });
                },
            },
            {
                label: "<i class='plus icon'></i> 將選取加入清單",
                action: function(e, row) {
                    const GetFilNoArray = (data: Array<BatchBookingFileListModel>) => {
                        return data.map(item => {
                            return item.fsFILE_NO;
                        });
                    };
                    const FilterByArray = (data: Array<BatchBookingFileListModel>, array: Array<string>) => {
                        return data.filter(item => array.indexOf(item.fsFILE_NO) == -1);
                    };

                    const table_ = table.GetTable();
                    const allData = <Array<BatchBookingFileListModel>>table_.getData(); //所有資料
                    const selectData =(<Array<BatchBookingFileListModel>>table_.getSelectedData()).filter(x=>x.IsForBid!==true && x.IsExpired !==true); //選擇中資料
                    const targetData= <Array<BatchBookingFileListModel>>targettable.GetTable().getData(); //清單中資料
                    /**選擇中檔案編號 */ const selectFilenos = GetFilNoArray(selectData);
                    /**沒選中資料 */
                    const notSelectData = FilterByArray(allData, selectFilenos);
                    /**清單中檔案編號*/ const targetFilenos = GetFilNoArray(targetData);
                    /**差集資料*/ const uniqeData = FilterByArray(selectData, targetFilenos);
                    /**差集檔案編號 */ const uniqueFilenos = GetFilNoArray(uniqeData);
                    /**交集資料 */ const repeatData = FilterByArray(selectData, uniqueFilenos);
                    const chuck = 10;
                    const batchCount = Math.ceil(uniqeData.length / chuck);
                    for (let i = 1; i <= batchCount; i++) {
                        const startIndex = (i - 1) * chuck;
                        const endIndex = i * chuck <= uniqeData.length ? i * chuck : uniqeData.length;
                        const arr = uniqeData.slice(startIndex, endIndex);
                        targettable.GetTable().addData(arr);
                    }
                    SetData(table, notSelectData.concat(repeatData));
                    if (repeatData.length > 0) {
                        const RepeatLabel = repeatData
                            .map(data => {
                                return `<li>${data.fsFILE_NO}</li>`;
                            })
                            .join('');
                        WarningMessage(`清單中已包含檔案編號<ul class="ui list">${RepeatLabel}</ul>`);
                    }
                    // const demo=`<table class="ui table"><thead><tr><td>檔案編號</td><td?</td></tr></thead><tbody></tbody></table>`;
                },
            },
        ],
        rowFormatter: function(row) {
            const rowdata = <BatchBookingFileListModel>row.getData();
            const rowElement = row.getElement();
            const isAdded =
                targettable
                    .GetTable()
                    .getData()
                    .filter(data => {
                        return (<BatchBookingFileListModel>data).fsFILE_NO == rowdata.fsFILE_NO;
                    }).length > 0;
            const image = GetImage(rowdata.ImageUrl, rowdata.Title);
            /**版權是否提醒、訊息內容 */const licenseMSG = rowdata.IsAlert ? `【${rowdata.LicenseMessage}】` : "";
            rowElement.classList.add('ui', 'items');
            const itemdiv: HTMLDivElement = document.createElement('div');
            const btn = isAdded ? `<button type="button" class="ui mini right floated disabled grey button"  name="add" ><i class="plus icon"></i>已加入</button>`:
               rowdata.IsExpired?`<button type="button" class="ui mini right floated _darkGrey red button" disabled><i class="stop icon"></i>授權到期</button>`:
               rowdata.IsForBid ?`<button type="button" class="ui mini right floated _darkGrey red button" disabled><i class="stop icon"></i>禁止借調</button>`:
                                `<button type="button" class="ui mini right floated basic yellow button"  name="add"><i class="plus icon"></i>加入清單</button>`;
            itemdiv.className = 'item';
            itemdiv.innerHTML = `<div class="ui small image _styleImg">${image}</div>
                                 <div class="content">
                                     <span class="header">${rowdata.Title} </span>
                                     <div class="meta">版權：<span class="x-license-label">${rowdata.LicenseStr}</span>${licenseMSG}</div>
                                     <div class="description">${rowdata.Description} </div>
                                     <div class="_time">
                                         <span>檔案編號:${rowdata.fsFILE_NO} </span>
                                         ${btn}
                                     </div>
                               </div>`;
            rowElement.innerHTML = itemdiv.outerHTML;
        },
        rowClick: function(e, row) {
            const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
            const rowdata = <BatchBookingFileListModel>row.getData();
            switch (true) {
                /*點擊:加入清單*/
                case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name === 'add':
                    if(rowdata.IsAlert){
                        WarningMessage(rowdata.LicenseMessage);
                    }
                    const targettable_ = targettable.GetTable();
                    const isAdded =
                        targettable_.getData().filter(data => {
                            return (<BatchBookingFileListModel>data).fsFILE_NO == rowdata.fsFILE_NO;
                        }).length > 0;
                    if (isAdded) {
                        WarningMessage('檔案編號已加入批次清單');
                    } else {
                        targettable_.addData(row.getData());
                        row.delete();
                    }
                    break;
            }
        },
        selectableCheck:function(row:Tabulator.RowComponent){
            const rowdata=<BatchBookingFileListModel>row.getData();
            return !rowdata.IsForBid && !rowdata.IsExpired;
        },
        columns: [
            {
                title: '標題',
                field: prop('fsFILE_NO'),
                sorter: 'string',
                titleFormatter: function() {
                    const input = ` <div class="ui inverted transparent large labeled input" style="float:right;">
                                    <div class="ui label" style="font-size: 0.8em !important;">快速篩選：</div>                
                                    <input autocomplete="off" id="wordFilter" type="text" placeholder="請輸入篩選詞彙" class="prompt"></div>`;
                    return '檔案列表' + input;
                },
                headerMenu: [
                    {
                        label: "<i class='mouse pointer icon'></i> 選擇全部",
                        action: function(e, column) {
                            column.getTable().selectRow('all');
                        },
                    },
                    {
                        label: "<i class='window close icon'></i> 取消選取",
                        action: function(e, column) {
                            column.getTable().deselectRow();
                        },
                    },
                    {
                        label: "<i class='counterclockwise rotated mouse pointer icon'></i> 反向選取",
                        action: function(e, column) {
                            const allrows = column.getTable().getRows();
                            allrows.forEach(function(row) {
                                row.toggleSelect();
                               // row.isSelected() ? row.deselect() : row.select();
                            });
                        },
                    },
                ],
            },
        ],
    },
    false
);
/*--------------------------------------------
    調用清單列表
---------------------------------------------*/
const targettable = new tabulatorService(
    TargetTableId,
    {
        height: TabulatorSetting.height,
        layout: 'fitDataFill',
        paginationButtonCount: 3,
        index: prop('fsFILE_NO'),
        data: [],
        dataLoaded: function(data) {
            $(TargetTableId)
                .find('.tabulator-paginator')
                .hide();
        },
        placeholder: `<div class='ui basic center aligned segment'>
                  <h2 class="ui icon inverted grey header">
                  <i class='hand paper icon'></i>
                   <div class="content">將列表拖拉至此區</div>
                 </h2></div>`,
        groupBy:[prop('FileType'),prop('LicenseCode')],
        groupToggleElement: 'header',
        groupHeader: [
            function(value,count:number,data:Array<BatchBookingFileListModel>) {
                const type = value;
                const icon = getIconByMediaType(<MediaType>type);
                const typeStr = getEnumKeyByEnumValue(ChineseMediaType, type);
                return `<div class='group-header'>${icon}${typeStr} (${count} 個檔案)</div>`;
            },
            function(value,count:number,data:Array<BatchBookingFileListModel>){
                const msg=data.length===0?"無":`<span>${data[0].LicenseStr}</span>${IsNULLorEmpty(data[0].LicenseMessage)?"":"【"+data[0].LicenseMessage+"】"}`;
                return `<div class="ui label">${count}個檔案，版權所屬:<div class="x-license-group">${msg}</div></div>`;
            }
        ],
        movableRowsReceiver :function(fromRow:Tabulator.RowComponent,toRow:Tabulator.RowComponent,fromTable:Tabulator){
            const rowdata=<BatchBookingFileListModel>fromRow.getData();
            if(rowdata.IsExpired){
                WarningMessage(`<i class="stop icon"></i>檔案授權已到期,禁止借調!`);
                return false;
            }
            if(rowdata.IsForBid){
                WarningMessage(`<i class="stop icon"></i>檔案禁止借調!`);
                return false;
            }
            const FileNoArray = (<Array<BatchBookingFileListModel>>targettable.GetTable().getData()).map(rowdata => rowdata.fsFILE_NO);
            if (FileNoArray.indexOf(rowdata.fsFILE_NO) > -1) {
                WarningMessage('清單中已包含此筆檔案');
                return false;
            }
            targettable.GetTable().addRow(rowdata,undefined,toRow);
            return  true;
        },
        rowFormatter: function(row) {
            const rowdata = <BatchBookingFileListModel>row.getData();
            row.getElement().setAttribute('title', rowdata.Title);
        },
        columns: [
            { title: '檔案編號', field: prop('fsFILE_NO'), width: 185 },
            { title: '標題', field: prop('Title'), minWidth: 200 },
            {title:'版權',field:prop('LicenseStr'), minWidth: 150},
            {
                title: '操作',
                field: prop('fsFILE_NO'),
                frozen: true,
                // width: 60,
                formatter: function(cell) {
                    const deletebtn = DeleteButton('', '刪除此列');
                    return deletebtn;
                },
                cellClick: function(e, cell) {
                    cell.getTable().deleteRow(cell.getRow());
                },
            },
        ],
        footerElement: `<div style="min-height:36px;">
                    <button name="deleteTarget" class="ui red button">全部刪除</button>
                    <button name="addbooking" class="ui yellow button">確定調用</button>
                   </div>`,
    },
    false
);

/**
 * 列表篩選
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('Description'), type: Filter.Like, value: word },
        { field: prop('fsFILE_NO'), type: Filter.Like, value: word },
        { field: prop('Title'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});

/**
 * 創建主題列表
 */
const CreateSubItems = (dirId: number) => {
    route
        .GetSubjectList(dirId)
        .then(res => {
            Logger.res(route.api.GetSubjectList, '查詢主題', res, false);
            const data = <Array<SubjectListModel>>res.Data;
            if (data.length == 0) {
                updateXSubjectBtn('查無主題', '');
                $SubjectItems
                    .empty()
                    .html(`<div class="ui basic inverted fluid segment" style="width:100%;">沒有資料顯示</div>`);
                SetData(table, <Array<BatchBookingFileListModel>>[]);
            } else {
                const firstSubject = data[0];
                updateXSubjectBtn(firstSubject.fsSUBJECT_TITLE, firstSubject.fsSUBJECT_ID);
                Search({
                    SubjectId: firstSubject.fsSUBJECT_ID,
                    MediaType: MediaType.VIDEO /*第一次直接載入影片類型*/,
                });
                const fragment = document.createDocumentFragment();
                for (let item of data) {
                    const li = document.createElement('li');
                    li.className = 'x-subjectItem';
                    li.innerHTML = `<a>${item.fsSUBJECT_TITLE}</a>`;
                    li.setAttribute('data-subjectid', item.fsSUBJECT_ID);
                    li.onclick = function() {
                        updateXSubjectBtn(item.fsSUBJECT_TITLE, item.fsSUBJECT_ID);
                        Search({
                            SubjectId: item.fsSUBJECT_ID,
                            MediaType: <MediaType>$('.active.x-menuitem').attr('data-type') /*改取選單*/,
                        });
                    };
                    fragment.appendChild(li);
                }
                $SubjectItems.empty().append(fragment);
            }
        })
        .catch(error => {
            Logger.error(`查詢主題時發生錯誤,api=${route.api.GetSubjectList}`, error);
            $SubjectItems.empty().html(UI.Error.ErrorSegment('查無主題'));
            SetData(table, <Array<BatchBookingFileListModel>>[]);
            ErrorMessage(`查詢主題時發生錯誤`);
        });
};
/**查詢檔案列表 */
function Search(param: { SubjectId: string; MediaType: MediaType }) {
    $Msg.addClass('tabulator-loading'); /*Notice:這裡顯示,SetData消失*/
    $XTableLoader.show();
    route
        .GetFileList({
            SubjectId: param.SubjectId,
            MediaType: param.MediaType,
        })
        .then(res => {
            Logger.res(route.api.GetFileList, '查詢檔案列表', res, false);
            const data = <Array<BatchBookingFileListModel>>res.Data;
            SetData(table, data);
        })
        .catch(error => {
            Logger.error(`查詢檔案列表發生錯誤,api=${route.api.GetFileList}`, error);
            SetData(table, <Array<BatchBookingFileListModel>>[]);
        });
}
/**
 * 對指定列表設定資料
 * Notice:由於Tabulator會動態構建其元素，因此在填充數據時需要使其可見，以便能夠進行正確佈局表格所需的計算，因為DOM中隱藏的元素沒有維。
 * 如果在隱藏表的同時初始化表或調用setData函數，則在下次使表可見時可能會在表中發現圖形錯誤。
 * 要解決此問題，可以在表可見時調用表上的重繪函數，這將迫使Tabulator重新計算表中所有單元格的佈局。
 */
function SetData(table: ItabulatorService, data: Array<BatchBookingFileListModel>) {
    const table_ = table.GetTable();
    table_
        .setData(data)
        .then(() => {
            setTimeout(function() {
                $XTableLoader.fadeOut('fast');
                $Msg.removeClass('tabulator-loading');
            }, 100);
        })
        .catch(() => {
            $Msg.addClass('tabulator-error').removeClass('tabulator-loading');
            setTimeout(function() {
                $XTableLoader.fadeOut('fast');
                $Msg.removeClass('tabulator-error');
            }, 300);
        });
    table_.redraw();
}
/**影音圖文選單 */
$XMenuItem.click(function() {
    const subjectId = $XSubjectBtn.attr('data-subjectid');
    const mediatype = <MediaType>$(this).attr('data-Type');
    Search({
        MediaType: mediatype,
        SubjectId: subjectId,
    });
    $(this)
        .addClass('active')
        .siblings()
        .removeClass('active');
});
/**刪除已加入清單中的所有資料 */
$(document).on('click', "button[name='deleteTarget']", function() {
    targettable.GetTable().setData([]);
});
/**加入調用(與我的調用清單類似) */
$(document).on('click', "button[name='addbooking']", function() {
    const $form = $(CreateFormId);
    const data = <Array<BatchBookingFileListModel>>targettable.GetTable().getData();
    /**要提交的檔案編號 */
    const selectfilenos = data.map(subdata => {
        return subdata.fsFILE_NO;
    });
    ModalTask(CreateModalId, true, {
        closable: false,
        onShow: function() {
            $form.find('.dropdown').dropdown();
            $('#ddlReson').change(function() {
                const reasonId = Number($(this).val());
                SharedController.GetBookingOption(reasonId)
                    .then(data => {
                        const selectTemp = (list: Array<SelectListItem>, name: string, labelStr: string): string => {
                            if (list.length == 0) {
                                return '';
                            } else {
                                let options = '';
                                for (let i = 0; i < list.length; i++) {
                                    const item = list[i];
                                    options += `<option value="${item.Value}">${item.Text}</option> `;
                                }
                                return `<div class="field path">
                                         <label for="${name}">${labelStr}</label>
                                         <select class="ui inverted dropdown black label" id="${name}" name="${name}">${options}</select>
                                       </div>`;
                            }
                        };
                        const pathSelect = selectTemp(data.PathList, 'PathStr', '調用路徑');
                        const videoSelect = selectTemp(data.VideoProfileList, 'ProfileV', '轉出格式-影片');
                        const audioSelect = selectTemp(data.AudioProfileList, 'ProfileA', '轉出格式-聲音');
                        const waterSelect = selectTemp(data.WatermarkList, 'WatermarkStr', '浮水印');
                        const templete: string = `${pathSelect}${videoSelect}${audioSelect}${waterSelect}`;
                        $('#OptionArea')
                            .empty()
                            .append(templete);
                    })
                    .then(() => {
                        $form.find(".dropdown[name!='WatermarkStr']").dropdown();
                        if ($("select[name='WatermarkStr']").length > 0) {
                            $("select[name='WatermarkStr']").dropdown();
                        }
                    });
            });
            $('#showFileNos')
                .empty()
                .html(
                    selectfilenos
                        .map(fileno => {
                            return `<span class="ui black label">${fileno}</span>`;
                        })
                        .join('')
                );
        },
        onApprove: function() {
            if (selectfilenos.length == 0) {
                WarningMessage('清單中至少包含一筆檔案');
            } else {
                const $ddlReson = $form.find('#ddlReson');
                const $ProfileV = $form.find("select[name='ProfileV']").parent('.dropdown');
                const $ProfileA = $form.find("select[name='ProfileA']").parent('.dropdown');
                const $WatermarkStr = $form.find("select[name='WatermarkStr']").parent('.dropdown');
                const $PathStr = $form.find("select[name='PathStr']").parent('.dropdown');
                route
                    .SaveBooking({
                        ResonStr: <string>$ddlReson.children('option:selected').text(),
                        ResonId: Number($ddlReson.children('option:selected').val()),
                        DescStr: <string>$('#txtDescStr').val(),
                        ProfileVideo: $ProfileV.length > 0 ? <string>$ProfileV.dropdown('get value') : '',
                        ProfileAudio: $ProfileA.length > 0 ? <string>$ProfileA.dropdown('get value') : '',
                        WaterMark: $WatermarkStr.length > 0 ? <string>$WatermarkStr.dropdown('get value') : '',
                        PathStr: $PathStr.length > 0 ? <string>$PathStr.dropdown('get value') : '',
                        FileNos: selectfilenos.join('^'),
                    })
                    .then(res => {
                        Logger.res(route.api.SaveBooking, '批次新增調用', res, true);
                        if (res.IsSuccess) {
                            targettable.GetTable().setData([]);
                            window.top.postMessage(<RefreshBookingMessage>{eventid:'refreshBooking'},'/');
                        }
                    })
                    .catch(error => {
                        Logger.viewres(route.api.SaveBooking, '批次新增調用', error, true);
                    });
            }
        },
    });
});
