import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ChartDataSets, ChartOptions, ChartType} from 'chart.js';
import {BaseChartDirective, Label} from 'ng2-charts';
import {Period} from '../../../../core/models/period';
import {DashboardService} from '../../../../core/services/dashboard.service';
import {DashboardEnum} from '../../dashboard/dashboard.enum';
import {DATA} from '../../../../../mock-data/data-set';

@Component({
  selector: 'app-income-pie-block',
  templateUrl: './income-pie-block.component.html',
  styleUrls: ['./income-pie-block.component.sass']
})
export class IncomePieBlockComponent implements OnInit {

  exceptPeriods = ['Q1', 'Q2', 'Q3', 'Q4'];

  @Input() cardType: DashboardEnum;
  private _selectedPeriod: Period = Period.getDefault();
  @Input() title: string;

  public pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    legend: {position: 'bottom'},
    plugins: {
      datalabels: {
        formatter: (value, ctx) => {
          return ctx.chart.data.labels[ctx.dataIndex];
        },
      },
    }
  };
  public pieChartLabels: Label[] = ['Outbound', 'Inbound'];
  public pieChartData: number[];
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartColors = [{backgroundColor: ['black', 'grey']}];
  @ViewChild('chart', {static: true}) chart: BaseChartDirective;

  constructor(private dataService: DashboardService) {
  }

  ngOnInit() {
    this.updateData();
  }

  private updateData(): void {
    // TODO: until implements API
    this.pieChartData = DATA[0].data[0].data as number[];
    this.pieChartLabels = DATA[0].labels;
    // ---
  }

  private refresh(): void {
    this.chart.ngOnInit();
  }

  onPeriodChange(period: Period) {
    this._selectedPeriod = period;
    this.updateData();
  }

}
