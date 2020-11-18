import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BehaviourComponentService } from '../../../core/services/behaviour-component.service';
import { FilterService } from '../../../core/services/filter.service';
import { getDefaultTotalData, TotalAccountsEnum } from './total-accounts.enum';
import { LoadingService } from '../../../core/services/loading.service';
import { TotalClientsService } from './total-clients.service';
import { catchError, finalize } from 'rxjs/operators';
import { of, Subscription } from 'rxjs';
import { BarChartComponent } from '../../components/charts/bar-chart/bar-chart.component';
import { MatTable } from '@angular/material/table';
import { TotalClientsTableModel } from './total-clients-table.model';
import { TotalAccountListEnum } from './total-account-list.enum';

@Component({
  selector: 'app-total-clients',
  templateUrl: './total-clients.component.html',
  providers: [TotalClientsService],
  styleUrls: ['./total-clients.component.sass']
})
export class TotalClientsComponent implements OnInit, OnDestroy {

  @ViewChild(BarChartComponent, { static: false }) chartComponent: BarChartComponent;
  private subscriptions: Subscription[] = [];
  displayedColumns: string[] = ['accountName', 'lastDay', 'mtd', 'lastMonth', 'avgDaily', 'mAvgDaily'];
  @ViewChild('table', { static: false }) table: MatTable<TotalClientsTableModel[]>;
  tableData: TotalClientsTableModel[] = [{
    id: 1,
    rowName: 'NEW CLIENTS(FINISHED APP)',
    mtd: '0',
    lastDay: '0',
    avgDailyMonth: '0',
    avgDailyYear: '0',
    lastMonth: '0'
  },
  {
    id: 1,
    rowName: 'NEW CLIENTS DEPOSITS',
    mtd: '0',
    lastDay: '0',
    avgDailyMonth: '0',
    avgDailyYear: '0',
    lastMonth: '0'
  }];
  readonly cardType = TotalAccountsEnum;

  public totals: number[] = getDefaultTotalData().totals;
  card: any;
  readonly listType = TotalAccountListEnum;
  listData =
    [
      [
        { name: 'Israel', value: 800 },
        { name: 'Ukraine', value: 500 },
        { name: 'Spain', value: 300 },
      ],
      [
        { name: 'Jerusalem', value: 300 },
        { name: 'Kiev', value: 300 },
        { name: 'Madrid', value: 300 },
      ],
      [
        { name: 'Margin', value: 800 },
        { name: 'P Margin', value: 500 },
        { name: 'Cash', value: 300 },
      ],
      [
        { name: 'Broker', value: 800 },
        { name: 'Advisor Master', value: 500 },
        { name: 'Advisor client', value: 300 },
      ],
      [
        { name: 'IB UK', value: 800 },
        { name: 'IB LLC', value: 500 },
        { name: 'IB HK', value: 300 },
      ],
      [
        { name: 'EUR', value: 800 },
        { name: 'USD', value: 500 },
        { name: 'ILS', value: 300 },
      ]
    ];

  constructor(private filterService: FilterService,
    private behaviourComponentService: BehaviourComponentService,
    private totalClientService: TotalClientsService,
    private loading: LoadingService) {
  }

  ngOnInit(): void {
    this.behaviourComponentService.setNavMenuRangeDatepic(false);

    this.filterService.filtersChangedEvent.subscribe(() => {
      this.refreshTotals();
      this.refreshTable();
      this.refreshList();
    });

    this.refreshTotals();
    this.refreshTable();
    this.refreshList();
  }

  ngOnDestroy(): void {
    this.behaviourComponentService.setNavMenuRangeDatepic(true);
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private refreshChart() {
    this.chartComponent.ngOnInit();
  }

  private refreshTotals() {
    this.loading.setLoading(true);
    this.totalClientService.getTotalData()
      .pipe(catchError(err => {
        return of(getDefaultTotalData());
      }),
        finalize(() => {
          this.loading.setLoading(false);
        }))
      .subscribe(data => {
        this.totals = data.totals;
      });
  }

  private refreshTable() {
    this.loading.setLoading(true);
    this.totalClientService.getAll()
      .pipe(
        catchError(err => {
          return of(null);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data != null) {
          this.tableData = data;
          this.table.renderRows();
        }
      });
  }

  private refreshList() {
    this.loading.setLoading(true);
    this.totalClientService.getList()
      .pipe(
        catchError(err => {
          return of(null);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data != null) {
          this.listData = data
            .map(value => {
              return value.accountTotalList.map(val => {
                return {
                  name: val.key,
                  value: val.value
                };
              });
            });
        }
      });
  }

  getListName(listEnum: TotalAccountListEnum, $event: string) {
    this.totalClientService.filterList(listEnum, $event).subscribe(data => {
      this.listData[data.enum] = data.accountTotalList.map(value => {
        return {
          name: value.key,
          value: value.value
        };
      });
    });
  }
}


