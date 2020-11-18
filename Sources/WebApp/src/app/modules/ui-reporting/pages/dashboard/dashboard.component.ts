import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {DashboardEnum, getDefaultTotalData} from './dashboard.enum';
import {DashboardService} from '../../../core/services/dashboard.service';
import {BarChartComponent} from '../../components/charts/bar-chart/bar-chart.component';
import {ChartDataModel} from '../../../core/models/chart-data.model';
import {ApiService} from '../../../core/services/api.service';
import {ChartDataSets} from 'chart.js';
import {catchError, finalize} from 'rxjs/operators';
import {of, Subscription} from 'rxjs';
import {LoadingService} from '../../../core/services/loading.service';
import {Colors} from '../../../core/models/chart-color-set';
import * as Color from 'color';
import {FilterService} from '../../../core/services/filter.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.sass']
})
export class DashboardComponent implements OnInit, OnDestroy {

  public totals: number[] = getDefaultTotalData().totals;
  readonly cardType = DashboardEnum;

  chartData: ChartDataModel = ChartDataModel.GetDefaultChartDataModel();
  @ViewChild(BarChartComponent, {static: false}) chartComponent: BarChartComponent;
  private subscriptions: Subscription[] = [];
  chartFormat: 'string' | 'money' = 'money';

  constructor(private filterService: FilterService,
              private dashboardService: DashboardService,
              private api: ApiService,
              private loading: LoadingService) {
  }

  ngOnInit() {
    const sub = this.filterService.filtersChangedEvent.subscribe(() => {
      this.refreshData();
      this.refreshTotals();
    });
    this.subscriptions.push(sub);

    this.refreshData();
    this.refreshTotals();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  onClickCard(cardType: DashboardEnum) {
    this.dashboardService.setSelectedCardType(cardType);
  }

  private refreshData() {
    this.dashboardService.getChartData()
      .subscribe(data => {
        const chartData = data.data as ChartDataSets[];
        chartData.forEach((dataSet, index) => {
          dataSet.minBarLength = 10;
          const color = Colors.getColor(index, 1);
          dataSet.backgroundColor = Color(color).lighten(0.1).hex();
          dataSet.hoverBackgroundColor = color;
        });
        this.chartData = data;
        this.refreshChart();
      });
  }

  private refreshChart() {
    this.chartComponent.ngOnInit();
  }

  private refreshTotals() {
    this.loading.setLoading(true);
    this.dashboardService.getTotalData()
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

  resolveCardName() {
    switch (this.dashboardService.getSelectedCardType()) {
      case DashboardEnum.TOTAL_INCOME:
        return 'Total Income';
      case DashboardEnum.TOTAL_CLIENTS:
        return 'Total Clients';
      case DashboardEnum.TOTAL_AUM:
        return 'Total Aum';
      default:
        return '';
    }
  }
}
