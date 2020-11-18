import {TotalDataModel} from './total-data.model';

export enum DashboardEnum {
  TOTAL_INCOME,
  TOTAL_CLIENTS,
  TOTAL_AUM,
  AVG_INCOME_CLIENT,
  AVG_ACCOUNT_SIZE,
  AVG_DAILY_INCOME,
  DEPOSITS,
  WITHDRAWALS,
  DW_BALANCE,
  TOTAL_STOCKS,
  TOTAL_OPTIONS,
  TOTAL_FUTURES,
  TOTAL_NEW_CLIENTS,
  TOTAL_ABANDOMENT,
  NET_CHANGE_IN_CLIENTS
}

export function dashboardEnumToArrayKeys(): number[] {
  const a = Object.keys(DashboardEnum).map(el => +el);
  return a.filter(el => !isNaN(el));
}

export function getDefaultTotalData(): TotalDataModel {
  return {totals: new Array(Object.values(DashboardEnum).length).fill(0)};
}
