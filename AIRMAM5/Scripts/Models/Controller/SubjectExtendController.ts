import { resolve } from "dns";
import { type } from "jquery";
import { API_SubjectExtend } from "../Const/API";
import { Action } from "../Enum/Action";
import { ColFieldCodeType } from "../Enum/ColTypeEnum";
import { Controller } from "../Enum/Controller";
import { ThridSystemEnum } from "../Enum/ThridSystemEnum";
import { Ajax, AjaxPost } from "../Function/Ajax";
import { getEnumKeyByEnumValue } from "../Function/KeyValuePair";
import { GetUrl } from "../Function/Url";
import { IResponse } from "../Interface/Shared/IResponse";
import { SelectListItem } from "../Interface/Shared/ISelectListItem";
import { DraftOriginalResultModel, DraftResultModel } from "../Interface/SubExtend/DraftResultModel";
import { DraftSetModel } from "../Interface/SubExtend/DraftSetModel";
import { SaveDraftViewModel } from "../Interface/SubExtend/SaveDraftViewModel";
import { ColType, SearchDraftParameterModel } from "../Interface/SubExtend/SearchDraftParameterModel";
import { BaseController, IBaseController } from "./BaseController";
interface ExpendSelectListItem extends SelectListItem{
    DataType:keyof typeof ColFieldCodeType
}

/**
 * 主題擴充_介接第三方系統(文稿,公文系統等)接口
 */
export interface ISubjectExtendController extends IBaseController<API_SubjectExtend>{
    /**第三方系統可供給使用者查詢的條件 */
   GetSearchFactor(system:ThridSystemEnum): Promise<IResponse<object,Array<ExpendSelectListItem>>>;
   /**設置對應的資料Id(例如文稿對應,公文對應) */
   SetDraft(input:DraftSetModel):Promise<IResponse>;
   /**第三方系統之條件搜尋 */
   SearchDraft(type:ThridSystemEnum,input: (Omit<SearchDraftParameterModel<"NVARCHAR" | "INTEGER" | "CODE">, "GenericValue"> | Omit<SearchDraftParameterModel<"DATETIME">, "Value">)[]): Promise<IResponse<object,DraftResultModel>>  ;
}
/**
 * 主題擴充_介接第三方系統(文稿,公文系統等)路由
 */
export class SubjectExtendController extends BaseController<API_SubjectExtend>  implements ISubjectExtendController {
  constructor(){
     super({
        DraftDropdown:GetUrl(Controller.SubjExtend,Action.DraftDropdown).href,
        DraftSearch:GetUrl(Controller.SubjExtend,Action.DraftSearch).href,
        DraftSetSave:GetUrl(Controller.SubjExtend,Action.DraftSetSave).href
     });
   }
  GetSearchFactor(system:ThridSystemEnum): Promise<IResponse<object,Array<ExpendSelectListItem>>>{
        switch(system){
            case ThridSystemEnum.INEWS:
            case ThridSystemEnum.CONTRACT: 
            return Ajax<{type:keyof typeof ThridSystemEnum}>("POST",this.api.DraftDropdown,{type:<keyof typeof ThridSystemEnum>getEnumKeyByEnumValue(ThridSystemEnum,system)},true);               
        }
   }
   SetDraft(input:DraftSetModel):Promise<IResponse>{
    return AjaxPost<DraftSetModel>(this.api.DraftSetSave,input,false);
   }
   SearchDraft(type:ThridSystemEnum,input: (Omit<SearchDraftParameterModel<"NVARCHAR" | "INTEGER" | "CODE">, "GenericValue"> | Omit<SearchDraftParameterModel<"DATETIME">, "Value">)[]): Promise<IResponse<object,DraftResultModel>> {
    const parameters:Array<SearchDraftParameterModel<ColType>>=input.map(item=>{
        return Object.assign(<SearchDraftParameterModel<ColType>>{},item,
            item.FieldType==='DATETIME'?<SearchDraftParameterModel<ColType>>{Value:''}:<SearchDraftParameterModel<ColType>>{GenericValue:[]});                
        });
        return new Promise((resolve,reject)=>{
            Ajax<{ExecType:keyof typeof ThridSystemEnum;ExecParams:SearchDraftParameterModel<ColType>[];}>("POST",this.api.DraftSearch,{
                ExecType:<keyof typeof ThridSystemEnum>getEnumKeyByEnumValue(ThridSystemEnum,type),
                ExecParams:parameters
            },false).then(res=>{
                const data:DraftOriginalResultModel=res.Data;
                const pk=data.PKeyCol.split(';');
                const result:DraftResultModel={
                 PKeyCol:pk,
                 DataTitle:data.DataTitle,
                 DataList:data.DataList.map(item=>{
                     return Object.assign(item,{ 
                         PKeyCol:pk,
                        PKeyVal:[].concat(Object.keys(item).filter(key=>pk.includes(key)).map(key=>item[key]))
                     });
                    
                 })
                };
                 resolve(Object.assign(<IResponse<object,DraftResultModel>>res,<IResponse<object,DraftResultModel>>{Data:result}));
            }).catch(error=>{
                reject(Object.assign(<IResponse<object,DraftResultModel>>error,<IResponse<object,DraftResultModel>>{}));
            });
        });
    }

}

