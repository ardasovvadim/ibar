export class OpenPositionModel {
  symbol: string;
  position: number;
  markPrice: number;
  costBasisPrice: number;
  costBasisMoney: number;
  percentOfNav: number;
  fifoPnlUnrealized: number;
  description: string;
  assetCategory: string;
  underlyingListingExchange: string;
  currency: string;
  isin: string;
  conid: number;
  fxRateToBase: number;
  underlyingSymbol: string;
  putCall: string;
  multiplier: number;
  strike: number;
  expiry: Date;
  reportDate: Date;
}
