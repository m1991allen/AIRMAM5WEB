import { Controller } from '../Enum/Controller';
import { Action } from '../Enum/Action';
import { Get, Post, Ajax } from './Ajax';
import { setCalendar } from './Date';
import { IResponse } from '../Interface/Shared/IResponse';
import { IsNULLorEmpty, IsNULL, IsNullorUndefined } from './Check';
import { ErrorMessage } from './Message';
import { Logger } from '../Class/LoggerService';
import { RemoveLeaveConfirm, SetLeaveConfirm } from '../../Views/Shared/_windowParameter';

/**
 * 檢視燈箱
 * @param controller 控制器
 * @param action 動作
 * @param uiInit 初始化元件設定
 * @param parameters 參數
 * @param beforeSendEvent 載入事件
 * @param successEvent 成功事件
 * @param errorEvent 錯誤事件
 * @param completeEvent 完成事件
 */
export function DetailModal(
    controller: Controller,
    action: Action,
    parameters: object,
    uiInit?: { calendar?: boolean; calendarType?: 'datetime' | 'date' | 'month' | 'year'; dropdown?: boolean }
) {
    const modalId: string = '#DetailModal';
    Get(
        controller,
        action,
        parameters,
        false,
        function() {
            if ($(modalId).length > 0) {
                $(modalId).remove();
            }
        },
        function(data) {
            $('#DetailArea')
                .empty()
                .append(data);
        },
        function(error) {
            Logger.error(error);
            ErrorMessage('系統發生錯誤，無法開啟檢視視窗');
        },
        function() {
            if ($(modalId).length > 0) {
                $(modalId)
                    .modal({ closable: false })
                    .modal('show');
                if (uiInit != null && typeof uiInit !== 'undefined') {
                    if (typeof uiInit.dropdown !== 'undefined' && uiInit.dropdown) {
                        $(modalId + ' .dropdown').dropdown('refresh');
                    }
                    if (uiInit.calendar) {
                        setCalendar('.calendar', uiInit.calendarType);
                    }
                }
            }
        }
    );
}


/**
 * 產生燈箱
 * @param modalId 燈箱Id
 * @param url api路徑(呼叫View或PartialView)
 * @param parameters 參數
 * @param selector 指定燈箱塞入區域Id,若不填預設為#OtherArea
 */
export function ShowModal<T = object>(
    modalId: string,
    url: string,
    parameters?: T,
    selector?: string
): Promise<boolean> {
    return new Promise(resolve => {
        Ajax('GET', url, parameters, false)
            .then(view => {
                if (document.querySelectorAll(modalId).length > 0) {
                    $(modalId).remove();
                }
                IsNULLorEmpty(selector) ? $('#OtherArea').append(view) : $(selector).append(view);
                resolve(true);
            })
            .catch(error => {
                Logger.viewres(url, '呼叫頁面', error, false);
                resolve(false);
            });
    });
}

/**Modal事件 */
export function ModalTask(
    modalSelector: string,
    show: boolean,
    setting?: SemanticUI.ModalSettings.Param,
): JQuery<HTMLElement> {
    //setting.closable = IsNULLorEmpty(setting.closable) ? false : setting.closable;
    let newSetting: SemanticUI.ModalSettings.Param = Object.assign({}, setting);
     let isFormEditing:boolean=false;
    newSetting.closable = IsNULLorEmpty(setting.closable) ? false : setting.closable;
    newSetting.allowMultiple = IsNULLorEmpty(setting.allowMultiple) ||IsNullorUndefined(setting.allowMultiple)? true : setting.allowMultiple;
    const form=$(`${modalSelector} form`);
    newSetting.onShow = function() {   
        if (!IsNULL(setting.onShow)) {
            setting.onShow.call(this, arguments);
        }                
        form.trackEdit()
        .onEdit(function(){
            isFormEditing=true;
            SetLeaveConfirm(window.location.href);
        })
        .onCancel(function(){
            isFormEditing=false;
            RemoveLeaveConfirm(window.location.href);
        });
    };
    /*Notice:如果把 RemoveLeaveConfirm綁定在onApprove或onDeny,onHide,不知為何會影響原有return false的設定 */
    newSetting.onDeny=function(){
        if (!IsNULL(setting.onDeny)) {
            setting.onDeny.call(this, arguments);
        }
    };
    newSetting.onHidden = function() {
        form.isCancel();
        if (!IsNULL(setting.onHidden)) {
            setting.onHidden.call(this, arguments);
        }
    };
    return show
        ? $(modalSelector)
              .modal(newSetting)
              .modal('show')
        : $(modalSelector).modal(newSetting);
}
