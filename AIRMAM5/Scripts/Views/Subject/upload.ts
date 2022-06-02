import { IresumableService, resumableService, IsResumableSupport } from '../../Models/Class/resumableService';
import { MediaUploadType, MediaType } from '../../Models/Enum/MediaType';
import { SuccessMessage, ErrorMessage, WarningMessage } from '../../Models/Function/Message';
import { ModalTask } from '../../Models/Function/Modal';
import { ReplacementConfirmId, ReplacementModalId } from './metaData';
import { TitleEnum } from '../../Models/Enum/TitleEnum';
import { YesNo } from '../../Models/Enum/BooleanEnum';
import { UploadServiceConfig } from '../../Models/Interface/Subject/UploadServiceConfig';
import { SubjectTaskQuery } from '../../Models/Interface/Subject/SubjectTaskQuery';
import { KeyFrameStatus } from '../../Models/Enum/KeyFrameStatus';
import { GetMediaTypeByUploadType } from '../../Models/Function/ConvertMedia';
import { IResponse } from '../../Models/Interface/Shared/IResponse';
import { GetDropdown } from '../../Models/Function/Element';
import { Logger } from '../../Models/Class/LoggerService';
import { RemoveLeaveConfirm, SetLeaveConfirm } from '../Shared/_windowParameter';
import { setCalendar } from '../../Models/Function/Date';
import { FormValidField } from '../../Models/Const/FormValid';
import { CheckForm } from '../../Models/Function/Form';
import { IsNULLorEmpty, IsNullorUndefined } from '../../Models/Function/Check';
import { initSetting } from '../../Models/initSetting';




/*------------------變數-----------------------*/
// var selectMediaType: MediaUploadType;
/*上傳相關工作*/
class UploaduiWork{
    /**上傳按鈕 */public $UploadProgressBtn:JQuery<HTMLElement>;
    /**上傳數量 */public $UploadQuantity:JQuery<HTMLElement>;
    /**上傳視窗 */public $UploadWindow:JQuery<HTMLElement>;
    /**上傳燈箱Id*/public  UploadModalId:string;
    /**上傳燈箱內容元素*/ public $UploadWindowContent:JQuery<HTMLElement>;
    constructor(){
        this.$UploadProgressBtn=$('#UploadProgressBtn');
        this.$UploadQuantity= $('#UploadProgressBtn ._uploadQuantity');
        this.$UploadWindow=$('#UploadContent');
        this.UploadModalId='#UploadModal';
        this.$UploadWindowContent=$('#UploadContent .content');
    }
    
