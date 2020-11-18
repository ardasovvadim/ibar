export enum IncomeEnum {
  TOTAL_INCOME,
  TRADING_COMMISSIONS,
  INTEREST,
  BORROWING
}

// export function dashboardEnumToArrayKeys(): number[] {
//   const a = Object.keys(DashboardEnum).map(el => +el);
//   return a.filter(el => !isNaN(el));
// }
//
// export function getDefaultTotalData(): TotalDataModel {
//   return {totals: new Array(Object.values(DashboardEnum).length).fill(0)};
// }
