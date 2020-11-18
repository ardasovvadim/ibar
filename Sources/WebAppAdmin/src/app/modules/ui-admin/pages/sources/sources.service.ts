import {Injectable} from '@angular/core';
import {ApiService, OptionsModel} from '../../../core/services/api.service';
import {Observable, of} from 'rxjs';
import {HttpHeaders} from '@angular/common/http';
import {SourceDataModel} from './source-data.model';
import {catchError} from 'rxjs/operators';
import {Period} from '../../../core/models/period';
import {DateConvector} from '../../../core/utils/date-convector';

@Injectable()
export class SourcesService {

  constructor(private api: ApiService) {
  }

  getAllFiles(pageIndex: number, pageLength: number, sortBy?: string, searchName?: string, periodParam?: Period): Observable<SourceDataModel> {
    const options = this.getOptions(pageIndex, pageLength, sortBy);
    let stringPeriod = null;
    if (periodParam != null) {
      stringPeriod = {
        startDate: DateConvector.toUrlDate(periodParam.fromDate),
        endDate: DateConvector.toUrlDate(periodParam.toDate),
      };
    }
    const body = {
      period: stringPeriod,
      searchName
    };
    return this.api.post('sources/GetSourcesFilesList', body, options).pipe(catchError((err) => {
      return of(null);
    }));
  }

  private getOptions(pageIndex: number, pageLength: number, sortBy?: string): OptionsModel {
    const options = new OptionsModel();
    let headers = new HttpHeaders().append('Pagination', `${pageIndex};${pageLength}`);
    if (sortBy) {
      headers = headers.append('Sorting', sortBy);
    }
    options.headers = headers;
    return options;
  }

}

