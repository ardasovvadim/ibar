import {Component, Input, OnInit} from '@angular/core';
import {ChartOptions, ChartType} from 'chart.js';
import {Label} from 'ng2-charts';
import {DashboardService} from '../../../core/services/dashboard.service';
import {DashboardEnum} from '../../../ui-reporting/pages/dashboard/dashboard.enum';
import {DATA} from '../../../../mock-data/data-set';

@Component({
  selector: 'app-pie-block',
  templateUrl: './pie-block.component.html',
  styleUrls: ['./pie-block.component.sass']
})
export class PieBlockComponent implements OnInit {

  @Input() title: string;
  @Input() cardType: DashboardEnum;
  @Input() fromDate: Date;
  @Input() toDate: Date;
  @Input() idAccount: number;

  public pieChartOptions: ChartOptions = {
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
  public pieChartLabels: Label[];
  public pieChartData: any;
  public pieChartType: ChartType = 'pie';
  public pieChartLegend = true;
  public pieChartColors = [{backgroundColor: ['black', 'grey']}];

  constructor(private dataService: DashboardService) { }

  ngOnInit() {
    // TODO: until implements API
      this.pieChartData = DATA[0].data[0].data as number[];
      this.pieChartLabels = DATA[0].labels;
    // ---
  }

}
