import {IEntityModel} from '../../../../core/models/i-entity.model';

export class TradingPermission implements IEntityModel {
  id: number;
  name: string;
  value: boolean;
}
