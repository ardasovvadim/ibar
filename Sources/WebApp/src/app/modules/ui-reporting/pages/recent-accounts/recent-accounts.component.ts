import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {AccountInfoService} from './account-info.service';
import {TradeAccountModel} from '../../../core/models/trade-account.model';
import {Sort} from '@angular/material/sort';
import {DateConvector} from '../../../core/utils/date-convector';
import {MatTable} from '@angular/material/table';
import {ChartDataModel} from '../../../core/models/chart-data.model';
import {FilterService} from '../../../core/services/filter.service';
import {LoadingService} from '../../../core/services/loading.service';
import {catchError, finalize} from 'rxjs/operators';
import {of, Subscription} from 'rxjs';
import {PaginationParams} from '../../../ui-common/components/custom-pagination/custom-pagination.component';
import {Router} from '@angular/router';
import {IdNameModel} from '../../../core/models/id-name.model';

@Component({
  selector: 'app-sales',
  templateUrl: './recent-accounts.component.html',
  styleUrls: ['./recent-accounts.component.sass']
})
export class RecentAccountsComponent implements OnInit, OnDestroy {

  chartData: ChartDataModel = ChartDataModel.GetDefaultChartDataModel();
  // ---
  @ViewChild('table', {static: false}) table: MatTable<TradeAccountModel[]>;
  pageSizeOptions: number[] = [10, 20, 50];
  tableData: TradeAccountModel[] = [];
  dataLength = 0;
  displayedColumns: string[] = ['accountName', 'dateFunded', 'mobile', 'city', 'tools'];
  private subscriptions: Subscription[] = [];
  matSortActive = '';
  matSortDirection = '';

  private setPageIndex = (value: number) => this.accountInfoService.setPageIndex(value);
  get pageIndex(): number {
    return this.accountInfoService.pageIndex;
  }

  private setSortBy = (value: string) => this.accountInfoService.sortBy = value;
  get sortBy(): string {
    return this.accountInfoService.sortBy;
  }

  private setPageLength = (value: number) => this.accountInfoService.pageLength = value;
  get pageLength(): number {
    return this.accountInfoService.pageLength;
  }


  constructor(private accountInfoService: AccountInfoService,
              private filterService: FilterService,
              private loading: LoadingService,
              private router: Router) {
    this.setPageLength(this.accountInfoService.pageLength);
    this.setPageIndex(this.accountInfoService.pageIndex);
    this.setSortBy(this.accountInfoService.sortBy);
    if (this.sortBy !== '') {
      const sorting = this.sortBy.split(';');
      this.matSortActive = sorting[0];
      this.matSortDirection = sorting[1];
    }
  }

  ngOnInit() {
    this.refreshChart();
    this.refreshTable();

    const sub = this.filterService.filtersChangedEvent.subscribe($event => {
      if ($event.isPeriodChanged) {
        this.setPageIndex(0);
        this.refreshChart();
        this.refreshTable();
      } else if ($event.isTradeAccountChanged || $event.isMasterAccountChanged) {
        this.setPageIndex(0);
        this.refreshTable();
      }
    });
    this.subscriptions.push(sub);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  onSortChange($event: Sort) {
    if ($event.direction === '') {
      this.setSortBy('');
    } else {
      this.setSortBy(`${$event.active};${$event.direction}`);
    }

    this.setPageIndex(0);

    this.refreshTable();
  }

  resolveName(acc: TradeAccountModel): string {
    return TradeAccountModel.resolveName(acc);
  }

  dateToString(acc: TradeAccountModel) {
    return DateConvector.DateToString(acc.dateFunded);
  }

  private refreshTable() {
    this.loading.setLoading(true);
    this.accountInfoService.getAll(this.pageIndex, this.pageLength, this.filterService.selectedPeriod, this.sortBy)
      .pipe(
        catchError(err => {
          return of(null);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        debugger;
        if (data != null) {
          this.tableData = data.data;
          this.dataLength = data.dataLength;
          this.table.renderRows();

          const idNames: IdNameModel[] = [];
          data.data.forEach(acc => idNames.push({id: acc.id, name: acc.accountName}));
          this.accountInfoService.setIdNames(idNames);
          this.accountInfoService.setDataLength(data.dataLength);
        }
      });
  }

  private refreshChart() {
    this.loading.setLoading(true);
    this.accountInfoService.loadNewClients(this.filterService.selectedPeriod)
      .pipe(
        catchError(err => of(ChartDataModel.GetDefaultChartDataModel())),
        finalize(() => this.loading.setLoading(false))
      ).subscribe(data => this.chartData = data);
  }

  onChangePage($event: PaginationParams) {
    this.setPageLength($event.pageLength);
    this.setPageIndex($event.pageIndex);

    this.refreshTable();
  }

  goToDetails(acc: TradeAccountModel) {
    const index = this.accountInfoService.getIdNames().findIndex(a => a.id === acc.id);
    this.accountInfoService.setNavigateAccountIndex(index);

    this.router.navigate([`/clients/${acc.accountName}`]);
  }
}
