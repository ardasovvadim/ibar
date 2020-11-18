import { Component, ComponentFactoryResolver, OnInit, ViewChild, ViewContainerRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ClientsService } from '../clients.service';
import { NavModel } from '../models/nav.model';
import { ChartDataModel } from '../../../../core/models/chart-data.model';
import { Period } from '../../../../core/models/period';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PortfolioVm } from '../models/portfolioVm';
import { PortfolioChartType } from './portfolio-chart-type.enum';
import { LoadingService } from '../../../../core/services/loading.service';
import { PortfolioLineChartComponent } from './portfolio-line-chart/portfolio-line-chart.component';
import { finalize } from 'rxjs/operators';
import { OpenPositionModel } from './open-position.model';
import * as moment from 'moment';

@Component({
  selector: 'app-client-portfolio',
  templateUrl: './client-portfolio.component.html',
  styleUrls: ['./client-portfolio.component.sass']
})
export class ClientPortfolioComponent implements OnInit {

  // Portfolio line chart
  private selectedChartType: PortfolioChartType = PortfolioChartType.NAV;
  readonly chartType = PortfolioChartType;
  portfolioSelectedPeriod: Period = Period.getDefault();
  chartData: ChartDataModel = ChartDataModel.GetDefaultChartDataModel();
  @ViewChild('portLineChart', { static: false }) portfolioLineChart: PortfolioLineChartComponent;
  // ---
  portfolio: PortfolioVm = new PortfolioVm();
  // --- Open positions
  openPosDataLength = 0;
  openPosPageLength = 10;
  openPosPageIndex = 0;
  currOpenPosIndex = 1;
  selectedOpenPosStrDate = '';
  openPosStartDate: Date = null;
  isOpenPositions = true;
  openPositions: OpenPositionModel[] = [];
  // ---
  chartDataSelectedPeriod: Period = Period.getDefault();
  portfolioCurrentDate: Date = this.portfolioSelectedPeriod.toDate;
  nav: NavModel;
  @ViewChild('portfolios', { read: ViewContainerRef, static: true }) portfoliosContainer: ViewContainerRef;
  // TODO: ardasovvadim: default date
  date: Date = new Date();


  navForm: FormGroup;
  private accountNameParam: string;
  totalData: number[] = [];
  availableOpenPosDates: Date[] = [];

  constructor(private route: ActivatedRoute,
              private clientsService: ClientsService,
              private resolver: ComponentFactoryResolver,
              private fBuilder: FormBuilder,
              private loading: LoadingService) {
  }

  ngOnInit(): void {
    this.route.parent.params.subscribe(params => {
      this.accountNameParam = params['id'];
      this.resetFilters();
      this.refreshTotals();
      this.refreshData();
      this.refreshChartData();
      this.getAvailableDatesOpenPos();
    });

    this.initForms();
  }

  cardClick(cardType: PortfolioChartType) {
    this.selectedChartType = cardType;
    this.refreshChartData();
  }

  private updateData() {
    // TODO: ardasovvadim: until implement provide data from API
    // this.dataService.getChartData(this.selectedChartType, this.chartDataSelectedPeriod.fromDate,
    //   this.chartDataSelectedPeriod.toDate, this.idClient).subscribe(data => {
    //   this.barChartData = data.data;
    //   this.barChartLabels = data.labels;
    // });
    // ---
  }

  onChartPeriodChange(period: Period) {
    this.portfolioSelectedPeriod = period;
    this.refreshChartData();
    this.refreshTotals();
  }

  private refreshData() {
    this.clientsService.getPortfolioData(this.accountNameParam)
      .subscribe(data => {
        this.portfolio = data;
        this.clientsService.setAccountHeader({ id: data.accountName, name: data.name });
      });
  }

  private initForms() {
    this.navForm = this.fBuilder.group({
      navNav: [],
      navCash: [],
      navStock: [],
      navOptions: [],
      navCommodities: [],
      navInterestAccruals: [],
      navNavLong: [],
      navCashLong: [],
      navStockLong: [],
      navOptionsLong: [],
      navCommoditiesLong: [],
      navInterestAccrualsLong: [],
      navNavShort: [],
      navCashShort: [],
      navSockShort: [],
      navOptionsShort: [],
      navCommoditiesShort: [],
      navInterestAccrualsShort: []
    });
  }

