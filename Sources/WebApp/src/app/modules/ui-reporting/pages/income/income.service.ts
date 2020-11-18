import {Injectable} from '@angular/core';
import {BehaviorSubject, Observable, of} from 'rxjs';
import {ApiService} from '../../../core/services/api.service';
import {TotalClientsTableModel} from '../total-clients/total-clients-table.model';
import {MasterAccountService} from '../../../core/services/master-account.service';
import {FilterService} from '../../../core/services/filter.service';
import {DashboardEnum} from '../dashboard/dashboard.enum';
import {Period} from '../../../core/models/period';
import {MasterAccountModel} from '../../../core/models/master-account.model';
import {TradeAccountModel} from '../../../core/models/trade-account.model';
import {ChartDataModel} from '../../../core/models/chart-data.model';
import {catchError} from 'rxjs/operators';
import {DateConvector} from '../../../core/utils/date-convector';
import {IncomeEnum} from './income-barchart.enum';

@Injectable({
  providedIn: 'root'
})
export class IncomeService {

  baseUrl = 'accounts';
  selectedInfo = 'account';
  controllerUrl = 'accounts';
  private accountHeaderSubject = new BehaviorSubject<{ id: string; name: string }>(null);
  accountHeader = this.accountHeaderSubject.asObservable();
  private selectedCardType: DashboardEnum = DashboardEnum.TOTAL_INCOME;

  constructor(private api: ApiService,
              private masterAccountService: MasterAccountService,
              private filterService: FilterService) {
  }
  setAccountHeader(obj: { id: string; name: string }) {
    this.accountHeaderSubject.next(obj);
  }

  getTableData(): Observable<TotalClientsTableModel[]> {
    const idTradeAccounts = this.filterService.selectedTradeAccounts.map(acc => acc.id);
    const idMasterAccounts = this.filterService.selectedMasterAccounts.map(acc => acc.id);
    return this.api.post(`income/data/get-table`, {
      idTradeAccounts,
      idMasterAccounts
    });  }

  getChartData(chartType: IncomeEnum,
               period: Period = this.filterService.selectedPeriod,
               masterAccounts: MasterAccountModel[] = this.filterService.selectedMasterAccounts,
               tradeAccounts: TradeAccountModel[] = this.filterService.selectedTradeAccounts): Observable<ChartDataModel> {
    const body = this.getRequestBodyChartData(period, masterAccounts, tradeAccounts);
    const url = this.resolveUrl('income/', chartType);
    return this.api.post(url, body).pipe(
      catchError(err => {
        return of(ChartDataModel.GetDefaultChartDataModel());
      })
    );
  }
  private resolveUrl(url: string, chartType: IncomeEnum): string {
    switch (chartType) {
      case IncomeEnum.TOTAL_INCOME:
        url += 'total-income/';
        return url;
      case IncomeEnum.TRADING_COMMISSIONS:
        url += 'trade-commissions/';
        return url;
      case IncomeEnum.INTEREST:
        url += 'trade-interest/';
        return url;
      case IncomeEnum.BORROWING:
        url += 'trade-borrowing';
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
}
