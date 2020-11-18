import * as moment from 'moment';

export class DateConvector {
  static DateToString(date: Date): string {
    if (date) {
      return moment.utc(date).local().format('DD/MM/YYYY');
    }
    return '';
  }

  static DateTimeToString(date: Date): string {
    if (date) {
      return moment.utc(date).local().format('DD/MM/YYYY hh:mm:ss');
    }
    return '';
  }

  static toUrlDate(date: Date): string {
    return moment(date).format('YYYY-DD-MM');
  }
}
