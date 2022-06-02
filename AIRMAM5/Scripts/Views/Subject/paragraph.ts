import { ISubjectController } from '../../Models/Controller/SubjectController';
import { ModalTask } from '../../Models/Function/Modal';
import { ErrorMessage, SuccessMessage, WarningMessage } from '../../Models/Function/Message';
import { MediaType } from '../../Models/Enum/MediaType';
import { CurrentTimeToTimeCode } from '../../Models/Function/Frame';
// import { UpdateTabCount} from './Index';
import { TabNameEnum } from '../../Models/Enum/TabNameEnum';
import { videoPlayer } from '../../Models/Class/videoPlayer';
import { audioPlayer } from '../../Models/Class/audioPlayer';
import { StringEnum } from '../../Models/Enum/StringEnum';
import { FormValidField } from '../../Models/Const/FormValid';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { ParagraphCUModel } from '../../Models/Interface/Subject/ParagraphCUModel';
import { applyFill } from '../../Models/Function/Range';
import { Logger } from '../../Models/Class/LoggerService';

/*----------------------------------------------
    段落描述功能
-----------------------------------------------*/
export function Paragraph(route: ISubjectController, ModalId: string, type: string, player: videoPlayer | audioPlayer) {
    const $ParaTab = $(ModalId).find(`div[data-tab='${TabNameEnum.Paragraph}']`);
    // const CreateParaModalId = '#ParagraphAddModal' + ($('#ParagraphAddModal').length + 1);
    const CreateParaModalId = '#ParagraphAddModal';
    const EditParaModalId = '#ParagraphEditModal';
    const DeleteParaModalId = '#ParagraphDeleteModal';
    const DeleteParagraphConfirmId = '#DeleteParagraphConfirm';
    const valid = FormValidField.Subject;
    //---------移動複製燈箱位置,每次頁面刷新--------------------------------
    // const CopyPConfirm = () => {
    //     document.querySelectorAll(DeleteParagraphConfirmId).length <= 0
    //         ? $('#DeleteParagraphConfirm')
    //               .clone()
    //               .attr('id', DeleteParagraphConfirmId.replace('#', ''))
    //               .appendTo($ParaTab)
    //         : $(DeleteParagraphConfirmId).appendTo($ParaTab);
    // };
    //---------按鈕事件--------------------------------
    /*新增*/
    $ParaTab.on('click', "button[name='create']", function(event) {
        event.preventDefault();
        const fileNo: string = $(this).attr('data-Id');
        // document.querySelectorAll(CreateParaModalId).length <= 0
        //     ? $('#ParagraphAddModal')
        //           .clone()
        //           .attr('id', CreateParaModalId.replace('#', ''))
        //           .appendTo($ParaTab)
        //     : false;
        ModalTask(CreateParaModalId, true, {
            allowMultiple: false, //因為只有一層,若true會跑位
            closable: false,
            detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
            observeChanges: true /*modalDOM中的任何更改是否應自動刷新緩存的位置 */,
            // detachable: false,
            context: '#right-component',
            onShow: function(this: JQuery<HTMLElement>) {
                /*為了解決Modal中Modal關閉問題互相影響*/
                $(this).on('click', function(event) {
                    event.stopImmediatePropagation();
                });
                /*----- 設定取Player時間--------*/
                $("button[name='PlayerDuration']").click(function() {
                    const $Input = $(this).siblings('input');
                    const current = player.GetCurrentTime();
                    const currentTimecode = player.GetCurrentTimeCode();
                    $Input.val(currentTimecode);
                    $Input.attr('data-Seconds', current);
                });
            },
            onApprove: function() {
                const $BegInput = $(CreateParaModalId).find("input[name='BegTime']");
                const $EndInput = $(CreateParaModalId).find("input[name='EndTime']");
                const $Description = $(CreateParaModalId).find("textarea[name='Description']");
                const BegTime = Number($BegInput.attr('data-Seconds'));
                const EndTime = Number($EndInput.attr('data-Seconds'));
                const IsFormValid: boolean = true;
                // const IsFormValid: boolean = CheckForm('#CreateParagraphForm', valid.Paragraph);
                if (
                    IsNULLorEmpty($BegInput.val()) ||
                    IsNULLorEmpty($EndInput.val()) ||
                    IsNULLorEmpty($Description.val())
                ) {
                    ErrorMessage('開始時間、結束時間、描述皆為必填');
                } else if (BegTime > EndTime) {
                    WarningMessage('開始時間必須小於結束時間');
                } else {
                    if (IsFormValid) {
                        route
                            .AddParagraph({
                                FileCategory: <MediaType>type,
                                fsFILE_NO: fileNo,
                                SeqNo: 0,
                                BegTime: BegTime,
                                EndTime: EndTime,
                                Description: <string>$Description.val(),
                            })
                            .then(res => {
                                const Record = <ParagraphCUModel>res.Records;
                                if (res.IsSuccess) {
                                    $BegInput.val(StringEnum.Empty);
                                    $EndInput.val(StringEnum.Empty);
                                    $Description.val(StringEnum.Empty);
                                    $(CreateParaModalId).modal('hide');
                                    SuccessMessage(res.Message);
                                    // UpdateTabCount(TabNameEnum.Paragraph, 'update', +1);
                                    $('.subtable').trigger('updateTabCount',[fileNo,TabNameEnum.Paragraph,'+1']);
                                    // subtable.ReactivityUpdate(fileNo, {
                                    //     SegmentCount: UpdateTabCount(TabNameEnum.Paragraph, 'get'),
                                    // });
                                    $ParaTab.find('.cuslist').append(`
                                        <div class="item" data-BegTime="${BegTime}" data-EndTime="${EndTime}" data-SeqNo="${
                                        Record.SeqNo
                                    }">
                                            <div class="content">
                                             <div class="header">
                                             ${CurrentTimeToTimeCode(Record.BegTime, 'second')}
                                             ~
                                             ${CurrentTimeToTimeCode(Record.EndTime, 'second')}</div>
                                            <p>${Record.Description}</p>
                                            <div class="extra">
                                            <button type="button" class="ui _darkGrey right floated mini button"  name="edit" data-Id="${fileNo}"><i class="edit icon"></i>修改</button>
                                            <button type="button" class="ui _darkGrey right floated mini button"  name="delete" data-Id="${fileNo}"><i class="delete icon"></i>刪除</button>                         
                                           </div>
                                          </div>
                                        </div>`);
                                } else {
                                    ErrorMessage(res.Message);
                                }
                            })
                            .catch(error => {
                                Logger.viewres(route.api.AddParagraph, '新增段落描述', error, true);
                            });
                    }
                    return false;
                }
                return false;
            },
            onDeny: function(this: JQuery<HTMLElement>) {
                $(CreateParaModalId).modal('hide');
                return false;
            },
        });
    });
    /*編輯*/
    $ParaTab.on('click', "button[name='edit']", function(event) {
        event.preventDefault();
        const fileNo: string = $(this).attr('data-Id');
        const item = $(this).closest('.item');
        const seqno: number = Number(item.attr('data-SeqNo')) || item.prevAll().length + 1;
        route
            .EditParagraphView(ModalId, EditParaModalId, { fileNo: fileNo, type: type, seqno: seqno })
            .then(success => {
                if (success) {
                    const $BegInput = $(EditParaModalId).find("input[name='BegTime']");
                    const $EndInput = $(EditParaModalId).find("input[name='EndTime']");
                    ModalTask(EditParaModalId, true, {
                        allowMultiple: true,
                        // selector: {
                        //     close: '.actions .approve,.actions .deny,i.close',
                        // },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $ParaTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                            const $BegTimeValue = CurrentTimeToTimeCode(Number($BegInput.val()), 'second');
                            const $EndTimeValue = CurrentTimeToTimeCode(Number($EndInput.val()), 'second');
                            $BegInput.val($BegTimeValue);
                            $EndInput.val($EndTimeValue);
                            /*----- 設定取Player時間--------*/
                            $("button[name='PlayerDuration']").click(function() {
                                const $Input = $(this).siblings('input');
                                const current = player.GetCurrentTime();
                                const currentTimecode = player.GetCurrentTimeCode();
                                $Input.val(currentTimecode);
                                $Input.attr('data-Seconds', current);
                            });
                        },
                        onApprove: function() {
                            const BegTime = Number($BegInput.attr('data-Seconds'));
                            const EndTime = Number($EndInput.attr('data-Seconds'));
                            const $Description = $(EditParaModalId).find("textarea[name='Description']");
                            const IsFormValid: boolean = true;
                            // const IsFormValid: boolean = CheckForm('#EditParagraphForm', valid.Paragraph);
                            if (
                                IsNULLorEmpty($BegInput.val()) ||
                                IsNULLorEmpty($EndInput.val()) ||
                                IsNULLorEmpty($Description.val())
                            ) {
                                ErrorMessage('開始時間、結束時間、描述皆為必填');
                            } else if (BegTime > EndTime) {
                                WarningMessage('開始時間必須小於結束時間');
                            } else {
                                if (IsFormValid) {
                                    $(EditParaModalId).modal('hide');
                                    route
                                        .EditParagraph({
                                            FileCategory: <MediaType>type,
                                            fsFILE_NO: fileNo,
                                            SeqNo: seqno,
                                            BegTime: Number($BegInput.attr('data-Seconds')),
                                            EndTime: Number($EndInput.attr('data-Seconds')),
                                            Description: <string>$(EditParaModalId)
                                                .find("textarea[name='Description']")
                                                .val(),
                                        })
                                        .then(res => {
                                            const Record = <ParagraphCUModel>res.Records;
                                            if (res.IsSuccess) {
                                                SuccessMessage(res.Message);
                                                item.find('.header').text(
                                                    `${CurrentTimeToTimeCode(Record.BegTime, 'second')}
                                                    ~
                                                     ${CurrentTimeToTimeCode(Record.EndTime, 'second')}`
                                                );
                                                item.attr('data-begtime', Record.BegTime);
                                                item.attr('data-endtime', Record.EndTime);
                                                item.find('.content > p').text(Record.Description);
                                            } else {
                                                ErrorMessage(res.Message);
                                            }
                                        })
                                        .catch(error => {
                                            Logger.viewres(route.api.EditParagraph, '編輯段落描述', error, true);
                                        });
                                }
                            }
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(EditParaModalId).modal('hide');
                            return false;
                        },
                    });
                } else {
                    ErrorMessage('系統發生錯誤,無法開啟編輯段落描述燈箱,請重新再試');
                }
            });
    });
    /*刪除*/
    $ParaTab.on('click', "button[name='delete']", function(event) {
        event.preventDefault();
        const fileNo: string = $(this).attr('data-Id');
        const item = $(this).closest('.item');
        const seqno: number = Number(item.attr('data-SeqNo')) || item.prevAll().length + 1;
        // CopyPConfirm();
        route
            .DeleteParagraphView(ModalId, DeleteParaModalId, { fileNo: fileNo, type: type, seqno: seqno })
            .then(success => {
                if (success) {
                    /**刪除確認燈箱事件綁定 */
                    ModalTask(DeleteParagraphConfirmId, false, {
                        // allowMultiple: true,
                        selector: {
                            close: '.actions .ok,.actions .cancel,i.close',
                            deny: '.actions .cancel',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $ParaTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                        },
                        onApprove: function() {
                            route
                                .DeleteParagraph({
                                    fileNo: fileNo,
                                    type: <MediaType>type,
                                    seqno: seqno,
                                })
                                .then(res => {
                                    if (res.IsSuccess) {
                                        SuccessMessage(res.Message);
                                        item.remove();
                                        $('.subtable').trigger('updateTabCount',[fileNo,TabNameEnum.Paragraph, '-1']);
                                        // UpdateTabCount(TabNameEnum.Paragraph, 'update', -1);
                                        // subtable.ReactivityUpdate(fileNo, {
                                        //     SegmentCount: UpdateTabCount(TabNameEnum.Paragraph, 'get'),
                                        // });
                                    } else {
                                        ErrorMessage(res.Message);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.DeleteParagraph, '刪除段落描述', error, true);
                                });
                            $(DeleteParagraphConfirmId).modal('hide');
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(DeleteParagraphConfirmId).modal('hide');
                            return false;
                        },
                    }).modal('attach events', DeleteParaModalId + ' .actions .approve');
                    /**刪除燈箱綁定 */
                    const $BegInput = $(DeleteParaModalId).find("input[name='BegTime']");
                    const $EndInput = $(DeleteParaModalId).find("input[name='EndTime']");
                    ModalTask(DeleteParaModalId, true, {
                        //allowMultiple: true,
                        selector: {
                            close: '.actions .approve,.actions .deny,i.close',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $ParaTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                            const $BegTimeValue = CurrentTimeToTimeCode(Number($BegInput.val()), 'second');
                            const $EndTimeValue = CurrentTimeToTimeCode(Number($EndInput.val()), 'second');
                            $BegInput.val($BegTimeValue);
                            $EndInput.val($EndTimeValue);
                        },
                        onApprove: function() {
                            $(DeleteParaModalId).modal('hide');
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(DeleteParagraphConfirmId).modal('hide');
                            $(DeleteParaModalId).modal('hide');
                            return false;
                        },
                    });
                } else {
                    ErrorMessage('系統發生錯誤,無法開啟刪除段落描述燈箱,請重新再試');
                }
            });
    });
    /*段落點擊以撥放音頻*/
    $ParaTab.on('click', '.cuslist .item', function(e) {
        if ((<string>e.target.nodeName).toLowerCase() !== 'button') {
            const current: number = Number($(this).attr('data-BegTime'));
            const duration = player.GetTotoalTime();
            const $rangeSlider: HTMLInputElement = document
                .getElementById('CusModal')
                .querySelector("input[type='range']");
            const $currentTimer: HTMLSpanElement = document
                .getElementById('CusModal')
                .querySelector("span[name='currentTimer']");
            player.SetCurrentTime(current);
            $rangeSlider.value = ((current * 100) / duration).toString();
            applyFill($rangeSlider, (current * 100) / duration);
            $currentTimer.innerHTML = CurrentTimeToTimeCode(current, 'second');
        }
    });
}
