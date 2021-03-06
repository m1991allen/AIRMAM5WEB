import { YesNo } from '../../Models/Enum/BooleanEnum';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { tabulatorService } from '../../Models/Class/tabulatorService';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { ModalTask, ShowModal, DetailModal } from '../../Models/Function/Modal';
import { ISubjectController, SubjectController } from '../../Models/Controller/SubjectController';
import { MediaType, MediaUploadType } from '../../Models/Enum/MediaType';
import { IsImageValid } from '../../Models/Function/Image';
import { GetImage } from '../../Models/Templete/ImageTemp';
import { CreateFormId, DeleteModalId, EditFormId, EditModalId } from '../../Models/Const/Const.';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { ErrorMessage, InfoMessage, SuccessMessage } from '../../Models/Function/Message';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { TitleEnum } from '../../Models/Enum/TitleEnum';
import { SelectListItem } from '../../Models/Interface/Shared/ISelectListItem';
import { UploadTask } from './upload';
import { Paragraph } from './paragraph';
import { KeyFrame } from './keyFrame';
import { MetaData } from './metaData';
import { TabNameEnum } from '../../Models/Enum/TabNameEnum';
import { GetImageUrl } from '../../Models/Function/Url';
import { AirmamImage } from '../../Models/Const/Image';
import { videoPlayer } from '../../Models/Class/videoPlayer';
import { audioPlayer } from '../../Models/Class/audioPlayer';
import { WhiteCloseIcon } from '../../Models/Templete/IconTemp';
import { CheckForm, AddDynamicNullable} from '../../Models/Function/Form';
import { SubjectMessageSetting } from '../../Models/MessageSetting';
import { FormValidField } from '../../Models/Const/FormValid';
import { DirTreeData } from '../../Models/Interface/Dir/DirTreeData';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { PermissionDefinition } from '../../Models/Enum/PermissionDefinition';
import { DetailButton, EditButton, DeleteButton, UploadButton } from '../../Models/Templete/ButtonTemp';
import { setCalendar} from '../../Models/Function/Date';
import { DirTempleteModel } from '../../Models/Interface/Subject/DirTempleteModel';
import { SubjectListModel } from '../../Models/Interface/Subject/SubjectListModel';
import { SubjectSearchModel } from '../../Models/Interface/Subject/SubjectSearchModel';
import { FileListModel } from '../../Models/Interface/Subject/FileListModel';
import { SubjectUploadConfig } from '../../Models/Interface/Subject/SubjectUploadConfig';
import { AcceptExtension } from '../../Models/Interface/Subject/UploadServiceConfig';
import { GetMediaTypeByUploadType, GetMediaSearchType } from '../../Models/Function/ConvertMedia';
import { GetSelect, GetDropdown } from '../../Models/Function/Element';
import { UserFileSubjectAuthModel } from '../../Models/Interface/Subject/UserFileAuthModel';
import { Filter } from '../../Models/Enum/Filter';
import { WorkStatus } from '../../Models/Enum/WorkStatus';
import { DirController } from '../../Models/Controller/DirController';
import { Logger } from '../../Models/Class/LoggerService';
import { CreateOption } from '../../Models/Templete/FormTemp';

/*---------------------------------------
        ??????
-------------------------------------------*/   
/**?????????????????????*/const message = SubjectMessageSetting;
/**???????????????????????????*/const valid = FormValidField.Subject;
/**??????????????????*/const $SearchNode: JQuery<HTMLElement> = $('#SearchNode');
/**???????????????????????? */ var LastSelectRow: Tabulator.RowComponent;
/**??????????????? */ var _videoPlayer: videoPlayer;
/**???????????????*/ var _audioPlayer: audioPlayer;
/**???????????? */ var route: ISubjectController = new SubjectController();
/**????????????ModalId */ const CusModalId: string = '#CusModal';
/**????????????subtable??????????????????*/const ChangeRowConfirmModalId='#ChangeRowConfirm';
/**????????????Modal?????? */ export const $CusModal: JQuery<HTMLElement> = $(CusModalId);
/**??????Modal?????? */ export const UploadModalId = '#UploadModal';
/**???????????? */ const $Preview: JQuery<HTMLElement> = $CusModal.find("div[name='preview']");
/**?????????????????????????????? */ const $DataView: JQuery<HTMLElement> = $CusModal.find("div[name='dataview']");
/**??????????????????Id */ const CreateSubjectModalId = '#CreateSubjectModal';
/**?????????????????? */ const $CreateSubjectBtn: JQuery<HTMLElement> = $('#CreateSubjectBtn');
/**???????????? */  var table: ItabulatorService;
/**CusModal?????????????????? */  var subtable: ItabulatorService=null;
/**????????????????????? */ export var selectids: Array<string> = [];
/**??????????????????*/var pathName:string='';
/**?????????????????????????????????*/let selectRowData:FileListModel=null;
/**??????:?????????????????????????????????????????????????????????(?????????????????????????????????????????????)*/var isModalEditing:boolean=false;
/**?????????????????????????????? */ export const filterSelectIds = (fileno: string): void => {
    selectids = selectids.filter(item => item != fileno);
};
/**??????Modal??????*/
const prop = (key: keyof SubjectListModel): string => {
    return route.GetProperty<SubjectListModel>(key);
};
const prop2 = (key: keyof FileListModel): string => {
    return route.GetProperty<FileListModel>(key);
};
/**???????????? */
const lightbox=(src:string)=>{
    const lightbox = document.createElement('div');
    const img = GetImage(src, '?????????', ['x-open']);
    lightbox.innerHTML = img;
    lightbox.className = 'x-lightbox';
    lightbox.onclick = function() {
        lightbox.parentNode.removeChild(lightbox);
    };
    document.body.appendChild(lightbox);
};
//==============???????????????==============================

/**?????????????????????????????????????????????Modal */ 
// $CusModal.hide();
//Modal????????????
$CusModal.hide().on('click', 'i.cancel', function() {
    $CusModal.modal('hide');
    typeof _videoPlayer !== 'undefined' ? _videoPlayer.Destory() : false;
    typeof _audioPlayer !== 'undefined' ? _audioPlayer.Destory() : false;
    selectids = []; //??????????????????
});
//??????????????????,
$('#UploadProgressBtn').click(function() {  $('#UploadContent').modal('show');});


/*--------------------------------------------
    ?????????
---------------------------------------------*/
const tree=$.customtree({
    selector:'.tree',
    searchword:'',
    subjcount:true,
    showhide:false,
    dragAndDrop:false
});

