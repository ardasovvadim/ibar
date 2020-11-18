import {IEntityModel} from '../../../../core/models/i-entity.model';

export class NavModel implements IEntityModel {
  id: number;
  nav: string;
  cash: string;
  stock: string;
  options: string;
  commodities: string;
  interestAccruals: string;
  longNav: string;
  longCash: string;
  longStock: string;
  longOptions: string;
  longCommodities: string;
  longInterestAccruals: string;
  shortNav: string;
  shortCash: string;
  shortStock: string;
  shortOptions: string;
  shortCommodities: string;
  shortInterestAccruals: string;
}
