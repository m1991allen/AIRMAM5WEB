import { PasswordVisible } from '../../Models/Function/Password';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { GetUrl } from '../../Models/Function/Url';
import { Controller } from '../../Models/Enum/Controller';
import { Action } from '../../Models/Enum/Action';
import { CheckForm } from '../../Models/Function/Form';
import { CreateFormId } from '../../Models/Const/Const.';
import { FormValidField } from '../../Models/Const/FormValid';
import { RemoveLeaveConfirm, SetLeaveConfirm } from '../Shared/_windowParameter';

/**因為頁面表單要用MVC token認證,所以要宣告window方法給AjaxForm表單呼叫 */
declare global {
    interface Window {
        /**變更密碼表單提交事件 */
        OnBegin: (e: any) => void;
        /**變更密碼成功事件 */
        OnSuccess: (e: any) => void;
        /**變更密碼失敗事件 */
        OnFailure: (e: any) => void;
    }
}
const valid = FormValidField.User;
const $form = $(CreateFormId);

/**變更表單時關閉視窗提示 */
$form
    .on('input', 'input,textarea', function() {
        SetLeaveConfirm(window.location.href);
    })
    .on('change', 'input,textarea,select', function() {
        SetLeaveConfirm(window.location.href);
    });
/**清除表單後也要清除視窗提示 */
$('button[type="reset"]').click(function() {
    RemoveLeaveConfirm(window.location.href);
});
/**讓密碼是否可見 */
$('i.eye').click(function() {
    const input = $(this).siblings('input');
    const id = input.attr('id');
    if ($(this).hasClass('slash')) {
        $(this).removeClass('slash');
    } else {
        $(this).addClass('slash');
    }
    PasswordVisible(id);
});
/**表單提交前事件 */
window.OnBegin = function(e) {
    const IsValid: boolean = CheckForm(CreateFormId, valid.ChangePassword);
    return IsValid;
};
/**變更成功處理 */
window.OnSuccess = function(e) {
    if (e.IsSuccess) {
        SuccessMessage('密碼已變更，請重新登入');
        const api = GetUrl(Controller.Account, Action.LogOff).href;
        const token = <string>$('input[name="__RequestVerificationToken"]', $form).val();
        $.ajax({
            type: 'POST',
            url: api,
            data: {
                __RequestVerificationToken: token,
            },
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            complete: function() {
                RemoveLeaveConfirm(window.location.href);
                $form
                    .trigger('reset')
                    .find('button')
                    .addClass('disabled');
                window.top.location.replace(GetUrl(Controller.Account, Action.Login).href);
            },
        });
    } else {
        ErrorMessage(e.Message);
    }
};
/**變更失敗處理 */
window.OnFailure = function(e) {
    ErrorMessage(e.Message);
};
