import { GetImage } from './ImageTemp';
import { CurrentTimeToTimeCode, KeyTimeToSeconds } from '../Function/Frame';
import { ParaDescriptionModel } from '../Interface/Subject/ParaDescriptionModel';
import { KeyFrameDataModel } from '../Interface/Subject/KeyFrameDataModel';
import { IsNULLorEmpty } from '../Function/Check';
import { Color } from '../Enum/ColorEnum';

export namespace UI {
    export class Error {
        static ErrorSegment(content?: string, subcontent?: string): HTMLDivElement {
            const segment = document.createElement('div');
            segment.className = 'ui basic padded segment center aligned';
            segment.style.minHeight = '100%';
            segment.style.width = '100%';
            content = IsNULLorEmpty(content) ? 'Oops,載入資料發生錯誤' : content;
            subcontent = IsNULLorEmpty(subcontent) ? '看起來發生了一些狀況...' : subcontent;
            segment.innerHTML = `<div class="ui icon inverted red large header" style="margin:20px;">
                                  <i class="exclamation circle icon"></i>
                                  <div class="content">
                                    ${content}
                                      <div class="sub header">${subcontent}</div>
                                  </div>
                              </div>`;
            return segment;
        }
        static CorrectSegment(
            content?: string,
            subcontent?: string,
            iconClass?: string,
            color?: Color
        ): HTMLDivElement {
            const segment = document.createElement('div');
            segment.className = 'ui basic padded segment center aligned';
            segment.style.minHeight = '100%';
            segment.style.width = '100%';
            iconClass = IsNULLorEmpty(iconClass) ? 'database' : iconClass;
            color = IsNULLorEmpty(color) ? Color.黃 : color;
            content = IsNULLorEmpty(content) ? ' 目前沒有資料' : content;
            subcontent = IsNULLorEmpty(subcontent) ? '~請嘗試調整您的搜索查詢，然後重試~' : subcontent;
            segment.innerHTML = `<div class="ui icon inverted ${color} large header" style="margin:20px;">
                                  <i class="${iconClass} circle icon"></i>
                                  <div class="content">
                                    ${content}
                                      <div class="sub header">${subcontent}</div>
                                  </div>
                              </div>`;
            return segment;
        }
    }
    export class KeyFrame {
        /**
         * 關鍵影格
         * @param input 關鍵影格資訊
         * @param openRepresent 啟用設定代表圖按鈕
         * @param openEdit 啟用編輯按鈕
         * @param openDelete 啟用刪除按鈕
         */
        static KeyFrameCard(
            input: KeyFrameDataModel,
            openRepresent: boolean,
            openEdit: boolean,
            openDelete: boolean
        ): HTMLDivElement {
            const formatKeyTime = CurrentTimeToTimeCode(KeyTimeToSeconds(input.Time), 'second');
            const card = document.createElement('div');
            card.className = 'ui card';
            card.setAttribute('data-time', input.Time);
            const editbtn = openEdit
                ? ` <button class="ui icon black button" data-fileno="${input.fsFILE_NO}" data-time="${input.Time}" type="button" name="edit" data-inverted="" data-tooltip="編輯影格" data-position="top center"><i class="edit icon"></i></button>`
                : '';
            const deletebtn = openDelete
                ? ` <button class="ui icon black button" data-fileno="${input.fsFILE_NO}" data-time="${input.Time}"  type="button" name="delete"  data-inverted="" data-tooltip="刪除影格" data-position="top center"><i class="delete icon"></i></button>`
                : '';
            const representbtn =!openRepresent?"":
               input.IsHeadFrame
                ? ` <button name="cancelRepresent" data-fileno="${input.fsFILE_NO}" data-time="${input.Time}" type="button" class="ui  icon red button" data-inverted="" data-tooltip="代表圖" data-position="top center"><i class="icon star"></i></button>`
                : `<button name="setRepresent" type="button" data-fileno="${input.fsFILE_NO}" data-time="${input.Time}" class="ui icon black button"  data-inverted="" data-tooltip="設為代表圖" data-position="top center"><i class="icon star outline"></i></button>`;
            card.innerHTML = `  <div class="image">
                               ${GetImage(input.ImageUrl, input.Title)}
                                <span class="ui bottom right attached label" name="format">${formatKeyTime}</span>
                              </div>
                               <div class="content" style="display:none;">
                                <div class="meta">${input.Title}</div>
                                <div class="description">${input.Description} </div>
                              </div>
                              <div class="ui bottom attached mini buttons">${editbtn}${deletebtn} ${representbtn} </div> `;
            return card;
        }
    }
    export class Paragraph {
        /**
         * 產生單筆段落描述UI
         * @param input 段落描述資訊
         * @param openEdit 啟用編輯按鈕
         * @param openDelete 啟用刪除按鈕
         */
        static ParagraphItem(input: ParaDescriptionModel, openEdit: boolean, openDelete: boolean): HTMLDivElement {
            const item = document.createElement('div');
            item.className = 'item';
            item.setAttribute('data-BegTime', input.BegTime.toString());
            item.setAttribute('data-EndTime', input.EndTime.toString());
            item.setAttribute('data-SeqNo', input.SeqNo.toString());
            const editbtn = openEdit
                ? `<button type="button" class="ui _darkGrey right floated mini button"  name="edit" data-Id="${input.fsFILE_NO}"><i class="edit icon"></i>修改</button>`
                : '';
            const deletebtn = openDelete
                ? ` <button type="button" class="ui _darkGrey right floated mini button"  name="delete" data-Id="${input.fsFILE_NO}"><i class="delete icon"></i>刪除</button>`
                : '';
            item.innerHTML = `<div class="content">
                             <div class="header">
                             ${CurrentTimeToTimeCode(input.BegTime, 'second')}
                             ~
                             ${CurrentTimeToTimeCode(input.EndTime, 'second')}
                             </div>
                             <p>${input.Description}</p>
                             <div class="extra"> ${editbtn}${deletebtn} </div>
                           </div>`;
            return item;
        }
    }
}
