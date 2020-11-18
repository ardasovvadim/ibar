import * as moment from 'moment';

export class Period {

  constructor(name?: string, fromDate?: Date, toDate?: Date) {
    this.name = name;
    this.fromDate = fromDate;
    this.toDate = toDate;
  }

  private static _periods: Period[] = Period._generatePeriods();
  private static _defaultPeriod: Period = null;

  name: string;
  fromDate?: Date;
  toDate?: Date;

  static splitPeriod(period: Period): Period[] {
    const result = [];
    switch (period.name) {
      case 'LD': {
        const newPeriod = new Period('1', period.fromDate, period.toDate);
        result.push(newPeriod);
        break;
      }
      case 'Custom': {
        let start = moment(period.fromDate);
        const end = moment(period.toDate);
        let iteration = 1;
        do {
          const currentDate = moment(start);
          const newPeriod = new Period(iteration.toString(), currentDate.startOf('day').toDate(), currentDate.endOf('day').toDate());
          result.push(newPeriod);
          start = moment(start).add(1, 'days');
          ++iteration;
        }
        while (!start.isSame(moment(end).add(1, 'day'), 'day'));
        if (result.length < 14) {
          break;
        } else if (result.length >= 14 && result.length <= 30) {
          result.length = 0;
          const startWeek = moment(period.fromDate).week();
          const endWeek = moment(period.toDate).week();
          for (let i = startWeek, iteration = 1; i <= endWeek; ++i, ++iteration) {
            let start = moment().week(i).startOf('week');
            if (i === startWeek) {
              start = moment(period.fromDate);
            }
            let end = moment().week(i).endOf('week');
            if (i === endWeek) {
              end = moment(period.toDate);
            }
            const newPeriod = new Period(iteration.toString(), start.toDate(), end.toDate());
            result.push(newPeriod);
          }
        } else if (result.length > 30 && result.length <= 180) {
          const months = (result.length  - result.length % 30) / 30;
          result.length = 0;
          const startMonth = moment(period.fromDate).month();
          const endMonth = moment(period.toDate).month();
          for (let i = startMonth, iteration = 1; i <= endMonth; ++i, ++iteration) {
            let start = moment().month(i).startOf('month');
            if (i === startMonth) {
              start = moment(period.fromDate);
            }
            let end = moment().month(i).endOf('month');
            if (i === endMonth) {
              end = moment(period.toDate);
            }
            const newPeriod = new Period(iteration.toString(), start.toDate(), end.toDate());
            result.push(newPeriod);
          }
        } else if (result.length > 180 && result.length <= 730) {
          result.length = 0;
          const startQuarter = moment(period.fromDate).quarter();
          let endQuarter = moment(period.toDate).quarter();
          const startYear = moment(period.fromDate).year();
          const endYear = moment(period.toDate).year();
          endQuarter = endYear - startYear? (endYear - startYear)*12 + endQuarter: endQuarter;
          for (let i = startQuarter, iteration = 1; i <= endQuarter; ++i, ++iteration) {
            let start = moment().quarter(i).startOf('quarter');
            if (i === startQuarter) {
              start = moment(period.fromDate);
            }
            let end = moment().quarter(i).endOf('quarter');
            if (i === endQuarter) {
              end = moment(period.toDate);
            }
            const newPeriod = new Period(iteration.toString(), start.toDate(), end.toDate());
            result.push(newPeriod);
          }
        } else  if (result.length > 730) {
          result.length = 0;
          const startQuarter = moment(period.fromDate).year();
          const endQuarter = moment(period.toDate).year();
          for (let i = startQuarter, iteration = 1; i <= endQuarter; ++i, ++iteration) {
            let start = moment().year(i).startOf('year');
            if (i === startQuarter) {
              start = moment(period.fromDate);
            }
            let end = moment().year(i).endOf('year');
            if (i === endQuarter) {
              end = moment(period.toDate);
            }
            const newPeriod = new Period(iteration.toString(), start.toDate(), end.toDate());
            result.push(newPeriod);
          }
        }
        break;
      }
      case 'MTD': {
        let start = moment(period.fromDate);
        const end = moment(period.toDate);
        let iteration = 1;
        do {
          const currentDate = moment(start);
          const newPeriod = new Period(iteration.toString(), currentDate.startOf('day').toDate(), currentDate.endOf('day').toDate());
          result.push(newPeriod);
          start = moment(start).add(1, 'days');
          ++iteration;
        }
        while (!start.isSame(moment(end).add(1, 'day'), 'day'));
        break;
      }
      case 'YTD': {
        const quater = moment(period.toDate).quarter();
        for (let i = 1; i <= quater; ++i) {
          const start = moment(period.fromDate).quarter(i).startOf('quarter');
          let end = moment(period.fromDate).quarter(i).endOf('quarter');
          if (i === quater) {
            end = moment(period.toDate);
          }
          const newPeriod = new Period(i.toString(), start.toDate(), end.toDate());
          result.push(newPeriod);
        }
        break;
      }
      case 'LM': {
        const startWeek = moment(period.fromDate).week();
        const endWeek = moment(period.toDate).week();
        for (let i = startWeek, iteration = 1; i <= endWeek; ++i, ++iteration) {
          const start = moment().week(i).startOf('week');
          let end = moment().week(i).endOf('week');
          if (i === endWeek) {
            end = moment(period.toDate);
          }
          const newPeriod = new Period(iteration.toString(), start.toDate(), end.toDate());
          result.push(newPeriod);
        }
        break;
      }
      case 'L6M': {
        result.push(...this.getMonthPeriods(period, 6));
        break;
      }
      case 'L3M':
      case 'Q1':
      case 'Q2':
      case 'Q3':
      case 'Q4': {
        result.push(...this.getMonthPeriods(period, 3));
        break;
      }
    }
    return result;
  }

