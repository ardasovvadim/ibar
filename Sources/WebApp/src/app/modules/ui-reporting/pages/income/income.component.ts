import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {DashboardService} from '../../../core/services/dashboard.service';
import {BehaviourComponentService} from '../../../core/services/behaviour-component.service';
import {TotalClientsTableModel} from '../total-clients/total-clients-table.model';
import {catchError, finalize} from 'rxjs/operators';
import {of, Subscription} from 'rxjs';
import {LoadingService} from '../../../core/services/loading.service';
import {IncomeService} from './income.service';
import {MatTable} from '@angular/material/table';
import {ChartDataModel} from '../../../core/models/chart-data.model';
import {FilterService} from '../../../core/services/filter.service';
import {BarChartComponent} from '../../components/charts/bar-chart/bar-chart.component';
import {IncomeEnum} from './income-barchart.enum';

@Component({
  selector: 'app-income',
  templateUrl: './income.component.html',
  styleUrls: ['./income.component.sass']
})
export class IncomeComponent implements OnInit, OnDestroy {

  chartData: ChartDataModel = ChartDataModel.GetDefaultChartDataModel();
  readonly cardType = IncomeEnum;
  @ViewChild(BarChartComponent, {static: false}) chartComponent: BarChartComponent;
  private subscriptions: Subscription[] = [];
  @ViewChild('table', {static: false}) table: MatTable<TotalClientsTableModel[]>;
  displayedColumns: string[] = ['accountName', 'lastDay', 'mtd', 'lastMonth', 'avgDaily', 'mAvgDaily'];
  tableData: TotalClientsTableModel[] = [{
    id: 0,
    rowName: 'TOTAL INCOME',
    mtd: '0',
    lastDay: '0',
    avgDailyMonth: '0',
    avgDailyYear: '0',
    lastMonth: '0'
  },
    {
      id: 1,
      rowName: 'TRADING COMMISSIONS',
      mtd: '0',
      lastDay: '0',
      avgDailyMonth: '0',
      avgDailyYear: '0',
      lastMonth: '0'
    }];
  constructor(private filterService: FilterService,
              private dataService: DashboardService,
              private incomeService: IncomeService,
              private behaviourComponentService: BehaviourComponentService,
              private loading: LoadingService) {
  }

  ngOnInit() {
    const sub = this.filterService.filtersChangedEvent.subscribe(() => {
      this.refreshTable();
    });
    this.subscriptions.push(sub);
    this.refreshTable();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private refreshTable() {
    this.loading.setLoading(true);
    this.incomeService.getTableData()
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

}
