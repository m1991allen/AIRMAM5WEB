import { FunctionIdModel } from './FunctionIdModel';

/**
 * 角色可用功能更新參數
 */
export interface RoleFuncUpdateModel {
    /**角色Id */
    RoleId: string;
    /** 可使用 功能項目Id */
    FunctionIds: Array<FunctionIdModel>;
}
