import { SelectListItem } from '../Shared/ISelectListItem';

/**新增使用者:給前端使用 */
export interface UserCreateModel {
    /**使用者帳號 */
    UserName: string;
    /**密碼 */
    Password: string;
    /** 確認密碼 */
    ConfirmPassword: string;
    /**使用者姓名 */
    Name: string;
    /**使用者英文名 */
    EName: string;
    /**使用者職稱 */
    Title: string;
    /**隸屬單位 */
    DeptId: string;
    /**所屬群組角色 */
    GroupIds: string;
    /**電子郵件 */
    Email: string;
    /**連絡電話 */
    Phone: string;
    /**預設調用路徑(如：\\SERVER\FOLDER) */
    BookingTargetPath: string;
    /**檔案機密權限 */
    FileSecret: string;
    /**備註 */
    Description: string;
    /**所屬群組角色下拉選單 */
    RoleList: Array<string>;
    /**檔案機密權限下拉選單  */
    SecretList: Array<string>;
}
/**顯示下拉選單 */
export interface UserDropdownModel {
    /**隸屬單位下拉選單  */
    DeptList: Array<SelectListItem>;
    /**檔案機密權限顯示下拉選單  */
    FileSecretList: Array<SelectListItem>;
    /**所屬群組角色顯示下拉選單 */
    RoleGroupLst: Array<SelectListItem>;
}
/**新增使用者:傳給後端 */
export interface UserCreateRealModel extends UserCreateModel, UserDropdownModel {}
