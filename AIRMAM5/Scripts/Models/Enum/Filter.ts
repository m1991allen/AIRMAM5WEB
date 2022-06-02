/**
 * 篩選器過濾運算子
 */
export enum Filter {
    Like = 'like',
    Equal = '=',
    NotEqual = '!=',
    MoreThan = '>',
    MoreThanOrEqualTo = '>=',
    LessThan = '<',
    LessThanOrEqualTo = '<=',
    In = 'in',
    Regex = 'regex',
}
