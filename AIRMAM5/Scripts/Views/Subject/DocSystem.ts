import { FileListModel } from "../../Models/Interface/Subject/FileListModel";
import { Logger } from "../../Models/Class/LoggerService";
import { tabulatorService } from "../../Models/Class/tabulatorService";
import { ISubjectExtendController, SubjectExtendController } from "../../Models/Controller/SubjectExtendController";
import { ColFieldType } from "../../Models/Enum/ColTypeEnum";
import { ThridSystemEnum } from "../../Models/Enum/ThridSystemEnum";
import { IsNULLorEmpty, IsNullorUndefined } from "../../Models/Function/Check";
import { setCalendar } from "../../Models/Function/Date";
import { getEnumKeyByEnumValue } from "../../Models/Function/KeyValuePair";
import { ErrorMessage, SuccessMessage } from "../../Models/Function/Message";
import { ModalTask } from "../../Models/Function/Modal";
import { TabulatorSetting } from "../../Models/initSetting";
import { ItabulatorService } from "../../Models/Interface/Service/ItabulatorService";
import { SelectListItem } from "../../Models/Interface/Shared/ISelectListItem";
import { ColType, SearchDraftParameterModel } from "../../Models/Interface/SubExtend/SearchDraftParameterModel";
import { CreateOption } from "../../Models/Templete/FormTemp";

