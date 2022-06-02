import { EditTempleteModel } from './EditTempleteModel';
import { templateFieldTime } from './templateFieldTime';

/**編輯樣版(後端所需) */
export interface EditTempleteRealModel extends EditTempleteModel, templateFieldTime {}
