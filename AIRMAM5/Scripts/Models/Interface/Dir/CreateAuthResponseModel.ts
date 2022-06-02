import { SelectListItem } from "../Shared/ISelectListItem";
import { ShowAuthListModel } from "./ShowAuthListModel";

/**目錄新增 群組/使用者權限回覆的內容 */
export interface CreateAuthResponseModel{
    /**單一目錄/節點{G群組/U使用者} 權限資料 */
    DirAuthority: ShowAuthListModel,
    /**角色or帳號資料選單(取出 系統目錄未設定過權限的角色or帳號) */
    RoleOrUserList: Array<SelectListItem>
}