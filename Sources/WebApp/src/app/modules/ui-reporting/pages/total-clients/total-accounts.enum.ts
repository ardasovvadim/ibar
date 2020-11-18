import {TotalDataModel} from '../dashboard/total-data.model';

export enum TotalAccountsEnum {
  TOTAL_CLIENTS,
  NEW_CLIENTS_LD,
  NEW_CLIENTS_MTD
}

export function totalAccountsEnumToArrayKeys(): number[] {
  const a = Object.keys(TotalAccountsEnum).map(el => +el);
  return a.filter(el => !isNaN(el));
}

export function getDefaultTotalData(): TotalDataModel {
  return {totals: new Array(Object.values(TotalAccountsEnum).length).fill(0)};
}
