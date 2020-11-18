import {Component, OnInit} from '@angular/core';
import {AnalyticsService} from './analytics.service';
import {AnalyticsCardInfoModel} from './analytics-card-info.model';
import {IdNameModel} from '../../../core/models/id-name.model';
import {Period} from '../../../core/models/period';
import {AppPeriodHelper} from '../../../core/utils/app-period-helper';
import {ChartDataModel, getDefaultChartDataModel} from '../../../core/models/chart-data.model';
import {Router} from '@angular/router';
import {DateConvector} from '../../../core/utils/date-convector';

@Component({
  selector: 'app-analytics-page',
  templateUrl: './analytics-page.component.html',
  styleUrls: ['./analytics-page.component.sass']
})
export class AnalyticsPageComponent implements OnInit {

  cardInfo: AnalyticsCardInfoModel;
  selectedMasterAccountId: number;
  selectedPeriod: Period;
  chartData: ChartDataModel = getDefaultChartDataModel();
  selectedFtpCredentialId: number;

  constructor(private analyticsService: AnalyticsService,
              private router: Router) {

  }

  ngOnInit() {
    this.initiateFilters();
    this.refreshData();
  }

  private initiateFilters(): void {
    this.selectedMasterAccountId = this.analyticsService.selectedMasterAccountId;
    this.selectedPeriod = this.analyticsService.selectedPeriod;
    this.selectedFtpCredentialId = this.analyticsService.selectedFtpCredentialId;
  }

  private refreshCardInfoData() {
    this.analyticsService.getAnalyticsCardInfo(this.selectedMasterAccountId, this.selectedFtpCredentialId)
      .subscribe(data => this.cardInfo = data, () => this.cardInfo = null);
  }

  private refreshChartData() {
    this.analyticsService.getAnalyticsChartData(this.selectedMasterAccountId, this.selectedPeriod, this.selectedFtpCredentialId)
      .subscribe(
        data => this.chartData = data,
        () => this.chartData = getDefaultChartDataModel()
      );
  }

  masterAccountChanged($event: IdNameModel) {
    this.selectedMasterAccountId = $event.id;
    this.analyticsService.selectedMasterAccountId = $event.id;
    this.refreshData();
  }

  selectedPeriodChanged($event: Period) {
    this.selectedPeriod = $event;
    this.analyticsService.selectedPeriod = $event;
    this.refreshChartData();
  }

  setSelectedPeriod(name: string) {
    this.selectedPeriod = AppPeriodHelper.getPeriodByName(name);
    this.analyticsService.selectedPeriod = AppPeriodHelper.getPeriodByName(name);
    this.refreshChartData();
  }

  ftpCredentialChanged($event: IdNameModel) {
    this.selectedFtpCredentialId = $event.id;
    this.analyticsService.selectedFtpCredentialId = $event.id;
    this.refreshData();
  }

  refreshData() {
    this.refreshCardInfoData();
    this.refreshChartData();
  }

  details() {
    this.router.navigate(['/sources'], {
      queryParams: {
        fromDate: DateConvector.toUrlDate(this.selectedPeriod.fromDate),
        toDate: DateConvector.toUrlDate(this.selectedPeriod.toDate)
      }
    });
  }
}
