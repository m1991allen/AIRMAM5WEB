import { ISubjectController } from '../../Models/Controller/SubjectController';
import { ErrorMessage } from '../../Models/Function/Message';
import { ModalTask } from '../../Models/Function/Modal';
// import { UpdateTabCount } from './Index';
import { TabNameEnum } from '../../Models/Enum/TabNameEnum';
import { ItabulatorService } from '../../Models/Interface/Service/ItabulatorService';
import { videoPlayer } from '../../Models/Class/videoPlayer';
import { CurrentTimeToTimeCode, KeyTimeToSeconds } from '../../Models/Function/Frame';
import { SubjectModel } from '../../Models/Interface/Subject/SubjectModel';
import { KeyFrameCUModel } from '../../Models/Interface/Subject/KeyFrameCUModel';
import { UI } from '../../Models/Templete/CompoentTemp';
import { KeyFrameDataModel } from '../../Models/Interface/Subject/KeyFrameDataModel';
import { PermissionDefinition } from '../../Models/Enum/PermissionDefinition';
import { Logger } from '../../Models/Class/LoggerService';
import { FileListModel } from '../../Models/Interface/Subject/FileListModel';

/*----------------------------------------------
     關鍵影格功能
-----------------------------------------------*/
export function KeyFrame(
    route: ISubjectController,
    subtable: ItabulatorService,
    ModalId: string,
    query: SubjectModel,
    player: videoPlayer,
    DirAuth: Array<PermissionDefinition>
) {
    const $KeyframeTab = $(ModalId).find(`div[data-tab='${TabNameEnum.KeyFrame}']`);
    const CreateKeyFrameModalId = '#KeyFrameAddModal';
    const EditKeyFrameModalId = '#KeyFrameEditModal';
    const DeleteKeyFrameModalId = '#KeyFrameDeleteModal';
    const DeleteKeyFrameConfirmId = '#DeleteKeyFrameConfirm';
    //---------按鈕事件--------------------------------
    /**點擊影格*/
    $KeyframeTab.on('click', '.ui.card:not(.placecard)', function(e) {
        if ((<string>e.target.nodeName).toLowerCase() !== 'button') {
            const video: HTMLVideoElement = document.getElementById('CusModal').querySelector('video');
            const duration = video.duration;
            const $rangeSlider: HTMLInputElement = document
                .getElementById('CusModal')
                .querySelector("input[type='range']");
            const $currentTimer: HTMLSpanElement = document
                .getElementById('CusModal')
                .querySelector("span[name='currentTimer']");
            const keycodetime = $(this)
                .closest('.card')
                .attr('data-time');
            const totalseconds = KeyTimeToSeconds(keycodetime);
            video.currentTime = totalseconds;
            $rangeSlider.value = ((totalseconds * 100) / duration).toString();
            $currentTimer.innerHTML = CurrentTimeToTimeCode(totalseconds, 'second');
        }
    });
    /**新增影格*/
    $KeyframeTab.on('click', "button[name='create']", function(event) {
        event.preventDefault();
        const fileno: string = $(this).attr('id');
        ModalTask(CreateKeyFrameModalId, true, {
            allowMultiple: false, //因為只有一層,若true會跑位
            selector: {
                close: '.actions .approve,.actions .deny,i.close',
            },
            closable: false,
            // detachable: false,
            detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
            observeChanges: true /*modalDOM中的任何更改是否應自動刷新緩存的位置 */,
            context: '#right-component',
            // context: $KeyframeTab,
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
                const $form = $(CreateKeyFrameModalId).find('form');
                route
                    .AddKeyFrame({
                        fsSUBJECT_ID: query.SubjectId,
                        fsFILE_NO: fileno,
                        Title: <string>$form.find("input[name='Title']").val(),
                        SetTime: <string>$form.find("input[name='Time']").attr('data-Seconds'),
                        Description: <string>$form.find("textarea[name='Description']").val(),
                    })
                    .then(async res => {
                        Logger.res(route.api.AddKeyFrame, '新增關鍵影格', res);
                        const Data = <{ KeyframeImage: string }>res.Data;
                        const Record = <KeyFrameCUModel>res.Records;
                        if (res.IsSuccess) {
                            $('.subtable').trigger('updateTabCount',[fileno,TabNameEnum.KeyFrame, '+1']);
                            // UpdateTabCount(TabNameEnum.KeyFrame, 'update', +1);
                            // subtable.ReactivityUpdate(Record.fsFILE_NO, {
                            //     KeyFrameCount: UpdateTabCount(TabNameEnum.KeyFrame, 'get'),
                            // });
                            const info: KeyFrameDataModel = {
                                fsFILE_NO: Record.fsFILE_NO,
                                Title: Record.Title,
                                Description: Record.Description,
                                IsHeadFrame: false,
                                FilePath: '',
                                Time: Record.SetTime,
                                ImageUrl: Data.KeyframeImage,
                                FileInfo: '',
                            };
                            const showEditBtn = DirAuth.indexOf(PermissionDefinition.U) > -1 ? true : false;
                            const showDeleteBtn = DirAuth.indexOf(PermissionDefinition.D) > -1 ? true : false;
                            const card = UI.KeyFrame.KeyFrameCard(info, true, showEditBtn, showDeleteBtn);
                            $KeyframeTab.find('.four.stackable.cards').append(card);
                            $form.trigger('reset');
                        }
                    })
                    .catch(error => {
                        Logger.res(route.api.AddKeyFrame, '新增關鍵影格', error);
                    });
                $(CreateKeyFrameModalId).modal('hide');
                return false;
            },
            onDeny: function(this: JQuery<HTMLElement>) {
                const $form = $(CreateKeyFrameModalId).find('form');
                $form.trigger('reset');
                $(CreateKeyFrameModalId).modal('hide');
                return false;
            }
        });
    });
    /**編輯影格*/
    $KeyframeTab.on('click', "button[name='edit']", function(event) {
        event.preventDefault();
        const fileno: string = $(this).attr('data-fileno');
        const time: string = $(this).attr('data-time');
        const card = $(this).closest('.card');
        route
            .EditKeyframeView(ModalId, EditKeyFrameModalId, { fileno: fileno, time: time })
            .then(success => {
                if (success) {
                    ModalTask(EditKeyFrameModalId, true, {
                        allowMultiple: true,
                        selector: {
                            close: '.actions .approve,.actions .deny,i.close',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $KeyframeTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                        },
                        onApprove: function() {
                            const Title = <string>$(EditKeyFrameModalId)
                                .find("input[name='Title']")
                                .val();
                            const Description = <string>$(EditKeyFrameModalId)
                                .find("textarea[name='Description']")
                                .val();
                            route
                                .EditKeyFrame({
                                    fsSUBJECT_ID: query.SubjectId,
                                    fsFILE_NO: fileno,
                                    Title: Title,
                                    SetTime: time,
                                    Description: Description,
                                })
                                .then(res => {
                                    Logger.res(route.api.EditKeyFrame, '編輯關鍵影格', res);
                                    const Record = <KeyFrameCUModel>res.Records;
                                    if (res.IsSuccess) {
                                        card.find('.meta').text(Record.Title);
                                        card.find('.description').text(Record.Description);
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.EditKeyFrame, '編輯關鍵影格', error);
                                });
                            $(EditKeyFrameModalId).modal('hide');
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(EditKeyFrameModalId).modal('hide');
                            return false;
                        },
                    });
                } else {
                    Logger.viewres(route.api.ShowEditKeyFrameView, '編輯關鍵影格燈箱', '', false);
                    ErrorMessage('系統發生錯誤,無法開啟編輯關鍵影格燈箱,請重新再試');
                }
            })
            .catch(error => {
                Logger.viewres(route.api.ShowEditKeyFrameView, '編輯關鍵影格燈箱', error, false);
            });
    });
    /**刪除影格*/
    $KeyframeTab.on('click', "button[name='delete']", function(event) {
        event.preventDefault();
        const fileno: string = $(this).attr('data-fileno');
        const time: string = $(this).attr('data-time');
        const card = $(this).closest('.card');
        route
            .DeleteKeyframeView(ModalId, DeleteKeyFrameModalId, { fileno: fileno, time: time })
            .then(success => {
                if (success) {
                    /**刪除確認燈箱事件綁定 */
                    ModalTask(DeleteKeyFrameConfirmId, false, {
                        // allowMultiple: true,
                        selector: {
                            close: '.actions .ok,.actions .cancel,i.close',
                            deny: '.actions .cancel',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $KeyframeTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                        },
                        onApprove: function() {
                            route
                                .DeleteKeyFrame({
                                    fileno: fileno,
                                    time: time,
                                })
                                .then(res => {
                                    Logger.res(route.api.DeleteKeyFrame, '刪除關鍵影格', res);
                                    if (res.IsSuccess) {
                                        $('.subtable').trigger('updateTabCount',[fileno,TabNameEnum.KeyFrame, '-1']);
                                        // UpdateTabCount(TabNameEnum.KeyFrame, 'update', -1); //added_20200303
                                        // subtable.ReactivityUpdate(fileno, {
                                        //     KeyFrameCount: UpdateTabCount(TabNameEnum.KeyFrame, 'get'),
                                        // });
                                        card.remove();
                                    }
                                })
                                .catch(error => {
                                    Logger.viewres(route.api.DeleteKeyFrame, '刪除關鍵影格', error);
                                });
                            $(DeleteKeyFrameConfirmId).modal('hide');
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(DeleteKeyFrameConfirmId).modal('hide');
                            return false;
                        },
                    }).modal('attach events', DeleteKeyFrameModalId + ' .actions .approve', 'show');
                    /**刪除燈箱綁定 */
                    ModalTask(DeleteKeyFrameModalId, true, {
                        //  allowMultiple: true,
                        selector: {
                            close: '.actions .approve,.actions .deny,i.close',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $KeyframeTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                        },
                        onApprove: function() {
                            $(DeleteKeyFrameModalId).modal('hide');
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(DeleteKeyFrameConfirmId).modal('hide');
                            $(DeleteKeyFrameModalId).modal('hide');
                            return false;
                        },
                    });
                } else {
                    Logger.viewres(route.api.ShowEditKeyFrameView, '刪除關鍵影格燈箱', '', false);
                    ErrorMessage('系統發生錯誤,無法開啟刪除關鍵影格燈箱,請重新再試');
                }
            })
            .catch(error => {
                Logger.viewres(route.api.ShowEditKeyFrameView, '刪除關鍵影格燈箱', error, false);
            });
    });
    /*-------影格設為代表圖--------- */
    $KeyframeTab.on('click', "button[name='setRepresent']", function(event) {
        event.preventDefault();
        const fileno: string = $(this).attr('data-fileno');
        const time: string = $(this).attr('data-time');
        route
            .SetHeadFrame({ fileno: fileno, time: time })
            .then(res => {
                Logger.res(route.api.SetHeadFrame, '影格設為代表圖', res);
                if (res.IsSuccess) {
                    $(this)
                        .closest('.cards')
                        .find("button[name='cancelRepresent']")
                        .removeClass('red')
                        .addClass('black')
                        .attr('name', 'setRepresent')
                        .html('<i class="icon star outline"></i>');
                    $(this)
                        .attr('name', 'cancelRepresent')
                        .html(`<i class="icon star"></i>`)
                        .addClass('red')
                        .removeClass('black');
                    const newImgURL = $(this)
                        .closest('.card').find('img')
                        .attr('src');
                    const originimg = <HTMLImageElement>subtable
                        .GetRow(fileno)
                        .querySelectorAll('.tabulator-cell')[1]//注意,與綁定的欄位有關
                        .querySelector('img');
                    originimg.src = newImgURL;
                    player.SetPoster(newImgURL);
                    subtable.ReactivityUpdate(fileno, <FileListModel>{
                        ImageUrl: newImgURL,
                    });
                }
            })
            .catch(error => {
                Logger.viewres(route.api.SetHeadFrame, '影格設為代表圖', error);
                console.error(error);
            });
    });
    /*------顯示/隱藏影格詳細資料-------- */
    $KeyframeTab.on('click', "button[name='cardInfo']", function() {
        $KeyframeTab.find('.card .content:not(.extra)').slideToggle();
        $(this)
            .children('i')
            .toggleClass('slash');
    });
}
