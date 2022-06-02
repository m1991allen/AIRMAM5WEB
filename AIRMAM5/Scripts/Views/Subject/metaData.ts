import { ModalTask } from '../../Models/Function/Modal';
import { ISubjectController } from '../../Models/Controller/SubjectController';
import { MediaType, MediaUploadType } from '../../Models/Enum/MediaType';
import { SuccessMessage, ErrorMessage, WarningMessage, InfoMessage } from '../../Models/Function/Message';
import { IsNULLorEmpty } from '../../Models/Function/Check';
import { selectids, filterSelectIds} from './Index';
import { TabNameEnum } from '../../Models/Enum/TabNameEnum';
import { ReplacementTask } from './upload';
import { CheckForm, AddDynamicNullable } from '../../Models/Function/Form';
import { dayjs, setCalendar,SetDate } from '../../Models/Function/Date';
import { FormValidField } from '../../Models/Const/FormValid';
import { CreateMaterialModel } from '../../Models/Interface/Materia/CreateMaterialModel';
import { PermissionDefinition } from '../../Models/Enum/PermissionDefinition';
import { SubjectUploadConfig } from '../../Models/Interface/Subject/SubjectUploadConfig';
import { Logger } from '../../Models/Class/LoggerService';
import { UI } from '../../Models/Templete/CompoentTemp';
import { ColType, SearchDraftParameterModel } from '../../Models/Interface/SubExtend/SearchDraftParameterModel';
import { ColFieldType } from '../../Models/Enum/ColTypeEnum';
import { getEnumKeyByEnumValue } from '../../Models/Function/KeyValuePair';
import { Guid } from '../../Models/Function/Guid';
import { ThridSystemEnum } from '../../Models/Enum/ThridSystemEnum';
import { edateId, sdateId } from '../../Models/Const/Const.';
import { initSetting } from '../../Models/initSetting';

const EditMediaId = '#EditMediaModal';
const DeleteMediaModalId = '#DeleteMediaModal';
const RetransferConfirmId = '#RetransferConfirm';
const DeleteMediaConfirmId = '#DeleteMediaConfirm';
export const ReplacementModalId = '#ReplacementModal';
export const ReplacementConfirmId = '#ReplacementConfirm';
const valid = FormValidField.Subject;
/*----------------------------------------------
    媒體資料功能
-----------------------------------------------*/

