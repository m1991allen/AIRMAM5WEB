import { tabulatorService } from '../../Models/Class/tabulatorService';
import { dayjs, JsonDateToDate, setCalendar } from '../../Models/Function/Date';
import { Color } from '../../Models/Enum/ColorEnum';
import { AnnType, ChineseAnnType } from '../../Models/Enum/AnnEnum';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { DetailModal, ShowModal, ModalTask } from '../../Models/Function/Modal';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { SetDate } from '../../Models/Function/Date';
import { initSetting, TabulatorSetting } from '../../Models/initSetting';
import { CheckForm } from '../../Models/Function/Form';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';

import {
    SearchFormId,
    CreateFormId,
    CreateModalId,
    DeleteModalId,
    EditFormId,
    EditModalId,
    DeleteFormId,
    sdateId,
    edateId,
} from '../../Models/Const/Const.';
import { FormValidField } from '../../Models/Const/FormValid';
import { ErrorMessage } from '../../Models/Function/Message';
import { AnnController, IAnnController } from '../../Models/Controller/AnnController';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { AnnMessageSetting } from '../../Models/MessageSetting';
import { EditButton, DetailButton, DeleteButton } from '../../Models/Templete/ButtonTemp';
import { AnnCreateResponseModel } from '../../Models/Interface/Ann/AnnCreateResponseModel';
import { AnnListModel } from '../../Models/Interface/Ann/AnnListModel';
import { AnnSearchModel } from '../../Models/Interface/Ann/AnnSearchModel';
import { Filter } from '../../Models/Enum/Filter';
import { Logger } from '../../Models/Class/LoggerService';
import { GetDropdown } from '../../Models/Function/Element';
/*=====================????????????====================================*/
var table: ItabulatorService;
var route: IAnnController = new AnnController();
const valid = FormValidField.Ann;
const message = AnnMessageSetting;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $annUnit: JQuery<HTMLInputElement> = $('#annUnit');
const prop = (key: keyof AnnListModel): string => {
    return route.GetProperty<AnnListModel>(key);
};
const groupList = (MutipleSelectValue: object): Array<string> => {
    let selectList: Array<string> = [];
    for (let key in MutipleSelectValue) {
        selectList.push(MutipleSelectValue[key]);
    }
    return selectList;
};
/*------------------------------------------------------------*/
/*??????????????????,?????????????????????????????????,?????????????????????????????????*/
SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar(`${SearchFormId} .calendar`, 'date');

/*??????????????????*/
Search({
    sdate: <string>$sdate.val(),
    edate: <string>$edate.val(),
    type: StringEnum.Empty,
    dept: StringEnum.Empty,
});
/*????????????*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const $form = $(SearchFormId);
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const sdate = dayjs($sdate.val());
        const edate = dayjs($edate.val());
        const type = $form.find("select[name='annLevel']").closest('.dropdown');
        const dept = $form.find('#annUnit');
        if (edate.isBefore(sdate)) {
            $(this).addClass('error');
            ErrorMessage('?????????????????????(???)??????????????????(???)??????');
        } else {
            $(this).removeClass('error');
            Search({
                sdate: <string>$sdate.val(),
                edate: <string>$edate.val(),
                type: type.dropdown('get value'),
                dept: dept.dropdown('get value'),
            });
        }
    }
});

/**???????????? */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
    const $form = $(SearchFormId);
    const type = GetDropdown(SearchFormId, 'annLevel');
    const dept = $form.find('#annUnit');
    type.dropdown('set selected', StringEnum.All);
    dept.dropdown('set selected', StringEnum.All);
});

