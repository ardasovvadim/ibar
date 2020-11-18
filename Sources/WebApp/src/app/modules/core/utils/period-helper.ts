import {Period} from '../models/period';
import {DateConvector} from './date-convector';

export class PeriodHelper {
  static periodToStringPeriod(period: Period) {
    return {
      fromDate: DateConvector.toUrlDate(period.fromDate),
      toDate: DateConvector.toUrlDate(period.toDate)
    };
  }

  static arrayPeriodToArrayStringPeriod(periods: Period[]) {
    const result = [];
    periods.forEach(period => {
      result.push(this.periodToStringPeriod(period));
    });
    return result;
  }
}
