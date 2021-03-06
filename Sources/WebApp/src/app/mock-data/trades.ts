import {AccountTradesDetailsModel} from '../modules/ui-reporting/pages/clients/client-trades/client-trade-details.model';

export const TRADES: AccountTradesDetailsModel = {
  id: 0,
  isAPIOrder: 'apiOrder',
  reportDate: new Date(),
  assetCategory: 'assetCategory',
  buySell: 'buySell',
  closePrice: 0,
  conid: 0,
  currency: 'currency',
  description: 'description',
  expiry: new Date(),
  fxRateToBase: 0,
  ibCommission: 0,
  ibExecId: 'ibExeId',
  ibOrderID: 0,
  isin: 'isin',
  listingExchange: 'listingExcange',
  multiplier: 0,
  openCloseIndicator: 'openClose',
  orderReference: 'orderRefer',
  orderTime: new Date(),
  orderType: 'orderType',
  putCall: 'pc',
  quantity: 0,
  settleDateTarget: new Date(),
  strike: 0,
  symbol: 'symbol',
  taxes: 0,
  tradePrice: 0,
  transactionID: 0,
  underlyingSymbol: 'underSymbol',
  volatilityOrderLink: 0
};
