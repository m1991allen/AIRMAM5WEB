import { ShowModal, ModalTask } from '../../Models/Function/Modal';
import { DetailModalId } from '../../Models/Const/Const.';
import { IdModel } from '../../Models/Interface/Shared/IdModel';
import { videoPlayer } from '../../Models/Class/videoPlayer';
import { audioPlayer } from '../../Models/Class/audioPlayer';
import { MediaType } from '../../Models/Enum/MediaType';
import { KeyFrameDataModel } from '../../Models/Interface/Subject/KeyFrameDataModel';
import { UI } from '../../Models/Templete/CompoentTemp';
import { ParaDescriptionModel } from '../../Models/Interface/Subject/ParaDescriptionModel';
import { ErrorMessage } from '../../Models/Function/Message';
import { KeyTimeToSeconds, CurrentTimeToTimeCode } from '../../Models/Function/Frame';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { Logger } from '../../Models/Class/LoggerService';
/**
 * 共用的影音圖文檢視燈箱
 * @param api  api路徑
 * @param id id
 * @param type 媒體種類MediaType
 */
export function MediaDetail(api: string, id: string | number, type: MediaType): void {
    ShowModal<IdModel>(DetailModalId, api, { id: id }).then(IsSuccess => {
        if (IsSuccess) {
            let tempplayer: videoPlayer | audioPlayer = null;
            ModalTask(DetailModalId, true, {
                closable: false,
                onShow: async function() {
                    const modalId = DetailModalId.replace('#', '');
                    $(DetailModalId)
                        .find('.content')
                        .removeClass('scrolling');
                    $(DetailModalId)
                        .find('.tabs')
                        .find('.item')
                        .tab();
                    if (type == MediaType.VIDEO) {
                        const _videoPlayer = new videoPlayer(`div[name='video']`, '#videoMenu', '#fullScreenContainer');
                        tempplayer = _videoPlayer;
                        _videoPlayer.Load(
                            <string>$("div[name='video']").attr('data-url'),
                            <string>$("div[name='video']").attr('data-placeholder')
                        );
                        const $cards: JQuery<HTMLElement> = $(DetailModalId).find('.cards');
                        const cardsData: Array<KeyFrameDataModel> = JSON.parse($cards.attr('data-cards'));
                        if (cardsData.length > 0) {
                            const fragment: DocumentFragment = document.createDocumentFragment();
                            for (const Data of cardsData) {
                                const card = UI.KeyFrame.KeyFrameCard(Data, false, false, false);
                                fragment.appendChild(card);
                            }
                            $cards.empty().append(fragment);
                        } else {
                            $cards
                                .empty()
                                .append(
                                    UI.Error.CorrectSegment(null, '小秘訣:可以去【主題與檔案】新增關鍵影格').outerHTML
                                );
                        }
                        /**點擊影格*/
                        $cards.on('click', '.ui.card:not(.placecard)', function(e) {
                            if ((<string>e.target.nodeName).toLowerCase() !== 'button') {
                                const video: HTMLVideoElement = document.getElementById(modalId).querySelector('video');
                                const duration = video.duration;
                                const $rangeSlider: HTMLInputElement = document
                                    .getElementById(modalId)
                                    .querySelector("input[type='range']");
                                const $currentTimer: HTMLSpanElement = document
                                    .getElementById(modalId)
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
                        /*檢視詳細*/
                        $("input[name='showtype']").change(function() {
                            if ($(this).val() == YesNo.是) {
                                $cards.find('.card .content,.card .description').show();
                            } else {
                                $cards.find('.card .content,.card .description').hide();
                            }
                        });
                    }
                    if (type == MediaType.AUDIO) {
                        const _audioPlayer = new audioPlayer("div[name='audio']");
                        tempplayer = _audioPlayer;
                        _audioPlayer.Load(
                            <string>$("div[name='audio']").attr('data-url'),
                            <string>$("div[name='audio']").attr('data-placeholder')
                        );
                    }
                    if (type == MediaType.VIDEO || type == MediaType.AUDIO) {
                        const $paragraph: JQuery<HTMLElement> = $(DetailModalId).find('div.cuslist');
                        try {
                            const paragraphData: Array<ParaDescriptionModel> = JSON.parse($paragraph.attr('data-para'));
                            if (paragraphData.length > 0) {
                                const fragment: DocumentFragment = document.createDocumentFragment();
                                for (const Data of paragraphData) {
                                    const paragraph = UI.Paragraph.ParagraphItem(Data, false, false);
                                    fragment.appendChild(paragraph);
                                }
                                $paragraph.empty().append(fragment);
                            } else {
                                $paragraph
                                    .empty()
                                    .append(
                                        UI.Error.CorrectSegment(null, '小秘訣:可以去【主題與檔案】新增段落描述')
                                            .outerHTML
                                    );
                            }
                        } catch (error) {
                            Logger.error(error);
                            $paragraph.empty().append(UI.Error.ErrorSegment().outerHTML);
                        }
                        /*段落點擊以撥放音頻*/
                        $paragraph.on('click', '.item', function(e) {
                            if ((<string>e.target.nodeName).toLowerCase() !== 'button') {
                                const totalseconds: number = Number($(this).attr('data-BegTime'));
                                const video: HTMLVideoElement = document.getElementById(modalId).querySelector('video');
                                const duration = video.duration;
                                const $rangeSlider: HTMLInputElement = document
                                    .getElementById(modalId)
                                    .querySelector("input[type='range']");
                                const $currentTimer: HTMLSpanElement = document
                                    .getElementById(modalId)
                                    .querySelector("span[name='currentTimer']");
                                video.currentTime = totalseconds;
                                $rangeSlider.value = ((totalseconds * 100) / duration).toString();
                                $currentTimer.innerHTML = CurrentTimeToTimeCode(totalseconds, 'second');
                            }
                        });
                    }
                    if (type == MediaType.Doc || type == MediaType.PHOTO) {
                    }
                },
                onHide: function() {
                    if (type == MediaType.VIDEO || type == MediaType.AUDIO) {
                        // tempplayer.Pause();
                        tempplayer.Destory();
                    }
                },
            });
        } else {
            Logger.error('顯示檔案的影音圖文燈箱發生錯誤');
            ErrorMessage('顯示檔案的詳細內容發生錯誤');
        }
    });
}
