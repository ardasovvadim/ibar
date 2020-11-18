import {Injectable} from '@angular/core';
import {HttpParams} from '@angular/common/http';
import {ApiService} from '../../../core/services/api.service';
import {BehaviorSubject, Observable, of} from 'rxjs';
import {PortfolioVm} from './models/portfolioVm';
import {NAV} from '../../../../mock-data/portfolio';
import {DateConvector} from '../../../core/utils/date-convector';
import {NavModel} from './models/nav.model';
import {TradeModel} from './models/trade.model';
import {TRADES} from '../../../../mock-data/trades';
import {AccountDetailsModel} from './client-details/models/account-details.model';
import {TradingPermissionModel} from './client-details/models/trading-permission.model';
import {TradeAccountNoteModel} from './client-details/note/trade-account-note.model';
import {TradeRank} from './client-details/models/trade-rank.enum';
import {PortfolioChartType} from './client-portfolio/portfolio-chart-type.enum';
import {Period} from '../../../core/models/period';
import {PeriodHelper} from '../../../core/utils/period-helper';
import {ChartDataModel} from '../../../core/models/chart-data.model';
import {OpenPositionModel} from './client-portfolio/open-position.model';
import {TotalDataModel} from '../dashboard/total-data.model';
import {PaginationModel} from '../../../core/models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class ClientsService {

  baseUrl = 'accounts';
  selectedInfo = 'account';
  controllerUrl = 'accounts';
  private accountHeaderSubject = new BehaviorSubject<{ id: string; name: string }>(null);
  accountHeader = this.accountHeaderSubject.asObservable();

  constructor(private api: ApiService) {
  }

  setAccountHeader(obj: { id: string; name: string }) {
    this.accountHeaderSubject.next(obj);
  }

  getAccountInfo(accountName: string): Observable<AccountDetailsModel> {
    return this.api.get(`${this.controllerUrl}/account-info/${accountName}`);
  }

  getPortfolioData(accountName: string): Observable<PortfolioVm> {
    return this.api.get(`${this.controllerUrl}/portfolio/${accountName}`);
  }

  getNavComponents(idClient: number): Observable<NavModel> {
    // TODO: ardasovvadim: temporary
    return of(NAV);
    // ---
    const url = `${this.controllerUrl}portfolio`;
    const params = new HttpParams()
      .set('id', idClient.toString());
    return this.api.get(url, {params}) as Observable<NavModel>;
  }

  getTradingPermissions(): Observable<TradingPermissionModel[]> {
    return this.api.get(`${this.controllerUrl}/TradingPermission`);
  }

  addNewTradeAccountNote(tradeNote: TradeAccountNoteModel): Observable<number> {
    return this.api.post(`${this.controllerUrl}/trade-note`, tradeNote);
  }

  deleteTradeAccountNote(id: number): Observable<number> {
    return this.api.delete(`${this.controllerUrl}/trade-note/${id}`);
  }

  changeTradeAccountRank(param: { tradeRank: TradeRank; id: number }): Observable<number> {
    return this.api.put(`${this.controllerUrl}/trade-rank/`, param);
  }

  getPortfolioChartData(accountName: string, selectedPeriod: Period, selectedChartType: PortfolioChartType): Observable<ChartDataModel> {
    const periods = PeriodHelper.arrayPeriodToArrayStringPeriod(Period.splitPeriod(selectedPeriod));
    const body = {
      accountName,
      periods,
      chartType: selectedChartType
    };
    return this.api.post(`${this.controllerUrl}/portfolio/chart-data`, body);
  }

  getOpenPositions(date: string, accountName: string, pageLength: number, pageIndex: number): Observable<PaginationModel<OpenPositionModel>> {
    const body = {
      dateString: date,
      accountName,
      pageLength,
      pageIndex
    };
    return this.api.post(`${this.controllerUrl}/portfolio/open-positions`, body);
  }

  getPortfolioTotals(accountName: string, period: Period): Observable<TotalDataModel> {
    const periodString = PeriodHelper.periodToStringPeriod(period);
    const body = {
      accountName,
      periodString
    };
    return this.api.post(`${this.controllerUrl}/portfolio/totals`, body);
  }

  getAvailableDatesOpenPos(accountName: string): Observable<Date[]> {
    return this.api.get(`${this.controllerUrl}/portfolio/open-pos-dates/${accountName}`);
  }

  setSelectedInfo(page: string) {
    this.selectedInfo = page;
  }
}
