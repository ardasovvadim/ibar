import {TradingPermission} from './trading-permission.model';
import {IEntityModel} from '../../../../core/models/i-entity.model';

export class AccountModel implements IEntityModel {
  id: number;
  name: string;
  country?: string;
  city?: string;
  postcode?: number;
  street?: string;
  email?: string;
  accountNumber?: number;
  accountType?: string;
  dateOpened: Date;
  currency?: string;
  customerType?: string;
  dateFunded?: Date;
  cash?: number;
  master?: string;
  dateClosed?: Date;
  tradingPermissions: TradingPermission[];
  notes?: string[];
}
