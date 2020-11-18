import {IEntityModel} from '../../../../core/models/i-entity.model';

export class TradeModel implements IEntityModel {
  id: number;
  date: string;
  symbol: string;
  quantity: string;
  tradePrice: string;
  closePrice: string;
  positionValue: string;
  realizedPL: string;
  description: string;
  assetCategory: string;
  listingExcange: string;
  currency: string;
  isin: string;
  conid: string;
  fxToBase: string;
  underSymbol: string;
  pc: string;
  multi: string;
  strike: string;
  expiry: string;
  settleDate: string;
  taxes: string;
  ibCommission: string;
  openClose: string;
  transactionId: string;
  ibOrderId: string;
  ibExeId: string;
  orderRefer: string;
  volOrder: string;
  orderTime: string;
  orderType: string;
  buySell: string;
  apiOrder: string;
}