    /**是否顯示上傳按鈕*/
    changeWorkBtn(showWorkBtn: boolean): void{
        this.$UploadProgressBtn.css({ display:  showWorkBtn?'block':'none' });    
    }
    /**更新上傳數量標籤 */
    changeWorkCount (count: number|'+1'|'-1'): void{
        let q=Number(this.$UploadQuantity.text());
        switch(true){
            case count===null || count ===undefined || count<0:
                q=0;
                break;
            case count==='+1':
                q++;
                break;
            case count==='-1':
                q--;
                if(q<0){q=0;}
                break;
            default:
                q=<number>count;
                break;
        }
        this.$UploadQuantity.text(q);
    }
    /**產生新的工作列 */
    createWorkItem (uniqueid: string, filename: string): HTMLDivElement{
        const workitem = document.createElement('div');
        workitem.className = 'ui right aligned grid _styleLoad';
        workitem.setAttribute('data-uniqueid', uniqueid);
        workitem.innerHTML = `<span class="left aligned twelve wide column">${filename}</span>
                              <span class="center aligned two wide column _colorPerc">0%</span>
                               <a style="display:none;" class="right aligned one wide column" title="重新上傳" name="retry" data-uniqueid="${uniqueid}"  data-tooltip="重新上傳" data-position="left center" data-inverted=""><i class="undo icon"></i></a>
                               <a class="right aligned one wide column" title="取消並移除" name="cancelFile" data-uniqueid="${uniqueid}" data-tooltip="取消並移除工作" data-position="left center" data-inverted=""><i class="trash alternate outline icon"></i></a>
                               <a class="right aligned one wide column" title="刪除工作" style="display:none;" name="deleteFile" data-uniqueid="${uniqueid}"  data-tooltip="刪除工作列" data-position="left center" data-inverted=""><i class="trash alternate outline icon"></i></a>
                               <div class="ui tiny yellow active progress">
                                 <div class="bar"></div>
                               </div>`;
        return workitem;
    }
    /**產生上傳駐列的元素 */
    createUploadItem(type:MediaType|MediaUploadType,file:ResumableFile):HTMLDivElement|false{
        const item = document.createElement('div');
        const KB = ui.getfileSize(file.size, 'KB');
        switch(type){
            case MediaType.PHOTO:
            case MediaUploadType.PHOTO:
                const image: HTMLImageElement = document.createElement('img');
                image.src = URL.createObjectURL(file.file); //將圖片src改為data:image/jpeg;base64
                image.className = 'ui mini circular image';
                item.innerHTML = `<div class="item" data-uniqueid="${file.uniqueIdentifier}" >  
                                <div class="right floated content">
                                   <button class="ui basic red icon button" type="button" name="removeFile"><i class="icon close"></i></button>
                                </div>     
                                <div class="content">
                                   <div class="ui grey inverted header">
                                     ${image.outerHTML}    
                                      <div class="content">
                                       ${file.fileName}
                                       <div class="sub header">${KB}KB</div>
                                      </div>
                                   </div>                       
                                  </div>
                                </div>
                              </div>`;
                return item;
            case MediaType.VIDEO:
            case MediaType.AUDIO:
            case MediaType.Doc:
            case MediaUploadType.VIDEO:
            case MediaUploadType.AUDIO:
            case MediaUploadType.DOC:
                item.innerHTML = `<div class="item" data-uniqueid="${file.uniqueIdentifier}">
                                  <div class="right floated content">
                                     <button class="ui basic red icon button" type="button" name="removeFile"><i class="icon close"></i></button>
                                  </div>
                                  <div class="content">
                                  <div class="ui sub  yellow inverted header">${file.fileName}</div>
                                    ${KB}
                                  </div>
                                </div>`;
                return item;
            default:
                return false;
        }
    }
    /**上傳視窗中插入工作列 */
    insertWorkToWindow  (uniqueid: string, filename: string){
        const workitem = this.createWorkItem(uniqueid, filename);
        this.$UploadWindowContent.append(workitem);
    }
    /**更新工作列元素 */
    updateWindowItem(uniqueid: string,type:'success'|'suceessfail'|'error'|'cancel'|'progress'|'retry'|'delete',
                     whoisfail?:'置換'|'上傳'|'',progress?:number
    ){
       const item= this.$UploadWindow.find("div._styleLoad[data-uniqueid='" + uniqueid + "']");
       const txt=IsNullorUndefined(whoisfail)||IsNULLorEmpty(whoisfail)?"上傳":whoisfail;
       switch(type){
           case 'success':
            item.removeClass('error').addClass('success');
            item.find('._colorPerc').text(`100%`);
            item.find('.progress')
                .addClass('green')
                .removeClass('yellow red')
            item.find('a').hide();
            item.find('.progress>.bar').css({ width: '100%' });
            setTimeout(() => { item.remove();}, 500);
               break;
            case 'suceessfail':
                item.addClass('error');
                item.find('._colorPerc').text(`${txt}失敗`);
                item.find('.progress').addClass('error').removeClass('yellow');
                item.find('a[name !="deleteFile"]').hide();
                item.find('a[name="deleteFile"]').show();
                break;
            case 'progress':
                if(!IsNullorUndefined(progress)){
                    item.find('._colorPerc').text(`${progress}%`);
                    item.find('.progress>.bar').css({ width: '' + progress + '%' });
                }
                break;
            case 'error':
                item.remove('success').addClass('error');
                item.find('._colorPerc').text(`${txt}失敗`);
                item.find('.progress')
                    .addClass('red')
                    .removeClass('yellow green');
                item.find('a[name="retry"],a[name="deleteFile"]').show(); //0414
                item.find('a[name="cancelFile"]').hide();
                break;
            case 'cancel':
                item.find('._colorPerc').text(`${txt}取消`);
                item.find("a[name='cancelFile'],a[name='retry'],a[name='deleteFile']").remove(); //0414 修正
                setTimeout(() => { item.remove(); }, 500);
                break;
            case 'retry':
                item.find('a[name="retry"],a[name="deleteFile"]').show(); //0414
                break;
            case 'delete':
                item.remove();
                break;
       }
    }
    /**依照檔案取副檔名*/
    getExtensionByFileName (fileName: string):string {
        return fileName.slice(((fileName.lastIndexOf('.') - 1) >>> 0) + 2);
    }
    /**
     * 依照檔案副檔名回傳MediaUploadType
     * @param config 服務初始化所需參數
     * @param fileExtension 檔案副檔名
     */
     getMediaUploadType(config: UploadServiceConfig, fileExtension: string): MediaUploadType {
        const fileExt=fileExtension.toLowerCase();
        const v=config.AcceptExtension.find(x=>x.MediaType===MediaUploadType.VIDEO);
        const a=config.AcceptExtension.find(x=>x.MediaType===MediaUploadType.AUDIO);
        const p=config.AcceptExtension.find(x=>x.MediaType===MediaUploadType.PHOTO);
        const d=config.AcceptExtension.find(x=>x.MediaType===MediaUploadType.DOC);

        const vformat=v.FileExtension.toLowerCase().split(';').filter(x=>x);
        const aformat=a.FileExtension.toLowerCase().split(';').filter(x=>x);
        const pformat=p.FileExtension.toLowerCase().split(';').filter(x=>x);
        const dformat=d.FileExtension.toLowerCase().split(';').filter(x=>x);

        switch(true){
            case vformat.includes(fileExt):
                return MediaUploadType.VIDEO;
            case aformat.includes(fileExt):
                return MediaUploadType.AUDIO;
            case pformat.includes(fileExt):
                return MediaUploadType.PHOTO;
            case dformat.includes(fileExt):
                return MediaUploadType.DOC;
            default:
                return MediaUploadType.Unknown;
        }
    }
    /**
     * 解析各種類的副檔名並儲存至傳入的陣列
     *  @param config 服務初始化所需參數
     *  @param saveArray 要儲存的文字空陣列
     */
    ExtensionToArray(config: UploadServiceConfig, saveArray: Array<string>): Array<string> {
        for (var i = 0; i < config.AcceptExtension.length; i++) {
            const fileExtension = config.AcceptExtension[i].FileExtension;
            const ExtArray = fileExtension.split(';');
            ExtArray.forEach(function(element, index) {
                if (element.length > 0) {
                    ExtArray[index] = '.' + element;
                }
            });
            saveArray = saveArray.concat(ExtArray);
        }
        return saveArray;
    }
    /**
     * 取得檔案大小
     * @param filesize 檔案大小bytes
     * @param type 要以KB或MB顯示
     */
    getfileSize(filesize: number, type: 'KB' | 'MB'): string {
        const MB = 9.53674316 * Math.pow(10, -7) * filesize;
        const KB = (MB * 1024).toFixed(2); //取至小數後2位
        return type === 'MB' ? `${MB}MB` : `${KB}KB`;
    }
}

