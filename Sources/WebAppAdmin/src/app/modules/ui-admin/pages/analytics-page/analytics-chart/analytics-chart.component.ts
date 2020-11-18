import {Component, Input, OnInit} from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexGrid,
  ApexLegend,
  ApexPlotOptions,
  ApexStroke,
  ApexTitleSubtitle,
  ApexTooltip,
  ApexXAxis
} from 'ng-apexcharts';
import {ChartDataModel} from '../../../../core/models/chart-data.model';

@Component({
  selector: 'app-analytics-chart',
  templateUrl: './analytics-chart.component.html',
  styleUrls: ['./analytics-chart.component.sass']
})
export class AnalyticsChartComponent implements OnInit {

  @Input() set data(chartData: ChartDataModel) {
    this.chartOptions.series = chartData.data.map(d => ({name: d.label, data: d.data}));
    this.chartOptions.xaxis = {categories: chartData.labels};
  }

  chartOptions: Partial<ChartOptions>;

  constructor() {
    this.chartOptions = {
      series: [
        {
          name: 'My-series',
          data: [10, 41, 35, 51, 49, 62, 69, 91, 148]
        },
        {
          name: 'Next',
          data: [23, 64, 10, 56, 13, 65, 34, 67, 78]
        },
        {
          name: 'Next 2',
          data: [23, 64, 10, 56, 13, 65, 34, 67, 78]
        },
        {
          name: 'Next 3',
          data: [23, 64, 10, 56, 13, 65, 34, 67, 78]
        }
      ],
      legend: {
        offsetY: 10,
        height: 40,
        markers: {
          radius: 3
        }
      },
      colors: [
        '#556EE6',
        '#F1B44C',
        '#34C38F',
        '#F52A77'
      ],
      chart: {
        height: 400,
        type: 'line',
        zoom: {
          enabled: false
        },
        fontFamily: 'Roboto, sans-serif',
        toolbar: {
          show: false
        }
      },
      grid: {
        row: {
          colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
          opacity: 0.5
        }
      },
      xaxis: {
        categories: ['Jann', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep']
      }
    };
  }

  ngOnInit() {
  }

}

export class ChartOptions {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  legend: ApexLegend;
  plotOptions: ApexPlotOptions;
  tooltip: ApexTooltip;
  colors: string[];
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
}