  checkEmpty(value: any) {
    return typeof value === 'undefined' || value == null ? '' : value;
  }

  private refreshChartData() {
    this.loading.setLoading(true);
    this.clientsService.getPortfolioChartData(
      this.accountNameParam,
      this.portfolioSelectedPeriod,
      this.selectedChartType
    )
      .pipe(
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(
        data => {
          this.chartData = data;
          this.refreshChart();
        },
        () => {
          this.chartData = ChartDataModel.GetDefaultChartDataModel();
        }
      );
  }

  private refreshChart() {
    this.portfolioLineChart.refreshChart();
  }

  openPositionDateChanged(date: string) {
    this.selectedOpenPosStrDate = date;
    this.openPosPageIndex = 0;
    this.openPosDataLength = 0;
    this.currOpenPosIndex = 1;
    this.clientsService.getOpenPositions(date, this.accountNameParam, this.openPosPageLength, this.openPosPageIndex)
      .subscribe(data => {
        this.openPositions = data.data;
        this.openPosDataLength = data.dataLength;
      },
        () => {
          this.openPositions = [];
          this.openPosDataLength = 0;
        });
  }

  private refreshTotals() {
    this.clientsService.getPortfolioTotals(this.accountNameParam, this.portfolioSelectedPeriod)
      .subscribe(data => this.totalData = data.totals,
        () => this.totalData = []);
  }

  resolveChartName(): string {
    switch (this.selectedChartType) {
      case PortfolioChartType.NAV:
        return 'NAV';
      case PortfolioChartType.OPEN_POSITIONS:
        return 'OPN POSITION VALUE';
      case PortfolioChartType.TOTAL_UN:
        return 'TOTAL UN P/L';
      default:
        return '';
    }
  }

  nextOpenPos(number: number = 1) {
    if (this.currOpenPosIndex < this.openPosDataLength) {
      if (this.currOpenPosIndex + number > this.openPosDataLength) {
        this.currOpenPosIndex = this.openPosDataLength;
      } else {
        this.currOpenPosIndex += number;
      }

      if (this.openPositions.length - this.currOpenPosIndex <= 3) {
        this.loadNextOpenPos();
      }
    }
  }

  private loadNextOpenPos() {
    this.clientsService.getOpenPositions(this.selectedOpenPosStrDate, this.accountNameParam, this.openPosPageLength, this.openPosPageIndex + 1)
      .subscribe(data => {
        this.openPositions.push(...data.data);
        ++this.openPosPageIndex;
        this.openPosDataLength = data.dataLength;
      },
        () => {
          this.openPositions = [];
          this.openPosDataLength = 0;
        });
  }

  prevOpenPos() {
    if (this.currOpenPosIndex > 1) {
      --this.currOpenPosIndex;
    }
  }

  getAvailableDatesOpenPos() {
    this.clientsService.getAvailableDatesOpenPos(this.accountNameParam)
      .subscribe(
        data => {
          this.availableOpenPosDates = data.sort();
          if (data.length > 0) {
            this.isOpenPositions = true;
            this.openPosStartDate = this.availableOpenPosDates[this.availableOpenPosDates.length - 1];
          } else {
            this.isOpenPositions = false;
          }
        },
        () => {
          this.availableOpenPosDates = [];
          this.isOpenPositions = false;
        }
      );
  }

  collapseOpenPos(number = -1) {
    if (number === -1 || this.currOpenPosIndex - 1 <= 0) {
      this.currOpenPosIndex = 1;
    } else {
      this.currOpenPosIndex -= number;
    }
  }

  private resetFilters() {
    this.availableOpenPosDates = [];
    this.openPositions = [];
    this.openPosStartDate = null;
    this.selectedOpenPosStrDate = '';
    this.currOpenPosIndex = 1;
    this.openPosDataLength = 0;
    this.openPosPageIndex = 0;
  }
}
