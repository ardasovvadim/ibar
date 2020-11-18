import {TotalDataModel} from '../../dashboard/total-data.model';
import {TotalAccountsEnum} from '../../total-clients/total-accounts.enum';

export enum TotalClientTotalsEnum {
  TRADER_STATUS,
  API,
  ACTIVE
}

export function totalClientsEnumToArrayKeys(): number[] {
  const a = Object.keys(TotalClientTotalsEnum).map(el => +el);
  return a.filter(el => !isNaN(el));
}

export function getDefaultTotalData(): TotalDataModel {
  return {totals: new Array(Object.values(TotalClientTotalsEnum).length).fill('0')};
}
