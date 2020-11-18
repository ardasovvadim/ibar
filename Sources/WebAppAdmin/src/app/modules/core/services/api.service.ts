import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams, HttpResponse} from '@angular/common/http';
import {environment} from '../../../../environments/environment';
import {Observable} from 'rxjs';
import {LoadingService} from './loading.service';
import {finalize} from 'rxjs/operators';

@Injectable()
export class ApiService {

  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient,
              private loading: LoadingService) {
  }

  get(url: string, options?: any, setLoading = true): Observable<any> {
    let obs = this.http.get(`${this.apiUrl}/${url}`, options);
    if (setLoading) {
      this.loading.setLoading(true);
      obs = obs.pipe(finalize(() => this.loading.setLoading(false)));
    }
    return obs;
  }

  post(url: string, body: any = null, options?: any, setLoading = true): Observable<any> {
    let obs = this.http.post(`${this.apiUrl}/${url}`, body, options);
    if (setLoading) {
      this.loading.setLoading(true);
      obs = obs.pipe(finalize(() => this.loading.setLoading(false)));
    }
    return obs;
  }

  put(url: string, body: any = null, options?: any, setLoading = true): Observable<any> {
    let obs = this.http.put(`${this.apiUrl}/${url}`, body, options);
    if (setLoading) {
      this.loading.setLoading(true);
      obs = obs.pipe(finalize(() => this.loading.setLoading(false)));
    }
    return obs;
  }

  downloadXml(path: string, setLoading = true): Observable<any> {
    const url = this.apiUrl + path;
    let obs = this.http.get(url, { responseType: 'blob', observe: 'response' });
    if (setLoading) {
      this.loading.setLoading(true);
      obs = obs.pipe(finalize(() => this.loading.setLoading(false)));
    }
    return obs;
  }

  delete(url: string, options?: any, setLoading = true): Observable<any> {
    let obs = this.http.delete(`${this.apiUrl}/${url}`, options);
    if (setLoading) {
      this.loading.setLoading(true);
      obs = obs.pipe(finalize(() => this.loading.setLoading(false)));
    }
    return obs;
  }

}

export class OptionsModel {
  params?: HttpParams;
  headers?: HttpHeaders;
}