var ui=new UploaduiWork();

/*-----------------------------------事件---------------------------------------------*/
/**
 * 移除檔案事件
 * @param up 已經初始化的上傳服務
 * @param dropArea 要上傳的檔案列區域
 */
const RemoveEvent = (up: IresumableService, dropArea: HTMLElement): void => {
    $(dropArea).on('click', "button[name='removeFile']", function() {
        const item = $(this).closest('.item');
        const uniqueid = item.attr('data-uniqueid');
        const file = up.GetFromUniqueIdentifier(uniqueid);
        SuccessMessage(`移除檔案${file.fileName}`);
        up.RemoveFile(file);
        item.remove();
    });
};
/**
 * 檔案上傳成功事件
 * @param up 已經初始化的上傳服務
 * @param config 上傳設定檔
 * @param query 上傳參數
 */
const SuccessEvent = (
    up: IresumableService,
    config: UploadServiceConfig,
    query: SubjectTaskQuery,
    uploadWay: 'upload' | 'replacement',
    callback?:(res:IResponse)=>void
): void => {
    /**檔案上傳成功處理事件(但伺服器不一定儲存成功) */
    up.FileSuccess(function(file: ResumableFile, message: IResponse) {
        if (message.IsSuccess) {
            SuccessMessage(message.Message);
            up.RemoveFile(file); /*移除檔案*/
            ui.updateWindowItem(file.uniqueIdentifier,'success');
            if (uploadWay == 'upload') {
                const extension =ui.getExtensionByFileName(file.fileName);
                const uploadtype =ui.getMediaUploadType(config, extension);
                $(initSetting.TableId).trigger('addcount',[query.SubjId,uploadtype,1]);
            }
        } else {
            ErrorMessage(message.Message);
            ui.updateWindowItem(file.uniqueIdentifier,'suceessfail');
        }
        ui.changeWorkCount('-1');
        Logger.log(`檔案${message.IsSuccess ? '成功' : '失敗'},filename=${file.fileName},progress=${file.progress(false) *100}%` );
        try{
            if(!IsNullorUndefined(callback)){
                callback(message);
            }
        }catch(error){
            Logger.error(error);
        }
    });
};
/**
 * 檔案上傳失敗事件
 * @param up 已經初始化的上傳服務
 */
