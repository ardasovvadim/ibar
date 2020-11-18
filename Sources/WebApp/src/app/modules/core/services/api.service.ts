import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders, HttpParams, HttpResponse} from '@angular/common/http';
import {environment} from '../../../../environments/environment';
import {Observable} from 'rxjs';

@Injectable()
export class ApiService {

  private baseApiUrl = environment.apiUrl;

  constructor(private httpClient: HttpClient) {
  }

  get(path: string, options?: OptionsModel): Observable<any> {
    const url = this.baseApiUrl + path;
    return this.httpClient.get<any>(url, options);
  }

  downloadXml(path: string): Observable<any> {
    const url = this.baseApiUrl + path;
    return this.httpClient.get(url, { responseType: 'blob', observe: 'response' });
  }

  post(path: string, body: any = {}, options?: OptionsModel): Observable<any> {
    const url = this.baseApiUrl + path;
    return this.httpClient.post<any>(url, body, options);
  }

  put(path: string, body: any = new HttpParams()): Observable<any> {
    const url = this.baseApiUrl + path;
    return this.httpClient.put<any>(url, body);
  }

  delete(path: string): Observable<any> {
    const url = this.baseApiUrl + path;
    return this.httpClient.delete<void>(url);
  }

}

export class OptionsModel {
  params?: HttpParams;
  headers?: HttpHeaders;
}
