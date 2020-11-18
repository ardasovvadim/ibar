import {Injectable} from '@angular/core';
import {ApiService} from '../../../core/services/api.service';
import {Observable} from 'rxjs';
import {AnalyticsCardInfoModel} from './analytics-card-info.model';
import {ChartDataModel, getDefaultChartDataModel} from '../../../core/models/chart-data.model';
import {Period, periodsToStringPeriods} from '../../../core/models/period';
import {AppPeriodHelper} from '../../../core/utils/app-period-helper';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsService {

  private readonly controllerPath = 'analytics';
  selectedMasterAccountId = 0;
  selectedPeriod: Period = AppPeriodHelper.getDefault();
  selectedFtpCredentialId = 0;

  constructor(private api: ApiService) {
  }

  getAnalyticsCardInfo(masterAccount: number, selectedFtpCredId: number): Observable<AnalyticsCardInfoModel> {
    return this.api.get(`${this.controllerPath}/${masterAccount}/${selectedFtpCredId}`);
  }

  getAnalyticsChartData(masterAccountId: number, period: Period, ftpCredentialId: number): Observable<ChartDataModel> {
    const body = {
      masterAccountId,
      ftpCredentialId,
      periods: periodsToStringPeriods(AppPeriodHelper.splitPeriod(period))
    };
    return this.api.post(`${this.controllerPath}/chart-data`, body);
  }
}
