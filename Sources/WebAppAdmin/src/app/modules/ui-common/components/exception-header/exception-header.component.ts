import {Component, OnInit} from '@angular/core';
import {ExceptionHeaderService} from '../../../core/services/exception-header.service';
import {ExceptionMessage} from './exception-message';
import {LoadingService} from '../../../core/services/loading.service';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';

@Component({
  selector: 'app-exception-header',
  templateUrl: './exception-header.component.html',
  styleUrls: ['./exception-header.component.sass']
})
export class ExceptionHeaderComponent implements OnInit {

  exceptions: ExceptionMessage[] = [];
  sending = false;

  constructor(private exceptionService: ExceptionHeaderService,
              private loading: LoadingService) {
  }

  ngOnInit() {
    this.exceptionService.exception.subscribe(e => {
      e.sent = false;
      this.exceptions.push(e);
      window.scroll(0, 0);
    });
  }

  close() {
    this.exceptions.shift();
  }

  sendReport(exceptionMessage: ExceptionMessage) {
    if (this.sending || exceptionMessage.sent) {
      return;
    }

    this.sending = true;
    this.loading.setLoading(true);
    this.exceptionService.sendReport(exceptionMessage)
      .pipe(
        finalize(() => this.loading.setLoading(false)),
        catchError(() => {
          this.exceptions.unshift({messages: ['Can\`t send bug report']});
          this.sending = false;
          return of(null);
        })
      )
      .subscribe(() => {
        this.sending = false;
        exceptionMessage.sent = true;
      });
  }
}
