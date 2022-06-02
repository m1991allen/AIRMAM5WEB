
declare namespace FormanticUI{
    interface Calendar{
      settings:CalendarSettings;
      (behavior: 'refresh'): JQuery;
      (behavior:'popup',mode:'show'|'hide'):JQuery;//???
      (behavior: 'focus'): JQuery;
      (behavior: 'blur'): JQuery;
      (behavior: 'clear'): JQuery;
      (behavior: 'get date'): JQuery;
      (behavior: 'set date',updateInput:boolean,fireChange:boolean): JQuery;
      (behavior:'get mode'):string|'year'|'month'|'day'|'hour'|'minute';
      (behavior: 'set mode',mode:'year'|'month'|'day'|'hour'|'minute'): JQuery;
      (behavior: 'get startDate'): JQuery;
      (behavior: 'set startDate',date:Date|string): JQuery;
      (behavior: 'get endDate'): JQuery;
      (behavior: 'set endDate',date:Date|string): JQuery;
      (behavior: 'get focusDate'): JQuery;
      (behavior: 'set focusDate',date:Date|string): JQuery;
      (behavior: 'set minDate',date:Date|string): JQuery;
      (behavior: 'set maxDate',date:Date|string): JQuery;
      (settings?: CalendarSettings): JQuery;
    }
    type CalendarSettings=CalendarSettings.Param;
    namespace  CalendarSettings{
      type Param =(
        Pick<_Impl,'onBeforeChange'>|
        Pick<_Impl,'onChange'>|
        Pick<_Impl, 'onShow'> |
        Pick<_Impl, 'onVisible'> |
        Pick<_Impl, 'onHide'> |
        Pick<_Impl, 'onHidden'> |
        Pick<_Impl, 'onSelect'> &
        Partial<Pick<_Impl, keyof _Impl>>
      );
      interface _Impl {
        type?: "datetime"|"date"|"time"|"month"|"year";     // picker type, can be 'datetime', 'date', 'time', 'month', or 'year'
        firstDayOfWeek?: number;    // day for first day column (0 = Sunday)
        constantHeight?:boolean; // add rows to shorter months to keep day calendar height consistent (6 rows)
        today?:boolean;      // show a 'today/now' button at the bottom of the calendar
        closable?:boolean;     // close the popup after selecting a date/time
        monthFirst?:boolean;// month before day when parsing/converting date from/to text
        touchReadonly?: boolean;  // set input to readonly on touch devices
        inline?:boolean;        // create the calendar inline instead of inside a popup
        on?: null|"focus"|"click"; 
        disabledDates?:Array<{date: Date, message: string}>;
        disabledDaysOfWeek?:Array<number>;//number只能是0~6,0=週日,1=周一
        enabledDates?:Array<Date>; // when to show the popup (defaults to 'focus' for input, 'click' for others)
        initialDate?: null|"now"|string;    // date to display initially when no date is selected (null = now)
        eventDates?:null|Array<{date:string|Date;message:string;class: string;}>;
        eventClass?:string;
        startMode?:boolean;  // display mode to start in, can be 'year', 'month', 'day', 'hour', 'minute' (false = 'day')
        minDate?: null|Date|string;     // minimum date/time that can be selected, dates/times before are disabled
        maxDate?:  null|Date|string;        // maximum date/time that can be selected, dates/times after are disabled
        ampm?:boolean;          // show am/pm in time mode
        disableYear?: boolean; // disable year selection mode
        disableMonth?:boolean; // disable month selection mode
        disableMinute?:boolean; // disable minute selection mode
        formatInput?: boolean;   // format the input text upon input blur and module creation
        startCalendar?: null|JQuery;  // jquery object or selector for another calendar that represents the start date of a date range
        endCalendar?: null|JQuery;   // jquery object or selector for another calendar that represents the end date of a date range
        multiMonth?:number;      // show multiple months when in 'day' mode
        minTimeGap?:5|10|15|20|30;//最小時間間隔，只能為5、10、15、20、30
        currentCentury?:number;//世紀,例如:1800,1900,2000,3000
        centuryBreak?:number;//世紀要加到2位數的年份（00到centuryBreak-1),例如 currentCentury=1800,centuryBreak=40-> 1800-1839
        selectAdjacentDays?:boolean;//可以選擇上個月和下個月的相鄰日期  
        showWeekNumbers?:boolean;//在左側顯示週數
        text?: {
            days:Array<string>;
            months: Array<string>;
            monthsShort:Array<string>;
            today:string;
            now:string;
            am: string;
            pm: string;
        },
        formatter?: {
            header?(date:Date,mode:string,settings:any):any;
            yearHeader?(date:Date,settings:any):any;
            monthHeader?(date:Date,settings:any):any;
            dayHeader?(date:Date,settings:any):any;
            hourHeader?(date:Date,settings:any):any;
            minuteHeader?(date:Date,settings:any):any;
            dayColumnHeader?(date:Date,settings:any):any;
            datetime?(date:Date,settings:any):any;
            date?(date:Date,settings:any):any;
            time?(date:Date,settings:any,forCalendar:any):any;
            today?(settings:any):any;
            cell?(cell:any, date:Date, cellOptions:{ mode: string, adjacent: boolean, disabled: boolean, active: boolean, today: boolean }):any;
          },
        className?: {
            calendar:string;
            active:string;
            popup:string;
            grid: string;
            column:string;
            table: string;
            prev: string;
            next:string;
            prevIcon:string;
            nextIcon:string;
            link: string;
            cell:string;
            disabledCell: string;
            adjacentCell:string;
            activeCell: string;
            rangeCell: string;
            focusCell:string;
            todayCell: string;
            today: string;
        }
            
        selector?: {
            popup?:string;
            input?:string;
            activator?:string;
        },
        regExp?: {
          dateWords?:any;
          dateNumbers?: any;
        },
        error?: {
          popup?: string;
          method?:string;
        },
        parser?: {
            date?(text, settings):void;
          },
          onBeforeChange?(date:Date|null, text:string, mode):string|boolean|Date;
          onChange?(date:Date|null, text:string, mode):string|boolean|Date;
          onShow?():void;
          onVisible?():void;
          onHide?():void;
          onHidden?():void;
          isDisabled?(date:Date|null, mode):boolean;
          onSelect?(date:Date|null, text:string):string|boolean|Date;
      }
    }
  }
  
  
  
// TypeScript Version: 0.1
/// <reference types="jquery" />
/// <reference path="global.d.ts" />
declare const calendar:FormanticUI.Calendar;
interface JQuery {
  calendar:FormanticUI.Calendar;
}
