import { Injectable } from '@angular/core';
import {ApiService} from '../../../core/services/api.service';
import {Observable} from 'rxjs';

@Injectable()
export class ControlService {

  constructor(private api: ApiService) {
  }

  startJob(job: string): Observable<any> {
    return this.api.get(`monitor/start/${job}`);
  }

  stopJob(job: string): Observable<any> {
    return this.api.get(`monitor/stop/${job}`);
  }

  reloadJob(job: string): Observable<any> {
    return this.api.get(`monitor/reload/${job}`);
  }

  getStatusJob(job: string): Observable<any> {
    return this.api.get(`monitor/status/${job}`);
  }

}