const ErrorEvent = (up: IresumableService): void => {
    up.FileError(function(file: ResumableFile, message: IResponse) {
        ErrorMessage(message.Message);
        const uniqueid = file.uniqueIdentifier;
        ui.updateWindowItem(file.uniqueIdentifier,'error','上傳');
        ui.changeWorkCount('-1');
        Logger.error(`檔案錯誤,filename=${file.fileName}`, message);
    });
};
/**
 * 進度更新事件
 * @param up 已經初始化的上傳服務
 *  */
const ProgressEvent = (up: IresumableService): void => {
    up.FileProgress(function(file) {
        const progress = (file.progress(false) * 100).toFixed(2);
        ui.updateWindowItem(file.uniqueIdentifier,'progress','上傳',Number(progress));
        Logger.log(`進度更新,filename=${file.fileName},progress=${progress}%`);
    });
};
/**
 * 檔案重轉事件
 * @param up 已經初始化的上傳服務
 */
const RetryEvent = (up: IresumableService): void => {
    ui.$UploadWindow.on('click', "a[name='retry']", function() {
        const uniqueid = $(this).attr('data-uniqueid');
        up.FileRetry(uniqueid);
        ui.updateWindowItem(uniqueid,'retry','上傳');
        ui.changeWorkCount('+1');
        Logger.log(`檔案重轉,uniqueid=${uniqueid},filename=${up.GetFromUniqueIdentifier(uniqueid).fileName}`);
    });
};
/**
 * 檔案取消上傳事件
 * @param up 已經初始化的上傳服務
 */
const CancelEvent = (up: IresumableService, subjectName: string): void => {
    ui.$UploadWindow.on('click', "a[name='cancelFile']", function() {
        const uniqueid = $(this).attr('data-uniqueid');
        const file = up.GetFromUniqueIdentifier(uniqueid);
        up.FileRemove(uniqueid);
        ui.updateWindowItem(uniqueid,'cancel','上傳');
        ui.changeWorkCount('-1');
        Logger.log(`檔案取消上傳,uniqueid=${uniqueid},filename=${up.GetFromUniqueIdentifier(uniqueid).fileName}`);
    });
};
/**刪除失敗的上傳工作列 */
const DeleteEvent = (): void => {
    ui.$UploadWindow.on('click', "a[name='deleteFile']", function() {
        const uniqueid = $(this).attr('data-uniqueid');
        ui.updateWindowItem(uniqueid,'delete','上傳');
        Logger.log(`檔案移除失敗工作列,uniqueid=${uniqueid}}`);
    });
};
/**
 * 確認上傳事件
 * @param query 上傳參數
 * @param dropArea 要上傳的檔案列區域
 */