(function($){
    const defaults:SubjectWindowOption={ };
    const DraftConfirmId='#DraftConfirm';
    var table:ItabulatorService,
        config:SubjectWindowOption,
        factors:Array<SearchDraftParameterModel<ColType> &{Text:string;Id:string;}>=[],
        route:ISubjectExtendController;
    var tableConfig:Tabulator.Options={
        height:TabulatorSetting.height,
        layout:"fitDataTable",
        autoResize:true,
        selectable: 1,
       // persistentLayout : true,//!佈局永久性
        paginationSize:TabulatorSetting.paginationSize,
        layoutColumnsOnNewData : true,
        responsiveLayout:"collapse",
        responsiveLayoutCollapseStartOpen : false,//預設折疊不要打開
        responsiveLayoutCollapseFormatter:function(data){
            let list = document.createElement("ul");
            list.classList.add('x-ul');
            data.forEach(function(col){
                let item = document.createElement("li");
                item.classList.add("x-li");
                item.innerHTML = "<strong class='x-li-title'>" + col.title + "</strong>" + col.value;
                list.appendChild(item);
            });
            return Object.keys(data).length ? list : "";
        },
        data:[],
        columns:[{title:'查詢結果',field:''}]
    };
    const checkInputElement=(datatype:ColType)=>{
        if(datatype=="DATETIME"){
            $("input.x-input[name !='datetime']").hide();
            $("input.x-input[name ='datetime']").show().siblings(".x-timelabel").show();
          }else{
            $("input.x-input[name !='datetime']").show();
            $("input.x-input[name ='datetime']").hide().siblings(".x-timelabel").hide();
          }
    };
    var methods:Subjectwindow={
         GetDropdown:(type:ThridSystemEnum)=>{
            setCalendar(`.calendar[name='datetime']`, 'date');
            route.GetSearchFactor(type)
            .then(res=>{
                  if(res.IsSuccess && !IsNullorUndefined(res.Data)){
                    const options=res.Data.map(item=>{
                        return CreateOption(Object.assign(<SelectListItem>{},item),{attrName:'dataType',attrValue:item.DataType});
                    })
                    /**第三方系統的下拉選單 */
                    checkInputElement(res.Data[0].DataType);
                    $("select[name='factorInput']").empty().append(options).on('change',(e)=>{
                        const optiontype= <ColType>$("select[name='factorInput'] option:selected").attr("dataType").toUpperCase();
                        checkInputElement(optiontype);
                      }).dropdown('refresh');
                   
                  }else{
                    $("select[name='factorInput']").empty().append(CreateOption(Object.assign(<SelectListItem>{},{Text:'沒有可選擇的項目',Value:'',Selected:false}))).dropdown('refresh').dropdown('clear');
                  }
                  
                        
            })
            .catch(error=>{
                $("select[name='factorInput']").empty().append(CreateOption(Object.assign(<SelectListItem>{},{Text:'沒有可選擇的項目',Value:'',Selected:false}))).dropdown('refresh').dropdown('clear');
            });
         },
         AddFactor:(item:SearchDraftParameterModel<ColType> &{Text:string;Id:string;}):boolean=>{
            if(item.FieldType===ColFieldType.日期){
            　　if(IsNullorUndefined(item.GenericValue[0])||IsNULLorEmpty(item.GenericValue[0].trim())){
                   ErrorMessage(`${item.Text}的開始日期皆為必填`);
                   return false;
                }
                else if(IsNullorUndefined(item.GenericValue[1])||IsNULLorEmpty(item.GenericValue[1].trim())){
                    ErrorMessage(`${item.Text}的結束日期皆為必填`);
                    return false;
                }else{
                    item.Value="";
                    item.GenericValue=[ item.GenericValue[0].trim(), item.GenericValue[1].trim()];
                    factors.push(item);
                    return true;
                }
            }
            else if( IsNULLorEmpty(item.Value.trim())||IsNullorUndefined(item.Value)){
                ErrorMessage(`${item.Text}的值不能為空值`);
                return false;
            }
            else{
                item.Value=item.Value.trim();
                item.GenericValue=[];
                factors.push(item);
                return true;
            }
           
         },
         RemoveFactor:(removeKey:string,value:any)=>{
            if(value===undefined||value===null){
                return false;
            }
            const index = factors.findIndex(x => x[removeKey]===value);
            if (index !== undefined && index !==null){factors.splice(index, 1);}
         },
         ClearFactor:():Array<SearchDraftParameterModel<ColType> &{Text:string;Id:string;}>=>{
            factors=[];
            return factors;
        },
         CurrentFactors:():Array<SearchDraftParameterModel<ColType> &{Text:string;Id:string;}>=>{
            return factors;
        },
        SearchTable:(fileData:FileListModel,input:{ subjectId:string, mediatype:string,type:ThridSystemEnum},items:Array<SearchDraftParameterModel<ColType>>)=>{
            route.SearchDraft(input.type,items).then(res=>{
                if(res.IsSuccess){
                    if(!IsNullorUndefined(res.Data)){
                      const title=res.Data.DataTitle;
                      const columns:Array<Tabulator.ColumnDefinition>=[
                         {
                           title:"操作", field:res.Data.PKeyCol[0]??"", frozen:true, 
                          formatter:function(cell:Tabulator.CellComponent,formatterParams){
                            return `<button type="button" class="ui mini blue button">設定</button>`;
                          },
                          cellClick: function(e, cell:Tabulator.CellComponent) {
                            cell.getElement().querySelector('button').classList.add('loading');
                            const rowData=cell.getRow().getData();
                            ModalTask(DraftConfirmId,true,{
                                allowMultiple: true,
                                selector: {
                                    close: '.actions .deny,i.close',
                                },
                                closable: false,
                                detachable: true /*如果設置為false將阻止模態移至調光器內部 */,
                                observeChanges: true,
                                context:'#right-component',
                                onShow: function(this: JQuery<HTMLElement>) {
                                    $(this).on('click', function(event) { event.stopImmediatePropagation();}); /*為了解決Modal中Modal關閉問題互相影響*/
                                    $(DraftConfirmId).attr('style','width:calc(100% - 50px)');
                                    const div=document.createElement('div');
                                    div.setAttribute('style','display:flex;max-width:100%;')
                                    let list = document.createElement("ul");
                                    list.classList.add('x-ul');
                                    const rowData=cell.getRow().getData();
                                    if(!IsNullorUndefined(rowData)){
                                        Object.keys(rowData).forEach(function(key:string,index:number){
                                            if(key in title && !IsNULLorEmpty(title[key])){
                                                let item = document.createElement("li");
                                                item.classList.add("x-li");
                                                item.innerHTML = "<strong class='x-li-title'>" + title[key] + "</strong>" + rowData[key];
                                                list.appendChild(item);
                                            }
                                        });
                                    }
                                    div.appendChild(list);
                                    $(this).find('.content').addClass('scrolling').empty()
                                    .append(`<p>確定要將以下文稿設定至檔案【${fileData.Title}】?</p>`)
                                    .append(`<div class="x-docsystem ui items"><div class="item"> <a class="ui small image"><img src="${fileData.ImageUrl}" onerror="this.src='http://localhost/AIRMAM5/Images/noImage.png'" name=""></a><div class="content"><a class="ui inverted grey header">${fileData.Title}</a><div class="meta" style="color:#fff;">${fileData.fsFILE_NO}</div> </div></div></div>`)
                                    .append(div);
                                },
                                onApprove: function() {
                                    const fileno=$("button[name='docsystem']").attr('data-id');
                                    route.SetDraft({
                                        FileNo:fileno,
                                        ExecType:<keyof typeof ThridSystemEnum>getEnumKeyByEnumValue(ThridSystemEnum,input.type).toUpperCase(),
                                        Fields:res.Data.PKeyCol,
                                        Values:Object.keys(rowData).filter(key=>res.Data.PKeyCol.includes(key)).map(key=>rowData[key])//要確認
                                    }).then(res=>{
                                        Logger.res(route.api.DraftSetSave,'對應設置',res,true);
                                        if(res.IsSuccess){
                                            $('#DraftConfirm').modal('hide');
                                            $("div[name='docsystemview']").trigger('render');
                                            $('.subtable').trigger('render',[input.subjectId,input.mediatype,fileno]);
                                        }
                                    })
                                    .catch(error=>{
                                        Logger.viewres(route.api.DraftSetSave,'對應設置',error,true);
                                    })
                                    cell.getElement().querySelector('button').classList.remove('loading');
                                    return false;
                                },
                                onDeny: function(this: JQuery<HTMLElement>) {
                                    cell.getElement().querySelector('button').classList.remove('loading');
                                }
                            });
                          }
                         },
                         {title:"",width:30, minWidth:30, hozAlign:"center", resizable:false, headerSort:false,formatter:"responsiveCollapse"}
                        ];     
                      Object.keys(title).forEach(key=>{
                          
                      columns.push({
                          title:title[key],
                          field:key,
                          sorter:'string',
                          formatter:function(cell,formatterParams){
                              return cell.getValue();
                          }
                      });
                     });
                     tableConfig.columns=columns;
                     tableConfig.data=res.Data.DataList;
                     table=new tabulatorService('#doctable',tableConfig,false); 
                    }
                    SuccessMessage(res.Message);
                }else{
                    ErrorMessage(res.Message);
                }
            }).catch(error=>{
                methods.ClearTble();
                ErrorMessage(IsNULLorEmpty(error.Message)||IsNullorUndefined(error.Message)?'查詢第三方API失敗!':error.Message);
            });
         
        },
        ClearTble:()=>{
            tableConfig.columns=[{title:'查詢結果',field:''}];
            tableConfig.data=[];
            table=new tabulatorService('#doctable',tableConfig,false); 
        }
    };
    $.subjectwindow=function(options){
        config=$.extend({},defaults,options);
        route=new SubjectExtendController();
        factors=[];
        config=$.extend({},defaults);
        table=new tabulatorService('#doctable',tableConfig,false); 
        $.extend(this,methods,<SubjectWindowConfig>{config:config});
        return this;
    }
})(jQuery);
