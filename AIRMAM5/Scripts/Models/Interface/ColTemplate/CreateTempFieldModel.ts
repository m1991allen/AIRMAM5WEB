/**新增欄位 */
export interface CreateTempFieldModel extends templateField {
    /** 是否多行 (對應原欄位[fsMULTILINE]) */
    IsMultiline: boolean;
    /** 是否可為NULL(對應原欄位[fsISNULLABLE]) */
    IsNullable: boolean;
    /**是否要列為進階搜尋 (對應原欄位[fsIS_SEARCH]) */
    IsSearch: boolean;
}
