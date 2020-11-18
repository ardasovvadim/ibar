import {DateConvector} from '../utils/date-convector';

export class Period {
  name?: string;
  fromDate?: Date;
  toDate?: Date;
}

export class StringPeriod {
  fromDate: string;
  toDate: string;
}

export function periodsToStringPeriods(periods: Period[]): StringPeriod[] {
  const result: StringPeriod[] = [];

  periods.forEach(p => result.push({
    fromDate: DateConvector.toUrlDate(p.fromDate),
    toDate: DateConvector.toUrlDate(p.toDate)
  }));

  return result;
}