export function MetaData(
    route: ISubjectController,
    ModalId: string,
    SubjectId: string,
    type: MediaType | string,
    subjectName: string,
    DirAuth: Array<PermissionDefinition>
) {
    const EditMediaFormId = '#EditMediaForm';
    const $MetaTab = $(ModalId).find(`div[data-tab='${TabNameEnum.MetaData}']`).eq(0);
    /*預設查詢日期,先預設日期再初始化日曆,會在日曆上顯示預設日期*/
    SetDate(sdateId, dayjs().add(-7, 'day'), 'YYYY-MM-DD');
    SetDate(edateId, dayjs(), 'YYYY-MM-DD');
    setCalendar(`.calendar`, 'date2');  
    /**第三方系統對應 */
    const plugin=  $.subjectwindow({});
    //---------按鈕事件--------------------------------
    /**加入借調 */
    $MetaTab.off('click',"button[name='addMateria']").on('click', "button[name='addMateria']", function(event) {
        event.preventDefault();
        if(!IsNULLorEmpty($(this).attr('alertMessage'))){
            WarningMessage($(this).attr('alertMessage'));
        }
        $(this).addClass('loading');
        const input: CreateMaterialModel = {
            FileCategory: <MediaType>type,
            FileNo: $(this).attr('data-Id'),
            MaterialDesc: '',
            MaterialNote: '',
            ParameterStr: '0;0;0',
        };
        route
            .AddBooking([input])
            .then(res => {
                res.IsSuccess
                    ? $(this)
                          .addClass('disabled')
                          .text('已加入清單')
                    : $(this)
                          .addClass('disabled')
                          .text('再試一次');
            })
            .catch(error => {
                Logger.viewres(route.api.AddingBooking, '加入借調', error, false);
                $(this)
                    .addClass('disabled')
                    .text('再試一次');
            });
        $(this).removeClass('loading');
    });

    /**刪除 */
    $MetaTab.off('click', "button[name='delete']").on('click', "button[name='delete']", function(event) {
        event.preventDefault();
        const _this = $(this);
        _this.addClass('loading');
        const fileno: string = $(this).attr('data-Id');
        route.DeleteMediaView(ModalId, DeleteMediaModalId, type, fileno).then(success => {
            if (success) {
                const $deleteForm=$(DeleteMediaModalId).find('form');
                $deleteForm.trackEdit()
                .onEdit(function(){   $('.subtable').trigger('editing',[true]);})
                .onCancel(function(){  $('.subtable').trigger('editing',[false]); });
                /**刪除確認燈箱事件綁定 */
                ModalTask(DeleteMediaConfirmId, false, {
                    allowMultiple: true,
                    selector: {
                        close: '.actions .ok,.actions .cancel,i.close',
                        deny: '.actions .cancel',
                    },
                    closable: false,
                    detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                    observeChanges: true,
                    context: '#right-component',
                    onShow: function(this: JQuery<HTMLElement>) {
                        /*為了解決Modal中Modal關閉問題互相影響*/
                        $(this).on('click', function(event) {
                            event.stopImmediatePropagation();
                        });
                    },
                    onApprove: function() {
                        const reason: string = <string>$deleteForm.find("textarea[name='Reason']").val();
                        if (IsNULLorEmpty(reason)) {
                            WarningMessage('刪除理由必填!');
                            $(DeleteMediaModalId).modal('show'); //確認
                        } else {
                            route
                                .DeleteMedia({ FileNo: fileno, FileCategory: <MediaType>type, Reason: reason })
                                .then(res => {
                                    Logger.res(route.api.DeleteMedia, '刪除媒體資料', res);
                                    if (res.IsSuccess) {
                                        $(initSetting.TableId).trigger('addcount',[SubjectId,type,-1]);
                                        $('.subtable').trigger('nextclick',[fileno]);
                                        filterSelectIds(fileno);
                                    }
                                });
                            $deleteForm.isCancel();
                        }
                        $(DeleteMediaConfirmId).modal('hide');
                        return false;
                    },
                    onDeny: function(this: JQuery<HTMLElement>) {
                        $(DeleteMediaConfirmId).modal('hide');
                        $deleteForm.isCancel();
                        return false;
                    },
                });

                /**刪除燈箱綁定 */
                ModalTask(DeleteMediaModalId, true, {
                    allowMultiple: true,
                    selector: {
                        close: '.actions .deny,i.close',
                    },
                    closable: false,
                    detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                    observeChanges: true,
                    context: '#right-component',
                    onShow: function(this: JQuery<HTMLElement>) {
                        /*為了解決Modal中Modal關閉問題互相影響*/
                        $(this).on('click', function(event) {
                            event.stopImmediatePropagation();
                        });
                        _this.removeClass('loading');
                    },
                    onApprove: function() {
                        $(DeleteMediaModalId).modal('hide');
                        $(DeleteMediaConfirmId).modal('show');
                        return false;
                    },
                    onDeny: function(this: JQuery<HTMLElement>) {
                        $(DeleteMediaConfirmId).modal('hide');
                        $(DeleteMediaModalId).modal('hide');
                        return false;
                    },
                });
            } else {
                ErrorMessage('系統發生錯誤,無法開啟刪除媒體資料燈箱,請重新再試');
            }
        });
    });

    /**修改 */
    $MetaTab.off('click', "button[name='edit']").on('click', "button[name='edit']", function(event) {
        event.preventDefault();
        const _this = $(this);
        _this.addClass('loading');
        const fileno: string = $(this).attr('data-Id');
        route
            .EditMediaView(ModalId, EditMediaId, { SubjectId: SubjectId, fileNo: fileno, type: type })
            .then(success => {
                if (success) {
                    const $eform=$(EditMediaId).find('form');
                    ModalTask(EditMediaId, true, {
                        allowMultiple: true,
                        selector: {
                            close: '.actions .deny,i.close',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                            $(ModalId)
                                .find('.dropdown:not(.x-hashtag)')
                                .dropdown({
                                    fullTextSearch: 'exact',
                                    match: 'text',
                                });
                                $(ModalId).find('.ui.dropdown.x-hashtag')
                                .dropdown({
                                    allowAdditions: true,
                                    keys: {
                                        delimiter: 54
                                    },
                                    
                                    onChange:function(value, text, $selectedItem){
                                        const newValue=value.replace(/,/g,"^");
                                        $("input[name='HashTag']").val(newValue);
                                    }
                                });
                            _this.removeClass('loading');
                            setCalendar(`${ModalId} .calendar`, 'date');
                            $(EditMediaId)
                                .find('.checkbox')
                                .checkbox()
                                .checkbox('set checked');
                            $eform.trackEdit()
                            .onEdit(function(){ $('.subtable').trigger('editing',[true]);})
                            .onCancel(function(){ $('.subtable').trigger('editing',[false]); });
                        },
                        onApprove: function() {
                            const $Form = $(EditMediaFormId);
                            /**
                             * Notice:動態驗證條件,注意深淺拷貝問題
                             *深拷貝初始的驗證條件,因為編輯的驗證會隨選單進行初始化,所以要由此去進行擴增
                             */
                            const copyValid = Object.assign({}, valid.EditMedia);
                            const validObject = AddDynamicNullable(EditMediaFormId, copyValid);
                            const isFormValid = CheckForm(EditMediaFormId,Object.assign(validObject,valid.EditMedia));
                            if (isFormValid) {
                                route
                                    .EditMedia($Form.serialize())
                                    .then(res => {
                                        res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                                        if (res.IsSuccess) {
                                            $(EditMediaId).modal('hide');
                                             $MetaTab.trigger('render',[fileno]);
                                        }
                                    })
                                    .catch(error => {
                                        Logger.viewres(route.api.EditMedia, '編輯媒體資料', error);
                                    });
                                $eform.isCancel();
                            }
                           
                            return false;
                        },
                        onDeny: function() {
                            $eform.isCancel();
                            $(EditMediaId).modal('hide');
                            return false;
                        },
                    });
                } else {
                    Logger.viewres(route.api.ShowEditMedia, '開啟編輯媒體資料燈箱', '');
                }
            });
    });

    /**批次修改 */
    $MetaTab.off('click', "button[name='batchedit']").on('click', "button[name='batchedit']", function(event) {
        event.preventDefault();
        const _this = $(this);
        _this.addClass('loading');
        const fileno: string = $(this).attr('data-Id');
        const filenos = selectids;
        if (filenos.length == 0) {
            WarningMessage('至少勾選一個檔案');
        } else {
            route
                .EditBatchMediaView(ModalId, EditMediaId, { SubjectId: SubjectId, type: type, fileNo: filenos })
                .then(success => {
                    if (success) {
                        const $eform=$(EditMediaId).find('form');
                        let skipnames: string[] = []; //要略過驗證的元素名稱
                        if (filenos.length > 1) {
                            //如果批次選擇的檔案數量等於1,仍需判斷必填欄位
                            const skipelement = $(EditMediaFormId)
                                .find('.required.field')
                                .children('input,textarea,.dropdown,select,.calendar');
                            skipelement.each(function(this, index: number, element: HTMLElement) {
                                const $element = $(element);
                                const name = $element.hasClass('dropdown')
                                    ? $element.closest('select').attr('name')
                                    : $element.hasClass('calendar')
                                    ? $element.children('input').attr('name')
                                    : $element.attr('name');
                                skipnames.push(name);
                            });
                        }

                        ModalTask(EditMediaId, true, {
                            allowMultiple: true,
                            selector: {
                                close: '.actions .deny,i.close',
                            },
                            closable: false,
                            detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                            observeChanges: true,
                            context: '#right-component',
                            // context: $MetaTab,
                            onShow: function(this: JQuery<HTMLElement>) {
                                /*為了解決Modal中Modal關閉問題互相影響*/
                                $(this).on('click', function(event) {
                                    event.stopImmediatePropagation();
                                });
                                $(EditMediaId)
                                .find('.dropdown:not(.x-hashtag)')
                                .dropdown({
                                    fullTextSearch: 'exact',
                                    match: 'text',});
                                $(EditMediaId).find('.ui.dropdown.x-hashtag')
                                .dropdown({
                                    allowAdditions: true,
                                    keys: {
                                        delimiter: 54
                                    },
                                    
                                    onChange:function(value, text, $selectedItem){
                                        const newValue=value.replace(/,/g,"^");
                                        $("input[name='HashTag']").val(newValue);
                                    }
                                });
                                setCalendar(`${EditMediaId} .calendar`, 'date');
                                $(EditMediaId)
                                    .find('.cuschecked')
                                    .checkbox('set checked');
                                $(EditMediaId)
                                    .find('.checkbox')
                                    .checkbox({
                                        onChecked: function(this: HTMLInputElement) {
                                            /*勾選=>如果為必填要從略過名稱列中移除*/
                                            const $checkbox = $(this).parent('.checkbox');
                                            $checkbox.addClass('cuschecked');
                                            const $sibling = $checkbox.siblings(
                                                'input,textarea,.dropdown,select,.calendar'
                                            );
                                            const includename = $sibling.hasClass('dropdown')
                                                ? $sibling.children('select').attr('name')
                                                : $sibling.hasClass('calendar')
                                                ? $sibling.children('input').attr('name')
                                                : $sibling.attr('name');

                                            skipnames = skipnames.filter(name => name != includename);
                                        },
                                        onUnchecked: function(this: HTMLInputElement) {
                                            /*取消勾選=>如果為必填要加入略過名稱列*/
                                            const $checkbox = $(this).parent('.checkbox');
                                            const $field = $(this).closest('.field');
                                            const $prompt = $field.find('.prompt.label');
                                            $checkbox.removeClass('cuschecked');
                                            $field.removeClass('error');
                                            $prompt.remove();
                                            const $sibling = $checkbox.siblings(
                                                'input,textarea,.dropdown,select,.calendar'
                                            );
                                            const skipname = $sibling.hasClass('dropdown')
                                                ? $sibling.children('select').attr('name')
                                                : $sibling.hasClass('calendar')
                                                ? $sibling.children('input').attr('name')
                                                : $sibling.attr('name');
                                            skipnames.push(skipname);
                                        },
                                    });
                                $eform.trackEdit()
                                .onEdit(function(){ $('.subtable').trigger('editing',[true]);})
                                .onCancel(function(){ $('.subtable').trigger('editing',[false]); });
                            },
                            onApprove: function() {
                                const $Form = $(EditMediaFormId);

                                /**
                                 * Notice:動態驗證條件,因為批次編輯的驗證要以勾選為主,如果是已勾選且必填的欄位才要驗證,否則都不用驗證
                                 * skipname=>要忽略驗證的元素名稱,因為批次編輯的流程改為如果不勾選就沒有要驗證該欄位並送出欄位資料,與其他編輯流程不一樣
                                 */
                                const copyValid = {};
                                const validObject = AddDynamicNullable(EditMediaFormId, copyValid, skipnames);
                                Logger.log(`要略過驗證的欄位名稱=${skipnames}`);
                                const needValidate =
                                    $(EditMediaFormId).find('.checkbox.checked').length == 0 ? false : true;
                                if (needValidate) {
                                    const isFormValid = CheckForm(EditMediaFormId, Object.assign(validObject,valid.EditMedia));
                                    if (isFormValid) {
                                        route
                                            .EditMedia($Form.serialize())
                                            .then(res => {
                                                res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                                                if (res.IsSuccess) {
                                                    $(EditMediaId).modal('hide');
                                                    $MetaTab.trigger('render',[fileno]);                                            
                                                }
                                            })
                                            .catch(error => {
                                                Logger.viewres(route.api.EditMedia, '編輯媒體資料', error, false);
                                            });
                                        $eform.isCancel();
                                    }
                                } else {
                                    InfoMessage('無編輯項目，關閉視窗');
                                    $(EditMediaId).modal('hide');
                                    $eform.isCancel();
                                }
                                return false;
                            },
                            onDeny:function(){
                                $eform.isCancel();
                                $(EditMediaId).modal('hide');
                                return false;
                            }
                        });
                    } else {
                        Logger.viewres(route.api.ShowEditMedia, '開啟編輯媒體資料燈箱', '');
                    }
                });
        }
        _this.removeClass('loading');
    });

    /**重轉 */
    $MetaTab.off('click', "button[name='reTransfer']").on('click', "button[name='reTransfer']", function(event) {
        event.preventDefault();
        const _this = $(this);
        _this.addClass('loading');
        const fileno: string = $(this).attr('data-Id');
        ModalTask(RetransferConfirmId, true, {
            allowMultiple: false, //因為只有一層,若true會跑位
            selector: {
                close: '.actions .deny,i.close',
            },
            closable: false,
            detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
            observeChanges: true,
            context: '#right-component',
            // context: $MetaTab,
            onShow: function(this: JQuery<HTMLElement>) {
                /*為了解決Modal中Modal關閉問題互相影響*/
                $(this).on('click', function(event) {
                    event.stopImmediatePropagation();
                });
                _this.removeClass('loading');
            },
            onApprove: function() {
                route.Retransfer({ SubjectId: SubjectId, fileNo: fileno, type: type }).then(res => {
                    res.IsSuccess ? SuccessMessage(res.Message) : ErrorMessage(res.Message);
                });
                $(RetransferConfirmId).modal('hide');
                return false;
            },
            onDeny: function(this: JQuery<HTMLElement>) {
                $(RetransferConfirmId).modal('hide');
                return false;
            },
        });
    });

    /**置換檔案 */
    $MetaTab.off('click', "button[name='replaceMent']").on('click', "button[name='replaceMent']", function(event) {
        event.preventDefault();
        const _this = $(this);
        _this.addClass('loading');
        const fileno: string = $(this).attr('data-Id');
        const Type =
            type == MediaType.VIDEO
                ? MediaUploadType.VIDEO
                : type == MediaType.AUDIO
                ? MediaUploadType.AUDIO
                : type == MediaType.PHOTO
                ? MediaUploadType.PHOTO
                : type == MediaType.Doc
                ? MediaUploadType.DOC
                : MediaUploadType.Unknown;
        route
            .ReplacementView(ModalId, ReplacementModalId, {
                type: Type,
                SubjectId: SubjectId,
                fileNo: fileno,
            })
            .then(success => {
                if (success) {
                    const $rform=$(ReplacementModalId).find('form');
                    const MediaFileExtension = $('#ReSaveJson').attr('data-MediaFileExtension');
                    const UploadConfig: SubjectUploadConfig = JSON.parse($('#ReConfigJson').attr('data-UploadConfig'));
                    const AllowMediaFormat: Array<{ FileExtension: string; MediaType: string }> = JSON.parse(
                        MediaFileExtension
                    );
                    //如果頁面取不到可接受的副檔名,就禁止輸入
                    if (IsNULLorEmpty(Type)) {
                        WarningMessage(`無法判斷原檔案${fileno}的檔案類型,請重整頁面後再嘗試重置`);
                        $(ReplacementModalId)
                            .find('.approve')
                            .addClass('disabled');
                    } else {
                        $(ReplacementModalId)
                            .find('.approve')
                            .removeClass('disabled');
                    }
                    /**置換檔案燈箱 */
                    ModalTask(ReplacementModalId, true, {
                        allowMultiple: true,
                        selector: {
                            close: '.actions .deny,i.close',
                        },
                        closable: false,
                        detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                        observeChanges: true,
                        context: '#right-component',
                        // context: $MetaTab,
                        onShow: function(this: JQuery<HTMLElement>) {
                            /*為了解決Modal中Modal關閉問題互相影響*/
                            $(this).on('click', function(event) {
                                event.stopImmediatePropagation();
                            });
                            _this.removeClass('loading');
                            $('.dropdown').dropdown();
                            const $KFquestion =$rform.find("div[name='KFquestion']");
                            if (type == MediaType.VIDEO) {
                                $KFquestion.children('.header').text('置換會刪除原始高解、低解與關鍵影格檔案!');
                                $KFquestion.children('label').text('1.是否保留關鍵影格描述與段落描述?');
                            }
                            if (type == MediaType.AUDIO) {
                                $KFquestion.children('.header').text('置換會刪除原始高解、低解檔案!');
                                $KFquestion.children('label').text('1.是否保留段落描述?');
                            }
                            if (type == MediaType.Doc || type == MediaType.PHOTO) {
                                $KFquestion.hide();
                            }
                            $rform.trackEdit()
                            .onEdit(function(){ $('.subtable').trigger('editing',[true]);})
                            .onCancel(function(){ $('.subtable').trigger('editing',[false]); });

                            ReplacementTask(subjectName, $MetaTab, <MediaUploadType>Type, {
                                config: {
                                    uploadPath: UploadConfig.TargetUrl,
                                    chuckSize: UploadConfig.UploadFileBuffer,
                                    AcceptExtension: AllowMediaFormat,
                                    simultaneousUploads: UploadConfig.SimultaneousUploads,
                                    TimeoutSec: UploadConfig.TimeoutSec,
                                },
                                query: {
                                    SubjId: SubjectId,
                                    LoginId: UploadConfig.LoginId,
                                    FileNo: fileno,
                                    Folder: UploadConfig.TempFolder,
                                    PreId: 0,
                                    DeleteKF: null,
                                    FileSecret: 0, //Added_20200302:機密等級。
                                    DateInFileNo:''
                                },
                            });
                        },
                        onApprove: function() {
                            $(ReplacementModalId).modal('hide');
                            $rform.isCancel();
                            return false;
                        },
                        onDeny: function(this: JQuery<HTMLElement>) {
                            $(ReplacementConfirmId).modal('hide');
                            $(ReplacementModalId).modal('hide');
                            $rform.isCancel();
                            return false;
                        },
                    });
                } else {
                    Logger.viewres(route.api.ShowReplacementView, '開啟置換燈箱', '');
                }
            });
    });
    /**顯示第三方系統DIV */
    $MetaTab.off('click',"button[name='docsystem']").on('click',"button[name='docsystem']",function(event){
        $("div[name='docsystemview']").fadeIn();
        $('#SearchForm .x-input').val("");
        switch (type) {
            case MediaType.VIDEO:
                plugin.ClearTble();
                plugin.GetDropdown(ThridSystemEnum.INEWS);
                break;
            case MediaType.Doc:
                plugin.ClearTble();
                plugin.GetDropdown(ThridSystemEnum.CONTRACT);
                break;
            default:
                ErrorMessage('沒有支援的第三方系統');
                $(this).addClass('disabled');
                break;
        }
       
    });
    /**資料tab元素的頁面更新 */
    $MetaTab.off('render').on('render',function(event,fileno:string){
        route.MediaView( { fsSUBJECT_ID: SubjectId, type: type, fileNo:fileno  },DirAuth)
        .then(view => {
            $MetaTab.html(view);
        }).catch(error=>{
            $MetaTab.html( UI.Error.ErrorSegment(
                '更新編輯媒體資料頁面發生錯誤',
                '請重新再試'
            ));
        });
    });
    /**第三方系統查詢表單 */
    const $systemForm=$('#SearchForm');
    /**隱藏第三方系統系統DIV */
     $(".x-delete").off('click').click(function(){
        $("div[name='docsystemview']").fadeOut();
        plugin.ClearFactor();
        $("div[name='doclist']").empty();
        $systemForm.isCancel();
     });
     /**第三系統查詢 */
     $systemForm.trackEdit()
     .onEdit(function(){ $('.subtable').trigger('editing',[true]);})
     .onCancel(function(){$('.subtable').trigger('editing',[false]); });
     $systemForm.off('submit').submit(function(event){
         event.preventDefault();
         const factors=plugin.CurrentFactors();
         if(factors.length===0){
             WarningMessage(`至少輸入一個條件`);
             return false;
         }
         $systemForm.isCancel();
         const fileno: string = $(this).attr('data-Id');
        switch (type) {
            case MediaType.VIDEO:
                $('.subtable').trigger('selectCallback',[fileno,(rowData)=>{
                    plugin.SearchTable(rowData,{subjectId:SubjectId,mediatype:type,type:ThridSystemEnum.INEWS},factors);
                }]);
                break;
            case MediaType.Doc:
                $('.subtable').trigger('selectCallback',[fileno,(rowData)=>{
                    plugin.SearchTable(rowData,{subjectId:SubjectId,mediatype:type,type:ThridSystemEnum.CONTRACT},factors);
                }]);
                break;
            default:
                ErrorMessage('沒有支援的第三方系統');
                $(this).addClass('disabled');
                break;
        }

     });
     $("button[name='docfactoradd']").off('click').click(function(){
         const factorOption:HTMLOptionElement=<HTMLOptionElement>$("select[name='factorInput'] option:selected")[0];
         const input=<SearchDraftParameterModel<ColType> &{Text:string;Id:string;}>{
             Id:Guid(),
             Field:factorOption.value,
             FieldType:factorOption.getAttribute("datatype").toUpperCase(),
             Value:$("input.x-input[name !='datetime']").eq(0).val()??"",
             Text:factorOption.innerText,
             GenericValue:[$("input.x-input[name ='datetime']").eq(0).val()??"",$("input.x-input[name ='datetime']").eq(1).val()??""]
         };
         if(dayjs(input.GenericValue[1]).isBefore(dayjs(input.GenericValue[0]))){
             WarningMessage('結束日期要在開始日期之後');
             return false;
         }
         const isSuccess=plugin.AddFactor(input);

         if(isSuccess){
            const docItem=`<div class="x-doc-item" data-id="${input.Id}">
            <div class="x-doc-delete"><i class="ui icon delete"></i></div>
            <div class="x-doc-type">${getEnumKeyByEnumValue(ColFieldType,input.FieldType)}</div>
            <div class="x-doc-column">${input.Text}</div>
            <div class="x-doc-input">${input.FieldType !==ColFieldType.日期?input.Value:input.GenericValue.join("~")}</div>
            </div>`;
           $("div[name='doclist']").append(docItem);
         }
       
     });
     /**第三方系統條件刪除 */
     $("div[name='docsystemview']")
     .on("click",".x-doc-item .x-doc-delete",function(){
        const pItem= $(this).parent(); 
        try{    
            plugin.RemoveFactor('Id',pItem.attr('data-id')); 
         }finally{
            pItem.remove();
         }
     })
     .on('render',function(){
        $MetaTab.trigger('render',[$("button[name='docsystem']").attr('data-id')]);
     });
   
}

