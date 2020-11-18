import {Injectable} from '@angular/core';
import {ApiService} from '../../../core/services/api.service';
import {Period} from '../../../core/models/period';
import {OptionsModel} from '../../../core/models/options';
import {HttpHeaders} from '@angular/common/http';
import {TradeAccountModel} from '../../../core/models/trade-account.model';
import {Observable, of} from 'rxjs';
import {PaginationModel} from '../../../core/models/pagination.model';
import {ChartDataModel} from '../../../core/models/chart-data.model';
import {DateConvector} from '../../../core/utils/date-convector';
import {FilterService} from '../../../core/services/filter.service';
import {IdNameModel} from '../../../core/models/id-name.model';
import {BehaviourComponentService} from '../../../core/services/behaviour-component.service';
import {catchError} from 'rxjs/operators';
import {MathHelper} from '../../../core/utils/math-helper';

@Injectable({
  providedIn: 'root'
})
export class AccountInfoService {

  private readonly controllerPath = 'accounts';
  // for saving data
  private accountIdNames: IdNameModel[] = [];
  private navigateAccountIndex = 0;
  // ---
  pageIndex = 0;
  pageLength = 10;
  dataLength = 0;
  sortBy = '';
  navigateLeftPageIndex = this.pageIndex;
  navigateRightPageIndex = this.pageIndex;

  // ---

  constructor(private api: ApiService,
              private filterService: FilterService,
              private behaviourComponentService: BehaviourComponentService) {
  }

  getAll(pageIndex: number, pageLength: number, period: Period, sortBy?: string): Observable<PaginationModel<TradeAccountModel>> {
    const options = this.getOptions(pageIndex, pageLength, sortBy);
    const idTradeAccounts = this.filterService.selectedTradeAccounts.map(acc => acc.id);
    const idMasterAccounts = this.filterService.selectedMasterAccounts.map(acc => acc.id);
    const stringPeriod = {
      startDate: DateConvector.toUrlDate(period.fromDate),
      endDate: DateConvector.toUrlDate(period.toDate)
    };
    return this.api.post(`${this.controllerPath}/all`, {
      idTradeAccounts,
      idMasterAccounts,
      periods: [stringPeriod]
    }, options);
  }

  private getOptions(pageIndex: number, pageLength: number, sortBy?: string): OptionsModel {
    const options = new OptionsModel();
    let headers = new HttpHeaders().append('Pagination', `${pageIndex};${pageLength}`);
    if (sortBy) {
      headers = headers.append('Sorting', sortBy);
    }
    options.headers = headers;
    return options;
  }

  loadNewClients(period: Period): Observable<ChartDataModel> {
    const stringPeriod = {
      startDate: DateConvector.toUrlDate(period.fromDate),
      endDate: DateConvector.toUrlDate(period.toDate),
    };
    return this.api.post(`${this.controllerPath}/newclients`, stringPeriod);
  }

  getIdNames(): IdNameModel[] {
    return this.accountIdNames;
  }

  setIdNames(value: IdNameModel[]) {
    this.accountIdNames = value;
  }

  setNavigateAccountIndex(index: number) {
    this.navigateAccountIndex = index;
    this.emitButtonNavEvent();

    if (this.navigateAccountIndex < 3) {
      this.getNextListTradeAccountsIdName('prev');
    } else if (this.accountIdNames.length - this.navigateAccountIndex <= 3) {
      this.getNextListTradeAccountsIdName('next');
    }
  }

  private emitButtonNavEvent() {
    const event = {isPrev: this.isPrev(), isNext: this.isNext()};
    this.behaviourComponentService.emitNavMenuButtonNavEvent(event);
  }

  isPrev(): boolean {
    return ((this.navigateAccountIndex - 1) >= 0) || (this.navigateLeftPageIndex > 0);
  }

  isNext(): boolean {
    const val = MathHelper.Round(this.dataLength / this.pageLength);
    return ((this.navigateAccountIndex + 1) < this.accountIdNames.length) || (val > this.navigateRightPageIndex);
  }

  getPrev(): IdNameModel {
    this.setNavigateAccountIndex(this.navigateAccountIndex - 1);
    if (this.navigateAccountIndex < 3) {
      this.getNextListTradeAccountsIdName('prev');
    }
    return this.accountIdNames[this.navigateAccountIndex];
  }

  getNext(): IdNameModel {
    this.setNavigateAccountIndex(this.navigateAccountIndex + 1);
    if (this.accountIdNames.length - this.navigateAccountIndex <= 3) {
      this.getNextListTradeAccountsIdName('next');
    }
    return this.accountIdNames[this.navigateAccountIndex];
  }

  private getNextListTradeAccountsIdName(option: 'prev' | 'next') {
    if (option === 'prev' && this.navigateLeftPageIndex > 0) {
      this.navigateLeftPageIndex -= 1;
      this.getTradeAccountsIdName(this.navigateLeftPageIndex)
        .pipe(catchError(() => of([])))
        .subscribe(data => {
          if (data.length > 0) {
            this.accountIdNames.unshift(...data);
            this.navigateAccountIndex += this.pageLength;
          }
        });
    } else if (option === 'next' && (MathHelper.Round(this.dataLength / this.pageLength) > this.navigateRightPageIndex)) {
      this.navigateRightPageIndex += 1;
      this.getTradeAccountsIdName(this.navigateRightPageIndex)
        .pipe(catchError(() => of([])))
        .subscribe(data => {
          if (data.length > 0) {
            this.accountIdNames.push(...data);
          }
        });
    }
  }

  private getTradeAccountsIdName(pageIndex: number): Observable<IdNameModel[]> {
    const stringPeriod = {
      startDate: DateConvector.toUrlDate(this.filterService.selectedPeriod.fromDate),
      endDate: DateConvector.toUrlDate(this.filterService.selectedPeriod.toDate)
    };
    const body = {
      pageIndex,
      pageLength: this.pageLength,
      period: stringPeriod,
      tradeAccounts: this.filterService.selectedTradeAccounts,
      masterAccounts: this.filterService.selectedMasterAccounts,
      sortBy: this.sortBy
    };
    return this.api.post(this.controllerPath + '/id-name', body);
  }


  setDataLength(dataLength: number) {
    this.dataLength = dataLength;
  }

  setPageIndex(index: number) {
    this.pageIndex = this.navigateRightPageIndex = this.navigateLeftPageIndex = index;
  }
}
