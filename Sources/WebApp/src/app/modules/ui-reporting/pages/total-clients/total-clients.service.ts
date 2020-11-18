import { Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { MasterAccountService } from '../../../core/services/master-account.service';
import { FilterService } from '../../../core/services/filter.service';
import { Period } from '../../../core/models/period';
import { MasterAccountModel } from '../../../core/models/master-account.model';
import { TradeAccountModel } from '../../../core/models/trade-account.model';
import { Observable } from 'rxjs';
import { TotalDataModel } from '../dashboard/total-data.model';
import { DateConvector } from '../../../core/utils/date-convector';
import { TotalAccountsEnum, totalAccountsEnumToArrayKeys } from './total-accounts.enum';
import { PaginationModel } from '../../../core/models/pagination.model';
import { TotalClientsTableModel } from './total-clients-table.model';
import { TotalClientsListModel } from './total-clients-list.model';
import { HttpParams } from '@angular/common/http';

@Injectable()
export class TotalClientsService {

  private searchExpression = '';

  constructor(private api: ApiService,
    private masterAccountService: MasterAccountService,
    private filterService: FilterService) {
  }

  getTotalData(cardTypes: TotalAccountsEnum[] = totalAccountsEnumToArrayKeys(),
    period: Period = this.filterService.selectedPeriod,
    masterAccounts: MasterAccountModel[] = this.filterService.selectedMasterAccounts,
    tradeAccounts: TradeAccountModel[] = this.filterService.selectedTradeAccounts): Observable<TotalDataModel> {
    const body = this.getRequestBodyTotalData(cardTypes, period, masterAccounts, tradeAccounts);
    const url = 'total-accounts/data/get-totals';
    return this.api.post(url, body);
  }

  private getRequestBodyTotalData(dashboardTypes: TotalAccountsEnum[], period: Period, masterAccounts: MasterAccountModel[], tradeAccounts: TradeAccountModel[]) {
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

  getAll(): Observable<TotalClientsTableModel[]> {
    const idTradeAccounts = this.filterService.selectedTradeAccounts.map(acc => acc.id);
    const idMasterAccounts = this.filterService.selectedMasterAccounts.map(acc => acc.id);
    return this.api.post(`total-accounts/data/get-table`, {
      idTradeAccounts,
      idMasterAccounts
    });
  }

  filterList(list: number, value: string): Observable<TotalClientsListModel> {
    const idTradeAccounts = this.filterService.selectedTradeAccounts.map(acc => acc.id);
    const idMasterAccounts = this.filterService.selectedMasterAccounts.map(acc => acc.id);
    return this.api.post(`total-accounts/search`, {
      SearchExpression: value,
      Type: list,
      idTradeAccounts,
      idMasterAccounts
    });
  }

  getList(period: Period = this.filterService.selectedPeriod): Observable<Array<TotalClientsListModel>> {
    const idTradeAccounts = this.filterService.selectedTradeAccounts.map(acc => acc.id);
    const idMasterAccounts = this.filterService.selectedMasterAccounts.map(acc => acc.id);
    const stringPeriod = {
      startDate: DateConvector.toUrlDate(period.fromDate),
      endDate: DateConvector.toUrlDate(period.toDate)
    };
    return this.api.post(`total-accounts/data/get-list`, {
      period: stringPeriod,
      idTradeAccounts,
      idMasterAccounts
    });
  }
}
