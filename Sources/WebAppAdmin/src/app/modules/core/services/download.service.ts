import {Injectable} from '@angular/core';
import {catchError} from 'rxjs/operators';
import {of} from 'rxjs';
import {ApiService} from './api.service';
import * as FileSaver from 'file-saver';

@Injectable()
export class DownloadService {

  constructor(private api: ApiService) {
  }

  download(path: string): void {
    this.api.downloadXml(path)
      .pipe(catchError(err => {
        return of(null);
      }))
      .subscribe((data) => {
        if (data) {
          const fileName = data.headers.get('FileName');
          const blob = new Blob([data.body], {type: 'text/xml'});
          FileSaver.saveAs(blob, fileName);
        }
      });
  }
}
