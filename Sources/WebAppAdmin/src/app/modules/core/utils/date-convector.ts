import * as moment from 'moment';

export class DateConvector {
  static toString(date: Date): string {
    if (date) {
      return moment(date).format('DD/MM/YYYY');
    }
    return '';
  }
  static toUrlDate(date: Date): string {
    return moment(date).format('YYYY-DD-MM');
  }
  static fromUrlDate(date: string): Date {
    return moment(date, 'YYYY-DD-MM').toDate();
  }
  static dateTimeToString(date: Date): string {
    if (date) {
      return moment.utc(date).local().format('DD/MM/YYYY hh:mm:ss');
    }
    return '';
  }
}
