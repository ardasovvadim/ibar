import {Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {ChartDataModel} from '../models/chart-data.model';
import {ApiService} from './api.service';
import {MasterAccountModel} from '../models/master-account.model';
import {Period} from '../models/period';
import {catchError} from 'rxjs/operators';
import {MasterAccountService} from './master-account.service';
import {TradeAccountModel} from '../models/trade-account.model';
import {DateConvector} from '../utils/date-convector';
import {DashboardEnum, dashboardEnumToArrayKeys} from '../../ui-reporting/pages/dashboard/dashboard.enum';
import {TotalDataModel} from '../../ui-reporting/pages/dashboard/total-data.model';
import {FilterService} from './filter.service';

@Injectable()
export class DashboardService {

  private selectedCardType: DashboardEnum = DashboardEnum.TOTAL_INCOME;

  constructor(private api: ApiService,
              private masterAccountService: MasterAccountService,
              private filterService: FilterService) {
  }

  getSelectedCardType() {
    return this.selectedCardType;
  }

  setSelectedCardType(value: DashboardEnum) {
    this.selectedCardType = value;
    this.filterService.emitEvent();
  }

  getChartData(chartType: DashboardEnum = this.selectedCardType,
               period: Period = this.filterService.selectedPeriod,
               masterAccounts: MasterAccountModel[] = this.filterService.selectedMasterAccounts,
               tradeAccounts: TradeAccountModel[] = this.filterService.selectedTradeAccounts): Observable<ChartDataModel> {
    const body = this.getRequestBodyChartData(period, masterAccounts, tradeAccounts);
    const url = this.resolveUrl('dashboard/', chartType);
    return this.api.post(url, body).pipe(
      catchError(err => {
        return of(ChartDataModel.GetDefaultChartDataModel());
      })
    );
  }

  getTotalData(cardTypes: DashboardEnum[] = dashboardEnumToArrayKeys(),
               period: Period = this.filterService.selectedPeriod,
               masterAccounts: MasterAccountModel[] = this.filterService.selectedMasterAccounts,
               tradeAccounts: TradeAccountModel[] = this.filterService.selectedTradeAccounts): Observable<TotalDataModel> {
    const body = this.getRequestBodyTotalData(cardTypes, period, masterAccounts, tradeAccounts);
    const url = 'dashboard/GetTotals';
    return this.api.post(url, body);
  }

  private resolveUrl(url: string, chartType: DashboardEnum): string {
    switch (chartType) {
      case DashboardEnum.TOTAL_INCOME:
        url += 'total-income/';
        return url;
      case DashboardEnum.TOTAL_CLIENTS:
        url += 'total-clients/';
        return url;
      case DashboardEnum.TOTAL_AUM:
        url += 'total-aum/';
        return url;
      case DashboardEnum.AVG_INCOME_CLIENT:
        url += 'avg-income-client';
        return url;
      case DashboardEnum.AVG_ACCOUNT_SIZE:
        url += 'avg-account-size';
        return url;
      case DashboardEnum.AVG_DAILY_INCOME:
        url += 'avg-daily-income';
        return url;
      case DashboardEnum.DEPOSITS:
        url += 'deposits';
        return url;
      case DashboardEnum.WITHDRAWALS:
        url += 'withdrawals';
        return url;
      case DashboardEnum.DW_BALANCE:
        url += 'dw-balance';
        return url;
      case DashboardEnum.TOTAL_STOCKS:
        url += 'total-stocks';
        return url;
      case DashboardEnum.TOTAL_OPTIONS:
        url += 'total-options';
        return url;
      case DashboardEnum.TOTAL_FUTURES:
        url += 'total-futures';
        return url;
      case DashboardEnum.TOTAL_NEW_CLIENTS:
        url += 'total-new-clients';
        return url;
      case DashboardEnum.TOTAL_ABANDOMENT:
        url += 'total-abandoment';
        return url;
      case DashboardEnum.NET_CHANGE_IN_CLIENTS:
        url += 'net-change-in-clients';
        return url;
    }
  }

  private getRequestBodyChartData(period: Period, masterAccounts: MasterAccountModel[], tradeAccounts: TradeAccountModel[]): object {
    const periods = Period.splitPeriod(period);
    const stringPeriods = [];
    periods.forEach(p => {
      stringPeriods.push({
        startDate: DateConvector.toUrlDate(p.fromDate),
        endDate: DateConvector.toUrlDate(p.toDate),
      });
    });
    return {
      periods: stringPeriods,
      idMasterAccounts: this.masterAccountsToStringArray(masterAccounts),
      idTradeAccounts: this.tradeAccountsToNumberArray(tradeAccounts)
    };
  }

  private masterAccountsToStringArray(masterAccount: MasterAccountModel[]): number[] {
    const result: number[] = [];
    masterAccount.forEach(element => {
      result.push(element.id);
    });
    return result;
  }

  private tradeAccountsToNumberArray(tradeAccounts: TradeAccountModel[]): number[] {
    const result: number[] = [];
    tradeAccounts.forEach(element => result.push(element.id));
    return result;
  }

  private getRequestBodyTotalData(dashboardTypes: DashboardEnum[], period: Period, masterAccounts: MasterAccountModel[], tradeAccounts: TradeAccountModel[]) {
    const stringPeriod = {
      startDate: DateConvector.toUrlDate(period.fromDate),
      endDate: DateConvector.toUrlDate(period.toDate),
    };
    return {
      dashboardTypes: dashboardTypes,
      period: stringPeriod,
      idMasterAccounts: this.masterAccountsToStringArray(masterAccounts),
      idTradeAccounts: this.tradeAccountsToNumberArray(tradeAccounts)
    };
  }
}

