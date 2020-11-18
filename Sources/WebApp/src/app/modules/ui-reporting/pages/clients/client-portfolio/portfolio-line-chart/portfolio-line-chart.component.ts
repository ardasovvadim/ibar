import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ChartDataSets, ChartOptions, ChartType} from 'chart.js';
import {BaseChartDirective, Label} from 'ng2-charts';
import {ChartDataModel} from '../../../../../core/models/chart-data.model';

@Component({
  selector: 'app-portfolio-line-chart',
  templateUrl: './portfolio-line-chart.component.html',
  styleUrls: ['./portfolio-line-chart.component.sass']
})
export class PortfolioLineChartComponent implements OnInit {

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    legend: {
      display: false,
      position: 'bottom'
    },
    scales: {
      xAxes: [{}],
      yAxes: [{
        display: false,
      }],
    },
    plugins: {
      datalabels: {
        anchor: 'end',
        align: 'end',
      }
    }
  };
  public barChartType: ChartType = 'line';
  public barChartLegend = true;

  @Input() set chartData(value: ChartDataModel) {
    this.barChartData = value.data;
    this.barChartLabels = value.labels;
  }

  public barChartData: ChartDataSets[] = [];
  public barChartLabels: Label[] = [];


  @ViewChild(BaseChartDirective, {static: true}) chart: BaseChartDirective;

  constructor() {
  }

  ngOnInit() {
  }

  refreshChart() {
    this.chart.ngOnInit();
  }

}