/**???????????? */
ModalTask(CreateModalId, false, {
    closable: false,
    onShow: function() {
        SetDate('#fdSDATE', dayjs(), 'YYYY/MM/DD');
        SetDate('#fdEDATE', dayjs().add(7, 'day'), 'YYYY/MM/DD');
        setCalendar(`${CreateFormId} .calendar`, 'date');
    },
    onApprove: function() {
        const IsFormValid: boolean = CheckForm(CreateFormId, valid.Create);
        if (IsFormValid) {
            const $form = $(CreateFormId);
            const $ISHIDDEN: JQuery<HTMLElement> = $form.find("select[name='fsIS_HIDDEN']").closest('.dropdown');
            const $TYPE: JQuery<HTMLElement> = $form.find("select[name='fsTYPE']").closest('.dropdown');
            const $GroupList: JQuery<HTMLElement> = $form.find("select[name='GroupList']").closest('.dropdown');
            const $DEPT: JQuery<HTMLElement> = $form.find("select[name='fsDEPT']").closest('.dropdown');
            route
                .Create({
                    AnnounceId: 0,
                    fsTITLE: <string>$form.find("input[name='fsTITLE']").val(),
                    fsCONTENT: <string>$form.find("textarea[name='fsCONTENT']").val(),
                    fdSDATE: dayjs(<string>$form.find("input[name='fdSDATE']").val()).format('YYYY/MM/DD HH:mm:ss'),
                    fdEDATE: dayjs(<string>$form.find("input[name='fdEDATE']").val()).format('YYYY/MM/DD HH:mm:ss'),
                    fsTYPE: <string>$TYPE.dropdown('get value'),
                    fnORDER: Number($form.find("input[name='fnORDER']").val()),
                    GroupList: groupList($GroupList.dropdown('get value')),
                    fsIS_HIDDEN: $ISHIDDEN.dropdown('get value') == YesNo.??? ? YesNo.??? : YesNo.???,
                    fsDEPT: <string>$DEPT.dropdown('get value'),
                    fsNOTE: <string>$form.find("input[name='fsNOTE']").val(),
                })
                .then(res => {
                    Logger.res(route.api.Create, '????????????', res);
                    if (res.IsSuccess) {
                        const data = <AnnCreateResponseModel>res.Data;
                        table.AddRow(<AnnListModel>{
                            AnnounceId: data.fnANN_ID,
                            AnnEdate: data.fdEDATE,
                            AnnSdate: data.fdSDATE,
                            AnnContent: data.fsCONTENT,
                            AnnTitle: data.fsTITLE,
                            AnnType: data.fsTYPE,
                            AnnTypeName: $TYPE.dropdown('get value'),
                            AnnNote: data.fsNOTE,
                            AnnPublishDept: $annUnit
                                .find(`option[value='${data.fsDEPT}']`)
                                .text()
                                .replace(data.fsDEPT, ''),
                        });
                        $(CreateModalId).modal('hide');
                        $form.trigger('reset');
                    }
                })
                .catch(error => {
                    Logger.viewres(route.api.Create, '????????????', error);
                });
        }
        return false;
    },
    onDeny:function(){
        $(CreateFormId).trigger('reset').find('.dropdown').dropdown('clear');
    }
});
/**
 * ????????????
 */
