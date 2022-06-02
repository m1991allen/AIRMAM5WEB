import { CreateNodeModel, CreateNodeRealModel } from './CreateNodeModel';

/**編輯節點(前端介接) */
export interface EditNodeModel extends CreateNodeModel {}
/**編輯節點(傳入給後端) */
export interface EditNodeRealModel extends CreateNodeRealModel {}
