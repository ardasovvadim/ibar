import {Injectable} from '@angular/core';
import {Observable, Subject} from 'rxjs';
import {ApiService} from './api.service';
import {ExceptionMessage} from '../../ui-common/components/exception-header/exception-message';

@Injectable()
export class ExceptionHeaderService {

  private _exception = new Subject<ExceptionMessage>();

  constructor(private api: ApiService) {
  }

  get exception(): Observable<ExceptionMessage> {
    return this._exception.asObservable();
  }

  nextException(e: ExceptionMessage) {
    this._exception.next(e);
  }

  sendReport(e: ExceptionMessage): Observable<void> {
    return this.api.post('exception/report/send', {guid: e.guid});
  }

}