$(document).on('keyup', '#wordFilter', function() {
    const word = <string>$(this).val();
    const filter = [
        { field: prop('AnnounceId'), type: Filter.Like, value: word },
        { field: prop('AnnTitle'), type: Filter.Like, value: word },
        { field: prop('AnnContent'), type: Filter.Like, value: word },
        { field: prop('AnnSdate'), type: Filter.Like, value: word },
        { field: prop('AnnEdate'), type: Filter.Like, value: word },
        { field: prop('AnnPublishDept'), type: Filter.Like, value: word },
    ];
    const typeword =
        word.indexOf(getEnumKeyByEnumValue(ChineseAnnType, ChineseAnnType.??????)) > -1
            ? ChineseAnnType.??????
            : word.indexOf(getEnumKeyByEnumValue(ChineseAnnType, ChineseAnnType.??????)) > -1
            ? ChineseAnnType.??????
            : word.indexOf(getEnumKeyByEnumValue(ChineseAnnType, ChineseAnnType.??????)) > -1
            ? ChineseAnnType.??????
            : word.indexOf(getEnumKeyByEnumValue(ChineseAnnType, ChineseAnnType.????????????)) > -1
            ? ChineseAnnType.????????????
            : '';
    if (!IsNULLorEmpty(typeword)) {
        filter.push({ field: prop('AnnType'), type: Filter.Like, value: typeword });
    }
    table.SetFilter(filter);
});
/*????????????*/
function Search(SearchParams: AnnSearchModel) {
    table = new tabulatorService(initSetting.TableId, {
        height: TabulatorSetting.height,
        layout: TabulatorSetting.layout,
        ajaxURL: route.api.Search,
        ajaxContentType: 'json',
        ajaxConfig: 'POST',
        ajaxParams: SearchParams,
        index: prop('AnnounceId'),
        columns: [
            { title: 'ID', field: prop('AnnounceId'), width: 70, sorter: 'number', visible: false },
            {
                title: '????????????',
                field: prop('AnnTitle'),
                sorter: 'string',
                minWidth: 320,
                formatter: function(cell, formatterParams) {
                    const title = <string>cell.getValue();
                    const span: HTMLSpanElement = document.createElement('span');
                    span.innerHTML = title;
                    span.setAttribute('title', title);
                    return span.outerHTML;
                },
            },
            {
                title: '????????????',
                field: prop('AnnContent'),
                visible: false,
                // width: '30%',
                sorter: 'string',
                formatter: function(cell, formatterParams) {
                    const content = <string>cell.getValue();
                    const span: HTMLSpanElement = document.createElement('span');
                    span.innerHTML = content;
                    span.setAttribute('title', content);
                    return span.outerHTML;
                },
            },
            {
                title: '????????????(???)',
                field: prop('AnnSdate'),
                sorter: 'string',
                width: 165,
                mutator: function(value, data, type, mutatorParams, cell) {
                    //???????????????????????????,???????????????????????????
                    const convertDate = JsonDateToDate(value);
                    const date: string = IsNULLorEmpty(convertDate)
                        ? ''
                        : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                    return date;
                },
            },
            {
                title: '????????????(???)',
                field: prop('AnnEdate'),
                sorter: 'string',
                width: 165,
                mutator: function(value, data, type, mutatorParams, cell) {
                    //???????????????????????????,???????????????????????????
                    const convertDate = JsonDateToDate(value);
                    const date: string = IsNULLorEmpty(convertDate)
                        ? ''
                        : dayjs(convertDate).format('YYYY/MM/DD HH:mm:ss');
                    return date;
                },
            },
            {
                title: '????????????',
                field: prop('AnnType'),
                sorter: 'string',
                width: 105,
                formatter: function(cell, formatterParams) {
                    let value: string = cell.getValue();
                    const showlabel = (label: string, color: Color | '') => {
                        return `<span class="ui label ${color}">${label}</span>`;
                    };
                    switch (value) {
                        case AnnType.O:
                            return showlabel(getEnumKeyByEnumValue(ChineseAnnType, AnnType.O), Color.???);
                        case AnnType.Y:
                            return showlabel(getEnumKeyByEnumValue(ChineseAnnType, AnnType.Y), Color.???);
                        case AnnType.R:
                            return showlabel(getEnumKeyByEnumValue(ChineseAnnType, AnnType.R), Color.???);
                        case AnnType.D:
                            return showlabel(getEnumKeyByEnumValue(ChineseAnnType, AnnType.D), Color.?????????);
                        default:
                            return showlabel(value, '');
                    }
                },
            },
            { title: '????????????', field: prop('AnnPublishDept'), sorter: 'string', width: 115 },
            {
                title: '??????',
                field: prop('AnnounceId'),
                hozAlign: 'center',
                width: 150,
                // width: 150,
                formatter: function(cell, formatterParams) {
                    cell.getElement().classList.add('tabulator-operation');
                    const id: number = parseInt(cell.getValue());
                    const editbtn = EditButton(id, message.Controller);
                    const detailbtn = DetailButton(id, message.Controller);
                    const deletebtn = DeleteButton(id, message.Controller);
                    const btngroups: string = editbtn + detailbtn + deletebtn;
                    return btngroups;
                },
                cellClick: function(e, cell) {
                    const target: HTMLButtonElement | HTMLDivElement | HTMLElement = <any>e.target;
                    const rowdata = <AnnListModel>cell.getRow().getData();
                    const AnnId: number = rowdata.AnnounceId | 0;
                    switch (true) {
                        /*??????:??????*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('list icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'detail':
                            DetailModal(
                                Controller.Ann,
                                Action.ShowDetails,
                                { id: AnnId },
                                { calendar: false, calendarType: 'datetime', dropdown: false }
                            );
                            break;
                        /*??????:??????*/
                        case target instanceof HTMLElement && (<HTMLElement>target).className.indexOf('edit icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'edit':
                            ShowModal<IdModel>(EditModalId, route.api.ShowEdit, { id: AnnId }).then(IsSuccess => {
                                if (IsSuccess) {
                                    const $form = $(EditFormId);
                                    const $ISHIDDEN: JQuery<HTMLElement> = $form
                                        .find("select[name='fsIS_HIDDEN']")
                                        .closest('.dropdown');
                                    const $TYPE: JQuery<HTMLElement> = $form
                                        .find("select[name='fsTYPE']")
                                        .closest('.dropdown');
                                    const $GroupList: JQuery<HTMLElement> = $form
                                        .find("select[name='GroupList']")
                                        .closest('.dropdown');
                                    const $DEPT: JQuery<HTMLElement> = $form
                                        .find("select[name='fsDEPT']")
                                        .closest('.dropdown');

                                    route;
                                    ModalTask(EditModalId, true, {
                                        closable: false,
                                        onShow: function() {
                                            $form.find('.dropdown').dropdown();
                                            setCalendar(`${EditFormId} .calendar`, 'date');
                                        },
                                        onApprove: function() {
                                            const IsFormValid: boolean = CheckForm(EditFormId, valid.Edit);
                                            if (IsFormValid) {
                                                route
                                                    .Edit({
                                                        AnnounceId: AnnId,
                                                        fsTITLE: <string>$form.find("input[name='fsTITLE']").val(),
                                                        fsCONTENT: <string>(
                                                            $form.find("textarea[name='fsCONTENT']").val()
                                                        ),
                                                        fdSDATE: dayjs(
                                                            <string>$form.find("input[name='fdSDATE']").val()
                                                        ).format('YYYY/MM/DD HH:mm:ss'),
                                                        fdEDATE: dayjs(
                                                            <string>$form.find("input[name='fdEDATE']").val()
                                                        ).format('YYYY/MM/DD HH:mm:ss'),
                                                        fsTYPE: <string>$TYPE.dropdown('get value'),
                                                        fnORDER: Number($form.find("input[name='fnORDER']").val()),
                                                        GroupList: groupList($GroupList.dropdown('get value')),
                                                        fsIS_HIDDEN:
                                                            $ISHIDDEN.dropdown('get value') == YesNo.???
                                                                ? YesNo.???
                                                                : YesNo.???,
                                                        fsDEPT: <string>$DEPT.dropdown('get value'),
                                                        fsNOTE: <string>$form.find("input[name='fsNOTE']").val(),
                                                    })
                                                    .then(res => {
                                                        Logger.res(route.api.Edit, '????????????', res);
                                                        if (res.IsSuccess) {
                                                            const data = <AnnCreateResponseModel>res.Data;
                                                            table.ReactivityUpdate(AnnId, <AnnListModel>{
                                                                AnnounceId: data.fnANN_ID,
                                                                AnnEdate: data.fdEDATE,
                                                                AnnSdate: data.fdSDATE,
                                                                AnnContent: data.fsCONTENT,
                                                                AnnTitle: data.fsTITLE,
                                                                AnnType: data.fsTYPE,
                                                                AnnTypeName: $TYPE.dropdown('get value'),
                                                                AnnNote: data.fsNOTE,
                                                                AnnPublishDept: $annUnit
                                                                    .find(`option[value='${data.fsDEPT}']`)
                                                                    .text()
                                                                    .replace(data.fsDEPT, ''),
                                                            });
                                                            $(EditModalId).modal('hide');
                                                        }
                                                    })
                                                    .catch(error => {
                                                        Logger.viewres(route.api.Edit, '????????????', error);
                                                    });
                                            }

                                            return false;
                                        },
                                    });
                                } else {
                                    ErrorMessage('??????????????????????????????!');
                                }
                            });

                            break;
                        /**??????:?????? */

                        case target instanceof HTMLElement &&
                            (<HTMLElement>target).className.indexOf('delete icon') > -1:
                        case target instanceof HTMLButtonElement && (<HTMLButtonElement>target).name == 'delete':
                            ShowModal<IdModel>(DeleteModalId, route.api.ShowDelete, { id: AnnId }).then(IsSuccess => {
                                if (IsSuccess) {
                                    ModalTask(DeleteModalId, true, {
                                        closable: false,
                                        onShow: function() {
                                            $(DeleteModalId)
                                                .find('.dropdown')
                                                .dropdown();
                                            setCalendar(`${DeleteFormId} .calendar`, 'date');
                                        },
                                        onApprove: function() {
                                            route.Delete(AnnId).then(res => {
                                                Logger.res(route.api.Delete, '????????????', res);
                                                if (res.IsSuccess) {
                                                    table.RemoveRow(AnnId);
                                                    $(DeleteModalId).modal('hide');
                                                }
                                            });
                                            return false;
                                        },
                                    });
                                } else {
                                    ErrorMessage(`??????????????????????????????!`);
                                }
                            });

                            break;
                    }
                },
            },
        ],
    });
}
