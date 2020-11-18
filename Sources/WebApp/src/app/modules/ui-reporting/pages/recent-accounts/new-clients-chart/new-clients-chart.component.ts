import {Component, Input, OnInit} from '@angular/core';
import {ChartOptions, ChartType} from "chart.js";
import {Label} from "ng2-charts";
import {Colors} from "../../../../core/models/chart-color-set";

@Component({
  selector: 'app-new-clients-chart',
  templateUrl: './new-clients-chart.component.html',
  styleUrls: ['./new-clients-chart.component.sass']
})
export class NewClientsChartComponent implements OnInit {

  pieChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    legend: {
      position: 'top',
    },
    plugins: {
      datalabels: {
        formatter: (value, ctx) => {
          const label = ctx.chart.data.labels[ctx.dataIndex];
          return label;
        },
      },
    }
  };
  @Input() pieChartLabels: Label[] = [];
  @Input() pieChartData: number[] = [];
  pieChartType: ChartType = 'pie';
  pieChartLegend = true;
  pieChartColors = [
    {
      backgroundColor: Colors.COLORS
    },
  ];

  constructor() {
  }

  ngOnInit() {
  }

}