const ConfirmUpload = (up: IresumableService, query: SubjectTaskQuery, dropArea: HTMLElement): void => {
    const mediaTypeList = GetDropdown(ui.UploadModalId, 'MediaTypeList');
    const filedate= $(ui.UploadModalId).find("input[name='DateInFileNo']");
    const titleDefine = GetDropdown(ui.UploadModalId, 'titletype');
    const cusTitle = $(ui.UploadModalId).find("input[name='custitle']");
    const arcPreList = GetDropdown(ui.UploadModalId, 'ArcPreTempList');
    const fileSecretList = GetDropdown(ui.UploadModalId, 'FileSecretList'); //Added_20200302:機密等級。
    const fileLicenseList = GetDropdown(ui.UploadModalId, 'FileLicenseList'); //Added_20210913:版權。
    ModalTask(ui.UploadModalId, true, {
        onShow: function() {
            GetDropdown(ui.UploadModalId, 'FileSecret').dropdown();
            setCalendar(`${ui.UploadModalId} .calendar`, 'date');
            GetDropdown(ui.UploadModalId, 'FileLicense').dropdown(); //Added_20210913:版權。
        },
        onApprove: function() {
           const TitleDefineValue = titleDefine.dropdown('get value');
           const PreIdValue = Number(arcPreList.dropdown('get value'));
    
           $.fn.form.settings.rules=Object.assign($.fn.form.settings.rules,{
            checkCustitle:function(param) {  
                const isCusTitleType=param ===TitleEnum.CusTitle.toString();
                const titlevalue=cusTitle.val();
                const isCusTitleValueNotEmpty=!(IsNullorUndefined(titlevalue)||IsNULLorEmpty(titlevalue));
                return  !isCusTitleType ||   (isCusTitleType && isCusTitleValueNotEmpty);
            },
            checkPre:function(param){
                const isArcPreTitleType=param ===TitleEnum.ArcPreTitle.toString();
                 return  !isArcPreTitleType || (isArcPreTitleType && PreIdValue > 0);   
            }
        });
            
            const isFormValid = CheckForm("#UploadForm",FormValidField.Subject.Upload);
                if(isFormValid){
                    up.SetQuery({
                        MediaType: mediaTypeList.dropdown('get value'),
                        DateInFileNo: <string>filedate.val(),
                        SubjId: query.SubjId,
                        LoginId: query.LoginId,
                        TitleDefine: TitleDefineValue,
                        CustomTitle: <string>cusTitle.val(),
                        FileNo: query.FileNo,
                        Folder: query.Folder,
                        PreId: PreIdValue,
                        DeleteKF: query.DeleteKF,
                        FileSecret: Number(fileSecretList.dropdown('get value')), //Added_20200302:機密等級。
                        FileLicense: <string>fileLicenseList.dropdown('get value'),  //Added_20210913:版權。
                    }).then(success => {
                        if (success) {
                            const workItems = dropArea.querySelectorAll('.item');
                            const workItemLength: number = workItems.length;
                            if (workItemLength > 0) {
                                up.Upload();
                                const nowWorkBtnCount = Number(ui.$UploadQuantity.text()) || 0;
                                const count = nowWorkBtnCount + workItemLength;
                                ui.changeWorkBtn(count > 0);
                                ui.changeWorkCount(count);
                                for (let i = 0; i < workItemLength; i++) {
                                    const uniqueid = workItems[i].getAttribute('data-uniqueid');
                                    const file = up.GetFromUniqueIdentifier(uniqueid);
                                    ui.insertWorkToWindow(file.uniqueIdentifier, file.fileName);
                                }
                                $(ui.UploadModalId).modal('hide');
                            }else{
                                WarningMessage(`請選擇至少一個上傳的檔案`);
                            }
                        } else {
                            ErrorMessage(`上傳服務參數發生錯誤`);
                        }
                    });
                }
            return false;
        },
        onDeny: function() {
            RemoveLeaveConfirm(window.location.href);
        },
    });
};
/**
 * 確認重置檔案事件
 * @param reup 已經初始化的重置服務
 * @param query 上傳參數
 * @param mediaUploadType 上傳類別
 * @param dropArea 要上傳的檔案列區域
 * @param $MetaTab Media分頁元素
 */
