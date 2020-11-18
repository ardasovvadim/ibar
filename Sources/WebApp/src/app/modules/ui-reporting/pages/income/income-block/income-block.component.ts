import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ChartColor, ChartDataSets, ChartOptions, ChartType} from 'chart.js';
import {BaseChartDirective, Label} from 'ng2-charts';
import {Period} from '../../../../core/models/period';
import {MasterAccountModel} from '../../../../core/models/master-account.model';
import {DATA} from '../../../../../mock-data/data-set';
import {MasterAccountService} from '../../../../core/services/master-account.service';
import {ChartDataModel} from '../../../../core/models/chart-data.model';
import {IncomeService} from '../income.service';
import {IncomeEnum} from '../income-barchart.enum';
import {Colors} from '../../../../core/models/chart-color-set';
import * as Color from 'color';
import {FilterService} from '../../../../core/services/filter.service';

@Component({
  selector: 'app-income-block',
  templateUrl: './income-block.component.html',
  styleUrls: ['./income-block.component.sass']
})
export class IncomeBlockComponent implements OnInit {

  @Input() title = '';
  @Input() cardType: IncomeEnum;

  selectedMasterAccount: MasterAccountModel;
  selectedPeriod: Period = Period.getDefault();
  chartData: ChartDataModel = ChartDataModel.GetDefaultChartDataModel();
  chartFormat: 'string' | 'money' = 'money';

  public barChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    legend: {
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
  public barChartLabels: Label[] = ['2006', '2007', '2008', '2009', '2010', '2011', '2012'];
  public barChartType: ChartType = 'bar';
  public barChartLegend = true;
  public barChartData: ChartDataSets[] = [
    {data: [65, 59, 80, 81, 56, 55, 40], label: 'Series A', backgroundColor: 'grey', hoverBackgroundColor: 'lightgrey'},
    {data: [28, 48, 40, 19, 86, 27, 90], label: 'Series B', backgroundColor: 'black', hoverBackgroundColor: 'grey'}
  ];

  @ViewChild(BaseChartDirective, {static: true}) chart: BaseChartDirective;
  colors: Color[] = [
    {backgroundColor: '#252525'},
    {backgroundColor: '#666666'},
    {backgroundColor: '#999999'},
    {backgroundColor: '#B2B2B2'},
  ];

  constructor(private masterAccountService: MasterAccountService,
              private filterService: FilterService,
              private  incomeService: IncomeService) {
  }

  ngOnInit() {
    const sub = this.filterService.filtersChangedEvent.subscribe(() => {
      this.refreshBarCharts();
    });
    this.refreshBarCharts();
  }

  private refreshBarCharts() {
    this.incomeService.getChartData(this.cardType)
      .subscribe(data => {
        const chartData = data.data as ChartDataSets[];
        if(chartData) {
          chartData.forEach((dataSet, index) => {
            dataSet.minBarLength = 10;
            const color = Colors.getColor(index, 1);
            dataSet.backgroundColor = Color(color).lighten(0.1).hex();
            dataSet.hoverBackgroundColor = color;
          });
          this.chartData = data;
          this.refreshChart();
        }
      });
  }


  private refreshChart() {
   // this.chartComponent.ngOnInit();
  }

  update() {
    // TODO: until implements API
    this.barChartLabels = DATA[0].labels;
    this.barChartData = DATA[0].data;
    // ---
    this.chart.ngOnInit();
  }

  onAccountChange(masterAccount: MasterAccountModel) {
    this.selectedMasterAccount = masterAccount;
    this.update();
  }

  onPeriodChange(period: Period) {
    this.selectedPeriod = period;
    this.update();
  }

}
