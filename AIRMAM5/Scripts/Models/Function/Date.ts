import * as dayjs_ from 'dayjs';
import { SetLeaveConfirm } from '../../Views/Shared/_windowParameter';

import { SearchFormId } from '../Const/Const.';
/*===============================================================*/
/*宣告變數*/
export const dayjs = (<any>dayjs_).default || dayjs_;

/**
 * 時間格式化
 * @param jsondate JSON日期,格式為/Date()/
 * @param type 格式
 */
export function JsonDateToDate(jsondate: string): Date | null {
    let date: Date;
    try {
        if (jsondate == '' || jsondate == null) {
            return null;
        }
        date = eval(jsondate.replace(/\/Date\((.*?)\)\//gi, 'new Date($1)'));
        // date = new Date(parseInt(jsondate.replace('/Date(', '').replace(')/', ''), 10));
    } catch (ex) {
        console.error(`formateDate error:傳入的值=${jsondate},轉換的值=${date}`);
    }
    return date;
}

/**
 * 日曆設置(且變更時會呼叫SetLeaveConfirm)
 * @param calendarId 日曆選擇器
 * @param value 設置的初始日期
 */
export function setCalendar(
    calenderId: string,
    type: 'datetime' | 'date'|'date2' | 'month' | 'year',
    minDate?: Date,
    maxDate?: Date
) {
    const formatter = GetFormatter(type);
    $(calenderId).calendar({
        type: type==='date2'?'date':type, // 選擇器類型，可以是'datetime', 'date', 'time', 'month', 或 'year'
        firstDayOfWeek: 0, //  第一天欄的日期（0 =星期日）
        constantHeight: true, // 將行添加到較短的月份中，以保持日曆高度一致（6行）
        today: true, // 在日曆底部顯示“今天/現在”按鈕
        closable: true, //選擇日期/時間後關閉彈出窗口
        monthFirst: true, // month before day when parsing/converting date from/to text
        touchReadonly: true, // 在觸摸設備上輸入為只讀
        inline: false, // 內聯創建日曆，而不是在彈出窗口中創建日曆
        on: null, // 什麼時候顯示彈出窗口（輸入默認為'focus'，其他默認為'click'）
        initialDate: null, //  沒有選擇日期時最初顯示的日期（null =現在）
        eventDates: [
            {
                date: new Date(),
                message: '今天',
                class: 'green',
            },
        ],
        startMode: false, //開始顯示的模式，可以是 'year', 'month', 'day', 'hour', 'minute' (false = 'day')
        minDate: null, // 可以選擇的最小日期/時間，之前的日期/時間被禁用
        maxDate: null, //  可以選擇的最大日期/時間，之後的日期/時間被禁用
        ampm: true, // 在時間模式下顯示上午/下午(am/pm)
        disableYear: false, // 禁用年份選擇模式
        disableMonth: false, //禁用月份選擇模式
        disableMinute: true, // 禁用分鐘選擇模式
        formatInput: true, // 在輸入模糊(blur)和模塊(module)創建時格式化輸入文本
        startCalendar: null, //代表日期範圍開始日期的另一個日曆的jquery對像或選擇器
        endCalendar: null, //表示日期範圍結束日期的另一個日曆的jquery對像或選擇器
        multiMonth: 1, // 在'day'模式下顯示多個月
        formatter: formatter,

        onChange: function(date, text) {
            const formId =
                '#' +
                    $(calenderId)
                        .closest('form')
                        .attr('id') || '';
            if (formId !== SearchFormId) {
                SetLeaveConfirm(window.location.href);
            }
            switch (type) {
                case 'date':
                    return dayjs(date).format('YYYY/MM/DD');
                case 'date2':
                    return dayjs(date).format('YYYY-MM-DD');
                case 'datetime':
                    return dayjs(date).format('YYYY/MM/DD  HH:mm:ss');
                case 'month':
                    return dayjs(date).format('MM');
                case 'year':
                    return dayjs(date).format('YYYY');
            }
        },
        text: {
            days: ['日', '一', '二', '三', '四', '五', '六'],
            months: [
                '一月',
                '二月',
                '三月',
                '四月',
                '五月',
                '六月',
                '七月',
                '八月',
                '九月',
                '十月',
                '十一月',
                '十二月',
            ],
            monthsShort: [
                '一月',
                '二月',
                '三月',
                '四月',
                '五月',
                '六月',
                '七月',
                '八月',
                '九月',
                '十月',
                '十一月',
                '十二月',
            ],
            today: '今天',
            now: '現在',
            am: 'AM',
            pm: 'PM',
        },
        regExp: {
            dateWords: /[^A-Za-z\u00C0-\u024F]+/g,
            dateNumbers: /[^\d:]+/g,
        },

        error: {
            popup: '此頁面未包含必需的日期組件',
            method: '日期組件的方法未定義',
        },
        className: {
            calendar: 'calendar',
            active: 'active',
            popup: 'ui popup',
            grid: 'ui equal width grid',
            column: 'column',
            table: 'ui celled center aligned unstackable table',
            prev: 'prev link',
            next: 'next link',
            prevIcon: 'chevron left icon',
            nextIcon: 'chevron right icon',
            link: 'link',
            cell: 'link',
            disabledCell: 'disabled',
            adjacentCell: 'adjacent',
            activeCell: 'active',
            rangeCell: 'range',
            focusCell: 'focus',
            todayCell: 'today',
            today: 'today link',
        },
    });
}

export function GetFormatter(type: 'datetime' | 'date'|'date2' | 'month' | 'year'): object {
    switch (type) {
        case 'date':
            return {
                date: function(date, settings) {
                    if (!date) return '';
                    return dayjs(date).format('YYYY/MM/DD');
                },
            };
        case 'date2':
            return {
                date: function(date, settings) {
                    if (!date) return '';
                    return dayjs(date).format('YYYY-MM-DD');
                },
            };
        case 'datetime':
            return {
                datetime: function(date, settings) {
                    if (!date) return '';
                    return dayjs(date).format('YYYY/MM/DD HH:mm:ss');
                },
            };
        //  case "time":
        //     return {
        //       time: function (date, settings, forCalendar) {
        //         if (!date) return '';
        //           return dayjs(date).format("HH:mm:ss");
        //       }
        //   };
        //  case "today":
        //     return {
        //       today: function (settings) {
        //           return settings.type === 'date' ? settings.text.today : settings.text.now;
        //       }
        //   };
    }
}

/**
 * 比較兩日期間距
 * @param sdate 開始時間
 * @param edate 結束時間
 * @param type 比較類型
 */
export function CompareDateRange(
    sdate: string,
    edate: string,
    type: 'month' | 'day' | 'minute' | 'second'
): { type: 'month' | 'day' | 'minute' | 'second' | 'error'; value: number } {
    const isDate: boolean = dayjs.isDate(sdate) && dayjs.isDate(edate);
    if (isDate) {
        const start = dayjs(sdate);
        const end = dayjs(edate);
    } else {
        return { type: 'error', value: -1 };
    }
}

/**
 * 秒數轉為時間(HH:mm:ss)格式
 * @param seconds
 */
export function SecondsToHHMMSS(seconds: number): string {
    if (seconds <= 0 || Number.isNaN(seconds)) {
        return '00:00:00';
    } else {
        let date = new Date(null);
        date.setSeconds(seconds);
        return date.toISOString().substr(11, 8);
    }
}

/**檢查日期格式 */
export function ValidDate(date:string,format:string):boolean{
    return dayjs(date, format).format(format) === date;
}

/**
 * 設置日期
 * @param inputId 選擇器Id
 * @param initDate 設置值
 * @param format 格式
 */
export function SetDate(inputId: string, initDate: Date, format: 'YYYY/MM/DD HH:mm:ss' | 'YYYY/MM/DD'|'YYYY-MM-DD') {
    switch (format) {
        case 'YYYY-MM-DD':
        case 'YYYY/MM/DD':
            $(inputId).val(dayjs(initDate).format(format));
            break;
        case 'YYYY/MM/DD HH:mm:ss':
        default:
            $(inputId).val(dayjs(initDate).format('YYYY/MM/DD HH:mm:ss'));
            break;
    }
}