const ConfirmReplacement = (
    reup: IresumableService,
    query: SubjectTaskQuery,
    mediaUploadType: MediaUploadType,
    dropArea: HTMLElement,
    $MetaTab: JQuery<HTMLElement>
): void => {
    ModalTask(ReplacementConfirmId, false, {
        allowMultiple: true,
        selector: {
            close: '.actions .ok,.actions .cancel,i.close',
            deny: '.actions .cancel',
        },
        closable: false,
        detachable: false,
        observeChanges: true,
        context: $MetaTab,
        onShow: function(this: JQuery<HTMLElement>) {
            /*為了解決Modal中Modal關閉問題互相影響*/
            $(this).on('click', function(event) {
                event.stopImmediatePropagation();
            });
        },
        onApprove: function() {
            const $KeepCheckbox = $(ReplacementModalId).find("input[name='KeepMedia']:checked");
            const fileSecretList = $(ReplacementModalId).find("select[name='FileSecretList']"); //Added_20200302:機密等級。
            const fileLicenseList = $(ReplacementModalId).find("select[name='FileLicenseList']");  //Added_20210913:版權。
            reup.SetQuery({
                MediaType: mediaUploadType,
                DateInFileNo:'',
                SubjId: query.SubjId,
                LoginId: query.LoginId,
                TitleDefine: TitleEnum.DontChangeTitle,
                CustomTitle: '',
                FileNo: query.FileNo,
                Folder: query.Folder,
                PreId: query.PreId,
                DeleteKF:
                    $KeepCheckbox.val() == KeyFrameStatus.Keep &&
                    (GetMediaTypeByUploadType(mediaUploadType) == MediaType.VIDEO ||
                        GetMediaTypeByUploadType(mediaUploadType) == MediaType.AUDIO)
                        ? YesNo.否
                        : YesNo.是, //刪除所有影格與段落描述
                FileSecret: Number(fileSecretList.closest('.dropdown').dropdown('get value')), //Added_20200302:機密等級。
                FileLicense: <string>fileLicenseList.closest('.dropdown').dropdown('get value'),  //Added_20210913:版權。
            }).then(success => {
                if (success) {
                    const workItems = dropArea.querySelectorAll('.item');
                    const workItemLength: number = workItems.length;

                    if (workItemLength > 0) {
                        reup.Upload();
                        const nowWorkBtnCount = Number(ui.$UploadQuantity.text()) || 0;
                        const count = nowWorkBtnCount + workItemLength;
                        ui.changeWorkCount(count);
                        for (let i = 0; i < workItemLength; i++) {
                            const uniqueid = workItems[i].getAttribute('data-uniqueid');
                            const file = reup.GetFromUniqueIdentifier(uniqueid);
                            ui.insertWorkToWindow(file.uniqueIdentifier, file.fileName);
                        }
                    }else{
                        WarningMessage(`請選擇一個上傳的檔案`);
                    }
                } else {
                    ErrorMessage(`置換服務參數發生錯誤`);
                }
            });
        },
        onDeny: function() {
            RemoveLeaveConfirm(window.location.href);
        },
    }).modal('attach events', ReplacementModalId + ' .actions .approve', 'show');
};

