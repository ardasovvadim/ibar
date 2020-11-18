import {TradingPermissionModel} from './trading-permission.model';
import {TradeAccountNoteModel} from '../note/trade-account-note.model';
import {TradeRank} from './trade-rank.enum';

export class AccountDetailsModel {
  id: number;
  accountName: string;
  name: string;
  countryResidentialAddress: string;
  cityResidentialAddress: string;
  streetResidentialAddress: string;
  postalCode: string;
  primaryEmail: string;
  currency: string;
  accountCapabilities: string;
  customerType: string;
  dateOpened: Date;
  tradingPermissions: TradingPermissionModel[];
  tradeAccountNotes: TradeAccountNoteModel[];
  dateFunded: Date;
  tradeRank: TradeRank;
  masterAccount: string = null;
  dateClosed: Date;
}
