import { AnnMatainBaseModel } from './AnnMatainBaseModel';
import { CreateInfoModel, UpdateInfoModel } from '../Shared/IDate';

/**
 * 新增公告回應res.Data
 */
export interface AnnCreateResponseModel extends AnnMatainBaseModel, CreateInfoModel, UpdateInfoModel {
    /**是否隱藏 */
    IsHidden: boolean;
    /**公告Id */
    fnANN_ID: number;
}