/**上傳檔案任務 */
export const UploadTask = (subjectName: string, input: { config: UploadServiceConfig; query: SubjectTaskQuery }) => {
    //是否支持Resumable
    if (!IsResumableSupport()) {
        ErrorMessage('使用的瀏覽器不支援上傳元件,請嘗試使用現代瀏覽器');
        Logger.error(`resumable.js不支援`); //TODO 替代上傳的取代方法
    } else {
        //-----------上傳服務初始化-----------------------
        const up: IresumableService = new resumableService({
            api: input.config.uploadPath,
            chuckSize: input.config.chuckSize,
            fileExt: ui.ExtensionToArray(input.config, []),
            simultaneousUploads: input.config.simultaneousUploads,
            timeoutSeconds: input.config.TimeoutSec,
            maxFiles: undefined,
        });
        //--------------------------------------------------
        const browseButton: HTMLElement = document.getElementsByName('mediafile')[0]; //上傳按鈕
        const dropArea: HTMLElement = document.getElementsByName('fileList')[0]; //要上傳的檔案列區域
        up.AssignBrowse(browseButton);
        up.AssignDrop(dropArea);
        
        ConfirmUpload(up, input.query, dropArea);
        RemoveEvent(up, dropArea);
        SuccessEvent(up, input.config, input.query, 'upload');
        ErrorEvent(up);
        ProgressEvent(up);
        RetryEvent(up);
        CancelEvent(up, subjectName);
        DeleteEvent();
        //加入檔案事件
        up.FileAdded(function(file, event) {
            SetLeaveConfirm(window.location.href);
            const extension = ui.getExtensionByFileName(file.fileName); //取副檔名
            const uplaodtype= ui.getMediaUploadType(input.config,extension);
            const currentType = GetDropdown(ui.UploadModalId, 'MediaTypeList').dropdown('get value');
            if(currentType !=uplaodtype){
                WarningMessage(`${file.fileName}不符合上傳檔案類型`);
                return false;
            }
            const item=ui.createUploadItem(uplaodtype,file);
            if(item!==false){
                dropArea.appendChild(item);
            }else{
                Logger.log(`不允許上傳的檔案類型,filename=${file.fileName}`);
                up.RemoveFile(file);
            }
           
        });
        //開始上傳事件
        up.On('uploadStart', function() {
           const a=up.files();
            SetLeaveConfirm(window.location.href);
            //ui.changeWorkCount('-1');
            /*Notice:20200512會議中,主管與同仁說標題要固定為[上傳清單佇列],隨檔案狀態變動會讓使用者混淆 */
            //  $UploadWindowHeader.text(`主題【${subjectName}】上傳中...`);
        });
        //完成事件
        up.Complete(function() {
            RemoveLeaveConfirm(window.location.href);
            /*Notice:20200512會議中,主管與同仁決定標題固定為[上傳清單佇列] */
            // $UploadWindowHeader.text(`主題【${subjectName}】上傳工作已完成`);
        });
        //當偵測到下拉選單的媒體變化時要移除所有resumable檔案
        $("#MediaTypeList").on("clearfile",function(event){
            up.FileCancel();
        });
    }
};
/**重置檔案任務 */
export const ReplacementTask = (
    subjectName: string,
    $MetaTab: JQuery<HTMLElement>,
    mediaUploadType: MediaUploadType,
    input: { config: UploadServiceConfig; query: SubjectTaskQuery }
) => {
    let AddFileUnqieId: Array<string> = [];
    //-----------上傳服務初始化-----------------------
    if (!IsResumableSupport()) {
        ErrorMessage('使用的瀏覽器不支援上傳元件,請嘗試使用現代瀏覽器');
        Logger.error(`resumable.js不支援`); //TODO 替代上傳的取代方法
    } else {
        const acceptExtensions = input.config.AcceptExtension;
        const reup: IresumableService = new resumableService({
            api: input.config.uploadPath,
            chuckSize: input.config.chuckSize,
            fileExt: acceptExtensions[0].FileExtension.split(';'),
            simultaneousUploads: input.config.simultaneousUploads,
            timeoutSeconds: input.config.TimeoutSec,
            maxFiles: 1,
        });
        const browsInput = document.getElementsByName('remediafile')[0]; //上傳按鈕
        const dropArea = document.getElementsByName('refileList')[0]; //要上傳的檔案列區域
        browsInput.setAttribute('accept', acceptExtensions[0].FileExtension.replace(/;/g, ','));
        reup.AssignBrowse(browsInput);
        reup.AssignDrop(dropArea);
        ConfirmReplacement(reup, input.query, mediaUploadType, dropArea, $MetaTab);
        SuccessEvent(reup, input.config, input.query, 'replacement',function(res){
            $MetaTab.trigger('render',[input.query.FileNo]);
        });
        ErrorEvent(reup);
        ProgressEvent(reup);
        RetryEvent(reup);
        CancelEvent(reup, subjectName);
        DeleteEvent();
        //開始上傳事件
        reup.On('uploadStart', function() {
            SetLeaveConfirm(window.location.href);
            /*Notice:20200512會議中,主管與同仁決定標題固定為[上傳清單佇列] */
            //    $UploadWindowHeader.text(`主題【${subjectName}】上傳中...`);
        });
        //完成事件
        reup.Complete(function() {
            RemoveLeaveConfirm(window.location.href);
            /*Notice:20200512會議中,主管與同仁決定標題固定為[上傳清單佇列] */
            //   $UploadWindowHeader.text(`主題【${subjectName}】上傳工作已完成`);
        });
        reup.FileAdded((file, event) => {       
            $('#ReplacementForm').trigger('edit');//告知表單被編輯
            if (AddFileUnqieId.length >= 1) {
                WarningMessage('置換只能選擇一個檔案');
                return false;
            }
            else {
                SetLeaveConfirm(window.location.href);
                AddFileUnqieId.push(file.uniqueIdentifier);
                const item=ui.createUploadItem(mediaUploadType,file);
                if(item!==false){
                    dropArea.appendChild(item);
                }else{
                    Logger.log(`不允許上傳的檔案類型,filename=${file.fileName}`);
                    reup.RemoveFile(file);
                }
            }
        });
        //移除檔案事件
        $(dropArea).off('click').on('click', "button[name='removeFile']", function() {
            const item = $(this).closest('.item');
            const uniqueid = item.attr('data-uniqueid');
            const file = reup.GetFromUniqueIdentifier(uniqueid);
            AddFileUnqieId = AddFileUnqieId.filter(x => x != uniqueid);
            SuccessMessage(`移除檔案${file.fileName}`);
            reup.RemoveFile(file);
            item.remove();
            if(reup.files.length===0){
                $('#ReplacementForm').trigger('cancel');//告知表單編輯取消
            }
        });
    }
    
};