tree.nodeClick(function(event, node){
    const isUsingQueue = node.itree.a.attributes.usingqueue; 
    if ((isUsingQueue && node.itree.a.attributes.isqueue == YesNo.???) || !isUsingQueue) {
        /*??????Queue???*/
        $('#TipResult').hide();
        $('#ShowResult').show();
        $CreateSubjectBtn.attr('data-nodeId', node.id);
        /*???????????????????????????????????????*/
        SubjectController.GetDirFunctionAuth(MediaType.SUBJECT, Number(node.id)).then(json => {
            Logger.log('????????????:' + JSON.stringify(json));
            const data = json;
            const subjAuth = <Array<PermissionDefinition>>data.LimitSubject.split(',');
            subjAuth.indexOf(PermissionDefinition.I) > -1 ? $CreateSubjectBtn.show() : $CreateSubjectBtn.hide();
            SubjectList({ id: Number(node.id) }, data);
        });
    } else {
        $('#TipResult').show();
        $('#ShowResult').hide();
    }
    pathName=node.itree.a.attributes.title;
    // $CusModal.find("div[name='showpath']").html(node.itree.a.attributes.title);
});
/**???????????????????????? */
$('#ReloadTreeBtn').click(function() {
    // tree.reload();
    $SearchNode.val(StringEnum.Empty);
    tree.rebuild();
});
/**
 * ????????????
 * Notice:????????????????????????
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
                Logger.error(`???????????????????????????,api=${DirController.api.GetDir},??????:`, error);
                tree.removeAll(); //TODO ????????????
                return false;
            })
            .then(success => {
                tree.searchWord(word);
            });
    } else {
        tree.reload();
    }
});
/*-------------------------------------------------
    ??????
--------------------------------------------------*/

