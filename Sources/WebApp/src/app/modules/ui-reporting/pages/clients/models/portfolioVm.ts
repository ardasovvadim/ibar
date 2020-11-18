export class PortfolioVm {
  accountId: number;
  accountName: string;
  name: string;

  total: number;
  cash: number;
  stock: number;
  options: number;
  commodities: number;
  interestAccruals: number;

  totalLong: number;
  cashLong: number;
  stockLong: number;
  optionsLong: number;
  commoditiesLong: number;
  interestAccrualsLong: number;

  totalShort: number;
  cashShort: number;
  stockShort: number;
  optionsShort: number;
  commoditiesShort: number;
  interestAccrualsShort: number;
}
