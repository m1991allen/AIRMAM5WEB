import { ColFieldCodeType } from "../../Enum/ColTypeEnum";

/**自訂欄位類型 */
export type ColType=keyof typeof ColFieldCodeType;

/**第三方系統(如文稿,公文)_搜詢條件參數 */
export interface SearchDraftParameterModel<ColType>{
    Field:string;
    FieldType:ColType|'';
    Value:string;
    GenericValue:[string,string]|[];
}