/**??????:???????????? */
$CreateSubjectBtn.click(function() {
    const queueId = Number($(this).attr('data-nodeId'));
    ShowModal<IdModel>(CreateSubjectModalId, route.api.ShowCreate, { id: queueId })
        .then(success => {
            if (success) {
                const $Title: JQuery<HTMLElement> = $(CreateSubjectModalId).find("input[name='Title']");
                const $Description: JQuery<HTMLElement> = $(CreateSubjectModalId).find("input[name='Description']");
                const $dynamicFields: JQuery<HTMLElement> = $(CreateSubjectModalId).find('.dynamicFields');
                $(".dropdown:not([name='SubjectPreId'])").dropdown();
                $(".dropdown[name='SubjectPreId']").dropdown({
                    fullTextSearch: 'exact',
                    match: 'text',
                    onChange: function(value, text, $selectedItem) {
                        //Notice:$selectedItem???semantic ui??????????????????item,option???????????????dataset???????????????
                        const optionValue = (type: 'title' | 'descr') => {
                            return $("select[name='SubjectPreId']")
                                .find("option[value='" + value + "']")
                                .attr('data-' + type + '');
                        };
                        if (Number(value) > 0) {
                            $dynamicFields.hide();
                            $Title.val(optionValue('title'));
                            $Description.val(optionValue('descr'));
                        } else {
                            $dynamicFields.show();
                            $Title.val('');
                            $Description.val('');
                        }
                    },
                });
                /*???????????????????????? */
                ModalTask(CreateSubjectModalId, true, {
                    onShow: function() {
                        setCalendar(`${CreateSubjectModalId} .calendar`, 'date');
                    },
                    onApprove: function() {
                        /*??????????????????????????????,??????????????????????????????????????????????????????,??????????????????????????????*/
                        const copyValid = Object.assign( {},valid.CreateSubject);
                        /**Notice:??????????????????,???????????????????????? */
                        const validObject = $dynamicFields.is(':hidden')
                            ? valid.CreateSubject
                            : AddDynamicNullable(CreateFormId, copyValid);
                        const isFormValid = CheckForm(CreateFormId, validObject);
                        if (isFormValid) {
                            route
                                .CreateSubject($(CreateFormId).serialize())
                                .then(res => {
                                    const data = <{ fsSUBJ_ID: string; fsTITLE: string }>res.Data;
                                    if (res.IsSuccess) {
                                        $(CreateFormId).trigger('reset');
                                        $(CreateSubjectModalId).modal('hide');
                                        SuccessMessage(res.Message);
                                        table.AddRow(<SubjectListModel>{
                                            fsSUBJECT_ID: data.fsSUBJ_ID,
                                            fsSUBJECT_TITLE: data.fsTITLE,
                                            VideoCount: 0,
                                            AudioCount: 0,
                                            PhotoCount: 0,
                                            DocCount: 0,
                                        });
                                    } else {
                                        ErrorMessage(res.Message);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.Create, '????????????', error, true);
                                });
                        }
                        return false;
                    },
                });
            } else {
                ErrorMessage(`????????????????????????????????????`);
            }
        })
        .catch(error => {
            Logger.viewres(route.api.ShowCreate, '????????????????????????', error, true);
        });
});
/**
 * ????????????
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('fsSUBJECT_ID'), type: Filter.Like, value: word },
        { field: prop('fsSUBJECT_TITLE'), type: Filter.Like, value: word },
        { field: prop('VideoCount'), type: Filter.Like, value: word },
        { field: prop('AudioCount'), type: Filter.Like, value: word },
        { field: prop('PhotoCount'), type: Filter.Like, value: word },
        { field: prop('DocCount'), type: Filter.Like, value: word },
    ];
    table.SetFilter(filter);
});
/**?????????????????? */
function SubjectList(SearchParams: SubjectSearchModel, DirAuth: UserFileSubjectAuthModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: '90%',
        layout: 'fitColumns',
        addRowPos: 'top',
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('fsSUBJECT_ID'),
        columns: [
            { title: '????????????', field: prop('fsSUBJECT_ID'), width: 125, sorter: 'string' },
            { title: '??????', field: prop('fsSUBJECT_TITLE'), minWidth: 145, sorter: 'string' },
            {
                title: '???',
                field: prop('VideoCount'),
                sorter: 'number',
                width: 75,
                formatter: function(cell, formatterParams) {
                    const count: number = cell.getValue();
                    return count == 0 ? `<span>${count}</span>` : `<a class="link">${count}</a>`;
                },
                cellClick: function(e, cell) {
                    const rowdata = <SubjectListModel>cell.getRow().getData();
                    if (rowdata.VideoCount > 0) {
                        const type = MediaType.VIDEO;
                        SubjectController.GetDirFunctionAuth(type, SearchParams.id).then(json => {
                            Logger.log('?????????:' + JSON.stringify(json));
                            const data = json;
                            const vAuth = <Array<PermissionDefinition>>data.LimitVideo.split(','); //const vAuth = <Array<PermissionDefinition>>data.LimitAuth.split(',');
                            const subjectId = rowdata.fsSUBJECT_ID;
                            const preview = route.Preview({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' }, vAuth);
                            const listres = route.List({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' });
                            const mediaview = route.MediaView(
                                { fsSUBJECT_ID: subjectId, type: type, fileNo: '' },
                                vAuth
                            );
                            (async () => {
                                $('#LoadingModel').show();
                                await Promise.all([preview, listres, mediaview])
                                    .then(data => {
                                        $Preview.empty().append(data[0]);
                                        FileList(
                                            subjectId,
                                            rowdata.fsSUBJECT_TITLE,
                                            type,
                                            <Array<FileListModel>>data[1].Data,
                                            vAuth
                                        );
                                        $DataView.empty().append(`
                                        <div class="ui top attached tabular menu tabs">
                                           <a class="active item" data-tab="${TabNameEnum.MetaData}">????????????</a>
                                           <a class="item" data-tab="${TabNameEnum.KeyFrame}">????????????(<span>0</span>)</a>
                                           <a class="item" data-tab="${TabNameEnum.Paragraph}">????????????(<span>0</span>)</a>
                                           ${WhiteCloseIcon()}
                                        </div>
                                        <div class="pathlabel" name="showpath">${pathName}</div>
                                        <div class="ui bottom attached active tab" data-tab="${TabNameEnum.MetaData}">${data[2]}</div>
                                        <div class="ui bottom attached tab" data-tab="${TabNameEnum.KeyFrame}"></div>
                                        <div class="ui bottom attached tab" data-tab="${TabNameEnum.Paragraph}"></div>
                                        `);
                                    })
                                    .then(() => {
                                        ModalTask(CusModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $('#LoadingModel').hide();
                                                $CusModal.find('.tabs .item').tab();
                                                _videoPlayer = new videoPlayer(
                                                    "div[name='video']",
                                                    '#videoMenu',
                                                    '#fullScreenContainer'
                                                );

                                                // Resize();
                                            },
                                        });
                                    });
                            })();
                        });
                    }
                },
            },
            {
                title: '???',
                field: prop('AudioCount'),
                sorter: 'number',
                width: 75,
                formatter: function(cell, formatterParams) {
                    const count: number = cell.getValue();
                    return count == 0 ? `<span>${count}</span>` : `<a class="link">${count}</a>`;
                },
                cellClick: function(e, cell) {
                    const rowdata = <SubjectListModel>cell.getRow().getData();
                    if (rowdata.AudioCount > 0) {
                        const type = MediaType.AUDIO;
                        SubjectController.GetDirFunctionAuth(type, SearchParams.id).then(json => {
                            Logger.log('?????????:' + JSON.stringify(json));
                            const data = json;
                            const aAuth = <Array<PermissionDefinition>>data.LimitAudio.split(','); //const aAuth = <Array<PermissionDefinition>>data.LimitAuth.split(',');
                            const subjectId = rowdata.fsSUBJECT_ID;
                            const preview = route.Preview({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' }, aAuth);
                            const listres = route.List({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' });
                            const mediaview = route.MediaView(
                                { fsSUBJECT_ID: subjectId, type: type, fileNo: '' },
                                aAuth
                            );
                            (async () => {
                                $('#LoadingModel').show();
                                await Promise.all([preview, listres, mediaview])
                                    .then(data => {
                                        $Preview.empty().append(data[0]);
                                        FileList(
                                            subjectId,
                                            rowdata.fsSUBJECT_TITLE,
                                            type,
                                            <Array<FileListModel>>data[1].Data,
                                            aAuth
                                        );
                                        $DataView.empty()
                                        .append(`<div class="ui top attached tabular menu tabs">
                                                     <a class="active item" data-tab="${TabNameEnum.MetaData}">????????????</a>
                                                     <a class="item" data-tab="${ TabNameEnum.Paragraph}">????????????(<span>0</span>)</a>
                                                    ${WhiteCloseIcon()}
                                                 </div> 
                                                  <div class="pathlabel" name="showpath">${pathName}</div>
                                                 <div class="ui bottom attached active tab" data-tab="${TabNameEnum.MetaData}">${data[2]}</div>
                                                  <div class="ui bottom attached tab" data-tab="${ TabNameEnum.Paragraph}"></div>`);
                                    })
                                    .then(() => {
                                        ModalTask(CusModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $('#LoadingModel').hide();
                                                $CusModal.find('.tabs .item').tab();
                                                _audioPlayer = new audioPlayer("div[name='audio']");

                                                // Resize();
                                            },
                                        });
                                    });
                            })();
                        });
                    }
                },
            },
            {
                title: '???',
                field: prop('PhotoCount'),
                sorter: 'number',
                width: 75,
                formatter: function(cell, formatterParams) {
                    const count: number = cell.getValue();
                    return count == 0 ? `<span>${count}</span>` : `<a class="link">${count}</a>`;
                },
                cellClick: function(e, cell) {
                    const rowdata = <SubjectListModel>cell.getRow().getData();
                    if (rowdata.PhotoCount > 0) {
                        const type = MediaType.PHOTO;
                        SubjectController.GetDirFunctionAuth(type, SearchParams.id).then(json => {
                            Logger.log('?????????:' + JSON.stringify(json));
                            const data = json;
                            const pAuth = <Array<PermissionDefinition>>data.LimitPhoto.split(','); //const pAuth = <Array<PermissionDefinition>>data.LimitAuth.split(',');
                            const subjectId = rowdata.fsSUBJECT_ID;
                            const preview = route.Preview({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' }, pAuth);
                            const listres = route.List({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' });
                            const mediaview = route.MediaView(
                                { fsSUBJECT_ID: subjectId, type: type, fileNo: '' },
                                pAuth
                            );
                            (async () => {
                                $('#LoadingModel').show();
                                await Promise.all([preview, listres, mediaview])
                                    .then(data => {
                                        $Preview.empty().append(data[0]);
                                        FileList(
                                            subjectId,
                                            rowdata.fsSUBJECT_TITLE,
                                            type,
                                            <Array<FileListModel>>data[1].Data,
                                            pAuth
                                        );
                                        $DataView.empty()
                                                .append(`<div class="ui top attached tabular menu tabs">
                                                            <a class="active item" data-tab="${ TabNameEnum.MetaData}">????????????</a>
                                                            ${WhiteCloseIcon()}
                                                         </div>
                                                         <div class="pathlabel" name="showpath">${pathName}</div>
                                                         <div class="ui bottom attached active tab" data-tab="${
                                                             TabNameEnum.MetaData
                                                         }">${data[2]}</div>`);
                                    })
                                    .then(() => {
                                        ModalTask(CusModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $('#LoadingModel').hide();
                                                $CusModal.find('.tabs .item').tab();

                                                //  Resize();
                                            },
                                        });
                                    });
                            })();
                        });
                    }
                },
            },
            {
                title: '???',
                field: prop('DocCount'),
                sorter: 'number',
                width: 75,
                formatter: function(cell, formatterParams) {
                    const count: number = cell.getValue();
                    return count == 0 ? `<span>${count}</span>` : `<a class="link">${count}</a>`;
                },
                cellClick: function(e, cell) {
                    const rowdata = <SubjectListModel>cell.getRow().getData();
                    if (rowdata.DocCount > 0) {
                        const type = MediaType.Doc;
                        SubjectController.GetDirFunctionAuth(type, SearchParams.id).then(json => {
                            Logger.log('?????????:' + JSON.stringify(json));
                            const data = json;
                            const dAuth = <Array<PermissionDefinition>>data.LimitDoc.split(','); //const dAuth = <Array<PermissionDefinition>>data.LimitAuth.split(',');
                            const subjectId = rowdata.fsSUBJECT_ID;
                            const preview = route.Preview({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' }, dAuth);
                            const listres = route.List({ fsSUBJECT_ID: subjectId, type: type, fileNo: '' });
                            const mediaview = route.MediaView(
                                { fsSUBJECT_ID: subjectId, type: type, fileNo: '' },
                                dAuth
                            );
                            (async () => {
                                $('#LoadingModel').show();
                                await Promise.all([preview, listres, mediaview])
                                    .then(data => {
                                        $Preview.empty().append(data[0]);
                                        FileList(
                                            subjectId,
                                            rowdata.fsSUBJECT_TITLE,
                                            type,
                                            <Array<FileListModel>>data[1].Data,
                                            dAuth
                                        );
                                        $DataView.empty()
                                                   .append(`<div class="ui top attached tabular menu tabs">
                                                        <a class="active item" data-tab="${TabNameEnum.MetaData}">????????????</a>
                                                        ${WhiteCloseIcon()}
                                                     </div>
                                                     <div class="pathlabel" name="showpath">${pathName}</div>
                                                     <div class="ui bottom attached active tab" data-tab="${
                                                         TabNameEnum.MetaData
                                                     }">${data[2]}</div>`);
                                    })
                                    .then(() => {
                                        ModalTask(CusModalId, true, {
                                            closable: false,
                                            allowMultiple: true,
                                            onShow: function() {
                                                $('#LoadingModel').hide();
                                                $CusModal.find('.tabs .item').tab();

                                                // Resize();
                                            },
                                        });
                                    });
                            })();
                        });
                    }
                },
            },
            {
                title: '??????',
                field: prop('fsSUBJECT_ID'),
                hozAlign: 'left',
                width: 200,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const subjectid: string = cell.getValue();
                    const subjAuth = <Array<PermissionDefinition>>DirAuth.LimitSubject.split(',');
                    const detailbtn =
                        subjAuth.indexOf(PermissionDefinition.V) > -1
                            ? DetailButton(subjectid, message.Controller)
                            : '';
                    const editbtn =
                        subjAuth.indexOf(PermissionDefinition.U) > -1 ? EditButton(subjectid, message.Controller) : '';
                    const deletebtn =
                        subjAuth.indexOf(PermissionDefinition.D) > -1
                            ? DeleteButton(subjectid, message.Controller)
                            : '';
                    const _vAuth = <Array<PermissionDefinition>>DirAuth.LimitVideo.split(',');
                    const _aAuth = <Array<PermissionDefinition>>DirAuth.LimitAudio.split(',');
                    const _pAuth = <Array<PermissionDefinition>>DirAuth.LimitPhoto.split(',');
                    const _dAuth = <Array<PermissionDefinition>>DirAuth.LimitDoc.split(',');
                    const uploadbtn =
                        _vAuth.indexOf(PermissionDefinition.I) > -1 ||
                        _aAuth.indexOf(PermissionDefinition.I) > -1 ||
                        _pAuth.indexOf(PermissionDefinition.I) > -1 ||
                        _dAuth.indexOf(PermissionDefinition.I) > -1
                            ? UploadButton(subjectid, message.Controller)
                            : '';
                    //DirAuth.indexOf(PermissionDefinition.I) > -1 ? UploadButton(subjectid, message.Controller) : '';
                    const btngroups: string = editbtn + detailbtn + deletebtn + uploadbtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const id: string = cell.getValue();
                    const rowdata = <SubjectListModel>cell.getRow().getData();
                    switch (true) {
                        /*????????????*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(Controller.Subject, Action.ShowDetails, { id: id }, { dropdown: true });
                            break;
                        /*????????????*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: id })
                                .then(IsSuccess => {
                                    if (IsSuccess) {
                                        ModalTask(EditModalId, true, {
                                            closable: false,
                                            onShow: function() {
                                                $(EditFormId)
                                                    .find('.dropdown')
                                                    .dropdown();
                                                setCalendar(`${EditModalId} .calendar`, 'date');
                                            },
                                            onApprove: function() {
                                                const validObject = AddDynamicNullable(EditFormId, valid.EditSubject);
                                                const isFormValid = CheckForm(EditFormId, validObject);
                                                if (isFormValid) {
                                                    route
                                                        .EditSubject($(EditFormId).serialize())
                                                        .then(res => {
                                                            const data = <{ fsSUBJ_ID: string; fsTITLE: string }>(
                                                                res.Data
                                                            );
                                                            if (res.IsSuccess) {
                                                                SuccessMessage(res.Message);
                                                                $(EditModalId).modal('hide');
                                                                table.ReactivityUpdate(id, {
                                                                    fsSUBJECT_ID: data.fsSUBJ_ID,
                                                                    fsSUBJECT_TITLE: data.fsTITLE,
                                                                });
                                                            } else {
                                                                ErrorMessage(res.Message);
                                                            }
                                                        })
                                                        .catch(error => {
                                                            Logger.viewres(route.api.Edit, '????????????', error, true);
                                                        });
                                                }
                                                return false;
                                            },
                                        });
                                    } else {
                                        ErrorMessage('??????????????????,??????????????????????????????');
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.ShowEdit, '????????????????????????', error, true);
                                });
                            break;
                        /*????????????*/
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            const rowElement = table.GetRow(id);
                            ShowModal(DeleteModalId,route.api.ShowDelete,{ subjid: id }).then(success=>{
                                if(success){
                                      ModalTask(DeleteModalId,true,{
                                          closable:false,
                                          onApprove:function(){
                                            route
                                            .DeleteSubject({ fsSUBJECT_ID: id })
                                            .then(res => {
                                                if (res.IsSuccess) {
                                                    SuccessMessage(res.Message);
                                                    table.RemoveRow(id);
                                                } else {
                                                    ErrorMessage(res.Message);
                                                }
                                            })
                                            .catch(error => {
                                                Logger.viewres(route.api.Delete, '????????????', error, true);
                                            });
                                          }
                                      });
                                }else{
                                    Logger.viewres(route.api.ShowDelete, '??????????????????', '', true);
                                }
                            }).catch(error=>{
                                Logger.viewres(route.api.ShowDelete, '??????????????????', error, true);
                            });
                            break;
                        /**?????????????????? */
                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('upload icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'upload':
                            ShowModal<IdModel>(UploadModalId, route.api.ShowUpload, { id: SearchParams.id })
                                .then(view => {
                                    /**
                                     * MediaFileExtension ???????????????????????????????????????
                                     * AllowMediaFormat ???????????????????????????????????????Array
                                     * UploadConfig ????????????????????????
                                     * ArcPreTempDropdown ????????????????????????
                                     * MediaTypDropdown ??????????????????
                                     * TitleTypeDropdown ??????????????????
                                     * titleInput ????????????Input
                                     * fileList ????????????
                                     * fileInput ????????????input[type='file']
                                     */
                                    const SaveJson = $('#SaveJson').attr('data-MediaFileExtension');
                                    const ConfigJson = $('#ConfigJson').attr('data-UploadConfig');
                                    const TempleteJson = $('#DirTemplate').attr('data-DirTemplate');
                                    const AllowMediaFormat: Array<AcceptExtension> = JSON.parse(SaveJson);
                                    const UploadConfig: SubjectUploadConfig = JSON.parse(ConfigJson);
                                    const DirTemplate: Array<DirTempleteModel> = JSON.parse(TempleteJson);
                                    const $UploadModal: JQuery<HTMLElement> = $(UploadModalId);
                                    const ArcPreTempSelect = GetSelect(UploadModalId, 'ArcPreTempList');
                                    const MediaTypDropdown = GetDropdown(UploadModalId, 'MediaTypeList');
                                    const TitleTypeDropdown = GetDropdown(UploadModalId, 'titletype');
                                    ['ArcPreTempList','FileSecretList','FileLicenseList'].forEach(name=>{
                                        GetSelect(UploadModalId, name).dropdown();
                                    });
                                    let titleInput: JQuery<HTMLElement> = $UploadModal.find("input[name='custitle']");
                                    const fileList = $("div[name='fileList']");
                                    const fileInput = $("input[name='mediafile']");
                                    MediaTypDropdown.dropdown({
                                        fullTextSearch: 'exact',
                                        match: 'text',
                                        onChange: function(value, text, $selectedItem) {
                                            const mediaType = GetMediaTypeByUploadType(value);
                                            const searchType = GetMediaSearchType(mediaType);
                                            $("#MediaTypeList").trigger("clearfile");
                                            const templeteId =
                                                DirTemplate.filter(item => item.SearchType === searchType)[0]
                                                    .fnTEMP_ID || 0;
                                            Logger.log(`??????????????????Id=${templeteId}`);
                                            route
                                                .GetArcPreList(mediaType, templeteId)
                                                .then(data => {
                                                    const options= (<Array<SelectListItem>>data).map(x=>{return CreateOption(x);});
                                                    // const SelectListItem = <Array<SelectListItem>>data;
                                                    // const options = (() => {
                                                    //     let items = '';
                                                    //     for (let i = 0; i < SelectListItem.length; i++) {
                                                    //         items += `<option value="${SelectListItem[i].Value}">${SelectListItem[i].Text}</option>`;
                                                    //     }
                                                    //     return items;
                                                    // })();
                                                    ArcPreTempSelect.empty()
                                                        .append(options)
                                                        .closest('dropdown')
                                                        .dropdown('refresh');
                                                })
                                                .catch(error => {
                                                    ArcPreTempSelect.empty()
                                                        .empty()
                                                        .append(CreateOption("none"))
                                                        .closest('dropdown')
                                                        .dropdown('refresh');
                                                });
                                            fileList.empty();
                                            const acceptTypeArray = AllowMediaFormat.find(item => item.MediaType === value).FileExtension.split(';')
                                                                  .map(x=>{return '.'+x;});
                                            // acceptTypeArray.forEach(function(element, index) {
                                            //     if (element.length > 0) {
                                            //         acceptTypeArray[index] = '.' + element;
                                            //     }
                                            // });
                                            const acceptType = acceptTypeArray .filter(el=>el) .join(',');
                                            fileInput.attr('accept', acceptType);
                                            $("div[name='fileList']").empty();
                                        },
                                    });
                                    TitleTypeDropdown.dropdown({
                                        fullTextSearch: 'exact',
                                        match: 'text',
                                        onChange: function(value, text, $selectedItem) {
                                            const noneArcPreItem = ArcPreTempSelect.closest('.dropdown').dropdown(
                                                'get item',
                                                '0'
                                            );
                                            const otherItem = noneArcPreItem.siblings().first();
                                            switch (Number(value)) {
                                                case TitleEnum.CusTitle:
                                                    titleInput.removeAttr('disabled');
                                                    noneArcPreItem.removeClass('disabled');
                                                    break;
                                                case TitleEnum.ArcPreTitle:
                                                    titleInput.attr('disabled', 'disabled');
                                                    noneArcPreItem.addClass('disabled');
                                                    ArcPreTempSelect.closest('.dropdown')
                                                        .dropdown('set selected', otherItem.attr('data-value'))
                                                        .dropdown('refresh');
                                                    break;
                                                default:
                                                    titleInput.attr('disabled', 'disabled');
                                                    noneArcPreItem.removeClass('disabled');

                                                    break;
                                            }
                                        },
                                    });
                                    /*???????????????????????????,????????????????????????????????????????????????,???Upload.ts> ConfirmUpload */
                                    UploadTask(rowdata.fsSUBJECT_TITLE, {
                                        config: {
                                            uploadPath: UploadConfig.TargetUrl,
                                            chuckSize: UploadConfig.UploadFileBuffer,
                                            AcceptExtension: AllowMediaFormat,
                                            simultaneousUploads: UploadConfig.SimultaneousUploads,
                                            TimeoutSec: UploadConfig.TimeoutSec,
                                        },
                                        query: {
                                            SubjId: id,
                                            LoginId: UploadConfig.LoginId,
                                            FileNo: '',
                                            Folder: UploadConfig.TempFolder,
                                            PreId: 0,
                                            DeleteKF: null,
                                            FileSecret: 0, //Added_20200302:???????????????
                                            DateInFileNo:''
                                        },
                                    });
                                })
                                .catch(error => {
                                    Logger.error(`?????????????????????_upload????????????,??????:`, error);
                                });
                            break;
                        default:
                            break;
                    }
                },
            },
        ],
    });
}
/**???????????????????????? */
$(initSetting.TableId).on('addcount',function(event,subjectId:string,mediatype:MediaUploadType|MediaType,count?:number){
    const rowdata= <SubjectListModel>table.GetRowData(subjectId);
    const countNo=IsNullorUndefined(count)||IsNULLorEmpty(count)?1:count;
    switch(mediatype){
        case MediaUploadType.VIDEO:
            /*Notice:?????????????????????????????????????????????????????????,??????????????????????????????1??????????????????????????????,????????????*/
           // table.ReactivityUpdate(subjectId, <SubjectListModel>{VideoCount: rowdata.VideoCount + 1 });
            break;
        case MediaType.VIDEO:
            table.ReactivityUpdate(subjectId, <SubjectListModel>{VideoCount: rowdata.VideoCount + countNo });
            break;
        case MediaUploadType.AUDIO:
        case MediaType.AUDIO:
            table.ReactivityUpdate(subjectId, <SubjectListModel>{AudioCount:rowdata.AudioCount+countNo});
            break;
        case MediaUploadType.PHOTO:
        case MediaType.PHOTO:
            table.ReactivityUpdate(subjectId, <SubjectListModel>{ PhotoCount: rowdata.PhotoCount + countNo });
            break;
        case MediaUploadType.DOC:
        case MediaType.Doc:
            table.ReactivityUpdate(subjectId, <SubjectListModel>{ DocCount: rowdata.DocCount + countNo });
            break;
    }
});

/**ListView?????? */
export const FileList = (
    subjectId: string,
    subjectName: string,
    mediatype: MediaType,
    Data: Array<FileListModel & {SelectStatus?:boolean}>,
    DirAuth: Array<PermissionDefinition>
) => {
    const type = mediatype;
    let data=Data;
   data.forEach(item=>{item.SelectStatus=false;});
   subtable = new tabulatorService(
       '.subtable',
       {
           height: TabulatorSetting.height,
           layout: TabulatorSetting.layout,
           selectable: 1,
           addRowPos: 'top',
           data: data,
           paginationSize: 30 /*????????????*/,
           headerVisible: false /*???????????????*/,
           //headerVisible: true,
           index: prop2('fsFILE_NO'),
           rowContextMenu: [
               {
                   label: "<i class='mouse pointer icon'></i> ????????????",
                   action: function(e, row) {
                       const currentPage=row.getTable().getPage();
                       const pageSize=row.getTable().getPageSize();
                       if( currentPage!==false){
                           const startIndex=(currentPage-1)*pageSize;
                           const endIndex=currentPage*pageSize-1>row.getTable().getDataCount()?row.getTable().getDataCount():currentPage*pageSize-1;
                           for(let x=startIndex;x<=endIndex;x++){
                              const _row_= row.getTable().getRows()[x];
                              if(!IsNullorUndefined(_row_)){
                                 _row_.update(<FileListModel & {SelectStatus?:boolean}>{SelectStatus:true});
                                  _row_.select();
                                if(selectids.indexOf(_row_.getIndex())===-1){ selectids.push(_row_.getIndex());}
                              }
                           }
                           rowClickFunction(subjectId,type,row,DirAuth);
                       }
                    
                   },
               },
               {
                   label: "<i class='window close icon'></i> ????????????",
                   action: function(e, row) {
                       const currentPage=row.getTable().getPage();
                       const pageSize=row.getTable().getPageSize();
                       if( currentPage!==false){
                           const startIndex=(currentPage-1)*pageSize;
                           const endIndex=currentPage*pageSize-1>row.getTable().getDataCount()?row.getTable().getDataCount():currentPage*pageSize-1;
                           $('input[name="selectall"]').parent().checkbox('set unchecked');
                           for(let x=startIndex;x<=endIndex;x++){
                               const _row_= row.getTable().getRows()[x];
                               if(!IsNullorUndefined(_row_)){
                                   _row_.update(<FileListModel & {SelectStatus?:boolean}>{SelectStatus:false});
                                   _row_.deselect();
                                   selectids=selectids.filter(id=>id !==_row_.getIndex());
                               }
                           } 
                           rowClickFunction(subjectId,type,row,DirAuth);
                       }
                       
                   },
               },
               {
                   label: "<i class='mouse pointer icon'></i> ????????????",
                   action: function(e, row) {      
                       const newData=  (<Array<FileListModel & {SelectStatus?:boolean}>>row.getTable().getData()).map(item=>{
                           if(selectids.indexOf(item.fsFILE_NO)===-1){ selectids.push(item.fsFILE_NO); }
                           item.SelectStatus=true;
                           return item;
                        });
                        row.getTable().selectRow();
                        row.getTable().updateData(newData);
                        rowClickFunction(subjectId,type,row,DirAuth);
                        
                   },
               },
               {
                   label: "<i class='window close icon'></i> ????????????",
                   action: function(e, row) {
                      selectids=[];
                      const newData=  (<Array<FileListModel & {SelectStatus?:boolean}>>row.getTable().getData()).map(item=>{
                       item.SelectStatus=false;
                       return item;
                      });
                        row.getTable().deselectRow();
                        row.getTable().updateData(newData);
                        rowClickFunction(subjectId,type,row,DirAuth);
                       
                   },
               }
           ],
           rowFormatter: function(row: Tabulator.RowComponent) {
               const rowdata = <FileListModel>row.getData();
               const fileNo: string = rowdata.fsFILE_NO;
               row.getElement().setAttribute('title', rowdata.Title);
               //?????????????????????????????????,????????????????????????
               if (row.getPosition() == 0 && row !== LastSelectRow) {
                   row.select();
                   LastSelectRow = row; //??????????????????
                   // row.getElement().classList.add('tabulator-cusselected');
                   if (type == MediaType.VIDEO) {
                       // UpdateTabCount(TabNameEnum.KeyFrame, 'change', Number(rowdata.KeyFrameCount));
                       $('.subtable').trigger('updateTabCount',[fileNo,TabNameEnum.KeyFrame, Number(rowdata.KeyFrameCount)]);
                       const $KTab = $CusModal.find(`div[data-tab='${TabNameEnum.KeyFrame}']`);
                       route.KeyFramePlaceholderView(fileNo, rowdata.KeyFrameCount, DirAuth).then(placeholder => {
                           $KTab.empty().append(placeholder);
                           setTimeout(() => {
                               route.KeyFrameView({ type: type, fileNo: fileNo }, DirAuth);

                               KeyFrame(
                                   route,
                                   subtable,
                                   CusModalId,
                                   { fileNo: fileNo, type: type, SubjectId: subjectId },
                                   _videoPlayer,
                                   DirAuth
                               );
                           }, 500);
                       });
                       // _videoPlayer.Load(fileUrl, posterUrl);
                   }
                   if (type == MediaType.VIDEO || type == MediaType.AUDIO) {
                       const PTab = $CusModal.find(`div[data-tab='${TabNameEnum.Paragraph}']`);
                       $('.subtable').trigger('updateTabCount',[fileNo,TabNameEnum.Paragraph, Number(rowdata.SegmentCount)]);
                       // UpdateTabCount(TabNameEnum.Paragraph, 'change', Number(rowdata.SegmentCount));
                       route
                           .ParagraphDesView({ fsSUBJECT_ID: subjectId, type: type, fileNo: fileNo }, DirAuth)
                           .then(view => {
                               PTab.html(view);
                           })
                           .then(() => {
                               const _player = type == MediaType.VIDEO ? _videoPlayer : _audioPlayer;
                               _player.Load(rowdata.FileUrl, rowdata.ImageUrl);
                               Paragraph(route, CusModalId, type, _player);
                           });
                       /*----------????????????????????????-----------------------*/
                       const $NUllPreview = $Preview.find("div[name='NuLLPreview']");
                       $NUllPreview.length > 0 ? $NUllPreview.remove() : false;
                       const divType = type == MediaType.VIDEO ? 'video' : 'audio';
                       if (IsNULLorEmpty(rowdata.FileUrl)) {
                           IsImageValid(rowdata.ImageUrl).then(IsOK => {
                               const okImageURL = IsOK ? rowdata.ImageUrl : GetImageUrl(AirmamImage.NoImage).href;
                               const imageDiv = GetImage(okImageURL, rowdata.Title, [
                                   'ui',
                                   'image',
                                   'center',
                                   'aligned',
                               ]);
                               $Preview.find(`div[name="${divType}"]`).hide();
                               $Preview.prepend(`<div class='ui basic segment' name='NuLLPreview'>${imageDiv}</div>`);
                           });
                       } else {
                           $Preview.find(`div[name="${divType}"]`).show();
                           const _player = type == MediaType.VIDEO ? _videoPlayer : _audioPlayer;
                           _player.Load(rowdata.FileUrl, rowdata.ImageUrl);
                       }
                   }
                   MetaData(route, CusModalId, subjectId, type, subjectName, DirAuth);
               }
               return row;
           },
           rowClick: function(e, row: Tabulator.RowComponent) {
               const target=e.target;
               const rowdata = <FileListModel>row.getData();
               selectRowData=rowdata;
               if (row !== LastSelectRow) {
                   rowClickFunction(subjectId,type,row,DirAuth);

               }
               const noImage = GetImageUrl(AirmamImage.NoImage).href;
                   if(target instanceof HTMLImageElement){
                       IsImageValid(rowdata.ImageUrl).then(isok=>{
                           isok?lightbox(rowdata.ImageUrl):lightbox(noImage);
                       });
                   }
           },
           columns: [
               {
                   title: '??????',
                   field: 'SelectStatus',
                   width:50,
                   sorter: 'number',
                   hozAlign: 'center',
                   headerSort: false,
                   cellClick: function(e, cell) {
                       const target: HTMLLabelElement | HTMLInputElement | HTMLDivElement | HTMLElement = <any>( e.target);
                       const row = cell.getRow();
                       const id =(<FileListModel>row.getData()).fsFILE_NO;
                       const ischeck:boolean =(<FileListModel & {SelectStatus?:boolean}>row.getData()).SelectStatus===true;
                       if (target instanceof HTMLLabelElement || target instanceof HTMLInputElement) {
                           if (ischeck) {
                               $('input[name="selectall"]').parent().checkbox('set unchecked');
                               row.update(<{SelectStatus?:boolean}>{SelectStatus:false});
                               selectids = selectids.filter(item => item != id);
                           } else {
                               row.update(< {SelectStatus?:boolean}>{SelectStatus:true});
                               if (selectids.indexOf(id) <= -1) {  selectids.push(id); }
                           }
                           rowClickFunction(subjectId,type,row,DirAuth);
                       }
                   },
                   formatter: function(cell, formatterParams) {
                       const row = cell.getRow();
                       const id = (<FileListModel>row.getData()).fsFILE_NO;
                       const isCheck=cell.getValue()===true?'checked="checked"':'';
                       const checkbox = `<div class="ui checkbox" data-Id="${id}"><input type="checkbox" name="reconvert" ${isCheck}> <label></label></div>`;
                       return checkbox;
                   },
               },
               {
                   title: '????????????',
                   field: prop2('Title'),//NOTICE:?????????????????????????????????,???????????????????????????
                   sorter: 'string',
                   resizable: false,
                   formatter: function(cell, formatterParams, onRendered) {
                       const row = cell.getRow();
                       const rowdata = <FileListModel>row.getData();
                       const itemdiv: HTMLElement = document.createElement('div');
                       const workStatusStr =
                           rowdata.fsSTATUS == WorkStatus.InTransition
                               ? '<span class="ui label"><i class="notched circle loading blue icon"></i>?????????</span>'
                               : '';
                       itemdiv.className = 'ui items';
                       itemdiv.innerHTML = `<div class="item">
                                            <a class="ui small image">${GetImage(rowdata.ImageUrl, rowdata.Title)}</a>
                                            <div class="content">
                                              <a class="header">${rowdata.Title}</a>
                                              <div class="meta">${rowdata.fsFILE_NO}${workStatusStr}</div>
                                            </div>
                                           </div>`;
                       return itemdiv.outerHTML;
                   },
               },
           ],
       },
       false
   );
};
$('.subtable')
.on('render',function(event,subjectId:string,type:string,fileno:string){  
    //?????????????????????
    route.List({ fsSUBJECT_ID: subjectId, type: type, fileNo: fileno }).then(res=>{
        if(res.IsSuccess){
            let rowData=(<Array<FileListModel & {SelectStatus?:boolean}>>res.Data).find(item=>item.fsFILE_NO===fileno);
            rowData.SelectStatus=true;
            subtable.ReactivityUpdate(fileno,rowData);
        }
    });
})
.on('updateTabCount',function(event,fileno:string,tab:TabNameEnum,count:'-1'|'+1'|number){
    //?????????????????????????????????????????????????????????????????????
    let q=(<FileListModel & {SelectStatus?:boolean}>subtable.GetRowData(fileno)).SegmentCount;
    switch(count){
        case undefined:
        case null:
            break;
        case '+1':
            q++;
            break;
        case '-1':
            q--;
            break;
        default:
            q=count;
            break;
    }
    if(tab===TabNameEnum.KeyFrame){
        subtable.ReactivityUpdate(fileno,<FileListModel & {SelectStatus?:boolean}>{  KeyFrameCount:q });
    }
    if(tab===TabNameEnum.Paragraph){
        subtable.ReactivityUpdate(fileno,<FileListModel & {SelectStatus?:boolean}>{ SegmentCount:q});
    }
    $(CusModalId).find("a.item[data-tab='" + tab + "']> span").html(q.toString());
})
.on('nextclick',function(event,fileno){
    //????????????????????????,??????????????????rightComponet??????,?????????????????????,???????????????????????????????????????
    const row = subtable.GetTable().getRow(fileno);
    const prevRow = row.getPrevRow();
    const nextRow = row.getNextRow();
    const beUpdatedRow = prevRow !== false ? prevRow : nextRow;
    if (beUpdatedRow !== false) {
        beUpdatedRow.getElement().click();
        subtable.RemoveRow(row);
    } else {
        $CusModal.modal('hide');
    }
})
.on('editing',function(event,isEditing){
   //????????????????????????????????????[???????????????????????????????????????]??????????????????????????????
   isModalEditing= isEditing;
})
.on('selectCallback',function(event,fileno:string,callback?:Function){
   if(!IsNullorUndefined(callback) && !IsNullorUndefined(selectRowData)){
       callback(selectRowData);
   }
});
/**ListView???Row??????????????? */
const rowClickFunction=( subjectId: string,type:MediaType,row: Tabulator.RowComponent,DirAuth: Array<PermissionDefinition>)=>{
    if(isModalEditing){
         ModalTask(ChangeRowConfirmModalId,true,{
             allowMultiple: true,
             selector: {
                 close: '.actions .ok,.actions .cancel,i.close',
                 deny: '.actions .cancel',
             },
             closable: false,
             detachable: true /*???????????????false???????????????????????????????????? */,
             observeChanges: true,
             context: '#right-component',
             onShow: function(this: JQuery<HTMLElement>) {
                 /*????????????Modal???Modal????????????????????????*/
                 $(this).on('click', function(event) {
                     event.stopImmediatePropagation();
                 });
             },
             onApprove:function(){
                 isModalEditing=false;
                 rowChangeRender(subjectId,type,row,DirAuth);
             },
             onDeny:function(){
                 $(ChangeRowConfirmModalId).modal('hide');
                 return false;
             }
         });
    }else{
        rowChangeRender(subjectId,type,row,DirAuth);
    }
  
};
/**ListView???Row????????????????????? */
const rowChangeRender=( subjectId: string,type:MediaType,row: Tabulator.RowComponent,DirAuth: Array<PermissionDefinition>)=>{
      /*??????????????????????????????,??????????????????????????????????????? */
      $('#right-component').find('.modal').modal('hide');
      const rowdata= <FileListModel & {SelectStatus?:boolean}>row.getData();
      const fileNo: string =rowdata.fsFILE_NO;
      row.select();
      //?????????????????????????????????,????????????????????????
      LastSelectRow = row; //??????????????????
      /*--------?????????Video??????????????????-----------*/
      if (type == MediaType.VIDEO) {
          $('.subtable').trigger('updateTabCount',[fileNo,TabNameEnum.KeyFrame, Number(rowdata.KeyFrameCount)]);
          const $KTab = $CusModal.find(`div[data-tab='${TabNameEnum.KeyFrame}']`);
          route
              .KeyFramePlaceholderView(fileNo, rowdata.KeyFrameCount, DirAuth)
              .then(placeholder => {
                  $KTab.empty().append(placeholder);
              })
              .then(() => {
                  route.KeyFrameView({ type: type, fileNo: fileNo }, DirAuth);
              });
      }
      /*-------?????????Video???Audio??????????????????,Preview----------*/
      if (type == MediaType.VIDEO || type == MediaType.AUDIO) {
          const PTab = $CusModal.find(`div[data-tab='${TabNameEnum.Paragraph}']`);
          $('.subtable').trigger('updateTabCount',[fileNo,TabNameEnum.Paragraph, Number(rowdata.SegmentCount)]);
          route
              .ParagraphDesView({ fsSUBJECT_ID: subjectId, type: type, fileNo: fileNo }, DirAuth)
              .then(view => {
                  PTab.html(view);
              });
  
          /*----------????????????????????????-----------------------*/
          const $NUllPreview = $Preview.find("div[name='NuLLPreview']");
          const IsVideo = type == MediaType.VIDEO ? true : false;
          const divType = IsVideo ? 'video' : 'audio';
          $NUllPreview.length > 0 ? $NUllPreview.remove() : false;
          if (IsNULLorEmpty(rowdata.FileUrl)) {
              IsImageValid(rowdata.ImageUrl).then(IsOK => {
                  const okImageURL = IsOK ? rowdata.ImageUrl : GetImageUrl(AirmamImage.NoImage).href;
                  const imageDiv = GetImage(okImageURL, rowdata.Title, [ 'ui','image','center','aligned']);
                  $Preview.find(`div[name="${divType}"]`).hide();
                  $Preview.prepend(`<div class='ui basic segment' name='NuLLPreview'>${imageDiv}</div>`);
              });
              if (IsVideo) {
                  $('#videoMenu')
                      .find('button')
                      .addClass('disabled');
              }
          } else {
              $Preview.find(`div[name="${divType}"]`).show();
              const _player = type == MediaType.VIDEO ? _videoPlayer : _audioPlayer;
              _player.Load(rowdata.FileUrl, rowdata.ImageUrl);
          }
      }
      /*----------??????????????????-----------------------*/
      const $MTab = $CusModal.find(`div[data-tab='${TabNameEnum.MetaData}']`);
      route.MediaView({ fsSUBJECT_ID: subjectId, type: type, fileNo: fileNo }, DirAuth).then(view => {
          $MTab.html(view);
      });
      /*----------????????????Preview-----------------------*/
      if (type == MediaType.PHOTO || type == MediaType.Doc) {
          route
              .Preview(
                  {
                      fsSUBJECT_ID: subjectId,
                      fileNo: fileNo,
                      type: type,
                  },
                  DirAuth
              )
              .then(view => {
                  $Preview.empty().append(view);
              });
      }
};

/**??????????????????????????? */
$Preview.on('click', 'button[name="preImg"]', function() {
    const table_ = subtable.GetTable();
    const selectedRows = table_.getSelectedRows();
    const prevRow = selectedRows[0].getPrevRow();
    const pageSize = table_.getPageSize();
    if (prevRow !== false) {
        prevRow.getElement().dispatchEvent(new Event('click'));
        table_.scrollToRow(prevRow);
    } else {
        const nowPage = table_.getPage();
        if (nowPage !== false && nowPage > 1) {
            const toPageNo = nowPage - 1;
            table_.setPage(toPageNo).then(function() {
                const lastRow = table_.getRows()[pageSize * toPageNo - 1];
                lastRow.getElement().dispatchEvent(new Event('click'));
                table_.scrollToRow(lastRow);
            });
        } else {
            InfoMessage(`<span class="icon camera"></span>?????????????????????!`);
        }
    }
});

/**??????????????????????????? */
$Preview.on('click', 'button[name="nextImg"]', function() {
    const table_ = subtable.GetTable();
    const selectedRows = table_.getSelectedRows();
    const nextRow = selectedRows[0].getNextRow();
    const pageSize = table_.getPageSize();
    if (nextRow !== false) {
        nextRow.getElement().dispatchEvent(new Event('click'));
        table_.scrollToRow(nextRow);
    } else {
        const nowPage = table_.getPage();
        const maxPage = table_.getPageMax();
        if (nowPage !== false && nowPage < maxPage) {
            const toPageNo = nowPage + 1;
            table_.setPage(toPageNo).then(function() {
                const firstRow = table_.getRows()[pageSize * (toPageNo - 1)];
                firstRow.getElement().dispatchEvent(new Event('click'));
                table_.scrollToRow(firstRow);
            });
        } else {
            InfoMessage(`<span class="icon camera"></span>????????????????????????!`);
        }
    }
});
/**???????????? */
$Preview.on('click', 'img[name="lightbox"]', function() {
    const src = $(this).attr('src');
    lightbox(src);
});

/*---------------------------------------
        ??????
-------------------------------------------*/

/**???????????????????????????dataId */
export const UpdateParagraphCreateBtnId = (ModalId: string, fileno: string) => {
    $(ModalId)
        .find("div.tab[data-tab='" + TabNameEnum.Paragraph + "']> button[name='create']")
        .attr('data-Id', fileno);
};
