import {Utils} from './utils';
import {Period} from '../models/period';
import {DateConvector} from './date-convector';

export class QueryParamHelper {
  static isContainsPeriod(queryParams: any): boolean {
    const fromDate = queryParams['fromDate'];
    const toDate = queryParams['toDate'];
    return !Utils.isNullOrUndefined(fromDate) && !Utils.isNullOrUndefined(toDate);
  }
  static getPeriod(queryParams: any): Period {
    if (this.isContainsPeriod(queryParams)) {
      return {
        fromDate: DateConvector.fromUrlDate(queryParams['fromDate']),
        toDate: DateConvector.fromUrlDate(queryParams['toDate'])
      };
    }
  }
}
