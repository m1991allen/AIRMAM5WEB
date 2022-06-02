import { ReportController } from '../../Models/Controller/ReportController';
import { sdateId, edateId, SearchFormId } from '../../Models/Const/Const.';
import { dayjs, setCalendar,SetDate } from '../../Models/Function/Date';
import { ReportParameterModel } from '../../Models/Interface/Report/ReportParameterModel';
import { FormValidField } from '../../Models/Const/FormValid';
import { ReportMessageSetting } from '../../Models/MessageSetting';
import { CheckForm } from '../../Models/Function/Form';
import { ErrorMessage, SuccessMessage } from '../../Models/Function/Message';
import { ReportResponseModel } from '../../Models/Interface/Report/ReportResponseModel';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { Logger } from '../../Models/Class/LoggerService';
/*===============================================================*/
/*宣告變數*/
const route = new ReportController();
const valid = FormValidField.Report;
const message = ReportMessageSetting;
const $sdate: JQuery<HTMLInputElement> = $(sdateId);
const $edate: JQuery<HTMLInputElement> = $(edateId);
const $RptItem: JQuery<HTMLInputElement> = $('#rptItem');
const $Report: JQuery<HTMLDivElement> = $('#reportView');
const noURLHtml: string = ` <div class="ui basic padded segment center aligned" style="min-height:100%;width:100%;">
                         <div class="ui icon inverted yellow large header" style="margin:20px;">
                            <i class="database icon"></i>
                            <div class="content">
                                找不到報表路徑
                                <div class="sub header">~嘗試重整頁面或像管理員反應~</div>
                            </div>
                         </div>
                         </div>`;
const errorURLHtml: string = `<div class="ui basic padded segment center aligned" style="min-height:100%;width:100%;">
                          <div class="ui icon inverted yellow large header" style="margin:20px;">
                             <i class="database icon"></i>
                             <div class="content">
                                 查詢報表發生錯誤
                                 <div class="sub header">~嘗試重整頁面或像管理員反應~</div>
                             </div>
                          </div>
                          </div>`;
const Search = (input: ReportParameterModel) => {
    route
        .Search(input)
        .then(res => {
            if (res.IsSuccess) {
                SuccessMessage(res.Message);
                const data = <ReportResponseModel>res.Data;
                if (IsNULLorEmpty(data.rptUrl)) {
                    $Report.html(noURLHtml);
                } else {
                    $Report.html(`<iframe height="100%" src="${data.rptUrl}"></iframe>`);
                }
            } else {
                ErrorMessage(res.Message);
            }
        })
        .catch(error => {
            Logger.viewres(route.api.Search, '查詢', error, true);
        });
};
/*預設查詢日期*/
SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY/MM/DD');
SetDate(edateId, dayjs(), 'YYYY/MM/DD');
setCalendar('.calendar', 'date');
/**表單清空 */
$("button[name='reset']").click(function() {
    SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY/MM/DD');
    SetDate(edateId, dayjs(), 'YYYY/MM/DD');
});
/*頁面載入查詢*/
Search({
    StartDate: <string>$sdate.val(),
    EndDate: <string>$edate.val(),
    RptItem: <string>$RptItem.val(),
});
/*查詢*/
$(SearchFormId).submit(function(event) {
    event.preventDefault();
    const IsFormValid: boolean = CheckForm(SearchFormId, valid.Search);
    if (IsFormValid) {
        const sdate = dayjs($sdate.val());
        const edate = dayjs($edate.val());
        if (edate.isBefore(sdate)) {
            $(this).addClass('error');
            ErrorMessage('查詢日期的起訖範圍錯誤');
        } else {
            Search({
                StartDate: <string>$sdate.val(),
                EndDate: <string>$edate.val(),
                RptItem: <string>$RptItem.val(),
            });
        }
    }
});
