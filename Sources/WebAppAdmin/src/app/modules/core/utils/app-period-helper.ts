import {Period} from '../models/period';
import * as moment from 'moment';
import {unitOfTime} from 'moment';

export class AppPeriodHelper {
  private static readonly periods: Period[] = [
    {
      name: 'week',
      fromDate: moment().startOf('week').toDate(),
      toDate: moment().endOf('week').toDate()
    },
    {
      name: 'month',
      fromDate: moment().startOf('month').toDate(),
      toDate: moment().endOf('month').toDate()
    },
    {
      name: 'year',
      fromDate: moment().startOf('year').toDate(),
      toDate: moment().endOf('year').toDate()
    }
  ];

  static getDefault(): Period {
    return this.periods.find(p => p.name === 'week');
  }

  static splitPeriod(period: Period): Period[] {
    switch (period.name) {
      case 'week': {
        return this.splitOnUnitTime(period, 'day');
      }
      case 'month': {
        return this.splitOnUnitTime(period, 'week');
      }
      case 'year': {
        return this.splitOnUnitTime(period, 'month');
      }
      default: {
        const numberDays = moment(period.toDate).diff(moment(period.fromDate), 'days');
        if (numberDays <= 14) {
          return this.splitOnUnitTime(period, 'day');

          // return this.splitOnDays(period);
        }
        const numberMonths = moment(period.toDate).diff(moment(period.fromDate), 'months');
        if (numberMonths < 2) {
          return this.splitOnUnitTime(period, 'week');
        } else {
          return this.splitOnUnitTime(period, 'month');
        }
      }
    }
  }

  private static splitOnUnitTime(period: Period, unitOfTime: unitOfTime.StartOf): Period[] {
    const result: Period[] = [];

    let currentDate = moment(period.fromDate).endOf(unitOfTime);

    if (!moment(period.toDate).isAfter(currentDate, 'day')) {
      result.push(period);
      return result;
    }

    result.push({fromDate: period.fromDate, toDate: currentDate.toDate()});
    currentDate = currentDate.add(1, 'day').endOf(unitOfTime);

    while (moment(period.toDate).isAfter(currentDate)) {
      result.push({fromDate: currentDate.startOf(unitOfTime).toDate(), toDate: currentDate.endOf(unitOfTime).toDate()});
      currentDate = currentDate.endOf(unitOfTime).add(1, 'day').endOf(unitOfTime);
    }

    result.push({fromDate: currentDate.startOf(unitOfTime).toDate(), toDate: period.toDate});

    return result;
  }

  static getPeriodByName(name: string) {
    return this.periods.find(p => p.name === name);
  }
}
