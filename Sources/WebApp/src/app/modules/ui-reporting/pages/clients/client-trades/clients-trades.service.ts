import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { TotalDataModel } from '../../dashboard/total-data.model';
import { FilterService } from '../../../../core/services/filter.service';
import { HttpParams } from '@angular/common/http';
import { totalClientsEnumToArrayKeys, TotalClientTotalsEnum } from './client-trades-totals.enum';
import { TotalClientsTableModel } from '../../total-clients/total-clients-table.model';
import { AccountTradesDetailsModel } from './client-trade-details.model';
import { Period } from '../../../../core/models/period';
import { TotalClientsListModel } from '../../total-clients/total-clients-list.model';
import { DateConvector } from '../../../../core/utils/date-convector';
import { PaginationModel } from '../../../../core/models/pagination.model';
import { TradeAccountModel } from '../../../../core/models/trade-account.model';

@Injectable({
  providedIn: 'root'
})
export class ClientsTradesService {
  controllerUrl = 'accounts';
  constructor(private api: ApiService,
    private filterService: FilterService) {
  }

  getTotalData(clientId: string, enums: TotalClientTotalsEnum[] = totalClientsEnumToArrayKeys()): Observable<string[]> {
    const url = 'accounts/totals/get';
    return this.api.post(url, { clientId: clientId, enums: enums });
  }

  getTableData(clientId: string): Observable<TotalClientsTableModel[]> {
    return this.api.post(`accounts/table/data/get`, { clientId: clientId });
  }

  getList(clientId: string): Observable<Array<TotalClientsListModel>> {
    return this.api.post(`accounts/list/data/get`, { clientId: clientId });
  }

  getAvailableTradeDates(accountName: string): Observable<Date[]> {
    return this.api.get(`accounts/trades/trades-dates/${accountName}`);
  }

  getTrades(date: string, accountName: string, pageLength: number, pageIndex: number): Observable<PaginationModel<AccountTradesDetailsModel>> {
    const body = {
      dateString: date,
      accountName,
      pageLength,
      pageIndex
    };
    return this.api.post(`accounts/forms/data/get`, body);
  }
}