  private static getMonthPeriods(period: Period, numberOfMonths: number): Period[] {
    const result: Period[] = [];
    let start = moment(period.fromDate);
    for (let i = 0; i < numberOfMonths; ++i) {
      result.push(new Period((i+1).toString(), start.toDate(), moment(start).endOf('month').toDate()));
      start = moment(start).add(1, 'month');
    }
    return result;
  }

  private static _generatePeriods(): Period[] {
    const periods: Period[] = [];
    periods.push(new Period('LD', moment().toDate(), moment().toDate()));
    periods.push(new Period('MTD', moment().startOf('month').toDate(), moment().toDate()));
    periods.push(new Period('YTD', moment().startOf('year').toDate(), moment().toDate()));
    periods.push(new Period('LM',
      moment().subtract(1, 'month').startOf('month').toDate(),
      moment().subtract(1, 'month').endOf('month').toDate()));
    periods.push(new Period('L3M',
      moment().subtract(3, 'month').startOf('month').toDate(),
      moment().subtract(1, 'month').endOf('month').toDate()));
    periods.push(new Period('L6M',
      moment().subtract(6, 'month').startOf('month').toDate(),
      moment().subtract(1, 'month').endOf('month').toDate()));
    periods.push(new Period('Q1',
      moment().quarter(1).startOf('quarter').toDate(),
      moment().quarter(1).endOf('quarter').toDate()));
    periods.push(new Period('Q2',
      moment().quarter(2).startOf('quarter').toDate(),
      moment().quarter(2).endOf('quarter').toDate()));
    periods.push(new Period('Q3',
      moment().quarter(3).startOf('quarter').toDate(),
      moment().quarter(3).endOf('quarter').toDate()));
    periods.push(new Period('Q4',
      moment().quarter(4).startOf('quarter').toDate(),
      moment().quarter(4).endOf('quarter').toDate()));
    return periods;
  }

  static get periods(): Period[] {
    return this._periods;
  }

  static getDefault(): Period {
    if (!this._defaultPeriod)
      this._defaultPeriod = this._periods.find(p => p.name === 'MTD');
    return this._defaultPeriod;
  }

  private getFormatDate(date: Date): string {
    return moment(date).format('YYYY/MM/DD');
  }

  toString(): string {
    return `${this.fromDateToString()} - ${this.toDateToString()}`;
  }

  fromDateToString(): string {
    if (!this.toDate) {
      return '';
    }
    return this.getFormatDate(this.fromDate);
  }

  toDateToString(): string {
    if (!this.toDate) {
      return '';
    }
    return this.getFormatDate(this.toDate);
  }
}
