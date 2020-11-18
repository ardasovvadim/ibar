import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ChartDataSets, ChartOptions, ChartType} from 'chart.js';
import {BaseChartDirective, Label} from 'ng2-charts';
import {CurrencyPipe, formatCurrency} from '@angular/common';
import {CustomCurrencyPipe} from '../../../../ui-common/pipes/custom-currency.pipe';

@Component({
  selector: 'app-bar-chart',
  templateUrl: './bar-chart.component.html',
  styleUrls: ['./bar-chart.component.sass']
})
export class BarChartComponent implements OnInit {

  public barChartOptions: ChartOptions = {
    // responsive: true,
    maintainAspectRatio: false,
    scales: {xAxes: [{}], yAxes: [{ticks: {beginAtZero: true}}]},
    plugins: {
      datalabels: {
        anchor: 'end',
        align: 'end',
      }
    },
    tooltips: {
      callbacks: {
        label: (tooltipItem, data) => {
          if (this.format === 'money') {
            let val = this.currencyPipe.transform(tooltipItem.yLabel);
            val = this.customCurrencyPipe.transform(val);
            return val;
          } else if (this.format === 'string') {
            return tooltipItem.yLabel as string;
          }
        }
      }
    }
  };

  // DATA
  @Input() chartLabels: Label[] = ['1', '2'];
  @Input() chartData: ChartDataSets[] = [{data: [10, -15], label: 'a'}, {data: [4, -25], label: 'b'}];

  @Input() format: 'string' | 'money' = 'money';

  chartType: ChartType = 'bar';
  barChartLegend = true;

  @ViewChild(BaseChartDirective, {static: true}) chart: BaseChartDirective;

  constructor(private currencyPipe: CurrencyPipe,
              private customCurrencyPipe: CustomCurrencyPipe) {
  }

  ngOnInit() {
  }

  public refresh() {
    this.chart.ngOnInit();
  }

  changeChartType(chartType: ChartType) {
    this.chartType = chartType;
  }

}
