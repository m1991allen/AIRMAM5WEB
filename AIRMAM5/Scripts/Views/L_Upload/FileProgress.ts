import { IsNULLorEmpty } from '../../Models/Function/Check';
import { Color } from '../../Models/Enum/ColorEnum';
import { WorkStatus } from '../../Models/Enum/WorkStatus';
import { WorkProgressEasyChineseStatus } from '../../Models/Enum/WorkProgressStatus';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { Label } from '../../Models/Templete/LabelTemp';

/**
 * 依照進度回傳進度條Html
 * @param progress 進度,例如:46%
 * @param statusname 進度名稱,例如:轉檔完成
 */
export const GetProgressBarHtml = (workid: number, progress: string, statusname: string, colorType: string): string => {
    const progressPercent: number = IsNULLorEmpty(progress) ? 0 : Number(progress.replace('%', ''));
    statusname = IsNULLorEmpty(progress) ? '無資料' : statusname;
    const progressbar =
        `<div class="ui active inverted small ${colorType} progress" data-value="${progressPercent}" data-total="100" name="progress${workid}">` +
        ` <div class="bar" style="width:${progressPercent}%;"> <div class="progress">${progress}</div> </div><div class="label">${statusname}</div>`;
    return progressbar;
};
/**
 * 依照工作狀態取得標籤Html
 * @param status
 */
export const GetWorkStatusLabelHtml = (status: WorkStatus): string => {
    switch (status) {
        case WorkStatus.WaitForVerify:
        case WorkStatus.ConfirmFileStatus:
        case WorkStatus.VerifyReject:
        case WorkStatus.VerifyTapeStatus:
        case WorkStatus.VerifyOnTape:
        case WorkStatus.TapeBacktracking:
        case WorkStatus.OnSchedule:
        case WorkStatus.TransferToAP:
            const notyet = getEnumKeyByEnumValue(WorkProgressEasyChineseStatus, WorkProgressEasyChineseStatus.尚未開始);
            return Label(notyet, Color.灰);
        case WorkStatus.GearshiftProgramInit:
        case WorkStatus.InTransition:
        case WorkStatus.HighResolution:
        case WorkStatus.LowResolution:
        case WorkStatus.WaitForDownloading:
        case WorkStatus.DownloadingFromClound:
            const onprogress = getEnumKeyByEnumValue(
                WorkProgressEasyChineseStatus,
                WorkProgressEasyChineseStatus.正在執行
            );
            return Label(onprogress, Color.藍);
        case WorkStatus.TranscodingComplete:
            const done = getEnumKeyByEnumValue(WorkProgressEasyChineseStatus, WorkProgressEasyChineseStatus.已完成);
            return Label(done, Color.綠);
        case WorkStatus.CancelSchedule:
            const cancel = getEnumKeyByEnumValue(WorkProgressEasyChineseStatus, WorkProgressEasyChineseStatus.取消);
            return Label(cancel, Color.紅);
        case WorkStatus.ErrorUpdateDataBase:
        case WorkStatus.ErrorFileNotExist:
        case WorkStatus.ErrorDeleteFile:
        case WorkStatus.ErrorBuildDirectory:
        case WorkStatus.ErrorTransition:
        case WorkStatus.ErrorFilmVerifyFail:
        case WorkStatus.ErrorE6:
        case WorkStatus.ErrorE7:
        case WorkStatus.ErrorE8:
            const error = getEnumKeyByEnumValue(WorkProgressEasyChineseStatus, WorkProgressEasyChineseStatus.錯誤);
            return Label(error, Color.紅);
        default:
            const other = getEnumKeyByEnumValue(
                WorkProgressEasyChineseStatus,
                WorkProgressEasyChineseStatus.其他工作狀態
            );
            return Label(other, Color.紫);
    }
};
